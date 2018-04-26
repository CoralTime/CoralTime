using AutoMapper;
using CoralTime.Common.Middlewares;
using CoralTime.ViewModels.Errors;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.Api.v1
{
    [EnableQuery]
    public abstract class BaseODataController<T, S> : ODataController where T : class
    {
        protected readonly ILogger<T> _logger;
        protected readonly IMapper _mapper;
        protected readonly S _service;

        public BaseODataController(ILogger<T> logger, IMapper mapper, S service)
        {
            _logger = logger;
            _mapper = mapper;
            _service = service;
        }

        public BaseODataController(ILogger<T> logger, S service)
        {
            _logger = logger;
            _service = service;
        }

        protected IActionResult SendErrorODataResponse(Exception exception)
        {
            _logger.LogError($"Path: {(Request.Path)} , Query: {Request.QueryString}", exception);

            var error = ExceptionsODataChecker.CheckExceptions(exception);

            return BadRequest(error);
        }

        protected IActionResult SendInvalidModelResponse()
        {
            var errors = ControllerContext.ModelState.Values;
            return BadRequest(new List<ErrorODataView>
            {
                new ErrorODataView
                {
                    Source = "Other",
                    Title = "ModelState is invalid.",
                    Details = GetWrongKeys()
                }
            });
        }

        protected string GetBaseUrl() => $"{Request.Scheme}://{Request.Host.Host}:{Request.Host.Port}";
        
        private string GetWrongKeys()
        {
            var wrongKeys = ControllerContext.ModelState.Keys
                .SelectMany(key => ControllerContext.ModelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                .ToList();

            return string.Join(";", wrongKeys.Select(x => $"Key: {x.Field} Error: {x.Message}"));
        }

        private class ValidationError
        {
            public string Field { get; }

            public string Message { get; }

            public ValidationError(string field, string message)
            {
                Field = field != string.Empty ? field : null;
                Message = message;
            }
        }
    }
}