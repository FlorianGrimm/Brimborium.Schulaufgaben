// MIT - Florian Grimm


using Microsoft.AspNetCore.Mvc;

namespace Brimborium.Schulaufgaben.API;

public sealed class EditorAPI {
    private readonly EditingDocumentDescriptionLogic _Logic;

    public EditorAPI(
        EditingDocumentDescriptionLogic logic
        ) {
        this._Logic = logic;
    }

    public void Map(WebApplication app) {
        var groupAPI = app.MapGroup("/API");
        var groupAPIWorkDescription = groupAPI.MapGroup("/DocumentDescription");
        //
        groupAPIWorkDescription.MapGet("/", async (HttpContext httpContext) => {
            List<SADocumentDescription> result = await this._Logic
                .GetListAsync(httpContext.RequestAborted).ConfigureAwait(false);
            return TypedResults.Ok(result);
        });
        groupAPIWorkDescription.MapGet("/{id}", async (HttpContext httpContext, [FromRoute] Guid id) => {
            SADocumentDescription? result = await this._Logic
                .GetByIdAsync(id, httpContext.RequestAborted).ConfigureAwait(false);
            return TypedResults.Ok(result);
        });
        groupAPIWorkDescription.MapPut("/", async (HttpContext httpContext, [FromBody] SADocumentDescription value) => {
            SADocumentDescription? result = await this._Logic
                .CreateAsync(value, httpContext.RequestAborted).ConfigureAwait(false);
            return TypedResults.Ok(result);
        });
        groupAPIWorkDescription.MapPost("/{id}", async (HttpContext httpContext, [FromRoute] Guid id, [FromBody] SADocumentDescription value) => {
            SADocumentDescription? result = await this._Logic
                .UpdateAsync(id, value, httpContext.RequestAborted).ConfigureAwait(false);
            return TypedResults.Ok(result);
        });
        groupAPIWorkDescription.MapDelete("/{id}", async (HttpContext httpContext, [FromRoute] Guid id) => {
            await this._Logic
                .DeleteAsync(id, httpContext.RequestAborted).ConfigureAwait(false);
            return TypedResults.Ok();
        });
    }
}
