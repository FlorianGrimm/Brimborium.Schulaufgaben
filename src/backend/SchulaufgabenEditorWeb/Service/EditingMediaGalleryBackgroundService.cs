// MIT - Florian Grimm


namespace Brimborium.Schulaufgaben.Service;

public class EditingMediaGalleryBackgroundService
    : BackgroundService {
    private readonly EditingMediaGalleryLogic _EditingMediaGalleryLogic;

    public EditingMediaGalleryBackgroundService(
        EditingMediaGalleryLogic editingMediaGalleryLogic
        ) {
        this._EditingMediaGalleryLogic = editingMediaGalleryLogic;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        var task = this._EditingMediaGalleryLogic.StartThumbnailQueueAsync(stoppingToken);
        await this._EditingMediaGalleryLogic.InitialScanAsync(stoppingToken).ConfigureAwait(false);
        this._EditingMediaGalleryLogic.WireFolderWatch(stoppingToken);
        await task;
    }
}
