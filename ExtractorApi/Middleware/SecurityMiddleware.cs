namespace ExtractorApi.Middleware
{
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private const string API_KEY_SETTING_NAME = "Security:ExtractorApiKey";
        private const string API_KEY_RECEIVED = "ExtractorApiKey";

        public SecurityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(API_KEY_RECEIVED, out
                    var apiKeyReceived))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Api Key is missing");
                return;
            }

            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            var apiKeySettingValue = appSettings.GetValue<string>(API_KEY_SETTING_NAME);

            if (!apiKeySettingValue.Equals(apiKeyReceived))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }
            await _next(context);
        }
    }
}
