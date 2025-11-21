// MIT - Florian Grimm


namespace Brimborium.Schulaufgaben.Service;

public class EditingDocumentLogic {
    private readonly EditorPersistenceService _EditorPersistenceService;

    public EditingDocumentLogic(
        EditorPersistenceService editorPersistenceService) {
        this._EditorPersistenceService = editorPersistenceService;
    }

    public async Task<SADocument> CreateAsync(
        SADocumentDescription documentDescription,
        SADocument document,
        CancellationToken requestAborted) {
        var folderFQN = this._EditorPersistenceService.GetEditingDocumentDescriptionFolder(documentDescription);
        await this._EditorPersistenceService.WriteEditingDocumentAsync(documentDescription, document, requestAborted);
        return document;
    }
}

public sealed class AccessEditingDocumentLogic {
    private readonly IServiceProvider? _ServiceProvider;
    private EditingDocumentLogic? _Instance;

    public static AccessEditingDocumentLogic Create(EditingDocumentLogic instance) 
        =>new AccessEditingDocumentLogic(instance);

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

