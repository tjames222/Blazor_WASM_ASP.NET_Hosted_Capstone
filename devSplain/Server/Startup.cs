using Azure.Storage.Blobs;
using devSplain.Server.Services;
using Okta.AspNetCore;
using Radzen;
using Serilog;

namespace devSplain.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Information("Application starting...");
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Information("Configuring services...");
            try
            {
                services.AddScoped<DialogService>();
                services.AddScoped<NotificationService>();
                services.AddScoped<TooltipService>();
                services.AddScoped<ContextMenuService>();
                services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstanceUserAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
                services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstancePostAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
                //services.AddSingleton<IBlobService>(InitializeAzureBlobClientInstance(Configuration.GetSection("BlobStorage")));

                services.AddRazorPages();

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
                    options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
                    options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
                })
                .AddOktaWebApi(new OktaWebApiOptions()
                {
                    OktaDomain = Configuration["Okta:OktaDomain"]
                });

                services.AddAuthorization();

                // TODO: Future implementation
                // services.AddTransient<IClaimsTransformation, GroupsToRolesTransformerService>();
                services.AddControllersWithViews();
                services.AddControllers();
            }
            catch (Exception ex)
            {
                Log.Error("ERROR: {0}", ex);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Log.Information("Configuring application environment.");
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseWebAssemblyDebugging();
                }
                else
                {
                    app.UseExceptionHandler("/Error");
                    // TODO: The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseBlazorFrameworkFiles();
                app.UseStaticFiles();
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapRazorPages();
                    endpoints.MapControllers();
                    endpoints.MapFallbackToFile("index.html");
                });
            }
            catch (Exception ex)
            {
                Log.Error("ERROR: {0}", ex);
            }
        }

        // User container
        private static async Task<CosmosDbUserService> InitializeCosmosClientInstanceUserAsync(IConfigurationSection configurationSection)
        {
            Log.Information("Connecting to Database and container 1...");
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("UserContainerName").Value;
            string account = configurationSection.GetSection("Account").Value;
            string key = configurationSection.GetSection("Key").Value;
            Microsoft.Azure.Cosmos.CosmosClient client = new(account, key);
            CosmosDbUserService cosmosDbService = new(client, databaseName, containerName);
            Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmosDbService;
        }

        // Post container
        private static async Task<CosmosDbPostService> InitializeCosmosClientInstancePostAsync(IConfigurationSection configurationSection)
        {
            Log.Information("Connecting to Database and container 2...");
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("PostContainerName").Value;
            string account = configurationSection.GetSection("Account").Value;
            string key = configurationSection.GetSection("Key").Value;
            Microsoft.Azure.Cosmos.CosmosClient client = new(account, key);
            CosmosDbPostService cosmosDbPostService = new(client, databaseName, containerName);
            Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmosDbPostService;
        }

        // Azure Blob storage setup
        //private static BlobService InitializeAzureBlobClientInstance(IConfigurationSection configurationSection)
        //{
        //    Log.Information("Connecting to Blob Storage Container...");
        //    string containerName = configurationSection.GetSection("ContainerName").Value;
        //    string connectionString = configurationSection.GetSection("ConnectionString").Value;
        //    BlobServiceClient blobServiceClient = new(connectionString);
        //    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        //    BlobService blobService = new(blobServiceClient, containerClient);

        //    return blobService;
        //}
    }
}
