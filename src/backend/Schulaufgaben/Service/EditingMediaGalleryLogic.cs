// MIT - Florian Grimm

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.Threading.Channels;

namespace Brimborium.Schulaufgaben.Service;

public class EditingMediaGalleryOptions {
    public string? ThumbnailFolder { get; set; }
    public List<EditingMediaGallery> ListMediaGallery { get; set; } = [];
}
public sealed record EditingMediaGallery(
    string FolderPath
    );

/// <summary>
/// Media Info
/// </summary>
/// <param name="MediaGalleryId"></param>
/// <param name="FolderPath"></param>
/// <param name="ListMediaInfo"></param>
public sealed record MediaGallery(
    string MediaGalleryId,
    string FolderPath
    //List<SAMediaInfo> ListImage,
    //List<SAMediaInfo> ListAudio,
    //List<SAMediaInfo> ListVideo
    );

public enum MediaType { Unkown, Image, Audio, Video }

public sealed record QueueThumbnailItem(
    MediaGallery MediaGallery,
    MediaType MediaType,
    SAMediaInfo MediaInfo,
    TaskCompletionSource<DateTime>? Done = default);

public sealed class EditingMediaGalleryLogic {
    private readonly RepositoryEditingMediaGallery _RepositoryEditingMediaGallery;
    private readonly Channel<QueueThumbnailItem> _ThumbnailQueue;
    private Dictionary<string, MediaGallery> _DictMediaGallery;
    private string? _ThumbnailFolder;
    private EditingLucanetCache? _LucanetCache;
    private DateTime _LastScan = DateTime.UnixEpoch;

    public EditingMediaGalleryLogic(
        RepositoryEditingMediaGallery repositoryEditingMediaGallery,
        IOptionsMonitor<EditingMediaGalleryOptions> options
        ) {
        this._DictMediaGallery = new();
        this._RepositoryEditingMediaGallery = repositoryEditingMediaGallery;
        this._ThumbnailQueue = Channel.CreateBounded<QueueThumbnailItem>(new BoundedChannelOptions(2000));
        options.OnChange(this.OnOptionsChange);
        this.OnOptionsChange(options.CurrentValue);
    }

    private void OnOptionsChange(EditingMediaGalleryOptions options) {
        this._ThumbnailFolder = options.ThumbnailFolder;
        Dictionary<string, MediaGallery> nextDictMediaGallery = new();
        foreach (var sourceMediaGallery in options.ListMediaGallery) {
            // the sourceMediaGallery.FolderPath as md5
            string folderPath = sourceMediaGallery.FolderPath;
            if (string.IsNullOrEmpty(folderPath)) { continue; }
            string mediaGalleryId = "Gallery" + FilePersistenceUtility.GetMD5(folderPath);
            MediaGallery mediaGallery = new(
                mediaGalleryId,
                folderPath
                );
            nextDictMediaGallery.Add(mediaGalleryId, mediaGallery);
        }
        this._DictMediaGallery = nextDictMediaGallery;
        if (this._ThumbnailFolder is { Length: > 0 } thumbnailFolder) {
            if (this._LucanetCache is null) {
                this._LucanetCache = new EditingLucanetCache(thumbnailFolder);
            }
        }
    }

    public async Task InitialScanAsync(CancellationToken stoppingToken) {
        if (this._LucanetCache is not { } lucanetCache) {
            return;
        }

        bool failed = false;
        DateTime lastScan = DateTime.Now;
        this._LastScan = lastScan;

        foreach (var mediaGallery in this._DictMediaGallery.Values) {
            try {
                await this.ScanMediaGallertyAsync(mediaGallery, lastScan, stoppingToken);
            } catch {
                // TODO handle Errors
                failed = true;
            }
        }
        // remove old entries
        if (failed) {
            // skip
        } else {
            var isEmpty = new TaskCompletionSource<DateTime>();
            await this._ThumbnailQueue.Writer.WriteAsync(
                new QueueThumbnailItem(new MediaGallery("", ""), MediaType.Unkown, new SAMediaInfo(), isEmpty));
            await isEmpty.Task;
            lucanetCache.Commit();
            //lucanetCache.Cleanup(lastScan);
        }
    }

    public void WireFolderWatch(CancellationToken stoppingToken) {
        // TODO
    }

