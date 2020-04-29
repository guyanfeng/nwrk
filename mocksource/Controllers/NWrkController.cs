using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace mocksource.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NWrkController : ControllerBase
    {
        private readonly ILogger<NWrkController> _logger;

        public NWrkController(ILogger<NWrkController> logger)
        {
            _logger = logger;
        }

        public string Get()
        {
            return "mock source gateway";
        }

        [HttpGet("{id?}")]
        public string Get(string id)
        {
            return "get " + id;
        }

        [HttpPost()]
        public async Task<string> Post([FromForm]string id, [FromForm] string name)
        {
            await Task.Delay(100);
            return "一致";
        }
    }
}
