using Microsoft.OpenApi;

namespace WarehouseExecution.Api.Builder;

public static class Swagger
{
    private const string SwaggerVersion = "v1";
    public const string SwaggerName = "WarehouseExecution API";
    public const string SwaggerJsonUrl = "/swagger/v1/swagger.json";
    public const string SwaggerPrefix = "docs/swagger/ui";

    public static IServiceCollection BuildSwagger(this IServiceCollection services)
    {
        // Swagger (register services)
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(SwaggerVersion, new OpenApiInfo { Title = SwaggerName, Version = SwaggerVersion });
        });
        
        return services;
    }
}