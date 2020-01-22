using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.Extensions;
using Optional;
using Xunit;

namespace AspNetCoreTestServer.Core.Tests.Unit
{
    public class KestrelWebServerTests
    {
        [NotNull]
        private KestrelWebServer CreateKestrelWebServer(Func<IWebHostBuilder> webHostBuilderFactory,
            IPortResolver portResolver) => new KestrelWebServer(webHostBuilderFactory, portResolver);

        public class Startup
        {
            public void Configure([NotNull] IApplicationBuilder app)
            {
            }
        }

        [Fact]
        public async Task StartAsyncReturnsRunningStateWithTheEndpointItWasStartedOn()
        {
            var hostBuilder = Substitute.For<IWebHostBuilder>();
            var portResolver = Substitute.For<IPortResolver>();

            hostBuilder.ReturnsForAll(hostBuilder);
            hostBuilder.Build().Returns(Substitute.For<IWebHost>());
            portResolver.GetAvailableTcpPort().Returns(81);

            var server = CreateKestrelWebServer(() => hostBuilder, portResolver);
            var result = await server.StartAsync<object>(Assembly.GetExecutingAssembly(), default,
                new Dictionary<string, string>());
            result.Endpoint.AbsoluteUri.Should().Be("http://127.0.0.1:81/");
        }

        [Fact]
        public async Task StartAsyncSimpleStartsServerWithApplicationKeySetToAssemblyName()
        {
            var hostBuilder = Substitute.For<IWebHostBuilder>();

            hostBuilder.ReturnsForAll(hostBuilder);
            hostBuilder.Build().Returns(Substitute.For<IWebHost>());

            var server = CreateKestrelWebServer(() => hostBuilder, Substitute.For<IPortResolver>());
            var assembly = Assembly.GetExecutingAssembly();
            await server.StartAsync<object>(assembly);
            hostBuilder.Received().UseSetting(WebHostDefaults.ApplicationKey, assembly.FullName);
        }

        [Fact]
        public async Task StartAsyncStartsServerDoesNotSetContentRootWhenNone()
        {
            var hostBuilder = Substitute.For<IWebHostBuilder>();

            hostBuilder.ReturnsForAll(hostBuilder);
            hostBuilder.Build().Returns(Substitute.For<IWebHost>());

            var server = CreateKestrelWebServer(() => hostBuilder, Substitute.For<IPortResolver>());
            await server.StartAsync<object>(Assembly.GetExecutingAssembly(), Option.None<string>(),
                new Dictionary<string, string>());
            hostBuilder.DidNotReceive()
                       .UseSetting(Arg.Is<string>(s => s == WebHostDefaults.ContentRootKey), Arg.Any<string>());
        }

        [Fact]
        public async Task StartAsyncStartsServerWithApplicationKeySetToAssemblyName()
        {
            var hostBuilder = Substitute.For<IWebHostBuilder>();

            hostBuilder.ReturnsForAll(hostBuilder);
            hostBuilder.Build().Returns(Substitute.For<IWebHost>());

            var server = CreateKestrelWebServer(() => hostBuilder, Substitute.For<IPortResolver>());
            var assembly = Assembly.GetExecutingAssembly();
            await server.StartAsync<object>(assembly, default, new Dictionary<string, string>());
            hostBuilder.Received().UseSetting(WebHostDefaults.ApplicationKey, assembly.FullName);
        }

