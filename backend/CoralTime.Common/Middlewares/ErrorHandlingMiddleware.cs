using CoralTime.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CoralTime.Common.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private static ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware (RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke (HttpContext context /* other scoped dependencies */)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync (HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            var exseptionMessage = exception.Message;
            var exseptionInnerMessage = exception.InnerException;

            var message = "Oops. Something went wrong";

            if (exception is CoralTimeSafeEntityException)
            {
                _logger.LogWarning($"CoralTimeSafeException, {exception.Message} , {exception.StackTrace}");
                code = HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else if (exception is CoralTimeAlreadyExistsException)
            {
                _logger.LogWarning($"CoralTimeAlreadyExistsException, {exception.Message} , {exception.StackTrace}");
                code = HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else if (exception is CoralTimeEntityNotFoundException)
            {
                _logger.LogWarning($"CoralTimeEntityNotFoundException, {exception.Message} , {exception.StackTrace}");
                code = HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            
            else if (exception is CoralTimeUnauthorizedException)
            {
                code = HttpStatusCode.Unauthorized;
                message = exception.Message;
                _logger.LogWarning($"CoralTimeUnauthorizedException, {exception.Message} , {exception.StackTrace}");
            }
            else if (exception is CoralTimeDangerException)
            {
                code = HttpStatusCode.BadRequest;
                message = exception.Message;
                _logger.LogError($"CoralTimeDangerException, {exception.Message} , {exception.StackTrace}");
            }
            else
            {
                _logger.LogError($"\nException: {exception.Message}. \nInnerException: {exception.InnerException.Message}. \nStack Trace: \n{exception.StackTrace}", exception);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) code;

#if DEBUG
            message = "Debug message: " + exseptionMessage;
            if (exseptionInnerMessage != null)
            {
                message = message + ". InnerMessage: " + exseptionInnerMessage.Message;
            }
#endif
            //_logger.Log(LogLevel.Error, new EventId(), exseptionInnerMessage.Message, exception, (i, exception1) => i.ToString());

            return context.Response.WriteAsync(message);
        }
    }
}