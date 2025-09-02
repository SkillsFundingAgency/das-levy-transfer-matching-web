using Microsoft.Extensions.Primitives;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        const string dasCdn = "das-at-frnt-end.azureedge.net das-pp-frnt-end.azureedge.net das-mo-frnt-end.azureedge.net das-test-frnt-end.azureedge.net das-test2-frnt-end.azureedge.net das-prd-frnt-end.azureedge.net  das-demo-frnt-end.azureedge.net";

        context.Response.Headers.AddIfNotPresent("x-frame-options", new StringValues("DENY"));
        context.Response.Headers.AddIfNotPresent("x-content-type-options", new StringValues("nosniff"));
        context.Response.Headers.AddIfNotPresent("X-Permitted-Cross-Domain-Policies", new StringValues("none"));
        context.Response.Headers.AddIfNotPresent("x-xss-protection", new StringValues("0"));
        context.Response.Headers.AddIfNotPresent(
            "Content-Security-Policy",
            new StringValues(
                $"script-src 'self' 'unsafe-inline' 'unsafe-eval' {dasCdn} " +
                "https://das-prd-frnt-end.azureedge.net https://das-demo-frnt-end.azureedge.net https://das-pp-frnt-end.azureedge.net https://das-test-frnt-end.azureedge.net https://das-at-frnt-end.azureedge.net " +
                "*.googletagmanager.com *.google-analytics.com *.googleapis.com https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com; " +
                $"style-src 'self' 'unsafe-inline' {dasCdn} https://tagmanager.google.com https://fonts.googleapis.com https://*.rcrsv.io ; " +
                $"img-src 'self' {dasCdn} www.googletagmanager.com https://ssl.gstatic.com https://www.gstatic.com https://www.google-analytics.com ; " +
                $"font-src {dasCdn} https://fonts.gstatic.com https://*.rcrsv.io data: ;" +
                "connect-src 'self' https://*.google-analytics.com https://*.zendesk.com https://*.zdassets.com wss://*.zopim.com https://*.rcrsv.io ; " +
                "worker-src 'self' blob:;"));

        await next(context);
    }
}