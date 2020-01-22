using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreTestServer.Tests.Integration.Web.Controllers
{
    [Route("TestMvc")]
    public class TestMvcController : Controller
    {
        [HttpGet]
        [Route("Index")]
        public ViewResult Index() => View("Index");
    }
}