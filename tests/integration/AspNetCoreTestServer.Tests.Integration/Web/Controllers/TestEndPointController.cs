using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AspNetCoreTestServer.Tests.Integration.Web.Controllers
{
    [Route("TestEndPoint")]
    public class TestEndPointController : Controller
    {
        private readonly IConfiguration _configuration;

        public TestEndPointController(IConfiguration configuration) => _configuration = configuration;

        [HttpGet]
        [Route("Get")]
        public bool Get() => true;

        [HttpGet]
        [Route("TestConfiguration/{key}")]
        public string TestConfiguration(string key) => _configuration[key];
    }
}
