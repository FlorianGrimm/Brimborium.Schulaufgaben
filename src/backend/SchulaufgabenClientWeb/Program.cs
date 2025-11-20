using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Briumborium.Schulaufgaben.Data;

namespace SchulaufgabenClientWeb {
    public class Program {
        public static void Main(string[] args) {
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

            _ = app.MapStaticAssets();
            _ = app.UseAngularFileService();
            _ = app.MapRazorPages().WithStaticAssets();

            app.Run();
        }
    }
}
