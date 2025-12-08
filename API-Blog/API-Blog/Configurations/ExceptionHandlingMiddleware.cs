using Application.Common.ModelServices;
using Application.Exceptions;
using Shared.Logger;
using System.Text.Json;

namespace API_Blog.Configurations
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var correlationId = httpContext.Request.Headers.ContainsKey("X-Correlation-ID")
            ? httpContext.Request.Headers["X-Correlation-ID"].ToString()
            : Guid.NewGuid().ToString();

            httpContext.Response.Headers["X-Correlation-ID"] = correlationId;

            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    await _next(httpContext);
                }
                catch (Exception ex)
                {
                    await HandleException(httpContext, ex);
                }
            }
        }

        private Task HandleException(HttpContext httpContext, Exception ex)
        {
            var statusCode = StatusCodes.Status500InternalServerError;
            var errors = ex.Message.Split("|\n\b|").ToList();
            statusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound, //Không tìm thấy tài nguyên
                UnauthorizedException => StatusCodes.Status401Unauthorized, //Token không hợp lệ
                ForbiddenException => StatusCodes.Status403Forbidden, //Không có quyền truy cập
                ResourceNotFoundException => StatusCodes.Status404NotFound,
                BadRequestException => StatusCodes.Status400BadRequest, //Dữ liệu gửi lên sai
                ForgeException => StatusCodes.Status400BadRequest,
                UnprocessableRequestException => StatusCodes.Status422UnprocessableEntity, //Validation lỗi
                _ => statusCode
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                Logging.Error(ex, "Unhandled exception at {Path}", httpContext.Request.Path);
                errors = new List<string> { "Server Error" };
            }
            else
            {
                Logging.Warning("Handled exception at {Path} StatusCode {StatusCode} Message {Message}",
                    httpContext.Request.Path, statusCode, ex.Message);
            }

            var result = JsonSerializer.Serialize(ApiResult<string>.Failure(errors), new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;

            return httpContext.Response.WriteAsync(result);
        }
    }
}
