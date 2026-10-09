namespace PennyEcommerce.Middlewares;

public class ProductMaintenanceMiddleware
{
    private readonly RequestDelegate _next;

    public ProductMaintenanceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IConfiguration configuration)
    {
        bool isMaintenance =
            configuration.GetValue<bool>("MaintenanceMode");

        bool isProductPage =
            context.Request.Path.StartsWithSegments("/Product");

        if (isMaintenance && isProductPage)
        {
            context.Response.StatusCode = 503;
            context.Response.ContentType = "text/html; charset=utf-8";

            await context.Response.WriteAsync(
                "<h2>Chức năng sản phẩm đang bảo trì!</h2>" +
                "<p>Vui lòng quay lại sau.</p>"
            );

            return;
        }

        await _next(context);
    }
}