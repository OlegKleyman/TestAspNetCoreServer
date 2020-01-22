using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AspNetCoreTestServer.Core;
using AspNetCoreTestServer.Tests.Integration.Support;
using AspNetCoreTestServer.Tests.Integration.Web;
using FluentAssertions;
using Microsoft.AspNetCore;
using Optional;
using Refit;
using Xunit;

namespace AspNetCoreTestServer.Tests.Integration
{
    public class KestrelWebServerTests
    {
        private KestrelWebServer CreateKestrelWebServer() =>
            new KestrelWebServer(WebHost.CreateDefaultBuilder, new PortResolver());

        [Fact]
        public async Task StartAsyncStartsTheWebApplication()
        {
            var server = CreateKestrelWebServer();
            using var state = await server.StartAsync<Startup>(Assembly.GetAssembly(typeof(KestrelWebServerTests)));

            var service = RestService.For<ITestService>(state.Endpoint.AbsoluteUri);
            var result = await service.Get();
            result.Should().BeTrue();
        }

        [Fact]
        public async Task StartAsyncStartsTheWebApplicationAndReturnsContentFromTheContentRoot()
        {
            var server = CreateKestrelWebServer();
            using var state = await server.StartAsync<Startup>(Assembly.GetAssembly(typeof(KestrelWebServerTests)),
                @"..\..\..\Web".Some(), new Dictionary<string, string>());

            var client = new HttpClient { BaseAddress = state.Endpoint };
            var response = await client.GetAsync("test.txt");
            var result = await response.Content.ReadAsStringAsync();
            result.Should().Be("testing");
        }

        [Fact]
        public async Task StartAsyncStartsTheWebApplicationAndReturnsView()
        {
            var server = CreateKestrelWebServer();
            using var state = await server.StartAsync<Startup>(Assembly.GetAssembly(typeof(KestrelWebServerTests)),
                @"..\..\..\Web".Some(), new Dictionary<string, string>());

            var client = new HttpClient { BaseAddress = state.Endpoint };
            var response = await client.GetAsync("TestMvc/index");
            var result = await response.Content.ReadAsStringAsync();
            result.Should().Be("testing");
        }

        [Fact]
        public async Task StartAsyncStartsTheWebApplicationAndUsesTheConfigurationArgument()
        {
            var server = CreateKestrelWebServer();
            using var state = await server.StartAsync<Startup>(Assembly.GetAssembly(typeof(KestrelWebServerTests)),
                @"..\..\..\Web".Some(), new Dictionary<string, string>
                {
                    ["test:test2:test3"] = "testing"
                });

            var service = RestService.For<ITestService>(state.Endpoint.AbsoluteUri);
            var result = await service.TestConfiguration("test:test2:test3");
            result.Should().Be("testing");
        }
    }
}