        [Fact]
        public async Task StartAsyncStartsServerWithConfiguration()
        {
            var hostBuilder = Substitute.For<IWebHostBuilder>();

            hostBuilder.ReturnsForAll(hostBuilder);
            hostBuilder.Build().Returns(Substitute.For<IWebHost>());

            var configuration = new Dictionary<string, string>
            {
                ["test"] = "testing"
            };

            hostBuilder.WhenForAnyArgs(builder => builder.ConfigureAppConfiguration(default))
                       .Do(info =>
                       {
                           var action = info.Arg<Action<WebHostBuilderContext, IConfigurationBuilder>>();
                           var configurationBuilder = Substitute.For<IConfigurationBuilder>();
                           configurationBuilder.WhenForAnyArgs(builder => builder.Add(default))
                                               .Do(callInfo =>
                                                   callInfo.Arg<MemoryConfigurationSource>()
                                                           .InitialData.Should()
                                                           .BeEquivalentTo(configuration));
                           action(new WebHostBuilderContext(), configurationBuilder);
                           configurationBuilder.Received(1).Add(Arg.Any<MemoryConfigurationSource>());
                       });

            var server = CreateKestrelWebServer(() => hostBuilder, Substitute.For<IPortResolver>());
            await server.StartAsync<object>(Assembly.GetExecutingAssembly(), default, configuration);

            hostBuilder.Received(1)
                       .ConfigureAppConfiguration(Arg.Any<Action<WebHostBuilderContext, IConfigurationBuilder>>());
        }

        [Fact]
        public async Task StartAsyncStartsServerWithContentRootWhenSome()
        {
            var hostBuilder = Substitute.For<IWebHostBuilder>();

            hostBuilder.ReturnsForAll(hostBuilder);
            hostBuilder.Build().Returns(Substitute.For<IWebHost>());

            var server = CreateKestrelWebServer(() => hostBuilder, Substitute.For<IPortResolver>());
            const string contentRoot = "test";
            await server.StartAsync<object>(Assembly.GetExecutingAssembly(), contentRoot.Some(),
                new Dictionary<string, string>());
            hostBuilder.Received().UseSetting(WebHostDefaults.ContentRootKey, contentRoot);
        }

        [Fact]
        public async Task StartAsyncStartsServerWithKestrelSupport()
        {
            var hostBuilder = Substitute.For<IWebHostBuilder>();

            hostBuilder.ReturnsForAll(hostBuilder);
            hostBuilder.Build().Returns(Substitute.For<IWebHost>());

            var server = CreateKestrelWebServer(() => hostBuilder, Substitute.For<IPortResolver>());
            await server.StartAsync<object>(Assembly.GetExecutingAssembly(), default, new Dictionary<string, string>());
            hostBuilder.Received().UseKestrel();
        }

        [Fact]
        public async Task StartAsyncStartsServerWithLocalUrlOnRandomPort()
        {
            var hostBuilder = Substitute.For<IWebHostBuilder>();

            hostBuilder.ReturnsForAll(hostBuilder);
            hostBuilder.Build().Returns(Substitute.For<IWebHost>());

            var portResolver = Substitute.For<IPortResolver>();
            portResolver.GetAvailableTcpPort().Returns(80);

            var server = CreateKestrelWebServer(() => hostBuilder, portResolver);
            await server.StartAsync<object>(Assembly.GetExecutingAssembly(), default, new Dictionary<string, string>());
            hostBuilder.Received().UseSetting(WebHostDefaults.ServerUrlsKey, "http://127.0.0.1:80");
        }

        [Fact]
        public async Task StartAsyncStartsServerWithTheSpecifiedStartupClass()
        {
            var hostBuilder = Substitute.For<IWebHostBuilder>();

            hostBuilder.ReturnsForAll(hostBuilder);
            hostBuilder.Build().Returns(Substitute.For<IWebHost>());

            var collection = new ServiceCollection();
            collection.AddSingleton(Substitute.For<IHostingEnvironment>());
            hostBuilder.WhenForAnyArgs(builder => builder.ConfigureServices(default(Action<IServiceCollection>)))
                       .Do(
                           info =>
                           {
                               var action = info.Arg<Action<IServiceCollection>>();
                               action(collection);
                           });
            var server = CreateKestrelWebServer(() => hostBuilder, Substitute.For<IPortResolver>());
            await server.StartAsync<Startup>(Assembly.GetExecutingAssembly(), default,
                new Dictionary<string, string>());
            collection.BuildServiceProvider().GetService<IStartup>().Should().NotBeNull();
        }
    }
}