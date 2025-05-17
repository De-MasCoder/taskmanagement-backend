using Serilog;
using System.Net;

namespace task_management_api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (_env.IsDevelopment())
            {
                return context.Response.WriteAsJsonAsync(new
                {
                    error = new
                    {
                        message = exception.Message,
                        stackTrace = exception.StackTrace
                    }
                });
            }
            return context.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred." });
        }
    }

}
