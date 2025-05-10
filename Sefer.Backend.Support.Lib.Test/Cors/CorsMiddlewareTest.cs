namespace Sefer.Backend.Support.Lib.Test.Cors;

[TestClass]
public class CorsMiddlewareTest
{
    [TestMethod]
    public async Task Test_Invoke_NoOptionRequest()
    {
        var corsOptions = new CorsOptions { };
        var context = CreateContext("POST", "https://test.tld");

        await Invoke(context, corsOptions);

        context.Response.StatusCode.Should().Be(200);
        context.Response.Headers.AccessControlAllowOrigin.Should().BeNullOrEmpty();
        context.Response.Headers.AccessControlAllowCredentials.Should().BeNullOrEmpty();
    }

    [TestMethod]
    public async Task Test_Invoke_NoOptionRequestForAllowedOrigin()
    {
        const string origin = "https://test.tld";
        var options = new CorsOptions { AllowedOrigins = origin };
        var context = CreateContext("POST", origin);

        await Invoke(context, options);

        context.Response.StatusCode.Should().Be(200);
        context.Response.Headers.AccessControlAllowOrigin.ToString().Should().Be(origin);
        context.Response.Headers.AccessControlAllowCredentials.ToString().Should().Be("true");
    }

    [TestMethod]
    public async Task Test_Invoke_NoAcceptedOrigin()
    {
        var corsOptions = new CorsOptions { AllowedOrigins = "https://this.tld" };
        var context = CreateContext("OPTIONS", "https://test.tld");

        await Invoke(context, corsOptions);

        context.Response.StatusCode.Should().Be(400);
    }

    [TestMethod]
    public async Task Test_Invoke_AcceptedOrigin()
    {
        const string origin = "https://test.tld";
        var options = new CorsOptions { AllowedOrigins = origin, MaxAge = 3600 };
        var context = CreateContext("OPTIONS", origin);

        await Invoke(context, options);

        context.Response.StatusCode.Should().Be(204);
        context.Response.Headers.AccessControlAllowOrigin.ToString().Should().Be(origin);
        context.Response.Headers.AccessControlAllowCredentials.ToString().Should().Be("true");
        context.Response.Headers.AccessControlAllowMethods.ToString().Should().Be(CorsMiddleware.AllowedMethods);
        context.Response.Headers.AccessControlAllowHeaders.ToString().Should().Be(CorsMiddleware.AllowedHeaders);
        context.Response.Headers.AccessControlExposeHeaders.ToString().Should().Be(CorsMiddleware.AllowedHeaders);
        context.Response.Headers.Vary.ToString().Should().Be("Origin");
        context.Response.Headers.AccessControlMaxAge.ToString().Should().Be("3600");
        context.Response.Headers.AccessControlAllowOrigin.ToString().Should().Be(origin);
        context.Response.Headers.AccessControlAllowCredentials.ToString().Should().Be("true");
    }

    private static DefaultHttpContext CreateContext(string method, string origin)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = method;
        context.Request.Headers.Origin = origin;
        return context;
    }

    private static Task Invoke(HttpContext context, CorsOptions options)
    {
        static Task next(HttpContext value)
        {
            value.Response.StatusCode = 200;
            return Task.CompletedTask;
        }

        var middleware = new CorsMiddleware(next);
        return middleware.Invoke(context, Options.Create(options));
    }
}