using Apps.HeyGen.Api;
using Apps.HeyGen.Models.Dtos;
using Apps.HeyGen.Models.Inputs;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.HeyGen.Actions;

[ActionList("Brand glossaries")]
public class GlossaryActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Search brand glossaries", Description = "Find HeyGen brand glossaries visible to the connection.")]
    public async Task<GlossaryResults> SearchGlossaries()
    {
        var items = await Client.Paginate<Glossary>(() => new RestRequest("brand-glossaries"), 50, 100);
        return new GlossaryResults
        {
            BrandGlossaries = items.Select(x => new GlossaryDetails { BrandGlossaryId = x.Id, Name = x.Name }).ToList()
        };
    }

    [Action("Get brand glossary", Description = "Read pronunciation, do-not-translate and forced-translation rules.")]
    public async Task<GlossaryDetails> GetGlossary([ActionParameter] GlossaryInput input) =>
        Output(await Client.Data<Glossary>(new RestRequest(GlossaryPath(input.BrandGlossaryId))));

    [Action("Create brand glossary", Description = "Create a glossary with optional pronunciation and translation rules.")]
    public async Task<GlossaryDetails> CreateGlossary([ActionParameter] CreateGlossaryInput input)
    {
        var key = Requests.SubmissionKey(input.SubmissionKey);
        var name = Requests.Required(input.Name, "a glossary name").Trim();
        var body = Body(name, input.Terms, input.DoNotTranslate, input.ForcedTranslations);
        var request = Requests.Json("brand-glossaries", Method.Post, body).AddHeader("Idempotency-Key", key);
        return Output(await Client.Data<Glossary>(request));
    }

    [Action("Update brand glossary", Description = "Change the name or replace supplied rule lists. Omitted rule lists remain intact.")]
    public async Task<GlossaryDetails> UpdateGlossary([ActionParameter] GlossaryInput input,
        [ActionParameter] GlossaryFields changes)
    {
        var body = Body(changes.Name, changes.Terms, changes.DoNotTranslate, changes.ForcedTranslations);
        if (body.Values.All(x => x is null))
            throw new PluginMisconfigurationException("Provide at least one glossary field to update.");

        var request = Requests.Json(GlossaryPath(input.BrandGlossaryId), Method.Patch, body);
        return Output(await Client.Data<Glossary>(request));
    }

    [Action("Delete brand glossary", Description = "Permanently delete the selected HeyGen glossary.")]
    public async Task<GlossaryDeleted> DeleteGlossary([ActionParameter] GlossaryInput input)
    {
        await Client.ExecuteWithErrorHandling(new RestRequest(GlossaryPath(input.BrandGlossaryId), Method.Delete));
        return new GlossaryDeleted { BrandGlossaryId = input.BrandGlossaryId.Trim() };
    }

    private static string GlossaryPath(string id) =>
        $"brand-glossaries/{Requests.Id(id, "a brand glossary ID")}";

    private static Dictionary<string, object?> Body(string? name, string? terms, string? doNotTranslate, string? forced)
    {
        if (name is not null && name.Trim().Length is < 1 or > 64)
            throw new PluginMisconfigurationException("Enter a glossary name from 1 to 64 characters.");

        return new()
        {
            ["name"] = name?.Trim(),
            ["terms"] = string.IsNullOrWhiteSpace(terms) ? null : Requests.JsonInput<JArray>(terms, "pronunciations"),
            ["do_not_translate_terms"] = string.IsNullOrWhiteSpace(doNotTranslate) ? null :
                Requests.JsonInput<JArray>(doNotTranslate, "do-not-translate rules"),
            ["forced_translations"] = string.IsNullOrWhiteSpace(forced) ? null :
                Requests.JsonInput<JArray>(forced, "forced translations")
        };
    }

    private static GlossaryDetails Output(Glossary data)
    {
        var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        return new GlossaryDetails
        {
            BrandGlossaryId = data.Id,
            Name = data.Name,
            Terms = JsonConvert.SerializeObject(data.Terms ?? [], settings),
            DoNotTranslate = JsonConvert.SerializeObject(data.DoNotTranslate ?? [], settings),
            ForcedTranslations = JsonConvert.SerializeObject(data.ForcedTranslations ?? [], settings)
        };
    }
}
