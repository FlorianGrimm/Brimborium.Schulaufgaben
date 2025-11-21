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
}