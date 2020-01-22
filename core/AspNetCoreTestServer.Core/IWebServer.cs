using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Optional;

namespace AspNetCoreTestServer.Core
{
    public interface IWebServer
    {
        Task<RunningState> StartAsync<TStartup>(Assembly assembly, Option<string> contentRoot,
            IDictionary<string, string> configuration) where TStartup : class;

        Task<RunningState> StartAsync<TStartup>(Assembly assembly) where TStartup : class;
    }
}