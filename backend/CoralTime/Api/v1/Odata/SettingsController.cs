//using AutoMapper;
//using CoralTime.BL.ServicesInterfaces;
//using CoralTime.Common.Middlewares;
//using CoralTime.DAL.Models;
//using CoralTime.ViewModels.Errors;
//using CoralTime.ViewModels.Settings;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.OData;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace CoralTime.Api.v1.Odata
//{
//    [Route("api/v1/odata/[controller]")]
//    [EnableQuery]
//    [Authorize(Policy = "admin")]
//    public class SettingsController : _BaseController<SettingsController, ISettingsService>
//    {
//        public SettingsController(IMapper mapper, ILogger<SettingsController> logger, ISettingsService service)
//            : base(logger, mapper, service) { }

//        // GET: api/v1/odata/Settings
//        [HttpGet]
//        public IEnumerable<SettingsView> Get()
//        {
//            return _service.GetAllSettings().Select(_mapper.Map<Setting, SettingsView>);
//        }

//        // GET api/v1/odata/Settings("key")
//        [HttpGet("{id}")]
//        public IActionResult GetById(string id)
//        {
//            try
//            {
//                var value = _service.GetByName(id);
//                return new ObjectResult(_mapper.Map<Setting, SettingsView>(value));
//            }
//            catch (Exception e)
//            {
//                _logger.LogWarning($"GetById method with parameters ({id});\n {e}");
//                var errors = ExceptionsChecker.CheckProjectsException(e);
//                return BadRequest(errors);
//            }
//        }

//        // POST api/v1/odata/ProjectsSettings
//        [HttpPost]
//        public IActionResult Create([FromBody]SettingsView setting)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(new List<ErrorView>{new ErrorView
//                {
//                    Source = "Other",
//                    Title = "",
//                    Details = "ModelState is invalid."
//                } });

//            try
//            {
//                var result = _service.Create(setting);
//                var locationUri = $"{Request.Host}/api/v1/odata/Settings('{result.Name}')";
//                return Created(locationUri, _mapper.Map<Setting, SettingsView>(result));
//            }
//            catch (Exception e)
//            {
//                _logger.LogWarning($"Create method with parameters ({JsonConvert.SerializeObject(setting)};\n {e}");
//                var errors = ExceptionsChecker.CheckProjectsException(e);
//                return BadRequest(errors);
//            }
//        }

//        // PUT api/v1/odata/ProjectsSettings('key')
//        [HttpPut("{id}")]
//        public IActionResult Update(string id, [FromBody]dynamic setting)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(new List<ErrorView>{new ErrorView
//                {
//                    Source = "Other",
//                    Title = "",
//                    Details = "ModelState is invalid."
//                } });

//            setting["name"] = id;
//            try
//            {
//                var result = _service.Update(setting);
//                return new ObjectResult(_mapper.Map<Setting, SettingsView>(result));
//            }
//            catch (Exception e)
//            {
//                _logger.LogWarning($"Update method with parameters ({id}, {setting});\n {e}");
//                var errors = ExceptionsChecker.CheckSettingsException(e);
//                return BadRequest(errors);
//            }
//        }

//        // PATCH api/v1/odata/ProjectsSettings('key')
//        [HttpPatch("{id}")]
//        public IActionResult Patch(string id, [FromBody]dynamic setting)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(new List<ErrorView>{new ErrorView
//                {
//                    Source = "Other",
//                    Title = "",
//                    Details = "ModelState is invalid."
//                } });

//            setting["name"] = id;
//            try
//            {
//                var result = _service.Patch(setting);
//                return new ObjectResult(_mapper.Map<Setting, SettingsView>(result));
//            }
//            catch (Exception e)
//            {
//                _logger.LogWarning($"Patch method with parameters ({id}, {setting});\n {e}");
//                var errors = ExceptionsChecker.CheckSettingsException(e);
//                return BadRequest(errors);
//            }
//        }

//        // DELETE api/v1/odata/ProjectsSettings('key')
//        [HttpDelete("{id}")]
//        public IActionResult Delete(string id)
//        {
//            try
//            {
//                _service.Delete(id);
//                return Ok();
//            }
//            catch (Exception e)
//            {
//                _logger.LogWarning($"Delete method with parameters ({id});\n {e}");
//                var errors = ExceptionsChecker.CheckSettingsException(e);
//                return BadRequest(errors);
//            }
//        }
//    }
//}
