// MIT - Florian Grimm

using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Schulaufgaben.Service;

public class ClientPersistenceOptions {
    public string? Folder { get; set; }
}

public class ClientPersistenceValidateOptions : Microsoft.Extensions.Options.IValidateOptions<ClientPersistenceOptions> {
    public ValidateOptionsResult Validate(string? name, ClientPersistenceOptions options) {
        if (options.Folder is { Length: > 0 } folder) {
            if (System.IO.Directory.Exists(folder)) {
                //OK
            } else {
                return ValidateOptionsResult.Fail($"{options.Folder} does not exists.");
            }
        } else {
            return ValidateOptionsResult.Fail($"{nameof(options.Folder)} is empty.");
        }
        return ValidateOptionsResult.Success;
    }
}

public class ClientPersistenceService {

    private string _Folder;

    public ClientPersistenceService(
        ClientPersistenceOptions options
        ) {
        this._Folder = string.Empty;
        this.OnOptionsChange(options);
    }

    [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
    public ClientPersistenceService(
        IOptionsMonitor<ClientPersistenceOptions> options
        ) {
        this._Folder = string.Empty;
        options.OnChange(this.OnOptionsChange);
        this.OnOptionsChange(options.CurrentValue);
    }

    private void OnOptionsChange(ClientPersistenceOptions options) {
        if (options.Folder is { Length: > 0 } folder) {
            this._Folder = folder;
        }
    }

    public async Task<List<SAWorkDescription>> SAWorkDescriptionListReadAsync(CancellationToken cancellationToken) {
        var fileFQN = System.IO.Path.Combine(
            this._Folder,
            SAConstants.WorkDescriptionJson
            );
        return await FilePersistenceUtility.ReadListWorkDescriptionFileAsync(this._Folder, cancellationToken);
    }
}
