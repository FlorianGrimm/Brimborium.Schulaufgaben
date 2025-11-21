// MIT - Florian Grimm



namespace Brimborium.Schulaufgaben.Service;

public class EditingMediaGalleryOptions {
    public List<EditingMediaGallery> ListMediaGallery { get; set; } = [];
}
public sealed record EditingMediaGallery(
    string FolderPath
    );

public sealed class EditingMediaGalleryLogic {
    private readonly RepositoryEditingMediaGallery _RepositoryEditingMediaGallery;
    private List<EditingMediaGallery> _ListMediaGallery;

    public EditingMediaGalleryLogic(
        RepositoryEditingMediaGallery repositoryEditingMediaGallery,
        IOptionsMonitor<EditingMediaGalleryOptions> options
        ) {
        this._ListMediaGallery = [];
        this._RepositoryEditingMediaGallery = repositoryEditingMediaGallery;
        options.OnChange(this.OnOptionsChange);
        this.OnOptionsChange(options.CurrentValue);
    }

    private void OnOptionsChange(EditingMediaGalleryOptions options) {
        this._ListMediaGallery = new(options.ListMediaGallery);
    }

    public async Task InitialScanAsync(CancellationToken stoppingToken) {
        await Task.CompletedTask;
        //TODO
    }
}