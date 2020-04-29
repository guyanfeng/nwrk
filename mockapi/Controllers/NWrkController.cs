using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace mockapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NWrkController : ControllerBase
    {
        private readonly ILogger<NWrkController> _logger;
        private readonly static HttpClient _http;

        static NWrkController()
        {
            _http = new HttpClient();
            _http.BaseAddress = new Uri("http://127.0.0.1:5000");
            _http.Timeout = TimeSpan.FromSeconds(5);
        }

        public NWrkController(ILogger<NWrkController> logger)
        {
            _logger = logger;

        }

        [HttpGet]
        public ActionResult Get()
        {
            return Content("mock api gateway");
        }

        [HttpGet("{id?}")]
        public string Get(string id)
        {
            return "mock api get " + id;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] string id, [FromForm] string name)
        {
            var content = new FormUrlEncodedContent(
                new[]{
                    new KeyValuePair<string, string>("id", id),
                    new KeyValuePair<string, string>("name", name)
                });
            var rep = await _http.PostAsync("/nwrk", content);
            rep.EnsureSuccessStatusCode();
            var result = await rep.Content.ReadAsStringAsync();
            return new JsonResult(new NWrkResponse
            {
                id=  id,
                name = name,
                result = result
            });
        }
    }

    public class NWrkResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string result { get; set; }
    }
}
