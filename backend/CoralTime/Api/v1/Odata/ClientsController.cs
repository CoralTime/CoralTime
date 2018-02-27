using CoralTime.BL.Interfaces;
using CoralTime.ViewModels.Clients;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CoralTime.Api.v1.Odata
{
    [Route("api/v1/odata/[controller]")]
    [Authorize]
    public class ClientsController : BaseODataController<ClientsController, IClientService>
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
                return SendErrorResponse(e);
            }
        }

        // POST: api/v1/odata/Clients
        [HttpPost]
        [Authorize(Roles = "admin")]
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
                return SendErrorResponse(e);
            }
        }

        // GET api/v1/odata/Clients(2)
        [ODataRoute("Clients({id})")]
        [HttpGet("{id}")]
        public IActionResult GetById([FromODataUri] int id)
        {
            try
            {
                var result = _service.GetById(id);
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                return SendErrorResponse(e);
            }
        }

        // PUT: api/v1/odata/Clients(2)
        [ODataRoute("Clients({id})")]
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Update([FromODataUri]int id, [FromBody]dynamic clientData)
        {
            clientData.Id = id;

            try
            {
                var result = _service.Update(clientData);
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                return SendErrorResponse(e);
            }
        }

        // PATCH: api/v1/odata/Clients(30)
        [ODataRoute("Clients({id})")]
        [HttpPatch("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Patch([FromODataUri]int id, [FromBody]dynamic clientData)
        {
            clientData.Id = id;

            try
            {
                var result = _service.Update(clientData);
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                return SendErrorResponse(e);
            }
        }

        //DELETE :api/v1/odata/Clients(1)
        [HttpDelete("{id}")]
        [ODataRoute("Clients({id})")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete([FromODataUri]int id)
        {
            return BadRequest($"Can't delete the client with Id - {id}");
        }
    }
}