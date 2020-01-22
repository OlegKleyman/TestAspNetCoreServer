using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Optional;

namespace AspNetCoreTestServer.Core
{
    public class KestrelWebServer
    {
        private readonly Func<IWebHostBuilder> _webHostBuilderFactory;
        private readonly IPortResolver _portResolver;

        public KestrelWebServer(Func<IWebHostBuilder> webHostBuilderFactory, IPortResolver portResolver)
        {
            _webHostBuilderFactory = webHostBuilderFactory;
            _portResolver = portResolver;
        }

        public async Task<RunningState> StartAsync<TStartup>(Assembly assembly, Option<string> contentRoot, IDictionary<string, string> configuration) where TStartup : class
        {
            var url = $"http://127.0.0.1:{_portResolver.GetAvailableTcpPort()}";
            var webHostBuilder = _webHostBuilderFactory();
            contentRoot.MatchSome(s => webHostBuilder.UseContentRoot(s));
            var host = webHostBuilder
                       .UseKestrel()
                       .UseStartup<TStartup>()
                       .UseSetting(WebHostDefaults.ApplicationKey, assembly.FullName)
                       .UseUrls(url)
                       .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configuration))
                       .Build();
            await host.StartAsync();
            return new RunningState(host, new Uri(url));
        }

        public Task<RunningState> StartAsync<TStartup>(Assembly assembly) where TStartup : class =>
            StartAsync<TStartup>(assembly, Option.None<string>(), new Dictionary<string, string>());
    }
}