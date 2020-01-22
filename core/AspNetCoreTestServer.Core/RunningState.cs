using System;
using Microsoft.AspNetCore.Hosting;

namespace AspNetCoreTestServer.Core
{
    public class RunningState : IDisposable
    {
        private readonly IWebHost _webHost;

        public RunningState(IWebHost webHost, Uri endpoint)
        {
            _webHost = webHost;
            Endpoint = endpoint;
        }

        public Uri Endpoint { get; }

        public void Dispose()
        {
            _webHost?.Dispose();
        }
    }
}