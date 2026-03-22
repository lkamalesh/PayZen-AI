using Azure.Core;
using Microsoft.Extensions.Primitives;
using PaymentAI.Core.DTOs;
using PaymentAI.Core.Interfaces.Services;
using PaymentAI.Infrastructure.Services;

namespace PaymentAI.API.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMerchantService _merchantServices;
        private readonly ILogger<ApiKeyMiddleware> _logger;

        public ApiKeyMiddleware(RequestDelegate next, IMerchantService merchantServices, ILogger<ApiKeyMiddleware> logger)
        {
            _next = next;
            _merchantServices = merchantServices;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/payment", StringComparison.OrdinalIgnoreCase))
            {
                if (!context.Request.Headers.TryGetValue("x-api-key", out var apiKey) || StringValues.IsNullOrEmpty(apiKey))
                {
                    _logger.LogWarning("Missing API key for path {Path}", context.Request.Path);
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("API Key is missing");
                    return;
                }

                var merchant = await _merchantServices.GetMerchantByApikeyAsync(apiKey.ToString());
                if (merchant == null)
                {
                    _logger.LogWarning("Invalid API key used for path {Path}", context.Request.Path);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Invalid API Key!");
                    return;
                }

                _logger.LogDebug("API key validated for merchant {MerchantId}", merchant.MerchantId);
                context.Items["Merchant"] = merchant;
            }

            await _next(context);
        }
    }

}
