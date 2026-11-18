using Apps.HeyGen.Api;
using Apps.HeyGen.Files;
using Apps.HeyGen.Models.Dtos;
using Apps.HeyGen.Models.Inputs;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.HeyGen.Actions;

[ActionList("Templates")]
public class TemplateActions(InvocationContext invocationContext, IFileManagementClient files) : Invocable(invocationContext)
{
    [Action("Get template variables", Description = "Read editable variable names, types and defaults for an API-ready template.")]
    public async Task<TemplateDetails> GetTemplate([ActionParameter] TemplateInput input)
    {
        var template = await ReadTemplate(input.TemplateId);
        return new TemplateDetails
        {
            TemplateId = template.Id,
            Name = template.Name,
            Variables = JsonConvert.SerializeObject(template.Variables,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
        };
    }

    [Action("Create video from template", Description = "Fill an API-ready Studio template and submit a new video render.")]
    public async Task<VideoDetails> CreateTemplateVideo([ActionParameter] CreateTemplateVideoInput input)
    {
        var key = Requests.SubmissionKey(input.SubmissionKey);
        if ((input.TextVariable is null) != (input.Text is null))
            throw new PluginMisconfigurationException("Provide both a text variable name and its replacement text.");

        var template = await ReadTemplate(input.TemplateId);
        var variables = Defaults(template);
        if (!string.IsNullOrWhiteSpace(input.Variables))
        {
            var overrides = Requests.JsonInput<JObject>(input.Variables, "variable replacements");
            foreach (var variable in overrides.Properties())
            {
                if (!template.Variables.TryGetValue(variable.Name, out var definition)
                    || variable.Value is not JObject value || value["type"]?.Type != JTokenType.String
                    || (string?)value["type"] != definition.Type)
                    throw new PluginMisconfigurationException($"Choose a variable name defined on the template and provide the correct type for {variable.Name}.");

                variables[variable.Name] = variable.Value;
            }
        }

        if (input.TextVariable is not null)
        {
            var name = input.TextVariable.Trim();
            if (!template.Variables.TryGetValue(name, out var definition) || definition.Type != "text")
                throw new PluginMisconfigurationException("Choose a text variable from the selected template.");

            variables[name] = Text(input.Text!);
        }

        return await Render(input, variables, key);
    }

    [Action("Export template text as XLIFF", Description = "Export the current default values of a template's text variables.")]
    public async Task<FileOutput> ExportTemplate([ActionParameter] TemplateExportInput input)
    {
        var id = Requests.Id(input.TemplateId, "a template ID");
        var template = await ReadTemplate(input.TemplateId);
        var content = template.Variables.Where(x => x.Value.Type == "text" && !string.IsNullOrWhiteSpace(x.Value.Content))
            .ToDictionary(x => x.Key, x => x.Value.Content!, StringComparer.Ordinal);

        await using var stream = XliffScript.Write($"heygen:template:{id}", input.SourceLanguage, input.TargetLanguage, content);
        return new FileOutput { File = await files.UploadAsync(stream, "application/x-xliff+xml", $"{id}-template.xlf") };
    }

    [Action("Create template video from XLIFF", Description = "Apply approved XLIFF text to the same template and submit a new render.")]
    public async Task<VideoDetails> CreateFromXliff([ActionParameter] TemplateXliffInput input)
    {
        var key = Requests.SubmissionKey(input.SubmissionKey);
        var file = XliffScript.Read(await MediaFiles.ReadBytes(files, input.File, 2_000_000));
        var id = Requests.Id(input.TemplateId, "a template ID");
        if (file.Original != $"heygen:template:{id}")
            throw new PluginMisconfigurationException("Select the same template that was used to export the XLIFF file.");

        var template = await ReadTemplate(input.TemplateId);
        var current = template.Variables.Where(entry => entry.Value.Type == "text" && !string.IsNullOrWhiteSpace(entry.Value.Content))
            .ToDictionary(entry => entry.Key, entry => entry.Value.Content!, StringComparer.Ordinal);
        if (file.Units.Count != current.Count ||
            file.Units.Any(entry => !current.TryGetValue(entry.Key, out var value) || value != entry.Value.Source))
            throw new PluginMisconfigurationException("The XLIFF units no longer match the template text. Export the current template again and update the translation.");

        var variables = Defaults(template);
        foreach (var (name, unit) in file.Units)
            variables[name] = Text(unit.Target);
        return await Render(input, variables, key);
    }

    private Task<Template> ReadTemplate(string id) =>
        Client.Data<Template>(new RestRequest($"templates/{Requests.Id(id, "a template ID")}"));

    private Task<VideoDetails> Render(TemplateRenderInput input, Dictionary<string, object> variables, string key)
    {
        var request = Requests.Json($"templates/{Requests.Id(input.TemplateId, "a template ID")}", Method.Post, new()
        {
            ["title"] = input.Title,
            ["variables"] = variables,
            ["caption"] = input.BurnCaptions,
            ["brand_glossary_id"] = input.BrandGlossaryId,
            ["callback_id"] = input.CallbackId,
            ["enable_sharing"] = false
        }).AddHeader("Idempotency-Key", key);

        return Client.Data<VideoDetails>(request);
    }

    private static Dictionary<string, object> Defaults(Template template) =>
        template.Variables.Where(entry => entry.Value.Type == "text").ToDictionary(
            entry => entry.Key,
            entry => (object)Text(entry.Value.Content ?? ""),
            StringComparer.Ordinal);

    private static object Text(string content) => new { type = "text", content };
}
