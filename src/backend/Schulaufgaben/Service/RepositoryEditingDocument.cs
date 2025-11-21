// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Service;

public class RepositoryEditingDocument {
    private readonly EditorPersistenceService _EditorPersistenceService;

    public RepositoryEditingDocument(
        EditorPersistenceService editorPersistenceService
        ) {
        this._EditorPersistenceService = editorPersistenceService;
    }

    public async Task<SADocument?> GetDocumentAsync(
        SADocumentDescription documentDescription
        ) { 
        var folderFQN = this._EditorPersistenceService.GetEditingDocumentDescriptionFolder( documentDescription );
        await Task.CompletedTask;
        return null;
    }

    public async Task<SADocument> SetDocumentAsync(
        SADocumentDescription workDescription,
        SADocument value
        ) {
        var folderFQN = this._EditorPersistenceService.GetEditingDocumentDescriptionFolder(workDescription);
        await Task.CompletedTask;
        return value;
    }
}