    public async Task StartThumbnailQueueAsync(CancellationToken stoppingToken) {
        var reader = this._ThumbnailQueue.Reader;
        while (!stoppingToken.IsCancellationRequested) {
            if (reader.TryRead(out var queueItem)) {
                if (queueItem.Done is { } done) {
                    done.TrySetResult(DateTime.Now);
                    continue;
                }
                if (this._LucanetCache is { } lucanetCache) {
                    // var thumbnail = await this.GenerateThumbnailAsync(queueItem);
                    lucanetCache.Write(
                        queueItem.MediaGallery.MediaGalleryId,
                        FilePersistenceUtility.ConvertMediaTypeToString(queueItem.MediaType),
                        queueItem.MediaInfo.Path,
                        queueItem.MediaInfo.LastWriteTimeUtc,
                        queueItem.MediaInfo.Size,
                        queueItem.MediaInfo.LastScan
                        );
                }
                continue;
            }
            await reader.WaitToReadAsync(stoppingToken);
        }
    }

    private async Task ScanMediaGallertyAsync(MediaGallery mediaGallery, DateTime lastScan, CancellationToken stoppingToken) {
        System.IO.DirectoryInfo directoryInfo = new(mediaGallery.FolderPath);
        var listFiles = directoryInfo.EnumerateFiles("*.*", new EnumerationOptions() {
            IgnoreInaccessible = true,
            RecurseSubdirectories = true,
            ReturnSpecialDirectories = false
        });
        int startIndex = mediaGallery.FolderPath.Length + 1;
        ChannelWriter<QueueThumbnailItem>? writer = null;

        foreach (var fileInfo in listFiles) {
            var mediaType = FilePersistenceUtility.ConvertExtensionToMediaType(fileInfo.Extension);
            if (mediaType == MediaType.Unkown) { continue; }
            var fullName = fileInfo.FullName;
            var relativePath = fullName.Substring(startIndex);
            var relativePathNormalized = relativePath.Replace('\\', '/');
            SAMediaInfo mediaInfo = new SAMediaInfo() {
                Path = relativePathNormalized,
                Size = fileInfo.Length,
                LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
                LastScan = lastScan
            };
            writer ??= this._ThumbnailQueue.Writer;
            while (!writer.TryWrite(new QueueThumbnailItem(mediaGallery, mediaType, mediaInfo))) {
                await writer.WaitToWriteAsync(stoppingToken);
            }
        }
    }

    private static readonly Size _SizeThumbnail = new Size(64, 64);
    private static PngEncoder? _Endoder;
    private static readonly char[] _ArraySlash = new char[] { '/' };


#if false
    private async Task<byte[]?> GenerateThumbnailAsync(QueueThumbnailItem queueItem) {
        if (queueItem.MediaType == MediaType.Image) {
            var fullFQN = System.IO.Path.Combine(
                queueItem.MediaGallery.FolderPath,
                queueItem.MediaInfo.Path.TrimStart('/'));

            using (var targetStream = RecyclableMemory.Instance.GetStream()) {
                using (var sourceStream = RecyclableMemory.Instance.GetStream()) {
                    using (var sourceFile = System.IO.File.OpenRead(fullFQN)) {
                        await sourceFile.CopyToAsync(sourceStream).ConfigureAwait(false);
                        sourceStream.Position = 0;

                        Image? image = null;
                        try {
                            image = Image.Load(sourceStream);
                        } catch {
                            return null;
                        }
                        if (image == null) { return null; }

                        // TODO handle image.Metadata.

                        var size = image.Size;
                        if (size.Width < _SizeThumbnail.Width && size.Height < _SizeThumbnail.Height) {
                        } else {
                            image.Mutate(ctxt => ctxt.Resize(
                                new ResizeOptions {
                                    Mode = ResizeMode.Crop,
                                    Size = new Size(128, 128)
                                }));
                        }
                        {
                            var encoder = _Endoder ??= new SixLabors.ImageSharp.Formats.Png.PngEncoder();
                            image.Save(targetStream, encoder);
                            targetStream.Position = 0;
                            return targetStream.GetBuffer();
                        }
                    }
                }
            }
        }
        return null;
    }
#endif

