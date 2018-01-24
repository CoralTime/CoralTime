using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Middlewares;
using CoralTime.DAL.Models;
using CoralTime.Services;
using CoralTime.ViewModels.Errors;
using CoralTime.ViewModels.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.Api.v1.Odata
{
    [Route("api/v1/odata/[controller]")]
    [EnableQuery]
    [Authorize]
    public class TasksController :  BaseController<TasksController, ITasksService>
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
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var value = _service.GetById(id);
                return new ObjectResult(_mapper.Map<TaskType, TaskView>(value));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetById method with parameters ({id});\n {e}");
                var errors = ExceptionsChecker.CheckTasksException(e);
                return BadRequest(errors);
            }
        }

        // POST api/v1/odata/Tasks
        [HttpPost]
        public IActionResult Create([FromBody]TaskView taskTypeData)
        {
            if (!ModelState.IsValid)
                return BadRequest(new List<ErrorView>{new ErrorView
                {
                    Source = "Other",
                    Title = "",
                    Details = "ModelState is invalid."
                } });

            try
            {
                var result = _service.Create(taskTypeData, this.GetUserName());

                var locationUri = $"{Request.Host}/api/v1/odata/Tasks({result.Id})";

                return Created(locationUri, _mapper.Map<TaskType, TaskView>(result));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Create method with parameters ({JsonConvert.SerializeObject(taskTypeData)});\n {e}");
                var errors = ExceptionsChecker.CheckTasksException(e);
                return BadRequest(errors);
            }
        }

        // PUT api/v1/odata/Tasks(1)
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] dynamic taskTypeData)
        {
            if (!ModelState.IsValid)
                return BadRequest(new List<ErrorView>{new ErrorView
                {
                    Source = "Other",
                    Title = "",
                    Details = "ModelState is invalid."
                } });

            taskTypeData.Id = id;
            try
            {
                var result = _service.Update(taskTypeData, this.GetUserName());
                return new ObjectResult(_mapper.Map<TaskType, TaskView>(result));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Update methodwith parameters ({id}, {taskTypeData});\n {e}");
                var errors = ExceptionsChecker.CheckTasksException(e);
                return BadRequest(errors);
            }
        }

        // PATCH api/v1/odata/Tasks(1)
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] dynamic taskTypeData)
        {
            if (!ModelState.IsValid)
                return BadRequest(new List<ErrorView>{new ErrorView
                {
                    Source = "Other",
                    Title = "",
                    Details = "ModelState is invalid."
                } });

            taskTypeData.Id = id;
            try
            {
                var userName = this.GetUserName();

                var result = _service.Patch(taskTypeData, userName);
                return new ObjectResult(_mapper.Map<TaskType, TaskView>(result));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Patch method with parameters ({id}, {taskTypeData});\n {e}");
                var errors = ExceptionsChecker.CheckTasksException(e);
                return BadRequest(errors);
            }
        
        }

        //DELETE :api/v1/odata/Tasks(1)
        [Authorize(Policy = "admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _service.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Delete method with parameters ({id});\n {e}");
                var errors = ExceptionsChecker.CheckTasksException(e);
                return BadRequest(errors);
            }
        }
    }

 }
