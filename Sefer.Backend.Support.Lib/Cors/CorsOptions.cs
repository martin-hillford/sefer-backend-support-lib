namespace Sefer.Api.Support.Cors;

/// <summary>
/// Contains options cross origin resource sharing
/// </summary>
public class CorsOptions
{
    /// <summary>
    /// A list of allowed origins
    /// </summary>
    public string? AllowedOrigins { get; set; }

    /// <summary>
    /// Use a hash set to internally cache the origins
    /// </summary>
    private HashSet<string>? Cached { get; set; }

    /// <summary>
    /// Defines the how long the results of a preflight request can be cached (seconds).
    /// </summary>
    /// <remarks>https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Access-Control-Max-Age</remarks>
    public int MaxAge { get; set; }

    /// <summary>
    /// Creates a new CorsOptions object
    /// </summary>
    public ReadOnlyCollection<string> ParseAllowedOrigins()
    {
        return GetCachedOrigins().ToList().AsReadOnly();
    }

    private HashSet<string> GetCachedOrigins()
    {
        if (Cached != null) return Cached;
        var origins = AllowedOrigins?.Split(';') ?? [];
        Cached = origins.Select(Clean).ToHashSet();
        return Cached;
    }

    /// <summary>
    /// This method will return if this is allowed origin
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    public bool IsAllowed(string? origin)
    {
        if (origin == null) return false;
        var clean = Clean(origin);
        var allowed = GetCachedOrigins();
        if (string.IsNullOrEmpty(clean)) return false;
        return allowed.Contains(clean) || allowed.Any(a => origin.EndsWith(a));
    }

    /// <summary>
    /// Clear the incoming origin
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    private static string Clean(string? origin)
    {
        if (string.IsNullOrEmpty(origin)) return string.Empty;
        return origin.EndsWith('/') == false ? origin : origin[..^1];
    }
}