    public async Task<List<SAMediaInfo>> SearchAsync(
        SAMediaSearchRequest searchRequest,
        CancellationToken requestAborted) {
#if false
        if (this._LucanetCache is { } lucanetCache) {
            return lucanetCache.Search(searchRequest);
        } else {
#endif
        var searchAll = string.IsNullOrEmpty(searchRequest.Value);
        var listSearchFor = searchRequest.Value.Split('\t', ' ');
        List<SAMediaInfo> result = [];
        foreach (var mediaGallery in this._DictMediaGallery.Values) {
            System.IO.DirectoryInfo directoryInfo = new(mediaGallery.FolderPath);
            var listFiles = directoryInfo.EnumerateFiles("*.*", new EnumerationOptions() {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true,
                ReturnSpecialDirectories = false
            });
            int startIndex = mediaGallery.FolderPath.Length + 1;
            foreach (var fileInfo in listFiles) {
                var mediaType = FilePersistenceUtility.ConvertExtensionToMediaType(fileInfo.Extension);
                if (mediaType == MediaType.Unkown) { continue; }

                var fullName = fileInfo.FullName;
                var relativePath = fullName.Substring(startIndex);
                var relativePathNormalized = relativePath.Replace('\\', '/');
                if (searchAll) {
                    // OK
                } else {
                    bool found = true;
                    foreach (var searchFor in listSearchFor) {
                        if (relativePathNormalized.Contains(
                            searchFor,
                            StringComparison.OrdinalIgnoreCase)) {
                            // OK
                        } else {
                            found = false;
                            break;
                        }
                    }
                    if (!found) { continue; }
                }
                SAMediaInfo mediaInfo = new SAMediaInfo() {
                    Path = $"{mediaGallery.MediaGalleryId}/{relativePathNormalized}",
                    Size = fileInfo.Length,
                    LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
                    MediaType = FilePersistenceUtility.ConvertMediaTypeToString(mediaType)
                };
                result.Add(mediaInfo);
                if (100 < result.Count) {
                    break;
                }
            }
        }

        return result;
        //  }
    }

    public async Task<SAMediaGetResult?> GetMediaContentAsync(string name, CancellationToken requestAborted) {
        // The name is the relative path, format {MediaGalleryId}/{relativePath}.
        // The name must be a file.
        // The name cannot contain "..".
        // The name must be in the list of media galleries.
        var parts = name.Split(_ArraySlash, 2);
        if (parts.Length != 2) { return null; }
        string mediaGalleryId = parts[0];
        string relativeName = parts[1];
        return await this.GetMediaContentAsync(mediaGalleryId, relativeName, requestAborted);
    }

    public async Task<SAMediaGetResult?> GetMediaContentAsync(string mediaGalleryId, string relativeName, CancellationToken requestAborted) {
        if (string.IsNullOrEmpty(mediaGalleryId)) { return null; }
        if (string.IsNullOrEmpty(relativeName)) { return null; }
        if (relativeName.Contains("..")) { return null; }
        if (!this._DictMediaGallery.TryGetValue(mediaGalleryId, out var mediaGallery)) {
            return null;
        }
        var fullFQN = System.IO.Path.GetFullPath(
            System.IO.Path.Combine(mediaGallery.FolderPath, relativeName));
        if (!fullFQN.StartsWith(mediaGallery.FolderPath)) { return null; }
        System.IO.FileInfo fi = new(fullFQN);
        if (!fi.Exists) { return null; }
        string contentType = FilePersistenceUtility.ConvertExtensionToContentType(fi.Extension);
        if (string.IsNullOrEmpty(contentType)) { return null; }
        return new SAMediaGetResult(fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read), contentType);
    }

    public async Task<SAMediaGetResult?> GetMediaThumbnailAsync(string name, CancellationToken requestAborted) {
        var parts = name.Split(_ArraySlash, 3);
        if (parts.Length != 2) { return null; }
        string mediaGalleryId = parts[0];
        string relativeName = parts[1];
        return await this.GetMediaThumbnailAsync(mediaGalleryId, relativeName, requestAborted);
    }

    public async Task<SAMediaGetResult?> GetMediaThumbnailAsync(string mediaGalleryId, string relativeName, CancellationToken requestAborted) {
        if (string.IsNullOrEmpty(mediaGalleryId)) { return null; }
        if (string.IsNullOrEmpty(relativeName)) { return null; }
        if (relativeName.Contains("..")) { return null; }
        if (!this._DictMediaGallery.TryGetValue(mediaGalleryId, out var mediaGallery)) {
            return null;
        }

        var fullFQN = System.IO.Path.Combine(
            mediaGallery.FolderPath,
            relativeName.TrimStart('/'));


        Image? image = null;
        try {
            image = await Image.LoadAsync(fullFQN).ConfigureAwait(false);
        } catch {
            return null;
        }
        if (image == null) { return null; }

        // TODO handle image.Metadata.

        var size = image.Size;
        if (size.Width < _SizeThumbnail.Width && size.Height < _SizeThumbnail.Height) {
        } else {
            image.Mutate(ctxt => ctxt.Resize(
                new ResizeOptions {
                    Mode = ResizeMode.Crop,
                    Size = new Size(128, 128)
                }));
        }
        {
            var targetStream = RecyclableMemory.Instance.GetStream();
            var encoder = _Endoder ??= new SixLabors.ImageSharp.Formats.Png.PngEncoder();
            image.Save(targetStream, encoder);
            targetStream.Position = 0;
            return new SAMediaGetResult(targetStream, "image/png");
        }
    }
}

public sealed record SAMediaSearchRequest(
    MediaType MediaType,
    string Value);

public sealed record SAMediaGetResult(
    Stream Stream, string ContentType);
