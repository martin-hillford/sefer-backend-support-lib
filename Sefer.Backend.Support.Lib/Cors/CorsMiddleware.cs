namespace Sefer.Backend.Support.Lib.Cors;

public class CorsMiddleware(RequestDelegate next)
{
    public const string AllowedHeaders = "Accept, Content-Type, Origin, Authorization, X-BrowserToken, X-AccessToken, X-ProcessingTime, X-Authorization, X-RequestPath, X-Requested-With, Set-Cookie, X-SignalR-User-Agent";

    public const string AllowedMethods = "GET, POST, PUT, DELETE";

    private const bool WithCredentials = true;

    private readonly RequestDelegate _next = next;

    public Task Invoke(HttpContext context, IOptions<CorsOptions> options)
    {
        // For all the request set the origin header
        SetDefaultHeaders(context, options.Value);

        // Check with the this is options request to process or a regular request
        var isOptionsRequest = context.Request.Method.Equals("options", StringComparison.CurrentCultureIgnoreCase);
        if (!isOptionsRequest) return _next(context);

        // Check if the origin is an allowed origin
        var origin = context.Request.Headers.Origin.FirstOrDefault();
        if (!options.Value.IsAllowed(origin))
        {
            context.Response.StatusCode = 400;
        }
        // The origin is allowed, send the right headers
        else
        {
            SetAccessControlHeaders(context.Response.Headers, options.Value);
            context.Response.StatusCode = 204;
        }

        return Task.CompletedTask;
    }

    public static void SetDefaultHeaders(HttpContext context, CorsOptions options)
    {
        var origin = context.Request.Headers.Origin.FirstOrDefault();
        if (!options.IsAllowed(origin)) return;
        if (WithCredentials) context.Response.Headers.AccessControlAllowCredentials = "true";
        context.Response.Headers.AccessControlAllowOrigin = origin;
    }

    public static void SetAccessControlHeaders(IHeaderDictionary headers, CorsOptions options)
    {
        headers.AccessControlMaxAge = options.MaxAge.ToString();
        headers.AccessControlAllowMethods = AllowedMethods;
        headers.AccessControlAllowHeaders = AllowedHeaders;
        headers.AccessControlExposeHeaders = AllowedHeaders;
        headers.Vary = "Origin";
    }

}