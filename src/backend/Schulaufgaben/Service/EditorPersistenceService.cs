// MIT - Florian Grimm


namespace Brimborium.Schulaufgaben.Service;

public class EditorPersistenceOptions {
    public string? EditingFolder { get; set; }
    public string? PublishFolder { get; set; }
}

public class EditorPersistenceValidateOptions : Microsoft.Extensions.Options.IValidateOptions<EditorPersistenceOptions> {
    public ValidateOptionsResult Validate(string? name, EditorPersistenceOptions options) {
        if (options.EditingFolder is { Length: > 0 } editingFolder) {
            if (System.IO.Directory.Exists(editingFolder)) {
                //OK
            } else {
                return ValidateOptionsResult.Fail($"{nameof(options.EditingFolder)}:{options.EditingFolder} does not exists.");
            }
        } else {
            return ValidateOptionsResult.Fail($"{nameof(options.EditingFolder)} is empty.");
        }
        if (options.PublishFolder is { Length: > 0 } publishFolder) {
            if (System.IO.Directory.Exists(publishFolder)) {
                //OK
            } else {
                return ValidateOptionsResult.Fail($"{nameof(options.PublishFolder)}:{options.PublishFolder} does not exists.");
            }
        } else {
            return ValidateOptionsResult.Fail($"{nameof(options.PublishFolder)} is empty.");
        }
        return ValidateOptionsResult.Success;
    }
}

public class EditorPersistenceService {
    private string _EditingFolder;
    private string _PublishFolder;

    private EditorPersistenceService(
        EditorPersistenceOptions options
        ) {
        this._EditingFolder = string.Empty;
        this._PublishFolder = string.Empty;
        this.OnOptionsChange(options);
    }

    [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
    public EditorPersistenceService(
        IOptionsMonitor<EditorPersistenceOptions> options
        ) {
        this._EditingFolder = string.Empty;
        this._PublishFolder = string.Empty;
        options.OnChange(this.OnOptionsChange);
        this.OnOptionsChange(options.CurrentValue);
    }

    private void OnOptionsChange(EditorPersistenceOptions options) {
        if (options.EditingFolder is { Length: > 0 } editingFolder) {
            this._EditingFolder = editingFolder;
        }
        if (options.PublishFolder is { Length: > 0 } publishFolder) {
            this._PublishFolder = publishFolder;
        }
    }

    public async Task<List<SADocumentDescription>> ReadEditingListDocumentDescriptionListAsync(CancellationToken cancellationToken) {
        var fileFQN = System.IO.Path.Combine(
            this._EditingFolder,
            SAConstants.ListDocumentDescriptionJson
            );
        return await FilePersistenceUtility.ReadListDocumentDescriptionAsync(fileFQN, cancellationToken);
    }
    public async Task WriteEditingDocumentDescriptionListAsync(
        List<SADocumentDescription> value,
        CancellationToken cancellationToken) {
        var fileFQN = System.IO.Path.Combine(
            this._EditingFolder,
            SAConstants.ListDocumentDescriptionJson
            );
        await FilePersistenceUtility.WriteListDocumentDescriptionAsync(fileFQN, value, cancellationToken);
    }

    public string GetEditingDocumentDescriptionFolder(SADocumentDescription value) {
        var folderFQN = System.IO.Path.Combine(
            this._EditingFolder,
            value.Folder
            );
        return folderFQN;
    }

    public async Task<List<SADocumentDescription>> ReadPublishListWorkDescriptionListAsync(CancellationToken cancellationToken) {
        var fileFQN = System.IO.Path.Combine(
            this._PublishFolder,
            SAConstants.ListDocumentDescriptionJson
            );
        return await FilePersistenceUtility.ReadListDocumentDescriptionAsync(fileFQN, cancellationToken);
    }

    public async Task WritePublishListDocumentDescriptionListAsync(
        List<SADocumentDescription> value,
        CancellationToken cancellationToken) {
        var fileFQN = System.IO.Path.Combine(
            this._PublishFolder,
            SAConstants.ListDocumentDescriptionJson
            );
        await FilePersistenceUtility.WriteListDocumentDescriptionAsync(fileFQN, value, cancellationToken);
    }

    public async Task WriteEditingDocumentAsync(
        SADocumentDescription documentDescription,
        SADocument work,
        CancellationToken cancellationToken) {
        var folderFQN = this.GetEditingDocumentDescriptionFolder(documentDescription);
        var fileFQN = System.IO.Path.Combine(folderFQN, SAConstants.DocumentJson);
        await FilePersistenceUtility.WriteDocumentAsync(fileFQN, work, cancellationToken);
    }
}
