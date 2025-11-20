// MIT - Florian Grimm

using Brimborium.Schulaufgaben;
using Brimborium.Schulaufgaben.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _ = builder.Services
            .AddDbContext<ApplicationDbContext>(
                options => options.UseSqlite(connectionString));
        _ = builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        _ = builder.Services
            .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();
        _ = builder.Services.AddRazorPages();
        _ = builder.Services.AddAngularFileService()
            .Configure(options => {
                options.AngularPathPrefix.Add("client");
            });

        builder.Services.AddOptions<AppClientOptions>().BindConfiguration("Schulaufgaben");
        builder.Services
            .AddTransient<IConfigureOptions<AppClientOptions>, AppClientConfigureOptions>()
            .AddTransient<IValidateOptions<AppClientOptions>, AppClientValidateOptions>();

        builder.Services.AddSchulaufgabenClient()
            .Configure<IOptions<AppClientOptions>>((clientPersistenceOptions, appClientConfigureOptions) => {
                var srcOptionsValue = appClientConfigureOptions.Value;
                clientPersistenceOptions.Folder = srcOptionsValue.PublishFolder;
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            _ = app.UseMigrationsEndPoint();
        } else {
            _ = app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _ = app.UseHsts();
        }

        _ = app.UseHttpsRedirection();

        _ = app.UseRouting();

        _ = app.UseAuthorization();

        _ = app.UseAngularFileService();
        _ = app.MapStaticAssets();
        _ = app.MapRazorPages().WithStaticAssets();

        app.Run();
    }
}
