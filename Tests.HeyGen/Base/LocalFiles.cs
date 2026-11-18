using System.Text;
using System.Xml.Linq;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Tests.HeyGen.Base;

public class LocalFiles : IFileManagementClient, IDisposable
{
    private readonly string directory = Directory.CreateTempSubdirectory("heygen-test-").FullName;
    private readonly Dictionary<string, string> paths = new(StringComparer.Ordinal);

    public FileReference FromPath(string path)
    {
        var info = new FileInfo(path);
        paths[info.Name] = path;
        return new FileReference { Name = info.Name };
    }

    public long SizeOf(FileReference reference) => new FileInfo(paths[reference.Name]).Length;

    public Task<Stream> DownloadAsync(FileReference reference) =>
        Task.FromResult<Stream>(File.OpenRead(paths[reference.Name]));

    public async Task<FileReference> UploadAsync(Stream stream, string contentType, string fileName)
    {
        var safeName = Path.GetFileName(fileName);
        await using (var destination = File.Create(Path.Combine(directory, safeName)))
            await stream.CopyToAsync(destination);

        var reference = FromPath(Path.Combine(directory, safeName));
        reference.ContentType = contentType;
        return reference;
    }

    public async Task<FileReference> CopySourcesToTargets(FileReference reference)
    {
        await using var stream = await DownloadAsync(reference);
        var document = await XDocument.LoadAsync(stream, LoadOptions.PreserveWhitespace, CancellationToken.None);
        XNamespace ns = document.Root!.Name.Namespace;
        foreach (var unit in document.Descendants(ns + "trans-unit"))
            unit.Element(ns + "target")!.Value = unit.Element(ns + "source")!.Value;

        await using var translated = new MemoryStream(Encoding.UTF8.GetBytes(document.ToString(SaveOptions.DisableFormatting)));
        return await UploadAsync(translated, "application/x-xliff+xml", "translated.xlf");
    }

    public void Dispose() => Directory.Delete(directory, recursive: true);
}
