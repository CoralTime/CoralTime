using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.ViewModels.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CoralTime.Api.v1.Odata
{
    [Authorize]
    [Route("api/v1/odata/[controller]")]
    public class TasksController : BaseODataController<TasksController, ITasksService>
    {
        public TasksController(ITasksService service, IMapper mapper, ILogger<TasksController> logger)
            : base(logger, mapper, service) { }

        // GET: api/v1/odata/Tasks
        [HttpGet]
        public IActionResult Get() => new ObjectResult(_service.Get());

        // GET api/v1/odata/Tasks(2)
        [ODataRoute("Tasks({id})")]
        [HttpGet("{id}")]
        public IActionResult GetById([FromODataUri]int id)
        {
            try
            {
                var taskTypeViewResult = _service.GetById(id);
                return new ObjectResult(taskTypeViewResult);
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        // POST api/v1/odata/Tasks
        [HttpPost]
        public IActionResult Create([FromBody]TaskTypeView taskTypeView)
        {
            if (!ModelState.IsValid)
            {
                return SendInvalidModelResponse();
            }

            try
            {
                var taskTypeViewResult = _service.Create(taskTypeView);
                var locationUri = $"{Request.Host}/api/v1/odata/Tasks({taskTypeViewResult.Id})";

                return Created(locationUri, taskTypeViewResult);
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        // PUT api/v1/odata/Tasks(1)
        [ODataRoute("Tasks({id})")]
        [HttpPut("{id}")]
        public IActionResult Update([FromODataUri] int id, [FromBody]TaskTypeView taskTypeView)
        {
            if (!ModelState.IsValid)
            {
                return SendInvalidModelResponse();
            }

            taskTypeView.Id = id;

            try
            {
                var taskTypeViewResult = _service.Update(taskTypeView);
                return new ObjectResult(taskTypeViewResult);
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        // PATCH api/v1/odata/Tasks(1)
        [ODataRoute("Tasks({id})")]
        [HttpPatch("{id}")]
        public IActionResult Patch([FromODataUri] int id, [FromBody]TaskTypeView taskTypeView)
        {
            if (!ModelState.IsValid)
            {
                return SendInvalidModelResponse();
            }

            taskTypeView.Id = id;

            try
            {
                var taskTypeViewResult = _service.Update(taskTypeView);
                return new ObjectResult(taskTypeViewResult);
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        //DELETE :api/v1/odata/Tasks(1)
        [Authorize(Roles = "admin")]
        [ODataRoute("Tasks({id})")]
        [HttpDelete("{id}")]
        public IActionResult Delete([FromODataUri] int id)
        {
            try
            {
                var result = _service.Delete(id);
                return new ObjectResult(null);
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }
    }
}