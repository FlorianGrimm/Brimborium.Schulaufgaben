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

    public static async Task<List<SAWorkDescription>> ReadListWorkDescriptionFileAsync(
        string fileFQN,
        CancellationToken cancellationToken
        ) {
        List<SAWorkDescription>? result;
        if (System.IO.File.Exists(fileFQN)) {
            using (System.IO.FileStream fileStream = new(fileFQN, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                result = await System.Text.Json.JsonSerializer.DeserializeAsync<List<SAWorkDescription>>(
                    fileStream,
                    GetJsonSerializerOptions(),
                    cancellationToken);
            }
        } else {
            result = null;
        }
        return result ?? [];
    }

    public static async Task WriteListWorkDescriptionFileAsync(
        string fileFQN,
        List<SAWorkDescription> value,
        CancellationToken cancellationToken
        ) {

        using (System.IO.FileStream fileStream = new(fileFQN, FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
            await System.Text.Json.JsonSerializer.SerializeAsync<List<SAWorkDescription>>(
                fileStream,
                value,
                GetJsonSerializerOptions(),
                cancellationToken);
            await fileStream.FlushAsync();
        }
    }


    public static async Task<SAWork> ReadWorkFileAsync(
        string fileFQN,
        CancellationToken cancellationToken
        ) {
        SAWork? result;

        if (System.IO.File.Exists(fileFQN)) {
            using (System.IO.FileStream fileStream = new(fileFQN, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                result = await System.Text.Json.JsonSerializer.DeserializeAsync<SAWork>(
                    fileStream,
                    GetJsonSerializerOptions(),
                    cancellationToken);
            }
        } else {
            result = null;
        }

        return result ?? new SAWork();
    }

    public static async Task WriteWorkFileAsync(
        string fileFQN,
        SAWork value,
        CancellationToken cancellationToken
        ) {
        using (System.IO.FileStream fileStream = new(fileFQN, FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
            await System.Text.Json.JsonSerializer.SerializeAsync<SAWork>(
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