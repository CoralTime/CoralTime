using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CoralTime.Api.v1
{
    [Route("api/v1/[controller]")]
    public class SwaggerController : Controller
    {
        private readonly IHostingEnvironment _env;
        private string path;
        private string file;

        public SwaggerController(IHostingEnvironment env)
        {
            _env = env;
            path = _env.ContentRootPath;
            file = $"{path}\\Swagger\\swagger.json";
        }

        
        [HttpGet]
        public IActionResult Get()
        {
            var json = System.IO.File.ReadAllText(file);
            var result = json.Replace("/{id}", "(id)");
            return Content(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update()
        {
            var url = "/swagger/v1/swagger.json";

            var protocol = Request.IsHttps ? "https://" : "http://";

            var baseUrl = protocol + Request.Host;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responceString = await response.Content.ReadAsStringAsync();
                    System.IO.File.WriteAllText(file, responceString);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
        }
    }
}
