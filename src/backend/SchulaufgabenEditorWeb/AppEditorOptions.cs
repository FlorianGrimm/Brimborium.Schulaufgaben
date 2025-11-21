// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben;

public sealed class AppEditorOptions {
    public string? EditingFolder { get; set; }
    public string? PublishFolder { get; set; }
    public List<MediaGallery> ListMediaGallery { get; set; } = [];
}

public sealed class MediaGallery {
    public string? FolderPath { get; set; }
}

internal sealed class AppEditorConfigureOptions : IConfigureOptions<AppEditorOptions> {
    public void Configure(AppEditorOptions options) {
        if (string.IsNullOrEmpty(options.EditingFolder)) {
            options.EditingFolder = System.IO.Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                "Schulaufgaben");
        }
        if (string.IsNullOrEmpty(options.PublishFolder)) {
            options.PublishFolder = System.IO.Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                "Schulaufgaben");
        }
        if (options.EditingFolder is { Length: > 0 } editingFolder) { 
            System.IO.Directory.CreateDirectory(editingFolder);
        }
        if (options.PublishFolder is { Length: > 0 } publishFolder) {
            System.IO.Directory.CreateDirectory(publishFolder);
        }
    }
}

internal class AppEditorValidateOptions : IValidateOptions<AppEditorOptions> {
    public ValidateOptionsResult Validate(string? name, AppEditorOptions options) {
        if (options.EditingFolder is { Length: > 0 } editingFolder) {
            if (System.IO.Directory.Exists(editingFolder)) {
                // OK
            } else {
                return ValidateOptionsResult.Fail($"{nameof(options.EditingFolder)}: {options.EditingFolder} does not exists");
            }
        } else {
            return ValidateOptionsResult.Fail($"{nameof(options.EditingFolder)} is empty.");
        }

        if (options.PublishFolder is { Length: > 0 } publishFolder) {
            if (System.IO.Directory.Exists(publishFolder)) {
                // OK
            } else {
                return ValidateOptionsResult.Fail($"{nameof(options.PublishFolder)}: {options.PublishFolder} does not exists");
            }
        } else {
            return ValidateOptionsResult.Fail($"{nameof(options.PublishFolder)} is empty.");
        }
        return ValidateOptionsResult.Success;
    }
}