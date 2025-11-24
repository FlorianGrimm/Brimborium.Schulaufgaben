// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Service;

public static class SchulaufgabenServiceBuilderExtensions {
    public static OptionsBuilder<ClientPersistenceOptions> AddSchulaufgabenClient(
        this IServiceCollection serviceBuilder
        ) {
        var result = serviceBuilder.AddOptions<ClientPersistenceOptions>();
        serviceBuilder.AddTransient<Microsoft.Extensions.Options.IValidateOptions<ClientPersistenceOptions>, ClientPersistenceValidateOptions>();
        serviceBuilder.AddSingleton<ClientPersistenceService>();
        return result;
    }

    public static SchulaufgabenEditorBuilder AddSchulaufgabenEditor(
        this IServiceCollection serviceBuilder
        ) {
        var editorPersistenceOptionsBuilder = serviceBuilder.AddOptions<EditorPersistenceOptions>();
        var editingMediaGalleryOptionsBuilder = serviceBuilder.AddOptions<EditingMediaGalleryOptions>();
        SchulaufgabenEditorBuilder result = new SchulaufgabenEditorBuilder(
            serviceBuilder,
            editorPersistenceOptionsBuilder,
            editingMediaGalleryOptionsBuilder);

        serviceBuilder.AddTransient<Microsoft.Extensions.Options.IValidateOptions<EditorPersistenceOptions>, EditorPersistenceValidateOptions>();
        serviceBuilder.AddSingleton<EditorPersistenceService>();

        serviceBuilder.AddSingleton<RepositoryEditingDocumentDescription>();
        serviceBuilder.AddSingleton<EditingDocumentDescriptionLogic>();
        serviceBuilder.AddSingleton<AccessEditingDocumentDescriptionLogic>();

        serviceBuilder.AddSingleton<RepositoryEditingDocument>();
        serviceBuilder.AddSingleton<EditingDocumentLogic>();
        serviceBuilder.AddSingleton<AccessEditingDocumentLogic>();

        serviceBuilder.AddSingleton<RepositoryEditingMedia>();
        serviceBuilder.AddSingleton<EditingMediaLogic>();

        serviceBuilder.AddSingleton<RepositoryEditingMediaGallery>();
        serviceBuilder.AddSingleton<EditingMediaGalleryLogic>();

        return result;
    }
}

public sealed record SchulaufgabenEditorBuilder(
    IServiceCollection Services,
    OptionsBuilder<EditorPersistenceOptions> EditorPersistenceOptionsBuilder,
    OptionsBuilder<EditingMediaGalleryOptions> EditingMediaGalleryOptionsBuilder
    );