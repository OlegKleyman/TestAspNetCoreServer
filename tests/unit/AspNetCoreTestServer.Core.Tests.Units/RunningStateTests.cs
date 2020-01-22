using System;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using NSubstitute;
using Xunit;

namespace AspNetCoreTestServer.Core.Tests.Units
{
    public class RunningStateTests
    {
        [Fact]
        public void ConstructorInitializesEndpoint()
        {
            var endpoint = new Uri("http://test");
            new RunningState(Substitute.For<IWebHost>(), endpoint).Endpoint.Should().BeEquivalentTo(endpoint);
        }

        [Fact]
        public void DisposeDisposesWebHost()
        {
            var webHost = Substitute.For<IWebHost>();
            var state = new RunningState(webHost, new Uri("http://test"));
            state.Dispose();
            webHost.Received().Dispose();
        }
    }
}
