using System.Net;
using TestApiProj.Models;

namespace TestApiProj.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;


        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }


        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var errorDetails = new ErrorDetails
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = "An unexpected error occurred.",
                    StackTrace = _env.IsDevelopment() ? ex.StackTrace : null
                };

                await httpContext.Response.WriteAsync(errorDetails.ToString());
            }
        }

    }


}
