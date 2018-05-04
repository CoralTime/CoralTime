using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoralTime.Api.v1
{
    public abstract class BaseController<T, S> : Controller where T : class
    {
        protected readonly ILogger<T> _logger;
        protected readonly IMapper _mapper;
        protected readonly S _service;

        public BaseController(ILogger<T> logger, IMapper mapper, S service)
        {
            _logger = logger;
            _mapper = mapper;
            _service = service;
        }

        public BaseController(ILogger<T> logger, S service)
        {
            _logger = logger;
            _service = service;
        }

        public BaseController(ILogger<T> logger)
        {
            _logger = logger;
        }
        
        protected string GetBaseUrl() => $"{Request.Scheme}://{Request.Host.Host}:{Request.Host.Port}";
    }
}
