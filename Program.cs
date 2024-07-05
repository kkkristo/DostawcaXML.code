using DostawcaXML.Services;
using DostawcaXML.Services.Contracts;

namespace DostawcaXML
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IFileManagerService, FileManagerService>();
            builder.Services.AddScoped<IFileImporterService, FileImporterService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
           // app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
