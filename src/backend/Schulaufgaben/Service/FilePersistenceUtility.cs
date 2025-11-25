// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Service;

public static class FilePersistenceUtility {
    public static System.Text.Json.JsonSerializerOptions? JsonSerializerOptions;
    public static System.Text.Json.JsonSerializerOptions GetJsonSerializerOptions() {
        if (JsonSerializerOptions == null) {
            JsonSerializerOptions = new JsonSerializerOptions() {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
            };
        }
        return JsonSerializerOptions;
    }

    public static async Task<List<SADocumentDescription>> ReadListDocumentDescriptionAsync(
        string fileFQN,
        CancellationToken cancellationToken
        ) {
        List<SADocumentDescription>? result;
        if (System.IO.File.Exists(fileFQN)) {
            using (System.IO.FileStream fileStream = new(fileFQN, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                result = await System.Text.Json.JsonSerializer.DeserializeAsync<List<SADocumentDescription>>(
                    fileStream,
                    GetJsonSerializerOptions(),
                    cancellationToken);
            }
        } else {
            result = null;
        }
        return result ?? [];
    }

    public static async Task WriteListDocumentDescriptionAsync(
        string fileFQN,
        List<SADocumentDescription> value,
        CancellationToken cancellationToken
        ) {

        using (System.IO.FileStream fileStream = new(fileFQN, FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
            await System.Text.Json.JsonSerializer.SerializeAsync<List<SADocumentDescription>>(
                fileStream,
                value,
                GetJsonSerializerOptions(),
                cancellationToken);
            await fileStream.FlushAsync();
        }
    }


    public static async Task<SADocument> ReadDocumentAsync(
        string fileFQN,
        CancellationToken cancellationToken
        ) {
        SADocument? result;

        if (System.IO.File.Exists(fileFQN)) {
            using (System.IO.FileStream fileStream = new(fileFQN, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                result = await System.Text.Json.JsonSerializer.DeserializeAsync<SADocument>(
                    fileStream,
                    GetJsonSerializerOptions(),
                    cancellationToken);
            }
        } else {
            result = null;
        }

        return result ?? new SADocument();
    }

    public static async Task WriteDocumentAsync(
        string fileFQN,
        SADocument value,
        CancellationToken cancellationToken
        ) {
        var directoryFQN = System.IO.Path.GetDirectoryName(fileFQN);
        if (directoryFQN is null) { throw new ArgumentException(nameof(fileFQN)); }

        System.IO.Directory.CreateDirectory(directoryFQN);
        using (System.IO.FileStream fileStream = new(fileFQN, FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
            await System.Text.Json.JsonSerializer.SerializeAsync<SADocument>(
                fileStream,
                value,
                GetJsonSerializerOptions(),
                cancellationToken);
        }
    }

    public static string NormalizeFolderName(string name) {
        var invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
        if (name.StartsWith(' ')
            || name.EndsWith(' ')
            || name.Contains("__")
            || name.AsSpan().ContainsAny(invalidFileNameChars.AsSpan())) {
            var t = name.AsSpan().Trim();
            var sb = new StringBuilder().Append(t);
            foreach (var c in invalidFileNameChars) {
                sb.Replace(c, '_');
            }
            sb.Replace("____", "_");
            sb.Replace("___", "_");
            sb.Replace("__", "_");
            return sb.ToString();
        } else {
            return name;
        }
    }


    public static string GetMD5(string value) {
        return string.Join("",
            System.Security.Cryptography.MD5.HashData(
                Encoding.UTF8.GetBytes(value)
            ).Select(b => b.ToString("X2"))
            );
    }

    private static Dictionary<string, string>? _DictContentType;
    public static string ConvertExtensionToContentType(string extension) {
        if (_DictContentType is null) {
            _DictContentType = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { ".323", "text/h323" },
                { ".3g2", "video/3gpp2" },
                { ".3gp2", "video/3gpp2" },
                { ".3gp", "video/3gpp" },
                { ".3gpp", "video/3gpp" },
                { ".aac", "audio/aac" },
                { ".adt", "audio/vnd.dlna.adts" },
                { ".adts", "audio/vnd.dlna.adts" },
                { ".ai", "application/postscript" },
                { ".aif", "audio/x-aiff" },
                { ".aifc", "audio/aiff" },
                { ".aiff", "audio/aiff" },
                { ".art", "image/x-jg" },
                { ".asf", "video/x-ms-asf" },
                { ".au", "audio/basic" },
                { ".avi", "video/x-msvideo" },
                { ".avif", "image/avif" },
                { ".bmp", "image/bmp" },
                { ".cmx", "image/x-cmx" },
                { ".css", "text/css" },
                { ".csv", "text/csv" },
                { ".dib", "image/bmp" },
                { ".dlm", "text/dlm" },
                { ".eps", "application/postscript" },
                { ".etx", "text/x-setext" },
                { ".fdf", "application/vnd.fdf" },
                { ".fif", "application/fractals" },
                { ".flr", "x-world/x-vrml" },
                { ".flv", "video/x-flv" },
                { ".gif", "image/gif" },
                { ".hdf", "application/x-hdf" },
                { ".hdml", "text/x-hdml" },
                { ".htm", "text/html" },
                { ".html", "text/html" },
                { ".ico", "image/x-icon" },
                { ".ief", "image/ief" },
                { ".ifb", "text/calendar" },
                { ".IVF", "video/x-ivf" },
                { ".jfif", "image/pjpeg" },
                { ".jpe", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".jpg", "image/jpeg" },
                { ".json", "application/json" },
                { ".latex", "application/x-latex" },
                { ".lit", "application/x-ms-reader" },
                { ".lpk", "application/octet-stream" },
                { ".lsf", "video/x-la-asf" },
                { ".lsx", "video/x-la-asf" },
                { ".lzh", "application/octet-stream" },
                { ".m13", "application/x-msmediaview" },
                { ".m14", "application/x-msmediaview" },
                { ".m1v", "video/mpeg" },
                { ".m2ts", "video/vnd.dlna.mpeg-tts" },
                { ".m3u", "audio/x-mpegurl" },
                { ".m4a", "audio/mp4" },
                { ".m4v", "video/mp4" },
                { ".man", "application/x-troff-man" },
                { ".manifest", "application/x-ms-manifest" },
                { ".map", "text/plain" },
                { ".markdown", "text/markdown" },
                { ".md", "text/markdown" },
                { ".me", "application/x-troff-me" },
                { ".mid", "audio/mid" },
                { ".midi", "audio/mid" },
                { ".mmf", "application/x-smaf" },
                { ".mno", "text/xml" },
                { ".mny", "application/x-msmoney" },
                { ".mov", "video/quicktime" },
                { ".movie", "video/x-sgi-movie" },
                { ".mp2", "video/mpeg" },
                { ".mp3", "audio/mpeg" },
                { ".mp4", "video/mp4" },
                { ".mp4v", "video/mp4" },
                { ".mpa", "video/mpeg" },
                { ".mpe", "video/mpeg" },
                { ".mpeg", "video/mpeg" },
                { ".mpg", "video/mpeg" },
                { ".mpp", "application/vnd.ms-project" },
                { ".mpv2", "video/mpeg" },
                { ".ms", "application/x-troff-ms" },
                { ".msi", "application/octet-stream" },
                { ".mso", "application/octet-stream" },
                { ".mvb", "application/x-msmediaview" },
                { ".mvc", "application/x-miva-compiled" },
                { ".nc", "application/x-netcdf" },
                { ".nsc", "video/x-ms-asf" },
                { ".nws", "message/rfc822" },
                { ".ocx", "application/octet-stream" },
                { ".oda", "application/oda" },
                { ".odc", "text/x-ms-odc" },
                { ".ods", "application/oleobject" },
                { ".oga", "audio/ogg" },
                { ".ogg", "video/ogg" },
                { ".ogv", "video/ogg" },
                { ".ogx", "application/ogg" },
                { ".otf", "font/otf" },
                { ".pbm", "image/x-portable-bitmap" },
                { ".pdf", "application/pdf" },
                { ".pfx", "application/x-pkcs12" },
                { ".pgm", "image/x-portable-graymap" },
                { ".png", "image/png" },
                { ".pnm", "image/x-portable-anymap" },
                { ".pnz", "image/png" },
                { ".ppt", "application/vnd.ms-powerpoint" },
                { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                { ".ps", "application/postscript" },
                { ".qt", "video/quicktime" },
                { ".ras", "image/x-cmu-raster" },
                { ".rf", "image/vnd.rn-realflash" },
                { ".rgb", "image/x-rgb" },
                { ".rm", "application/vnd.rn-realmedia" },
                { ".rmi", "audio/mid" },
                { ".roff", "application/x-troff" },
                { ".rpm", "audio/x-pn-realaudio-plugin" },
                { ".rtf", "application/rtf" },
                { ".sgml", "text/sgml" },
                { ".smd", "audio/x-smd" },
                { ".smx", "audio/x-smd" },
                { ".smz", "audio/x-smd" },
                { ".snd", "audio/basic" },
                { ".spx", "audio/ogg" },
                { ".ssm", "application/streamingmedia" },
                { ".svg", "image/svg+xml" },
                { ".svgz", "image/svg+xml" },
                { ".tif", "image/tiff" },
                { ".tiff", "image/tiff" },
                { ".ts", "video/vnd.dlna.mpeg-tts" },
                { ".tsv", "text/tab-separated-values" },
                { ".ttc", "application/x-font-ttf" },
                { ".ttf", "application/x-font-ttf" },
                { ".tts", "video/vnd.dlna.mpeg-tts" },
                { ".txt", "text/plain" },
                { ".ustar", "application/x-ustar" },
                { ".vcs", "text/plain" },
                { ".vml", "text/xml" },
                { ".wav", "audio/wav" },
                { ".wax", "audio/x-ms-wax" },
                { ".wbmp", "image/vnd.wap.wbmp" },
                { ".webm", "video/webm" },
                { ".webp", "image/webp" },
                { ".wm", "video/x-ms-wm" },
                { ".wma", "audio/x-ms-wma" },
                { ".wml", "text/vnd.wap.wml" },
                { ".wmp", "video/x-ms-wmp" },
                { ".wmv", "video/x-ms-wmv" },
                { ".wmx", "video/x-ms-wmx" },
                { ".wmz", "application/x-ms-wmz" },
                { ".woff", "application/font-woff" }, // https://www.w3.org/TR/WOFF/#appendix-b
                { ".woff2", "font/woff2" }, // https://www.w3.org/TR/WOFF2/#IMT
                { ".wrl", "x-world/x-vrml" },
                { ".wrz", "x-world/x-vrml" },
                { ".wsdl", "text/xml" },
                { ".wtv", "video/x-ms-wtv" },
                { ".wvx", "video/x-ms-wvx" },
                { ".xaf", "x-world/x-vrml" },
                { ".xbm", "image/x-xbitmap" },
                { ".xdr", "text/plain" },
                { ".xht", "application/xhtml+xml" },
                { ".xhtml", "application/xhtml+xml" },
                { ".xml", "text/xml" },
                { ".xof", "x-world/x-vrml" },
                { ".xpm", "image/x-xpixmap" },
                { ".xps", "application/vnd.ms-xpsdocument" }
            };
        }

        if (_DictContentType.TryGetValue(extension, out var result)) {
            return result;
        }
        return "";
    }

    private static Dictionary<string, MediaType>? _DictMediaType;
    internal static MediaType ConvertExtensionToMediaType(string extension) {
        if (_DictMediaType is null) {
            _DictMediaType = new Dictionary<string, MediaType>(StringComparer.OrdinalIgnoreCase) {
                { ".323", MediaType.Unkown },
                { ".3g2", MediaType.Video },
                { ".3gp2", MediaType.Video },
                { ".3gp", MediaType.Video },
                { ".3gpp", MediaType.Video},
                { ".aac", MediaType.Audio },
                { ".adt", MediaType.Audio },
                { ".adts", MediaType.Audio },
                { ".ai", MediaType.Unkown },
                { ".aif", MediaType.Audio },
                { ".aifc", MediaType.Audio },
                { ".aiff", MediaType.Audio },
                { ".art", MediaType.Image },
                { ".asf", MediaType.Video },
                { ".au", MediaType.Audio },
                { ".avi", MediaType.Video },
                { ".avif", MediaType.Image },
                { ".bmp", MediaType.Image },
                { ".cmx", MediaType.Image },
                { ".css", MediaType.Unkown },
                { ".csv", MediaType.Unkown },
                { ".dib", MediaType.Image },
                { ".disco", MediaType.Unkown },
                { ".dlm", MediaType.Unkown },
                { ".eps", MediaType.Unkown },
                { ".etx", MediaType.Unkown },
                { ".fdf", MediaType.Unkown },
                { ".fif", MediaType.Unkown },
                { ".flr", MediaType.Unkown },
                { ".flv", MediaType.Video },
                { ".gif", MediaType.Image },
                { ".hdf", MediaType.Unkown },
                { ".htm", MediaType.Unkown },
                { ".html", MediaType.Unkown },
                { ".ico", MediaType.Image },
                { ".ief", MediaType.Image },
                { ".ifb", MediaType.Unkown },
                { ".IVF", MediaType.Video },
                { ".jfif", MediaType.Image },
                { ".jpe", MediaType.Image },
                { ".jpeg", MediaType.Image },
                { ".jpg", MediaType.Image },
                { ".lsf", MediaType.Video },
                { ".lsx", MediaType.Video },
                { ".lzh", MediaType.Unkown },
                { ".m13", MediaType.Unkown },
                { ".m14", MediaType.Unkown },
                { ".m1v", MediaType.Video },
                { ".m2ts", MediaType.Video },
                { ".m3u", MediaType.Audio },
                { ".m4a", MediaType.Audio },
                { ".m4v", MediaType.Video },
                { ".man", MediaType.Unkown },
                { ".manifest", MediaType.Unkown },
                { ".map", MediaType.Unkown },
                { ".markdown", MediaType.Unkown },
                { ".md", MediaType.Unkown },
                { ".me", MediaType.Unkown },
                { ".mid", MediaType.Audio },
                { ".midi", MediaType.Audio },
                { ".mmf", MediaType.Unkown },
                { ".mno", MediaType.Unkown },
                { ".mny", MediaType.Unkown },
                { ".mov", MediaType.Video },
                { ".movie", MediaType.Video },
                { ".mp2", MediaType.Video },
                { ".mp3", MediaType.Audio },
                { ".mp4", MediaType.Video },
                { ".mp4v", MediaType.Video },
                { ".mpa", MediaType.Video },
                { ".mpe", MediaType.Video },
                { ".mpeg", MediaType.Video },
                { ".mpg", MediaType.Video },
                { ".mpp", MediaType.Unkown },
                { ".mpv2", MediaType.Video },
                { ".ms", MediaType.Unkown },
                { ".msi", MediaType.Unkown },
                { ".mso", MediaType.Unkown },
                { ".mvb", MediaType.Unkown },
                { ".mvc", MediaType.Unkown },
                { ".nc", MediaType.Unkown },
                { ".nsc", MediaType.Video },
                { ".nws", MediaType.Unkown },
                { ".ocx", MediaType.Unkown },
                { ".oda", MediaType.Unkown },
                { ".odc", MediaType.Unkown },
                { ".ods", MediaType.Unkown },
                { ".oga", MediaType.Audio },
                { ".ogg", MediaType.Video },
                { ".ogv", MediaType.Video },
                { ".ogx", MediaType.Unkown },
                { ".otf", MediaType.Unkown },
                { ".pbm", MediaType.Image },
                { ".pdf", MediaType.Unkown },
                { ".pfx", MediaType.Unkown },
                { ".pgm", MediaType.Image },
                { ".png", MediaType.Image },
                { ".pnm", MediaType.Image },
                { ".pnz", MediaType.Image },
                { ".ppt", MediaType.Unkown },
                { ".pptx", MediaType.Unkown },
                { ".ps", MediaType.Unkown },
                { ".qt", MediaType.Video },
                { ".ras", MediaType.Image },
                { ".rf", MediaType.Image },
                { ".rgb", MediaType.Image },
                { ".rm", MediaType.Unkown },
                { ".rmi", MediaType.Audio },
                { ".roff", MediaType.Unkown },
                { ".rpm", MediaType.Audio },
                { ".rtf", MediaType.Unkown },
                { ".sgml", MediaType.Unkown },
                { ".smd", MediaType.Audio },
                { ".smx", MediaType.Audio },
                { ".smz", MediaType.Audio },
                { ".snd", MediaType.Audio },
                { ".spx", MediaType.Audio },
                { ".ssm", MediaType.Unkown },
                { ".svg", MediaType.Image },
                { ".svgz", MediaType.Image },
                { ".tif", MediaType.Image },
                { ".tiff", MediaType.Image },
                { ".ts", MediaType.Video },
                { ".tsv", MediaType.Unkown },
                { ".ttc", MediaType.Unkown },
                { ".ttf", MediaType.Unkown },
                { ".tts", MediaType.Video },
                { ".txt", MediaType.Unkown },
                { ".ustar", MediaType.Unkown },
                { ".vcs", MediaType.Unkown },
                { ".vml", MediaType.Unkown },
                { ".wav", MediaType.Audio },
                { ".wax", MediaType.Audio },
                { ".wbmp", MediaType.Image },
                { ".webm", MediaType.Video },
                { ".webp", MediaType.Image },
                { ".wm", MediaType.Video },
                { ".wma", MediaType.Audio },
                { ".wml", MediaType.Unkown },
                { ".wmp", MediaType.Video },
                { ".wmv", MediaType.Video },
                { ".wmx", MediaType.Video },
                { ".wmz", MediaType.Unkown },
                { ".woff", MediaType.Unkown }, // https://www.w3.org/TR/WOFF/#appendix-b
                { ".woff2", MediaType.Unkown }, // https://www.w3.org/TR/WOFF2/#IMT
                { ".wrl", MediaType.Unkown },
                { ".wrz", MediaType.Unkown },
                { ".wsdl", MediaType.Unkown },
                { ".wtv", MediaType.Video },
                { ".wvx", MediaType.Video },
                { ".xaf", MediaType.Unkown },
                { ".xbm", MediaType.Image },
                { ".xdr", MediaType.Unkown },
                { ".xht", MediaType.Unkown },
                { ".xhtml", MediaType.Unkown },
                { ".xml", MediaType.Unkown },
                { ".xof", MediaType.Unkown },
                { ".xpm", MediaType.Image },
                { ".xps", MediaType.Unkown }
            };
        }

        if (_DictMediaType.TryGetValue(extension, out var result)) {
            return result;
        }
        return MediaType.Unkown;
    }

    internal static string ConvertMediaTypeToString(MediaType mediaType) {
        return mediaType switch {
            MediaType.Image => "Image",
            MediaType.Audio => "Audio",
            MediaType.Video => "Video",
            _ => "Unkown",
        };
    }

    private static readonly char[] _ArraySlash = new char[] { '/' };

    public static (bool success, string mediaGalleryId, string relativeName) SplitPathIntoMediaGalleryIdAndPath(string name) {
        var parts = name.Split(_ArraySlash, 2);
        if (parts.Length != 2) { return (false, string.Empty, string.Empty); }
        string mediaGalleryId = parts[0];
        string relativeName = parts[1];
        if (string.IsNullOrEmpty(mediaGalleryId)) { return (false, string.Empty, string.Empty); }
        if (string.IsNullOrEmpty(relativeName)) { return (false, string.Empty, string.Empty); }
        if (relativeName.Contains("..")) { return (false, string.Empty, string.Empty); }
        return (true, mediaGalleryId, relativeName);
    }

    public static string CombineMediaGalleryIdAndPathIntoPath(string mediaGalleryId, string relativeName) {
        return $"{mediaGalleryId}/{relativeName}";
    }
}
