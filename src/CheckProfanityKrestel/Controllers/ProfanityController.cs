using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public CheckProfanityResult Get(Stream inputStream, EnumExecuteDetail exectionDetail)
        {
            var profanityList = new ProfanityListXmlSample();
            var service = new CheckProfanityService(profanityList);

            return service.CheckProfanity(inputStream, exectionDetail);
        }
    }
}
