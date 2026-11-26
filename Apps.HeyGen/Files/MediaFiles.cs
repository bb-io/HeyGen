using Apps.HeyGen.Api;
using Apps.HeyGen.Models.Dtos;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.HeyGen.Files;

internal static class MediaFiles
{
    private static readonly HttpClient Downloads = new(new HttpClientHandler { AllowAutoRedirect = false })
    {
        Timeout = TimeSpan.FromMinutes(3)
    };

    private static readonly Dictionary<string, string> FileTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".mp4"] = "video/mp4",
        [".webm"] = "video/webm",
        [".mp3"] = "audio/mpeg",
        [".wav"] = "audio/wav",
        [".png"] = "image/png",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".pdf"] = "application/pdf"
    };

    public static void Validate(FileReference file, string? kind)
    {
        var extension = Path.GetExtension(file.Name ?? "");
        if (!FileTypes.TryGetValue(extension, out var mime) ||
            (kind is not null && !mime.StartsWith(kind + "/", StringComparison.Ordinal)))
            throw new PluginMisconfigurationException("Select a supported HeyGen file with its original filename extension.");

        var limit = mime.StartsWith("image/", StringComparison.Ordinal) ? 32_000_000 : 50_000_000;
        if (file.Size > limit)
            throw new PluginMisconfigurationException($"The file exceeds the {limit / 1_000_000} MB upload limit. Use a HeyGen asset ID or a downloadable URL where available.");
    }

    public static string AssetBatchMime(FileReference file)
    {
        if (!FileTypes.TryGetValue(Path.GetExtension(file.Name ?? ""), out var mime))
            throw new PluginMisconfigurationException("Use MP4, WebM, MP3, WAV, PNG, JPEG or PDF files in the asset batch.");

        return mime;
    }

    public static async Task<long> AssetBatchSize(IFileManagementClient files, FileReference file)
    {
        const long limit = 209_715_200;
        var size = file.Size;
        if (size <= 0)
        {
            await using var source = await files.DownloadAsync(file);
            if (source.CanSeek)
            {
                size = source.Length - source.Position;
            }
            else
            {
                var buffer = new byte[64 * 1024];
                while (size <= limit)
                {
                    var count = await source.ReadAsync(buffer.AsMemory());
                    if (count == 0)
                        break;
                    size += count;
                }
            }
        }

        if (size <= 0 || size > limit)
            throw new PluginMisconfigurationException("Select a non-empty file of at most 200 MiB.");
        return size;
    }

    public static async Task<Asset> Upload(HeyGenClient client, IFileManagementClient files, FileReference file, string? key = null)
    {
        Validate(file, null);
        var filename = Path.GetFileName(file.Name);
        var mime = FileTypes[Path.GetExtension(filename)];
        var limit = mime.StartsWith("image/", StringComparison.Ordinal) ? 32_000_000 : 50_000_000;
        var bytes = await ReadBytes(files, file, limit);
        var request = new RestRequest("assets", Method.Post).AddFile("file", bytes, filename, mime);
        if (key is not null)
            request.AddHeader("Idempotency-Key", key);
        return await client.Data<Asset>(request);
    }

    public static async Task<byte[]> ReadBytes(IFileManagementClient files, FileReference file, int limit)
    {
        if (file is null)
            throw new PluginMisconfigurationException("Select a file.");
        if (file.Size > limit)
            throw new PluginMisconfigurationException($"The file is larger than {limit / 1_000_000} MB.");

        await using var source = await files.DownloadAsync(file);
        if (file.Size > 0)
        {
            var exact = new byte[(int)file.Size];
            try
            {
                await source.ReadExactlyAsync(exact);
            }
            catch (EndOfStreamException)
            {
                throw new PluginMisconfigurationException("The file is shorter than its recorded size. Select it again.");
            }

            if (source.ReadByte() != -1)
                throw new PluginMisconfigurationException("The file is larger than its recorded size. Select it again.");

            return exact;
        }

        using var buffer = new MemoryStream();
        var chunk = new byte[64 * 1024];
        while (true)
        {
            var count = await source.ReadAsync(chunk.AsMemory());
            if (count == 0)
                break;
            if (buffer.Length + count > limit)
                throw new PluginMisconfigurationException($"The file is larger than {limit / 1_000_000} MB.");
            await buffer.WriteAsync(chunk.AsMemory(0, count));
        }

        if (buffer.Length == 0)
            throw new PluginMisconfigurationException("Select a non-empty file.");

        return buffer.ToArray();
    }

    public static string PublicUrl(string value)
    {
        if (!Uri.TryCreate(value.Trim(), UriKind.Absolute, out var uri)
            || uri.Scheme is not ("https" or "http") || uri.IsLoopback || !string.IsNullOrEmpty(uri.UserInfo))
            throw new PluginMisconfigurationException("Enter a public HTTP or HTTPS media URL that HeyGen can download.");
        return uri.AbsoluteUri;
    }

    public static Task<FileOutput> Download(IFileManagementClient files, string? url,
        string name, string defaultMime, CancellationToken cancellationToken) =>
        Download(Downloads, files, url, name, defaultMime, cancellationToken);

    internal static async Task<FileOutput> Download(HttpClient httpClient, IFileManagementClient files, string? url,
        string name, string defaultMime, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new PluginMisconfigurationException("HeyGen has not provided this file. Check the job status and whether this artifact exists.");

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != "https"
            || uri.IsLoopback || !string.IsNullOrEmpty(uri.UserInfo))
            throw new PluginApplicationException("HeyGen returned an invalid download URL.");

        try
        {
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            deadline.CancelAfter(TimeSpan.FromMinutes(5));
            using var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, deadline.Token);
            if (!response.IsSuccessStatusCode)
                throw new PluginApplicationException("Could not download the HeyGen file. Check job status, then retry to refresh its URL.");

            var mime = response.Content.Headers.ContentType?.MediaType;
            if (mime == "video/webm" || (defaultMime == "video/mp4" && uri.AbsolutePath.EndsWith(".webm", StringComparison.OrdinalIgnoreCase)))
            {
                name = Path.ChangeExtension(name, ".webm");
                defaultMime = "video/webm";
            }
            await using var file = OpenTemporaryDownload();
            await response.Content.CopyToAsync(file, deadline.Token);
            file.Position = 0;
            deadline.Token.ThrowIfCancellationRequested();

            return new FileOutput { File = await files.UploadAsync(file, defaultMime, name) };
        }
        catch (UnauthorizedAccessException)
        {
            throw new PluginApplicationException("Blackbird could not access storage for the HeyGen download.");
        }
        catch (Exception exception) when (exception is HttpRequestException or IOException or OperationCanceledException)
        {
            throw new PluginApplicationException("The HeyGen download was interrupted. Run it again to refresh the download URL.");
        }
    }

    private static FileStream OpenTemporaryDownload()
    {
        var options = new FileStreamOptions
        {
            Mode = FileMode.CreateNew,
            Access = FileAccess.ReadWrite,
            Share = FileShare.None,
            Options = FileOptions.Asynchronous | FileOptions.SequentialScan | FileOptions.DeleteOnClose
        };

        if (!OperatingSystem.IsWindows())
            options.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;

        return new FileStream(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), options);
    }
}
