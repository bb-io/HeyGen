using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.HeyGen.Files;

internal static class XliffScript
{
    private static readonly XNamespace Ns = "urn:oasis:names:tc:xliff:document:1.2";
    private static readonly Regex Language = new(@"\A[A-Za-z]{2,8}(?:-[A-Za-z0-9]{1,8})*\z", RegexOptions.CultureInvariant);
    private static readonly Regex ProtectedText = new(@"</?[A-Za-z][^<>]*>|\{\{[^{}]+\}\}", RegexOptions.CultureInvariant);

    public static MemoryStream Write(string original, string sourceLanguage, string targetLanguage,
        IReadOnlyDictionary<string, string> content)
    {
        var sourceTag = LanguageTag(sourceLanguage);
        var targetTag = LanguageTag(targetLanguage);
        if (content.Count == 0)
            throw new PluginMisconfigurationException("HeyGen returned no editable text to export.");

        var body = new XElement(Ns + "body", content.Select(entry =>
            new XElement(Ns + "trans-unit",
                new XAttribute("id", entry.Key),
                new XAttribute(XNamespace.Xml + "space", "preserve"),
                new XElement(Ns + "source", entry.Value),
                new XElement(Ns + "target", new XAttribute("state", "needs-translation")))));
        var file = new XElement(Ns + "file",
            new XAttribute("original", original),
            new XAttribute("source-language", sourceTag),
            new XAttribute("target-language", targetTag),
            new XAttribute("datatype", "plaintext"),
            body);
        var document = new XDocument(new XElement(Ns + "xliff", new XAttribute("version", "1.2"), file));

        var stream = new MemoryStream();
        try
        {
            using var writer = XmlWriter.Create(stream, new XmlWriterSettings { NewLineHandling = NewLineHandling.Entitize });
            document.Save(writer);
        }
        catch (ArgumentException)
        {
            stream.Dispose();
            throw new PluginMisconfigurationException("The source contains characters that XLIFF cannot represent.");
        }

        stream.Position = 0;
        return stream;
    }

    public static TranslatedXliff Read(byte[] bytes)
    {
        XDocument document;
        try
        {
            using var stream = new MemoryStream(bytes);
            using var reader = XmlReader.Create(stream, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersInDocument = 4_000_000
            });
            document = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
        }
        catch (XmlException)
        {
            throw new PluginMisconfigurationException("Upload a valid HeyGen XLIFF 1.2 file.");
        }

        var root = document.Root;
        if (root?.Name != Ns + "xliff" || (string?)root.Attribute("version") != "1.2")
            throw new PluginMisconfigurationException("Use XLIFF 1.2 and keep the format of the exported HeyGen file.");
        var files = root.Elements(Ns + "file").ToList();
        if (files.Count != 1)
            throw new PluginMisconfigurationException("Use the exported HeyGen file with a single file element.");

        var file = files[0];
        var original = (string?)file.Attribute("original");
        if (string.IsNullOrWhiteSpace(original))
            throw new PluginMisconfigurationException("Keep the original reference from the exported HeyGen XLIFF file.");

        var targetLanguage = LanguageTag((string?)file.Attribute("target-language"));
        var entries = new Dictionary<string, TranslatedUnit>(StringComparer.Ordinal);
        foreach (var unit in file.Descendants(Ns + "trans-unit"))
        {
            var id = (string?)unit.Attribute("id");
            var source = unit.Element(Ns + "source");
            var target = unit.Element(Ns + "target");
            if (string.IsNullOrWhiteSpace(id) || source is null || target is null ||
                source.HasElements || target.HasElements || string.IsNullOrWhiteSpace(target.Value))
                throw new PluginMisconfigurationException("Keep every unit ID and source. Enter a non-empty plain-text target for each unit.");

            if (!ProtectedText.Matches(source.Value).Select(x => x.Value)
                .SequenceEqual(ProtectedText.Matches(target.Value).Select(x => x.Value), StringComparer.Ordinal))
                throw new PluginMisconfigurationException($"Keep the original placeholders and markup in XLIFF unit {id}.");

            if (!entries.TryAdd(id, new TranslatedUnit(source.Value, target.Value)))
                throw new PluginMisconfigurationException("Keep XLIFF unit IDs unique.");
        }

        if (entries.Count == 0)
            throw new PluginMisconfigurationException("The XLIFF file needs at least one translated unit.");
        return new TranslatedXliff(original, targetLanguage, entries);
    }

    private static string LanguageTag(string? value)
    {
        var tag = value?.Trim() ?? "";
        if (!Language.IsMatch(tag))
            throw new PluginMisconfigurationException("Enter a language tag such as en, hu or pt-BR.");
        return tag;
    }
}
