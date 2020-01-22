using System.Linq;
using System.Net.NetworkInformation;
using AspNetCoreTestServer.Core;
using FluentAssertions;
using Xunit;

namespace AspNetCoreTestServer.Tests.Integration
{
    public class PortResolverTests
    {
        [Fact]
        public void GetAvailableTcpPortReturnsAnOpenTcpPort()
        {
            var resolver = new PortResolver();
            var port = resolver.GetAvailableTcpPort();

            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var connections = properties.GetActiveTcpConnections();
            var listeners = properties.GetActiveTcpListeners();

            connections.Select(information => information.LocalEndPoint.Port)
                       .Union(listeners.Select(point => point.Port))
                       .Should()
                       .NotContain(port);
        }
    }
}
