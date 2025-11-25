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
    private string _FileFQN;
    private MediaCacheContext _MediaCacheContext;
    public readonly Lock Lock = new();

    public EditingMediaCache(string fileFQN) {
        this._FileFQN = fileFQN;
        this._MediaCacheContext = new MediaCacheContext(fileFQN);
        // Ensure database is created
        this._MediaCacheContext.Database.EnsureCreated();
        try {
            this._MediaCacheContext.ListFile.FirstOrDefault();
            this._MediaCacheContext.ListFolder.FirstOrDefault();
        } catch {
            this._MediaCacheContext.Database.EnsureDeleted();
            this._MediaCacheContext.Database.EnsureCreated();
        }
    }

    public DateTime GetLastWriteTimeUtc(
        string mediaGalleryId,
        string path) {
        using (this.Lock.EnterScope()) {
            var existingFile = this._MediaCacheContext.ListFile
                .FirstOrDefault(f => f.MediaGalleryId == mediaGalleryId && f.Path == path);
            if (existingFile != null) {
                return existingFile.LastWriteTimeUtc;
            } else {
                return DateTime.UnixEpoch;
            }
        }
    }

    public void Write(
        string mediaGalleryId,
        string mediaType,
        string path,
        DateTime lastWriteTimeUtc,
        long size,
        DateTime lastScan,
        RecyclableMemoryStream? thumbnail) {
        // Extract folder and name from path
        var lastSlashIndex = path.LastIndexOf('/');
        var folder = lastSlashIndex > 0 ? path.Substring(0, lastSlashIndex) : string.Empty;
        var name = lastSlashIndex >= 0 ? path.Substring(lastSlashIndex + 1) : path;

        using (this.Lock.EnterScope()) {
            // Check if file already exists
            var existingFile = this._MediaCacheContext.ListFile
            .FirstOrDefault(f => f.MediaGalleryId == mediaGalleryId && f.Path == path);

            if (existingFile != null) {
                // Update existing file
                existingFile.MediaType = mediaType;
                existingFile.Folder = folder;
                existingFile.Name = name;
                existingFile.LastWriteTimeUtc = lastWriteTimeUtc;
                existingFile.Size = size;
                existingFile.LastScan = lastScan;
                existingFile.Thumbnail = thumbnail?.GetBuffer();
            } else {
                // Add new file
                var newFile = new DbFile {
                    MediaGalleryId = mediaGalleryId,
                    MediaType = mediaType,
                    Folder = folder,
                    Path = path,
                    Name = name,
                    LastWriteTimeUtc = lastWriteTimeUtc,
                    Size = size,
                    LastScan = lastScan,
                    Thumbnail = thumbnail?.GetBuffer(),
                };
                this._MediaCacheContext.ListFile.Add(newFile);
            }

            // Save changes to database
            this._MediaCacheContext.SaveChanges();
        }
    }

    public void Cleanup(DateTime lastScan) {
        using (this.Lock.EnterScope()) {
            // Remove files where LastScan is older than the provided lastScan
            var filesToRemove = this._MediaCacheContext.ListFile
            .Where(f => f.LastScan < lastScan)
            .ToList();

            if (filesToRemove.Count > 0) {
                this._MediaCacheContext.ListFile.RemoveRange(filesToRemove);
                this._MediaCacheContext.SaveChanges();
            }
        }
    }

    public void Delete(string mediaGalleryId, string path) {
        using (this.Lock.EnterScope()) {
            // Find and remove the file from database
            var fileToRemove = this._MediaCacheContext.ListFile
            .FirstOrDefault(f => f.MediaGalleryId == mediaGalleryId && f.Path == path);

            if (fileToRemove != null) {
                this._MediaCacheContext.ListFile.Remove(fileToRemove);
                this._MediaCacheContext.SaveChanges();
            }
        }
    }

    public List<SAMediaInfo> Search(SAMediaSearchRequest searchRequest, Func<string, string, bool> checkIfExits) {
        var searchAll = string.IsNullOrEmpty(searchRequest.Value);

        List<DbFile> resultsDB;
        using (this.Lock.EnterScope()) {
            IQueryable<DbFile> query = this._MediaCacheContext.ListFile;

            // Filter by media type if specified
            if (searchRequest.MediaType != MediaType.Unkown) {
                var mediaTypeString = FilePersistenceUtility.ConvertMediaTypeToString(searchRequest.MediaType);
                query = query.Where(f => f.MediaType == mediaTypeString);
            }

            // If search value is provided, filter by name
            if (!searchAll) {
                var listSearchFor = searchRequest.Value.Replace('\\', '/')
                    .Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Search for files where the name contains any of the search terms
                foreach (var searchTerm in listSearchFor) {
                    var term = searchTerm.ToLower();
                    query = query.Where(f => f.Name.ToLower().Contains(term) || f.Path.ToLower().Contains(term));
                }
            }

            // Execute query and convert to SAMediaInfo
            resultsDB = query
               .OrderBy(f => f.Path)
               .ToList()
               ;
        }
        var results = resultsDB
            .Where(f => checkIfExits(f.MediaGalleryId, f.Path))
            .Select(f => new SAMediaInfo {
                Path = $"{f.MediaGalleryId}/{f.Path}",
                MediaType = f.MediaType,
                Size = f.Size,
                LastWriteTimeUtc = f.LastWriteTimeUtc,
                LastScan = f.LastScan
            })
            .ToList();

        return results;
    }

    private string GetDebuggerDisplay() {
        return this._FileFQN;
    }

    public (bool found, byte[]? thumbnail) GetThumbnail(string mediaGalleryId, string path) {
        using (this.Lock.EnterScope()) {
            var file = this._MediaCacheContext.ListFile
            .Where(f => f.MediaGalleryId == mediaGalleryId && f.Path == path)
            .FirstOrDefault();
            if (file is null) {
                return (found: false, thumbnail: null);
            } else {
                return (found: true, thumbnail: file.Thumbnail);
            }
        }
    }
}
