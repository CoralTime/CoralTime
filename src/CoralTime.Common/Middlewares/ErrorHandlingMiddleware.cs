using CoralTime.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text;
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
            var exseptionInnerMessage = exception?.InnerException;
            context.Response.ContentType = "application/json";
            var message = "Oops. Something went wrong";

            switch (exception)
            {
                case CoralTimeSafeEntityException ex:
                {
                    _logger.LogWarning($"CoralTimeSafeException, {ex.Message} , {ex.StackTrace}");
                    code = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    var sb = new StringBuilder();

                    // For Identity Errors
                    if (ex.errors.Count > 0)
                    {
                        foreach (var errorView in ex.errors)
                        {
                            //sb.Append(errorView.Details);
                            //sb.Append(errorView.Source);
                            sb.Append(errorView.Title);
                            sb.Append(". ");
                        }
                    }

                    message = sb.ToString();

                    break;
                }

                case CoralTimeAlreadyExistsException ex:
                {
                    _logger.LogWarning($"CoralTimeAlreadyExistsException, {ex.Message} , {ex.StackTrace}");
                    code = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;
                }

                case CoralTimeEntityNotFoundException ex:
                {
                    _logger.LogWarning($"CoralTimeEntityNotFoundException, {ex.Message} , {ex.StackTrace}");
                    code = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;
                }

                case CoralTimeUnauthorizedException ex:
                {
                    code = HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    _logger.LogWarning($"CoralTimeUnauthorizedException, {ex.Message} , {ex.StackTrace}");
                    break;
                }

                case CoralTimeDangerException ex:
                {
                    code = HttpStatusCode.BadRequest;
                    _logger.LogError($"CoralTimeDangerException, {ex.Message} , {ex.StackTrace}");
                    break;
                }

                case CoralTimeForbiddenException ex:
                {
                    code = HttpStatusCode.Forbidden;
                    message = exception.Message;
                    _logger.LogError($"CoralTimeForbiddenException, {exception.Message} , {exception.StackTrace}");
                    break;
                }

                default:
                {
                    var exMessage = exception.Message;
                    var exInnerExMessage = exception?.InnerException?.Message ?? string.Empty;
                    var exStackTrace = exception.StackTrace;

                    _logger.LogError($"\nException: {exMessage}. \n InnerException: {exInnerExMessage}. \nStack Trace: \n{exStackTrace}", exception);
                    break;
                }
            }

            context.Response.StatusCode = (int)code;

#if DEBUG
            message = "Debug message: " + exseptionMessage;
            if (exseptionInnerMessage != null)
            {
                message = message + ". InnerMessage: " + exseptionInnerMessage.Message;
            }
#endif
            _logger.Log(LogLevel.Error, new EventId(), exseptionInnerMessage?.Message, exception, (i, exception1) => i?.ToString());

            return context.Response.WriteAsync(message);
        }
    }
}