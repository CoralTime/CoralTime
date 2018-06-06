using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoralTime.Api.v1
{
    public abstract class BaseController<TController, TService> : Controller 
        where TController : class
    {
        protected readonly ILogger<TController> _logger;
        protected readonly TService _service;

        protected BaseController(ILogger<TController> logger, TService service)
        {
            _logger = logger;
            _service = service;
        }

        protected BaseController(ILogger<TController> logger) => _logger = logger;
        
        protected string GetBaseUrl() => $"{Request.Scheme}://{Request.Host.Host}:{Request.Host.Port}";
    }
}
