// MIT - Florian Grimm

using Brimborium.Schulaufgaben.Data;
using Brimborium.Schulaufgaben.Model;
using Microsoft.IO;
using System.Diagnostics;

namespace Brimborium.Schulaufgaben.Service;

/// <summary>
/// stores 
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class EditingMediaCache {
    private readonly string _FileFQN;
    private MediaCacheContext _MediaCacheContext;

    public EditingMediaCache(string fileFQN) {
        this._FileFQN = fileFQN;
        this._MediaCacheContext = new MediaCacheContext(fileFQN);
        // Ensure database is created
        this._MediaCacheContext.EnsureCreated();
        try {
            using (var connection = this._MediaCacheContext.GetConnection()) {
                var sqlStatement = this._MediaCacheContext.CreateSelectCommandText("", 1, false);
                using (var selectCommand = connection.CreateCommand()) {
                    selectCommand.CommandText = sqlStatement;
                    this._MediaCacheContext.Query(selectCommand, false);
                }
            }
        } catch {
            this._MediaCacheContext.EnsureDeleted();
            this._MediaCacheContext.EnsureCreated();
        }
    }

    public DateTime GetLastWriteTimeUtc(string mediaGalleryId, string path)
        => this._MediaCacheContext.GetLastWriteTimeUtc(mediaGalleryId, path);

    public void Write(
        string mediaGalleryId,
        string mediaType,
        string path,
        DateTime lastWriteTimeUtc,
        long size,
        DateTime lastScan,
        RecyclableMemoryStream? thumbnail
        ) {
        // Extract folder and name from path

        var lastSlashIndex = path.LastIndexOf('/');
        var folder = lastSlashIndex > 0 ? path.Substring(0, lastSlashIndex) : string.Empty;
        var name = lastSlashIndex >= 0 ? path.Substring(lastSlashIndex + 1) : path;
        using (this._MediaCacheContext.Lock.EnterScope()) {
            using (var connection = this._MediaCacheContext.GetConnection()) {
                // Check if file already exists
                var dbFile = new DbFile {
                    MediaGalleryId = mediaGalleryId,
                    MediaType = mediaType,
                    Folder = folder,
                    Path = path,
                    Name = name,
                    LastWriteTimeUtc = lastWriteTimeUtc,
                    Size = size,
                    LastScan = lastScan,
                    Thumbnail = thumbnail,
                };
                this._MediaCacheContext.Upsert(dbFile, connection);
            }
        }
    }

    public void Cleanup(DateTime lastScan) {
        if (this._MediaCacheContext is { } mediaCacheContext) {
            mediaCacheContext.Cleanup(lastScan);
        }
    }

    public void Delete(string mediaGalleryId, string path) {
        if (this._MediaCacheContext is { } mediaCacheContext) {
            mediaCacheContext.Delete(mediaGalleryId, path);
        }
    }

    public List<SAMediaInfo> Search(SAMediaSearchRequest searchRequest, Func<string, string, bool> checkIfExits) {
        List<DbFile> resultsDB = this._MediaCacheContext.Search(searchRequest, checkIfExits);
        var results = resultsDB
            .Where(f => checkIfExits(f.MediaGalleryId, f.Path))
            .Select(f => new SAMediaInfo {
                Path = FilePersistenceUtility.CombineMediaGalleryIdAndPathIntoPath(f.MediaGalleryId, f.Path),
                MediaType = f.MediaType,
                Size = f.Size,
                LastWriteTimeUtc = f.LastWriteTimeUtc,
                LastScan = f.LastScan
            })
            .ToList();

        return results;
    }

    private string GetDebuggerDisplay() => this._FileFQN;

    public (bool found, RecyclableMemoryStream? thumbnail) GetThumbnail(string mediaGalleryId, string path)
        => this._MediaCacheContext.GetThumbnail(mediaGalleryId, path);
}
