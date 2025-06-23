using Dusty.Shared;
using Dusty.Web.Components;
using Microsoft.Extensions.Options;
using NATS.Client.Core;
using NATS.Extensions.Microsoft.DependencyInjection;

namespace Dusty.Web;

public static class Startup
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var config = builder.AddConfig();
        builder.Services
            .AddHttpContextAccessor()
            .AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddNatsClient(nats => nats
            .ConfigureOptions(opts => opts with
            {
                Url = config.NatsUrl,
                AuthOpts = NatsAuthOpts.Default with
                {
                    Username = config.NatsUser,
                    Password = config.NatsPassword,
                },
            })
        );
        
        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAntiforgery();
        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        
        return app;
    }
    
    private static AppConfig AddConfig(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddEnvironmentVariables("DUST_");
        builder.Configuration.AddJsonFile("appConfig.json", optional: true);
        builder.Services.Configure<AppConfig>(builder.Configuration);
        builder.Services.AddTransient<AppConfig>(x => x.GetRequiredService<IOptions<AppConfig>>().Value);

        return builder.Configuration.Get<AppConfig>()!;
    }
}