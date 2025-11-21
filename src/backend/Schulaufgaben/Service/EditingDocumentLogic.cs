// MIT - Florian Grimm


namespace Brimborium.Schulaufgaben.Service;

public class EditingDocumentLogic {
    private readonly EditorPersistenceService _EditorPersistenceService;
    private readonly AccessEditingDocumentDescriptionLogic _AccessEditingDocumentDescriptionLogic;

    public EditingDocumentLogic(
        EditorPersistenceService editorPersistenceService,
        AccessEditingDocumentDescriptionLogic accessEditingDocumentDescriptionLogic
        ) {
        this._EditorPersistenceService = editorPersistenceService;
        this._AccessEditingDocumentDescriptionLogic = accessEditingDocumentDescriptionLogic;
    }


    public async Task<SADocument?> GetByIdAsync(Guid id, CancellationToken requestAborted) {
        var editingDocumentDescriptionLogic = this._AccessEditingDocumentDescriptionLogic.Get();
        var documentDescription = await editingDocumentDescriptionLogic.GetByIdAsync(id, requestAborted);
        if (documentDescription is null) { return null; }
        return await this._EditorPersistenceService.ReadEditingDocumentAsync(documentDescription, requestAborted);
    }

    public async Task<SADocument> CreateAsync(
        SADocumentDescription documentDescription,
        SADocument document,
        CancellationToken requestAborted) {
        var folderFQN = this._EditorPersistenceService.GetEditingDocumentDescriptionFolder(documentDescription);
        await this._EditorPersistenceService.WriteEditingDocumentAsync(documentDescription, document, requestAborted);
        return document;
    }


    public async Task<SADocument?> CreateAsync(SADocument document, CancellationToken requestAborted) {
        var editingDocumentDescriptionLogic = this._AccessEditingDocumentDescriptionLogic.Get();
        SADocumentDescription? documentDescription = null;
        if (document.Id == Guid.Empty) {
            document.Id = Guid.NewGuid();
        } else {
            documentDescription = await editingDocumentDescriptionLogic.GetByIdAsync(document.Id, requestAborted);
        }
        if (documentDescription is null) {
            // documentDescription does not exists.
            documentDescription = new SADocumentDescription() {
                Id = document.Id,
                Name = document.Name,
                Description = document.Description,
            };
            documentDescription = await editingDocumentDescriptionLogic.CreateAsync(documentDescription, requestAborted);
            if (documentDescription is null) { return null; }
        }
        // documentDescription exists.
        {
            var result = await this.CreateAsync(documentDescription, document, requestAborted);
            return result;
        }
    }

    public async Task<SADocument?> UpdateAsync(Guid id, SADocument value, CancellationToken requestAborted) {
        var editingDocumentDescriptionLogic = this._AccessEditingDocumentDescriptionLogic.Get();
        var documentDescription = await editingDocumentDescriptionLogic.GetByIdAsync(id, requestAborted);
        if (documentDescription is null) { return null; }
        return await this.UpdateAsync(documentDescription, value, requestAborted); 
    }
    public async Task<SADocument?> UpdateAsync(SADocumentDescription documentDescription, SADocument value, CancellationToken requestAborted) {
        await this._EditorPersistenceService.WriteEditingDocumentAsync(documentDescription, value, requestAborted);
        return value;
    }

    public async Task DeleteAsync(Guid id, CancellationToken requestAborted) {
        var editingDocumentDescriptionLogic = this._AccessEditingDocumentDescriptionLogic.Get();
        await editingDocumentDescriptionLogic.DeleteAsync(id, requestAborted); 
    }
}

public sealed class AccessEditingDocumentLogic {
    private readonly IServiceProvider? _ServiceProvider;
    private EditingDocumentLogic? _Instance;

    public static AccessEditingDocumentLogic Create(EditingDocumentLogic instance)
        => new AccessEditingDocumentLogic(instance);

    private AccessEditingDocumentLogic(EditingDocumentLogic instance) {
        this._Instance = instance;
    }

    [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
    public AccessEditingDocumentLogic(
        IServiceProvider serviceProvider
        ) {
        this._ServiceProvider = serviceProvider;
    }

    public EditingDocumentLogic Get() {
        if (this._Instance is { } result) {
            return result;
        }
        if (this._ServiceProvider is { } serviceProvider) {
            result = this._ServiceProvider.GetRequiredService<EditingDocumentLogic>();
            return this._Instance = result;
        }
        throw new NotSupportedException("Logic Error");
    }
}

