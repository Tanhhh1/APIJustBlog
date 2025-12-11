using System.Text.Json;
using System.Threading.RateLimiting;

namespace API_Blog.Register
{
    public static class RateLimitingRegister
    {
        public static void AddRateLimitingPolicies(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? "guest",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2000,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));

                options.AddPolicy("CrudPolicy", httpContext =>
                {
                    var key = $"{httpContext.User.Identity?.Name ?? "guest"}:{httpContext.Request.Path}";
                    return RateLimitPartition.GetTokenBucketLimiter(key, _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 100,
                        TokensPerPeriod = 20,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                        AutoReplenishment = true,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });

                options.AddPolicy("AuthPolicy", httpContext =>
                {
                    var key = $"{httpContext.User.Identity?.Name ?? "anonymous"}:{httpContext.Request.Path}";
                    return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, ct) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";
                    var payload = JsonSerializer.Serialize(new { message = "Too many requests. Try again later." });
                    await context.HttpContext.Response.WriteAsync(payload, ct);
                };
            });
        }
    }
}
