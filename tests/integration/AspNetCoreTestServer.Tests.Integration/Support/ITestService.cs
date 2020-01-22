using System.Threading.Tasks;
using Refit;

namespace AspNetCoreTestServer.Tests.Integration.Support
{
    public interface ITestService
    {
        [Get("/testEndPoint/Get")]
        Task<bool> Get();

        [Get("/testEndPoint/TestConfiguration/{key}")]
        Task<string> TestConfiguration(string key);
    }
}