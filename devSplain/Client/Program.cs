using devSplain.Client;
using devSplain.Shared.Data;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Serilog;

//var builder = WebAssemblyHostBuilder.CreateDefault(args);
//builder.RootComponents.Add<App>("#app");
//builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//await builder.Build().RunAsync();

public class Program
{
    public static async Task Main(string[] args)
    {
        // Serilog configuration
        Log.Logger = new LoggerConfiguration()
            .WriteTo.BrowserConsole()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.Seq("http://localhost:5341/#/events")
            .CreateLogger();

        Log.Information("Building client services");

        try
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddHttpClient("OktaWASM.ServerAPI", client =>
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped(sp =>
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("OktaWASM.ServerAPI"));

            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.Authority = builder.Configuration.GetValue<string>("Okta:Authority");
                options.ProviderOptions.ClientId = builder.Configuration.GetValue<string>("Okta:ClientId");
                options.ProviderOptions.DefaultScopes.Add("openid");
                options.ProviderOptions.DefaultScopes.Add("groups");
                options.ProviderOptions.ResponseType = "code";

                options.UserOptions.RoleClaim = "roles";
            }).AddAccountClaimsPrincipalFactory<RoleClaimsPrincipalFactory>();

            builder.Services.AddApiAuthorization();
            builder.Services.AddTransient<RoleClaimsPrincipalFactory>();
            builder.Services.AddSingleton<UserDataStoreSingleton>();
            builder.Services.AddSingleton<PostDataStoreSingleton>();
            builder.Services.AddSingleton<BlobDataStoreSingleton>();
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();
            builder.Services.AddScoped(typeof(AccountClaimsPrincipalFactory<RemoteUserAccount>), typeof(RoleClaimsPrincipalFactory));

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();

            await builder.Build().RunAsync();
        }
        catch (Exception ex)
        {
            Log.Error("ERROR: {0}", ex);
        }
    }
}