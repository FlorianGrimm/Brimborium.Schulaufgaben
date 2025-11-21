// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Service;

public class EditingDocumentDescriptionLogic {
    private readonly RepositoryEditingDocumentDescription _Repo;
    private readonly AccessEditingDocumentLogic _AccessEditingDocumentLogic;

    public EditingDocumentDescriptionLogic(
        RepositoryEditingDocumentDescription repo,
        AccessEditingDocumentLogic accessEditingDocumentLogic
        ) {
        this._Repo = repo;
        this._AccessEditingDocumentLogic = accessEditingDocumentLogic;
    }

    public async Task<List<SADocumentDescription>> GetListAsync(
        CancellationToken requestAborted) {
        return await this._Repo.GetListAsync(requestAborted).ConfigureAwait(false);
    }

    public async Task<SADocumentDescription?> GetByIdAsync(
        Guid id,
        CancellationToken requestAborted) {
        var list = await this._Repo.GetListAsync(requestAborted).ConfigureAwait(false);
        foreach (var item in list) {
            if (item.Id == id) { return item; }
        }
        return null;
    }

    public async Task<SADocumentDescription?> CreateAsync(
        SADocumentDescription value,
        CancellationToken requestAborted) {
        var currentState = await this._Repo.GetListAsync(requestAborted);
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

        var nextState = new List<SADocumentDescription>(currentState);
        nextState.Add(value);
        await this._Repo.SetListAsync(nextState, requestAborted);

        var editingDocumentLogic = this._AccessEditingDocumentLogic.Get();
        var document = new SADocument() {
            Id = value.Id,
            Name = value.Name,
            Description = value.Description,
        };
        await editingDocumentLogic.CreateAsync(value, document, requestAborted).ConfigureAwait(false);

        return value;
    }

    public async Task<SADocumentDescription?> UpdateAsync(
        Guid id,
        SADocumentDescription value,
        CancellationToken requestAborted) {
        var currentState = await this._Repo.GetListAsync(requestAborted);
        var nextState = new List<SADocumentDescription>(currentState.Count);
        SADocumentDescription? result = null;
        foreach (var currentWorkDescription in currentState) {
            if (currentWorkDescription.Id == id) {
                result = value;
                nextState.Add(value);
            } else {
                nextState.Add(currentWorkDescription);
            }
        }
        if (result is { }) {
            await this._Repo.SetListAsync(nextState, requestAborted).ConfigureAwait(false);
        }
        return result;
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken requestAborted) {
        var currentState = await this._Repo.GetListAsync(requestAborted);
        var nextState = new List<SADocumentDescription>(currentState.Count);
        SADocumentDescription? found = null;
        foreach (var currentWorkDescription in currentState) {
            if (currentWorkDescription.Id == id) {
                found = currentWorkDescription;
            } else {
                nextState.Add(currentWorkDescription);
            }
        }
        if (found is { }) {
            await this._Repo.SetListAsync(nextState, requestAborted).ConfigureAwait(false);
        }
    }
}

public sealed class AccessEditingDocumentDescriptionLogic {
    private readonly IServiceProvider? _ServiceProvider;
    private EditingDocumentDescriptionLogic? _Instance;

    public static AccessEditingDocumentDescriptionLogic Create(EditingDocumentDescriptionLogic instance)
        => new AccessEditingDocumentDescriptionLogic(instance);

    private AccessEditingDocumentDescriptionLogic(EditingDocumentDescriptionLogic instance) {
        this._Instance = instance;
    }

    [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
    public AccessEditingDocumentDescriptionLogic(
        IServiceProvider serviceProvider
        ) {
        this._ServiceProvider = serviceProvider;
    }


    public EditingDocumentDescriptionLogic Get() {
        if (this._Instance is { } result) {
            return result;
        }
        if (this._ServiceProvider is { } serviceProvider) {
            result = this._ServiceProvider.GetRequiredService<EditingDocumentDescriptionLogic>();
            return this._Instance = result;
        }
        throw new NotSupportedException("Logic Error");
    }
}

