using CoralTime.BL.Interfaces;
using CoralTime.Common.Middlewares;
using CoralTime.ViewModels.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CoralTime.Api.v1.Odata
{
    [Route("api/v1/odata/[controller]")]
    [EnableQuery]
    [Authorize]
    public class ClientsController : BaseController<ClientsController, IClientService>
    {
        public ClientsController(IClientService service, ILogger<ClientsController> logger)
            : base(logger, service) { }

        // GET: api/v1/odata/Clients
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_service.GetAllClients());
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Get method;\n {e}");
                var errors = ExceptionsChecker.CheckClientsException(e);
                return BadRequest(errors);
            }
        }

        // POST: api/v1/odata/Clients
        [HttpPost]
        [Authorize(Policy = "admin")]
        public IActionResult Create([FromBody] ClientView clientData)
        {
            try
            {
                var result = _service.Create(clientData);

                var locationUri = $"{Request.Host}/api/v1/odata/Clients({result.Id})";
                return Created(locationUri, result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Create method with parameters ({JsonConvert.SerializeObject(clientData)});\n {e}");
                var errors = ExceptionsChecker.CheckClientsException(e);
                return BadRequest(errors);
            }
        }

        // GET api/v1/odata/Clients(2)
        [EnableQuery]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var result = _service.GetById(id);
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetById method with parameters ({id});\n {e}");
                var errors = ExceptionsChecker.CheckClientsException(e);
                return BadRequest(errors);
            }
        }

        // PUT: api/v1/odata/Clients(2)
        [HttpPut("{id}")]
        [Authorize(Policy = "admin")]
        public IActionResult Update(int id, [FromBody]dynamic clientData)
        {
            clientData.Id = id;

            try
            {
                var result = _service.Update(clientData);
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Update method with parameters ({id}, {clientData});\n {e}");
                var errors = ExceptionsChecker.CheckClientsException(e);
                return BadRequest(errors);
            }
        }

        // PATCH: api/v1/odata/Clients(30)
        [HttpPatch("{id}")]
        [Authorize(Policy = "admin")]
        public IActionResult Patch(int id, [FromBody]dynamic clientData)
        {
            clientData.Id = id;

            try
            {
                var result = _service.Update(clientData);
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Patch method with parameters ({id}, {clientData});\n {e}");
                var errors = ExceptionsChecker.CheckClientsException(e);
                return BadRequest(errors);
            }
        }

        //DELETE :api/v1/odata/Clients(1)
        [HttpDelete("{id}")]
        [Authorize(Policy = "admin")]
        public IActionResult Delete(int id)
        {
            return BadRequest($"Can't delete the client with Id - {id}");
        }
    }
}
