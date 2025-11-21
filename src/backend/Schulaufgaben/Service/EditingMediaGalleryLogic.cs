// MIT - Florian Grimm

using System.Security.Cryptography;

namespace Brimborium.Schulaufgaben.Service;

public class EditingMediaGalleryOptions {
    public List<EditingMediaGallery> ListMediaGallery { get; set; } = [];
}
public sealed record EditingMediaGallery(
    string FolderPath
    );

public sealed record MediaGallery(
    string MediaGalleryId,
    string FolderPath,
    List<SAMediaInfo> ListImage,
    List<SAMediaInfo> ListVideo,
    List<SAMediaInfo> ListAudio
    );


public sealed class EditingMediaGalleryLogic {
    private readonly RepositoryEditingMediaGallery _RepositoryEditingMediaGallery;
    private Dictionary<string, MediaGallery> _DictMediaGallery;

    public EditingMediaGalleryLogic(
        RepositoryEditingMediaGallery repositoryEditingMediaGallery,
        IOptionsMonitor<EditingMediaGalleryOptions> options
        ) {
        this._DictMediaGallery = new();
        this._RepositoryEditingMediaGallery = repositoryEditingMediaGallery;
        options.OnChange(this.OnOptionsChange);
        this.OnOptionsChange(options.CurrentValue);
    }

    private void OnOptionsChange(EditingMediaGalleryOptions options) {
        Dictionary<string, MediaGallery> nextDictMediaGallery = new();
        foreach (var sourceMediaGallery in options.ListMediaGallery) {
            // the sourceMediaGallery.FolderPath as md5
            string mediaGalleryId = "Gallery" + string.Join("",
                MD5.HashData(
                Encoding.UTF8.GetBytes(sourceMediaGallery.FolderPath)
                ).Select(b => b.ToString("XX")));
            MediaGallery mediaGallery = new(
                mediaGalleryId,
                sourceMediaGallery.FolderPath,
                [], [], []
                );
            nextDictMediaGallery.Add(mediaGalleryId, mediaGallery);
        }
        this._DictMediaGallery = nextDictMediaGallery;
    }

    public async Task InitialScanAsync(CancellationToken stoppingToken) {
        await Task.CompletedTask;
        //TODO
    }

    public async Task<List<SAMediaInfo>> SearchAsync(string value, CancellationToken requestAborted) {
        List<SAMediaInfo> result = [];
        // TODO
        await Task.CompletedTask;
        return result;
    }

    public async Task<SAMediaGetResult?> GetAsync(string name, CancellationToken requestAborted) {
        // The name is the relative path, format {MediaGalleryId}/{relativePath}.
        // The name must be a file.
        // The name cannot contain "..".
        // The name must be in the list of media galleries.
        var parts = name.Split(new char[] { '/' }, 2);
        if (parts.Length != 2) { return null; }
        string mediaGalleryId = parts[0];
        string relativeName = parts[1];
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
}

public sealed record SAMediaGetResult(
    Stream Stream, string ContentType);
