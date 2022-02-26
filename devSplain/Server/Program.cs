using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using devSplain.Server;

public class Program
{
    public static void Main(string[] args)
    {
        // Serilog configuration
        Log.Logger = new LoggerConfiguration()
            .WriteTo.BrowserConsole()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.Seq("http://localhost:5341/#/events")
            .CreateLogger();

        Log.Information("Building Server side services.");

        CreateHostBuilder(args).Build().Run();
    }

    // This will allow development locally without changing connection strings
    // When deployed this will use the azure App Config Environment variables
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

// .NET 6 Minimal hosting model
//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllersWithViews();
//builder.Services.AddRazorPages();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseWebAssemblyDebugging();
//}
//else
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();

//app.UseBlazorFrameworkFiles();
//app.UseStaticFiles();

//app.UseRouting();


//app.MapRazorPages();
//app.MapControllers();
//app.MapFallbackToFile("index.html");

//app.Run();
