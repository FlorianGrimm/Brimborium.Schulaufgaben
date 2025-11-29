// MIT - Florian Grimm

using Microsoft.IO;
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
    );

public enum MediaType { Unkown, Image, Audio, Video }

public sealed record QueueMediaItem(
    MediaGallery MediaGallery,
    MediaType MediaType,
    SAMediaInfo MediaInfo,
    RecyclableMemoryStream? thumbnail,
    TaskCompletionSource<DateTime>? Done);

public sealed class EditingMediaGalleryLogic {
    private readonly IServiceProvider _ServiceProvider;
    private readonly RepositoryEditingMediaGallery _RepositoryEditingMediaGallery;
    private readonly Channel<QueueMediaItem> _MediaQueue;
    private Dictionary<string, MediaGallery> _DictMediaGallery;
    private string? _ThumbnailFolder;
    private EditingMediaCache? _EditingMediaCache;
    private DateTime _LastScan = DateTime.UnixEpoch;
    private List<FileSystemWatcher> _FileSystemWatchers = new();

    public EditingMediaGalleryLogic(
        IServiceProvider serviceProvider,
        RepositoryEditingMediaGallery repositoryEditingMediaGallery,
        IOptionsMonitor<EditingMediaGalleryOptions> options
        ) {
        this._DictMediaGallery = new();
        this._ServiceProvider = serviceProvider;
        this._RepositoryEditingMediaGallery = repositoryEditingMediaGallery;
        this._MediaQueue = Channel.CreateBounded<QueueMediaItem>(new BoundedChannelOptions(2000));
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
#if true
            if (this._EditingMediaCache is null) {
                var fileFQN = System.IO.Path.Combine(thumbnailFolder, "mediaCache.db");
                this._EditingMediaCache = new EditingMediaCache(fileFQN);
            }
#endif
        }
    }

    public async Task InitialScanAsync(CancellationToken stoppingToken) {
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
            await this._MediaQueue.Writer.WriteAsync(
                new QueueMediaItem(new MediaGallery("", ""), MediaType.Unkown, new SAMediaInfo(), null, isEmpty));
            await isEmpty.Task;
            if (this._EditingMediaCache is { } editingMediaCache) {
                editingMediaCache.Cleanup(lastScan);
            }
        }
    }

    public void WireFolderWatch(CancellationToken stoppingToken) {
        // Dispose existing watchers
        foreach (var watcher in this._FileSystemWatchers) {
            watcher.Dispose();
        }
        this._FileSystemWatchers.Clear();

        // Create new watchers for each media gallery
        foreach (var mediaGallery in this._DictMediaGallery.Values) {
            if (!Directory.Exists(mediaGallery.FolderPath)) {
                continue;
            }

            var watcher = new FileSystemWatcher(mediaGallery.FolderPath) {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
            };

            // Handle file created or changed - queue to _MediaQueue
            watcher.Created += (sender, e) => OnFileCreatedOrChanged(mediaGallery, e.FullPath);
            watcher.Changed += (sender, e) => OnFileCreatedOrChanged(mediaGallery, e.FullPath);

            // Handle file deleted - directly remove from database
            watcher.Deleted += (sender, e) => OnFileDeleted(mediaGallery, e.FullPath);

            // Handle file renamed
            watcher.Renamed += (sender, e) => {
                OnFileDeleted(mediaGallery, e.OldFullPath);
                OnFileCreatedOrChanged(mediaGallery, e.FullPath);
            };

            watcher.EnableRaisingEvents = true;
            this._FileSystemWatchers.Add(watcher);
        }
    }

    private async void OnFileCreatedOrChanged(MediaGallery mediaGallery, string fullPath) {
        try {
            var fileInfo = new FileInfo(fullPath);
            if (!fileInfo.Exists) { return; }
            await this.AddListMediaInfo(
                mediaGallery,
                this._LastScan,
                [fileInfo],
                CancellationToken.None).ConfigureAwait(false);
        } catch {
            // Log error but don't crash the watcher
            //Console.WriteLine($"Error processing file {fullPath}: {ex.Message}");
        }
    }

    private void OnFileDeleted(MediaGallery mediaGallery, string fullPath) {
        try {
            if (this._EditingMediaCache is not { } editingMediaCache) {
                return;
            }

            // Get relative path
            var relativePath = Path.GetRelativePath(mediaGallery.FolderPath, fullPath)
                .Replace('\\', '/');

            // Directly delete from database
            this._EditingMediaCache.Delete(mediaGallery.MediaGalleryId, relativePath);
        } catch {
            // Log error but don't crash the watcher
            //Console.WriteLine($"Error deleting file {fullPath}: {ex.Message}");
        }
    }

    public async Task StartThumbnailQueueAsync(CancellationToken stoppingToken) {
        var reader = this._MediaQueue.Reader;
        while (!stoppingToken.IsCancellationRequested) {
            if (reader.TryRead(out var queueItem)) {
                if (queueItem.Done is { } done) {
                    done.TrySetResult(DateTime.Now);
                    continue;
                }

                if (this._EditingMediaCache is { } editingMediaCache) {
                    var l = editingMediaCache.GetLastWriteTimeUtc(
                        queueItem.MediaGallery.MediaGalleryId,
                        queueItem.MediaInfo.Path);
                    if (queueItem.MediaInfo.LastWriteTimeUtc == l) {
                        continue;
                    }

                    using (var thumbnail = (queueItem.thumbnail) ?? (await this.GenerateThumbnailAsync(queueItem))) {
                        editingMediaCache.Write(
                            queueItem.MediaGallery.MediaGalleryId,
                            FilePersistenceUtility.ConvertMediaTypeToString(queueItem.MediaType),
                            queueItem.MediaInfo.Path,
                            queueItem.MediaInfo.LastWriteTimeUtc,
                            queueItem.MediaInfo.Size,
                            queueItem.MediaInfo.LastScan,
                            thumbnail
                            );
                    }
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
        await this.AddListMediaInfo(
            mediaGallery,
            lastScan,
            listFiles,
            stoppingToken);
    }

    private async Task AddListMediaInfo(MediaGallery mediaGallery, DateTime lastScan, IEnumerable<FileInfo> listFiles, CancellationToken stoppingToken) {
        int startIndex = mediaGallery.FolderPath.Length + 1;
        ChannelWriter<QueueMediaItem>? writer = null;
        foreach (var fileInfo in listFiles) {
            var (mediaType, relativePath) = this.GetMediaTypeRelativePath(mediaGallery, fileInfo);
            if (mediaType == MediaType.Unkown) { continue; }
            SAMediaInfo mediaInfo = new SAMediaInfo() {
                Path = relativePath,
                Size = fileInfo.Length,
                LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
                LastScan = lastScan
            };
            writer ??= this._MediaQueue.Writer;
            while (!writer.TryWrite(new QueueMediaItem(mediaGallery, mediaType, mediaInfo, null, null))) {
                await writer.WaitToWriteAsync(stoppingToken);
            }
        }
    }

    public (MediaType mediaType, string relativePath) GetMediaTypeRelativePath(MediaGallery mediaGallery, FileInfo fileInfo) {
        var mediaType = FilePersistenceUtility.ConvertExtensionToMediaType(fileInfo.Extension);

        var fullName = fileInfo.FullName;
        int startIndex = mediaGallery.FolderPath.Length + 1;
        var relativePath = fullName.Substring(startIndex);
        var relativePathNormalized = relativePath.Replace('\\', '/');

        return (mediaType, relativePathNormalized);
    }
    public string? GetMediaFQN(string mediaGalleryId, string relativeName) {
        if (string.IsNullOrEmpty(mediaGalleryId)) { return null; }
        if (string.IsNullOrEmpty(relativeName)) { return null; }
        if (relativeName.Contains("..")) { return null; }
        if (!this._DictMediaGallery.TryGetValue(mediaGalleryId, out var mediaGallery)) {
            return null;
        }
        var fileFQN = System.IO.Path.GetFullPath(
            System.IO.Path.Combine(mediaGallery.FolderPath, relativeName));
        if (!fileFQN.StartsWith(mediaGallery.FolderPath)) { return null; }

        return fileFQN;
    }

    private static readonly Size _SizeThumbnail = new Size(64, 64);
    private static PngEncoder? _Endoder;
    private static PngEncoder GetPngEncoder() =>
        _Endoder ??= new SixLabors.ImageSharp.Formats.Png.PngEncoder() { CompressionLevel = PngCompressionLevel.Level9 };

    private static readonly char[] _ArraySlash = new char[] { '/' };

    private async Task<RecyclableMemoryStream?> GenerateThumbnailAsync(QueueMediaItem queueItem) {
        if (queueItem.MediaType == MediaType.Image) {
            var mediaFQN = System.IO.Path.Combine(
                queueItem.MediaGallery.FolderPath,
                queueItem.MediaInfo.Path.TrimStart('/'));

            Image? image = null;
            for (int iRetry = 0; iRetry < 5; iRetry++) {
                try {
                    image = await Image.LoadAsync(mediaFQN).ConfigureAwait(false);
                    break; 
                } catch (System.IO.IOException error) when (error.HResult == -2147024864) {
                    await Task.Delay(iRetry * 1000).ConfigureAwait(false);
                    continue;
                } catch {
                    return null;
                }
            }
            if (image == null) { return null; }

            // TODO handle image.Metadata.

            var size = image.Size;
            if (size.Width < _SizeThumbnail.Width && size.Height < _SizeThumbnail.Height) {
            } else {
                image.Mutate(ctxt => ctxt.Resize(
                    new ResizeOptions {
                        Mode = ResizeMode.Crop,
                        Size = _SizeThumbnail
                    }));
            }
            {
                var targetStream = RecyclableMemory.Instance.GetStream();
                var encoder = GetPngEncoder();
                image.Save(targetStream, encoder);
                targetStream.Position = 0;
                return targetStream;
            }
        }
        return null;
    }

    public async Task<List<SAMediaInfo>> SearchAsync(
        SAMediaSearchRequest searchRequest,
        CancellationToken requestAborted) {
        if (this._EditingMediaCache is { } editingMediaCache) {
            var result = editingMediaCache
                .Search(searchRequest,
                this._CheckIfFileExists ??= this.checkIfFileExists);
            return result;
        }
        return await SearchFallbackAsync(searchRequest, requestAborted);
    }

    private Func<string, string, bool>? _CheckIfFileExists;
    private bool checkIfFileExists(string mediaGalleryId, string path) {
        if (!this._DictMediaGallery.TryGetValue(mediaGalleryId, out var mediaGallery)) {
            return false;
        }

        var mediaFQN = System.IO.Path.Combine(
            mediaGallery.FolderPath,
            path.TrimStart('/'));

        if (System.IO.File.Exists(mediaFQN)) {
            return true;
        } else {
            return false;
        }
    }

    public async Task<List<SAMediaInfo>> SearchFallbackAsync(
        SAMediaSearchRequest searchRequest,
        CancellationToken requestAborted) {
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
                var (mediaType, relativePathNormalized) = this.GetMediaTypeRelativePath(mediaGallery, fileInfo);
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
        var (success, mediaGalleryId, relativeName) = FilePersistenceUtility.SplitPathIntoMediaGalleryIdAndPath(name);
        if (!success) { return null; }
        return await this.GetMediaContentAsync(mediaGalleryId, relativeName, requestAborted);
    }

    public async Task<SAMediaGetResult?> GetMediaContentAsync(string mediaGalleryId, string relativeName, CancellationToken requestAborted) {
        if (this.GetMediaFQN(mediaGalleryId, relativeName) is not { } mediaFQN) {
            return null;
        }
        if (new System.IO.FileInfo(mediaFQN) is not { Exists: true } fi) {
            return null;
        }
        string contentType = FilePersistenceUtility.ConvertExtensionToContentType(fi.Extension);
        if (string.IsNullOrEmpty(contentType)) { return null; }
        return new SAMediaGetResult(fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read), contentType);
    }



    public async Task<SAMediaGetResult?> GetMediaThumbnailAsync(string name, CancellationToken requestAborted) {
        var (success, mediaGalleryId, relativeName) = FilePersistenceUtility.SplitPathIntoMediaGalleryIdAndPath(name);
        if (!success) { return null; }
        return await this.GetMediaThumbnailAsync(mediaGalleryId, relativeName, requestAborted);
    }

    public async Task<SAMediaGetResult?> GetMediaThumbnailAsync(string mediaGalleryId, string relativeName, CancellationToken requestAborted) {

        if (string.IsNullOrEmpty(mediaGalleryId)) { return null; }
        if (string.IsNullOrEmpty(relativeName)) { return null; }
        if (relativeName.Contains("..")) { return null; }
        if (!this._DictMediaGallery.TryGetValue(mediaGalleryId, out var mediaGallery)) {
            return null;
        }

        bool found = false;
        RecyclableMemoryStream? thumbnail = null;
        if (this._EditingMediaCache is { } editingMediaCache) {
            (found, thumbnail) = editingMediaCache.GetThumbnail(mediaGalleryId, relativeName);
            if (found) {
                if (thumbnail is { }) {
                    return new(thumbnail, "image/png");
                }
            }
        }

        var mediaFQN = System.IO.Path.Combine(
            mediaGallery.FolderPath,
            relativeName.TrimStart('/'));
        if (new System.IO.FileInfo(mediaFQN) is not { Exists: true } fileInfo) {
            return null;
        }

        Image? image = null;
        try {
            image = await Image.LoadAsync(mediaFQN, requestAborted).ConfigureAwait(false);
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
                    Size = _SizeThumbnail
                }));
        }
        {
            var targetStream = RecyclableMemory.Instance.GetStream();
            var encoder = GetPngEncoder();
            image.Save(targetStream, encoder);
            targetStream.Position = 0;
            if (found) {
                var (mediaType, relativePath) = this.GetMediaTypeRelativePath(mediaGallery, fileInfo);
                if (mediaType == MediaType.Unkown) {
                } else {
                    SAMediaInfo mediaInfo = new SAMediaInfo() {
                        Path = relativePath,
                        Size = fileInfo.Length,
                        LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
                        LastScan = this._LastScan
                    };
                    var updateThumbnailStream = RecyclableMemory.Instance.GetStream();
                    targetStream.CopyTo(updateThumbnailStream);
                    targetStream.Position = 0;
                    updateThumbnailStream.Position = 0;
                    this._MediaQueue.Writer.TryWrite(
                        new QueueMediaItem(mediaGallery, mediaType, mediaInfo, updateThumbnailStream, null)
                    );
                }

            }
            return new SAMediaGetResult(targetStream, "image/png");
        }
    }
}

public sealed record SAMediaSearchRequest(
    MediaType MediaType,
    string Value);

public sealed record SAMediaGetResult(
    Stream Stream, string ContentType);
