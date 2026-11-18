using Apps.HeyGen.Actions;
using Apps.HeyGen.Models.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.HeyGen.Base;

namespace Tests.HeyGen;

[TestClass]
public class TemplateAndScriptTests : TestBase
{
    [TestMethod]
    public async Task GetTemplateReturnsVariableSchema()
    {
        var id = Fixture("TemplateId");
        var response = await new TemplateActions(Context, Files).GetTemplate(new() { TemplateId = id });
        Assert.AreEqual(id, response.TemplateId);
        StringAssert.StartsWith(response.Variables, "{");
    }

    [TestMethod]
    public async Task CreateTemplateVideoReturnsJobId()
    {
        RequireWrites();
        var result = await new TemplateActions(Context, Files).CreateTemplateVideo(new()
        {
            TemplateId = Fixture("TemplateId"),
            SubmissionKey = SubmissionKey()
        });
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.VideoId));
    }

    [TestMethod]
    public async Task ExportVideoAndReadTranslatedScript()
    {
        var id = Fixture("EditableVideoId");
        var actions = new ScriptActions(Context, Files);
        var export = await actions.ExportVideoScript(new() { VideoId = id, SourceLanguage = "en", TargetLanguage = "es" });
        var translated = await Files.CopySourcesToTargets(export.File);
        var result = await actions.ReadVideoScript(new() { File = translated });
        Assert.AreEqual(id, result.VideoId);
        Assert.AreEqual("es", result.TargetLanguage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Script));
    }

    [TestMethod]
    public async Task ExportTemplateReturnsXliff()
    {
        var export = await new TemplateActions(Context, Files).ExportTemplate(TemplateExport());
        Assert.IsTrue(Files.SizeOf(export.File) > 0);
        StringAssert.EndsWith(export.File.Name, ".xlf");
    }

    [TestMethod]
    public async Task CreateTemplateRenderFromXliff()
    {
        RequireWrites();
        var actions = new TemplateActions(Context, Files);
        var input = TemplateExport();
        var export = await actions.ExportTemplate(input);
        var translated = await Files.CopySourcesToTargets(export.File);
        var result = await actions.CreateFromXliff(new()
        {
            TemplateId = input.TemplateId, File = translated, SubmissionKey = SubmissionKey()
        });
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.VideoId));
    }

    private TemplateExportInput TemplateExport() =>
        new() { TemplateId = Fixture("TemplateId"), SourceLanguage = "en", TargetLanguage = "es" };
}
