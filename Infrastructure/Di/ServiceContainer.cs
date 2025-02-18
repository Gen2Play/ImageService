using Application.Image.Interface;
using Infrastructure.Cloud;
using Infrastructure.Context;
using Infrastructure.Context.Initalization;
using Infrastructure.Images;
using Infrastructure.Tags;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Cloud;
using Shared.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Di;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
    {
        SharedServiceContainer.AddSharedServices<ApplicationDbContext>(services, config, config["MySerilog:ProductLog"]);

        services.Configure<CloudinarySetting>(config.GetSection("Cloudinary"));

        services.AddScoped<ICloudInterface, CloudService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<ITagService, TagService>();
        services.AddTransient<DatabaseInitializer>();
        services.AddTransient<ApplicationDbSeeder>();
        //services.AddServices(typeof(ICustomSeeder), ServiceLifetime.Transient);
        services.AddTransient<CustomSeederRunner>();
        return services;
    }

    public static IApplicationBuilder UseInFrastructure(this IApplicationBuilder app)
    {
        SharedServiceContainer.UseSharedPolicies(app);

        return app;
    }
    public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        await SharedServiceContainer.InitializeDatabasesAsync<ApplicationDbContext>(services, cancellationToken);

        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<DatabaseInitializer>()
            .InitializeAsync(cancellationToken);
    }
}
