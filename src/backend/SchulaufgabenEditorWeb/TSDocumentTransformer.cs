// MIT - Florian Grimm

using Microsoft.OpenApi;
using Microsoft.AspNetCore.OpenApi;

namespace SchulaufgabenEditorWeb;

public class TSDocumentTransformer : IOpenApiDocumentTransformer {
    public Task TransformAsync(
        OpenApiDocument document, 
        OpenApiDocumentTransformerContext context, 
        CancellationToken cancellationToken) {
        //System.Console.Error.WriteLine("HERE");
        //System.Diagnostics.Debugger.Launch();
        return Task.CompletedTask;
    }
}
