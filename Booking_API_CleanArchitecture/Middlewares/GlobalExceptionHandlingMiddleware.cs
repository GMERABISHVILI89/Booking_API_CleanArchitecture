using Booking_Domain.Models;
using Booking_Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Booking_API_CleanArchitecture.Middlewares
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {

        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                ProblemDetails problem = new()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Type = "Server Error",
                    Title = "Server Error",
                    Detail = "An Internal server has occurred"
                };


                var exceptionLog = new ExceptionLogs()
                {
                    CreateDate = DateTime.Now,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = e.Message,
                    StackTrace=e.StackTrace!,
                };

                await _context.ExceptionLogs.AddAsync(exceptionLog);
                await  _context.SaveChangesAsync();


                var json = JsonSerializer.Serialize(problem);

                await context.Response.WriteAsync(json);

                context.Response.ContentType = "application/json";
            }
        }
    }
}
