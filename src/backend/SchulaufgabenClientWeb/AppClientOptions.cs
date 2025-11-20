// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben;

public sealed class AppClientOptions {
    public string? PublishFolder { get; set; }
}

internal sealed class AppClientConfigureOptions : IConfigureOptions<AppClientOptions> {
    public void Configure(AppClientOptions options) {
        if (string.IsNullOrEmpty(options.PublishFolder)) {
            options.PublishFolder = System.IO.Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                "Schulaufgaben");
        }
        
    }
}
internal sealed class AppClientValidateOptions : IValidateOptions<AppClientOptions> {
    public ValidateOptionsResult Validate(string? name, AppClientOptions options) {
        if (options.PublishFolder is { Length: > 0 } publishFolder){
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