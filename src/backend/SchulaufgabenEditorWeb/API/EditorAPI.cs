// MIT - Florian Grimm


using Microsoft.AspNetCore.Mvc;

namespace Brimborium.Schulaufgaben.API;

public sealed class EditorAPI {
    private readonly EditorPersistenceService _EditorPersistenceService;

    public EditorAPI(
        EditorPersistenceService editorPersistenceService
        ) {
        this._EditorPersistenceService = editorPersistenceService;
    }

    public void Map(WebApplication app) {
        var groupAPI = app.MapGroup("/API");
        var groupAPIWorkDescription = groupAPI.MapGroup("/WorkDescription");
        //
        groupAPIWorkDescription.MapGet("/", async (HttpContext httpContext) => {
            List<SAWorkDescription> result = await this._EditorPersistenceService
                .GetWorkDescriptionListFromWorkingAsync(httpContext.RequestAborted).ConfigureAwait(false);
            return TypedResults.Ok(result);
        });
        groupAPIWorkDescription.MapGet("/{id}", async (HttpContext httpContext, [FromRoute] Guid id) => {
            SAWorkDescription? result = await this._EditorPersistenceService
                .GetWorkDescriptionByIdFromWorkingAsync(id, httpContext.RequestAborted).ConfigureAwait(false);
            return TypedResults.Ok(result);
        });
        groupAPIWorkDescription.MapPut("/", async (HttpContext httpContext, [FromBody] SAWorkDescription value) => {
            SAWorkDescription? result = await this._EditorPersistenceService
                .CreateWorkDescriptionFromWorkingAsync(value, httpContext.RequestAborted).ConfigureAwait(false);
            return TypedResults.Ok(result);
        });
        groupAPIWorkDescription.MapPost("/{id}", async (HttpContext httpContext, [FromRoute] Guid id, [FromBody] SAWorkDescription value) => {
            SAWorkDescription? result = await this._EditorPersistenceService
                .UpdateWorkDescriptionFromWorkingAsync(id, value, httpContext.RequestAborted).ConfigureAwait(false);
            return TypedResults.Ok(result);
        });
        groupAPIWorkDescription.MapDelete("/{id}", async (HttpContext httpContext, [FromRoute] Guid id) => {
            await this._EditorPersistenceService
                .DeleteWorkDescriptionFromWorkingAsync(id, httpContext.RequestAborted).ConfigureAwait(false);
            return TypedResults.Ok();
        });
    }
}
