using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckProfanityKrestel.Lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProfanityList.Check;

namespace CheckProfanityKrestel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfanityController : ControllerBase
    {
        private readonly ILogger<ProfanityController> _logger;

        public ProfanityController(ILogger<ProfanityController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "OK";
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<CheckProfanityResult> Post(EnumExecuteDetail exectionDetail)
        {
            var profanityList = new ProfanityListInMemoryService();
            var service = new CheckProfanityService(profanityList, null);
            
            byte[] buf = new byte[(int)Request.ContentLength.Value];
            await Request.Body.ReadAsync(buf, 0, (int)Request.ContentLength.Value);

            var ms = new MemoryStream(buf);

            return await service.CheckProfanity(ms, exectionDetail);
        }
    }
}
