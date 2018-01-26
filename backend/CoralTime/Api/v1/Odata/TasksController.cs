using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.DAL.Models;
using CoralTime.Services;
using CoralTime.ViewModels.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.Api.v1.Odata
{
    [Route("api/v1/odata/[controller]")]
    [Authorize]
    public class TasksController : BaseODataController<TasksController, ITasksService>
    {
        public TasksController(ITasksService service, IMapper mapper, ILogger<TasksController> logger)
            : base(logger, mapper, service) { }

        // GET: api/v1/odata/Tasks
        [HttpGet]
        public IEnumerable<TaskView> Get()
        {
            return _service.GetAllTaskTypes().Select(_mapper.Map<TaskType, TaskView>);
        }

        // GET api/v1/odata/Tasks(2)
        [ODataRoute("Tasks({id})")]
        [HttpGet("{id}")]
        public IActionResult GetById([FromODataUri]int id)
        {
            try
            {
                var value = _service.GetById(id);
                return new ObjectResult(_mapper.Map<TaskType, TaskView>(value));
            }
            catch (Exception e)
            {
                return SendErrorResponse(e);
            }
        }

        // POST api/v1/odata/Tasks
        [HttpPost]
        public IActionResult Create([FromBody]TaskView taskTypeData)
        {
            if (!ModelState.IsValid)
            {
                return SendInvalidModelResponse();
            }

            try
            {
                var result = _service.Create(taskTypeData, this.GetUserName());

                var locationUri = $"{Request.Host}/api/v1/odata/Tasks({result.Id})";

                return Created(locationUri, _mapper.Map<TaskType, TaskView>(result));
            }
            catch (Exception e)
            {
                return SendErrorResponse(e);
            }
        }

        // PUT api/v1/odata/Tasks(1)
        [ODataRoute("Tasks({id})")]
        [HttpPut("{id}")]
        public IActionResult Update([FromODataUri] int id, [FromBody] dynamic taskTypeData)
        {
            if (!ModelState.IsValid)
            {
                return SendInvalidModelResponse();
            }

            taskTypeData.Id = id;
            try
            {
                var result = _service.Update(taskTypeData, this.GetUserName());
                return new ObjectResult(_mapper.Map<TaskType, TaskView>(result));
            }
            catch (Exception e)
            {
                return SendErrorResponse(e);
            }
        }

        // PATCH api/v1/odata/Tasks(1)
        [ODataRoute("Tasks({id})")]
        [HttpPatch("{id}")]
        public IActionResult Patch([FromODataUri] int id, [FromBody] dynamic taskTypeData)
        {
            if (!ModelState.IsValid)
            {
                return SendInvalidModelResponse();
            }

            taskTypeData.Id = id;
            try
            {
                var userName = this.GetUserName();

                var result = _service.Patch(taskTypeData, userName);
                return new ObjectResult(_mapper.Map<TaskType, TaskView>(result));
            }
            catch (Exception e)
            {
                return SendErrorResponse(e);
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
                return SendErrorResponse(e);
            }
        }
    }
}