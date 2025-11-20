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

    public async Task<List<SAWorkDescription>> ReadEditingListWorkDescriptionListAsync(CancellationToken cancellationToken) {
        var fileFQN = System.IO.Path.Combine(
            this._EditingFolder,
            SAConstants.WorkDescriptionJson
            );
        return await FilePersistenceUtility.ReadListWorkDescriptionFileAsync(fileFQN, cancellationToken);
    }
    public async Task WriteEditingWorkDescriptionListAsync(
        List<SAWorkDescription> value,
        CancellationToken cancellationToken) {
        var fileFQN = System.IO.Path.Combine(
            this._EditingFolder,
            SAConstants.WorkDescriptionJson
            );
        await FilePersistenceUtility.WriteListWorkDescriptionFileAsync(fileFQN, value, cancellationToken);
    }

    public string GetEditingWorkDescriptionFolder(SAWorkDescription value) {
        var folderFQN = System.IO.Path.Combine(
            this._EditingFolder,
            value.Folder
            );
        return folderFQN;
    }

    public async Task<List<SAWorkDescription>> ReadPublishListWorkDescriptionListAsync(CancellationToken cancellationToken) {
        var fileFQN = System.IO.Path.Combine(
            this._PublishFolder,
            SAConstants.WorkDescriptionJson
            );
        return await FilePersistenceUtility.ReadListWorkDescriptionFileAsync(fileFQN, cancellationToken);
    }

    public async Task WritePublishListWorkDescriptionListAsync(
        List<SAWorkDescription> value,
        CancellationToken cancellationToken) {
        var fileFQN = System.IO.Path.Combine(
            this._PublishFolder,
            SAConstants.WorkDescriptionJson
            );
        await FilePersistenceUtility.WriteListWorkDescriptionFileAsync(fileFQN, value, cancellationToken);
    }

    private List<SAWorkDescription>? _StateWorkDescriptionListWorking;
    public async Task<List<SAWorkDescription>> GetStateWorkDescriptionListWorkingAsync(CancellationToken cancellationToken) {
        {
            if (this._StateWorkDescriptionListWorking is { } result) {
                return result;
            }
        }
        {
            if (await this.ReadEditingListWorkDescriptionListAsync(cancellationToken) is { } result) {
                return this._StateWorkDescriptionListWorking = result;
            }
        }
        {
            return this._StateWorkDescriptionListWorking = [];
        }
    }

    public async Task SetStateWorkDescriptionListWorkingAsync(
        List<SAWorkDescription> value,
        CancellationToken cancellationToken) {
        this._StateWorkDescriptionListWorking = new(value);
        await this.WriteEditingWorkDescriptionListAsync(value, cancellationToken).ConfigureAwait(false);
    }

    // Controller

    public async Task<List<SAWorkDescription>> GetWorkDescriptionListFromWorkingAsync(
        CancellationToken requestAborted) {
        return await this.GetStateWorkDescriptionListWorkingAsync(requestAborted).ConfigureAwait(false);
    }

    public async Task<SAWorkDescription?> GetWorkDescriptionByIdFromWorkingAsync(
        Guid id,
        CancellationToken requestAborted) {
        var list = await this.GetStateWorkDescriptionListWorkingAsync(requestAborted).ConfigureAwait(false);
        foreach (var item in list) {
            if (item.Id == id) { return item; }
        }
        return null;
    }

    public async Task<SAWorkDescription?> CreateWorkDescriptionFromWorkingAsync(
        SAWorkDescription value,
        CancellationToken requestAborted) {
        var currentState = await this.GetStateWorkDescriptionListWorkingAsync(requestAborted);
        if (value.Id == Guid.Empty) { value.Id = Guid.NewGuid(); }


        value.Folder = FilePersistenceUtility.NormalizeFolderName(
            $"{value.Id}-{value.Name}");

        foreach (var currentWorkDescription in currentState) {
            if (currentWorkDescription.Id == value.Id) {
                return null;
            }
            if (currentWorkDescription.Name == value.Name) {
                return null;
            }

            if (currentWorkDescription.Folder == value.Folder) {
                return null;
            }
        }

        var nextState = new List<SAWorkDescription>(currentState);
        nextState.Add(value);
        await this.WriteEditingWorkDescriptionListAsync(nextState, requestAborted);
        var folder = this.GetEditingWorkDescriptionFolder(value);
        System.IO.Directory.CreateDirectory(folder);

        return value;
    }

    public async Task<SAWorkDescription?> UpdateWorkDescriptionFromWorkingAsync(
        Guid id,
        SAWorkDescription value,
        CancellationToken requestAborted) {
        SAWorkDescription? result = null;
        await Task.CompletedTask;
        return result;
    }

    public async Task DeleteWorkDescriptionFromWorkingAsync(
        Guid id,
        CancellationToken requestAborted) {
        await Task.CompletedTask;
    }
}
