namespace Sefer.Api.Support;

public static class HttpRequestExtensions
{
    public static string? GetClientIpAddress(this IHttpContextAccessor contextAccessor) => contextAccessor?.HttpContext?.Request.GetClientIpAddress();

    public static string? GetClientIpAddress(this HttpRequest request)
    {
        var forwardedIp = GetAzureForwardedIp(request);
        if (!string.IsNullOrEmpty(forwardedIp)) return forwardedIp;

        var realIp = request?.Headers.Get("X-Real-IP");
        if (!string.IsNullOrEmpty(realIp)) return realIp!;

        return request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    public static string? Get(this IHeaderDictionary dictionary, string headerName)
    {
        try { if (dictionary.TryGetValue(headerName, out var value)) return value; }
        catch (Exception) { }
        return null;
    }

    private static string? GetAzureForwardedIp(HttpRequest request)
    {
        // The problems with azure is their linux apps are running behind
        // a proxy that will forward the request. This need to dealt with..
        var forwardedIp =
            request?.Headers.Get("x-Forwarded-For") ??
            request?.Headers.Get("X-Forwarded-For");

        // Azure also includes the port number for the forward,
        // this method is not interested in that..
        if (string.IsNullOrEmpty(forwardedIp)) return null;
        if (!forwardedIp.Contains(':')) return forwardedIp!;
        return forwardedIp.Split(':')[0]?.Trim();
    }

    /// <summary>
    /// This method returns the hostname that made the request.
    /// Header X-Forwarded-Host is given precedence over all others.
    /// </summary>
    public static string? GetHostname(this HttpRequest request)
    {
        var hostname =
            request?.Headers.Get("X-Forwarded-Host") ??
            request?.Host.Host ??
            request?.Headers.Get("Host");
        if (string.IsNullOrEmpty(hostname)) return null;
        return hostname;
    }
}