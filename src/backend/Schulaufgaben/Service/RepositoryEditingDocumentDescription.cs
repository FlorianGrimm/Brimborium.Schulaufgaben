// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Service;

public class RepositoryEditingDocumentDescription {
    private readonly EditorPersistenceService _EditorPersistenceService;

    public RepositoryEditingDocumentDescription(
        EditorPersistenceService editorPersistenceService
        ) {
        this._EditorPersistenceService = editorPersistenceService;
    }


    private List<SADocumentDescription>? _State;
    public async Task<List<SADocumentDescription>> GetListAsync(CancellationToken cancellationToken) {
        {
            if (this._State is { } result) {
                return result;
            }
        }
        {
            if (await this._EditorPersistenceService.ReadEditingListDocumentDescriptionListAsync(cancellationToken) is { } result) {
                return this._State = result;
            }
        }
        {
            return this._State = [];
        }
    }

    public async Task SetListAsync(
        List<SADocumentDescription> value,
        CancellationToken cancellationToken) {
        this._State = new(value);
        await this._EditorPersistenceService.WriteEditingDocumentDescriptionListAsync(value, cancellationToken).ConfigureAwait(false);
    }
}
