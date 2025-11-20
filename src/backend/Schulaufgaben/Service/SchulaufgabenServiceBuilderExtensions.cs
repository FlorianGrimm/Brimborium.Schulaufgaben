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

    public static OptionsBuilder<EditorPersistenceOptions> AddSchulaufgabenEditor(
        this IServiceCollection serviceBuilder
        ) {
        var result = serviceBuilder.AddOptions<EditorPersistenceOptions>();
        serviceBuilder.AddTransient<Microsoft.Extensions.Options.IValidateOptions<EditorPersistenceOptions>, EditorPersistenceValidateOptions>();
        serviceBuilder.AddSingleton<EditorPersistenceService>();
        return result;
    }
}