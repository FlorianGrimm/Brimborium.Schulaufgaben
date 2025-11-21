// MIT - Florian Grimm


using Microsoft.AspNetCore.Mvc;

namespace Brimborium.Schulaufgaben.API;

public sealed class EditorAPI {
    private readonly EditingDocumentDescriptionLogic _EditingDocumentDescriptionLogic;
    private readonly EditingDocumentLogic _EditingDocumentLogic;
    private readonly EditingMediaGalleryLogic _EditingMediaGalleryLogic;

    public EditorAPI(
        EditingDocumentDescriptionLogic editingDocumentDescriptionLogic,
        EditingDocumentLogic editingDocumentLogic,
        EditingMediaGalleryLogic editingMediaGalleryLogic
        ) {
        this._EditingDocumentDescriptionLogic = editingDocumentDescriptionLogic;
        this._EditingDocumentLogic = editingDocumentLogic;
        this._EditingMediaGalleryLogic = editingMediaGalleryLogic;
    }

    public void Map(WebApplication app) {
        var groupAPI = app.MapGroup("/API");

        {
            var groupAPIDocumentDescription = groupAPI.MapGroup("/DocumentDescription");
            //
            groupAPIDocumentDescription.MapGet("/", async (HttpContext httpContext) => {
                List<SADocumentDescription> result = await this._EditingDocumentDescriptionLogic
                    .GetListAsync(httpContext.RequestAborted).ConfigureAwait(false);
                return TypedResults.Ok(result);
            });
            groupAPIDocumentDescription.MapGet("/{id}", async (HttpContext httpContext, [FromRoute] Guid id) => {
                SADocumentDescription? result = await this._EditingDocumentDescriptionLogic
                    .GetByIdAsync(id, httpContext.RequestAborted).ConfigureAwait(false);
                return TypedResults.Ok(result);
            });
            groupAPIDocumentDescription.MapPut("/", async (HttpContext httpContext, [FromBody] SADocumentDescription value) => {
                SADocumentDescription? result = await this._EditingDocumentDescriptionLogic
                    .CreateAsync(value, httpContext.RequestAborted).ConfigureAwait(false);
                return TypedResults.Ok(result);
            });
            groupAPIDocumentDescription.MapPost("/{id}", async (HttpContext httpContext, [FromRoute] Guid id, [FromBody] SADocumentDescription value) => {
                SADocumentDescription? result = await this._EditingDocumentDescriptionLogic
                    .UpdateAsync(id, value, httpContext.RequestAborted).ConfigureAwait(false);
                return TypedResults.Ok(result);
            });
            groupAPIDocumentDescription.MapDelete("/{id}", async (HttpContext httpContext, [FromRoute] Guid id) => {
                await this._EditingDocumentDescriptionLogic
                    .DeleteAsync(id, httpContext.RequestAborted).ConfigureAwait(false);
                return TypedResults.Ok();
            });
        }

    
        {
            var groupAPIDocument = groupAPI.MapGroup("/Document");
            //
            // groupAPIDocument.MapGet("/", async (HttpContext httpContext) => {
            //     List<SADocumentDescription> result = await this._EditingDocumentLogic
            //         .GetListAsync(httpContext.RequestAborted).ConfigureAwait(false);
            //     return TypedResults.Ok(result);
            // });
            groupAPIDocument.MapGet("/{id}", async (HttpContext httpContext, [FromRoute] Guid id) => {
                SADocument? result = await this._EditingDocumentLogic
                    .GetByIdAsync(id, httpContext.RequestAborted).ConfigureAwait(false);
                return TypedResults.Ok(result);
            });
            groupAPIDocument.MapPut("/", async (HttpContext httpContext, [FromBody] SADocument value) => {
                SADocument? result = await this._EditingDocumentLogic
                    .CreateAsync(value, httpContext.RequestAborted).ConfigureAwait(false);
                return TypedResults.Ok(result);
            });
            groupAPIDocument.MapPost("/{id}", async (HttpContext httpContext, [FromRoute] Guid id, [FromBody] SADocument value) => {
                SADocument? result = await this._EditingDocumentLogic
                    .UpdateAsync(id, value, httpContext.RequestAborted).ConfigureAwait(false);
                return TypedResults.Ok(result);
            });
            groupAPIDocument.MapDelete("/{id}", async (HttpContext httpContext, [FromRoute] Guid id) => {
                await this._EditingDocumentLogic
                    .DeleteAsync(id, httpContext.RequestAborted).ConfigureAwait(false);
                return TypedResults.Ok();
            });
        }   

        {
            var groupAPIDocument = groupAPI.MapGroup("/Media");
            
            // returns the media as a stream.
            groupAPIDocument.MapGet("/{name}", async (HttpContext httpContext, [FromRoute] string name) => {
                var result = await this._EditingMediaGalleryLogic.GetAsync(name, httpContext.RequestAborted).ConfigureAwait(false);
                if (result is null) { return TypedResults.NotFound(); }
                //httpContext.Response.ContentType = result.ContentType;
                //await result.Stream.CopyToAsync(httpContext.Response.Body, httpContext.RequestAborted);
                return TypedResults.PhysicalFile(result.Stream, result.ContentType);
            });

            //
            groupAPIDocument.MapPost("/Search", async (HttpContext httpContext, [FromBody] string value) => {
                List<SAMediaInfo> result = await this._EditingMediaGalleryLogic.SearchAsync(value, httpContext.RequestAborted).ConfigureAwait(false);
                return TypedResults.Ok(result);
            });
        }    
    }
}
