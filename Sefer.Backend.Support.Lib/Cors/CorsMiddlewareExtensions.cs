namespace Sefer.Api.Support.Cors;

public static class CorsMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomCorsMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorsMiddleware>();
    }

    public static IHostApplicationBuilder AddCustomCorsMiddleware(this IHostApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSection("Cors");
        if(options == null) throw new Exception("Please ensure the cors configuration is added in the section 'Cors'");
        builder.Services.Configure<CorsOptions>(options);
        return builder;
    }
}