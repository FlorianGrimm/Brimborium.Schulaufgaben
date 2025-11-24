// MIT - Florian Grimm

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Brimborium.Schulaufgaben.Data;
using Brimborium.Schulaufgaben;
using Brimborium.Schulaufgaben.Service;
using System.Threading.Tasks;
using Scalar.AspNetCore;

namespace SchulaufgabenClientWeb;

public class Program {
    public static async Task<int> Main(string[] args) {
        try {
            await Run(args);
            return 0;
        } catch (AggregateException error) {
            Console.WriteLine(error.ToString());
            error.Handle((innerError) => {
                Console.WriteLine(innerError.ToString());
                return true;
            });
            return 1;
        } catch (Exception error) {
            Console.WriteLine(error.ToString());
            return 1;
        }
    }
    public static async Task Run(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi(options => {
        });
        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        //?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        if (connectionString is { Length: > 0 }) {

            _ = builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));
            _ = builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            _ = builder.Services
                .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
        } else {
            builder.Services.AddAuthorization();
        }
        _ = builder.Services.AddRazorPages();
        builder.Services.AddAngularFileService()
            .Configure(options => {
                options.AngularPathPrefix.Add("editor");
            });
        builder.Services.AddOptions<AppEditorOptions>().BindConfiguration("Schulaufgaben");
        var schulaufgabenEditorBuilder = builder.Services
            .AddTransient<IConfigureOptions<AppEditorOptions>, AppEditorConfigureOptions>()
            .AddTransient<IValidateOptions<AppEditorOptions>, AppEditorValidateOptions>()
            .AddSchulaufgabenEditor();
        schulaufgabenEditorBuilder.EditorPersistenceOptionsBuilder
            .BindConfiguration("Schulaufgaben")
            .Configure<IOptions<AppEditorOptions>>((editorPersistenceOptions, appEditorOptions) => {
                var srcOptionsValue = appEditorOptions.Value;
                editorPersistenceOptions.EditingFolder = srcOptionsValue.EditingFolder;
                editorPersistenceOptions.PublishFolder = srcOptionsValue.PublishFolder;
            });

        schulaufgabenEditorBuilder.EditingMediaGalleryOptionsBuilder
            .Configure<IOptions<AppEditorOptions>>((editorPersistenceOptions, appEditorOptions) => {
                var srcOptionsValue = appEditorOptions.Value;
                editorPersistenceOptions.ThumbnailFolder = srcOptionsValue.ThumbnailFolder;
                foreach (var item in srcOptionsValue.ListMediaGallery) {
                    if (item.FolderPath is { } folderPath) {
                        editorPersistenceOptions.ListMediaGallery.Add(
                            new EditingMediaGallery(folderPath));
                    }
                }

            });

        builder.Services.AddSingleton<EditorAPI>();
        builder.Services.AddHostedService<EditingMediaGalleryBackgroundService>();
        

        var app = builder.Build();

        _ = app.MapOpenApi().AllowAnonymous();
        _ = app.MapScalarApiReference();

        // TODO: prefetch
        // await app.Services.GetRequiredService<EditorPersistenceService>().SAWorkDescriptionListReadFromWorkingAsync(CancellationToken.None);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            _ = app.UseMigrationsEndPoint();
        } else {
            _ = app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _ = app.UseHsts();
        }

        //app.UseHttpsRedirection();

        _ = app.UseRouting();

        _ = app.UseAuthorization();

        _ = app.UseAngularFileService();
        _ = app.MapStaticAssets();
        _ = app.MapRazorPages().WithStaticAssets();
        app.Services.GetRequiredService<EditorAPI>().Map(app);

        await app.RunAsync();
    }
}
