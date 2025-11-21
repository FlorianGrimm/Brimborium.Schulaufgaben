// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Service;

public class EditingMediaLogic {
    private readonly RepositoryEditingMedia _RepositoryEditingMedia;

    public EditingMediaLogic(
        RepositoryEditingMedia repositoryEditingMedia
        ) {
        this._RepositoryEditingMedia = repositoryEditingMedia;
    }
}
