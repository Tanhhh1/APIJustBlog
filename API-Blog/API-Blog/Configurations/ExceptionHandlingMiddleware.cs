using Application.Exceptions;
using System.Text.Json;
using Application.Common.ModelServices;

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
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleException(httpContext, ex);
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

            var result = JsonSerializer.Serialize(ApiResult<string>.Failure(errors), new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;

            return httpContext.Response.WriteAsync(result);
        }
    }
}
