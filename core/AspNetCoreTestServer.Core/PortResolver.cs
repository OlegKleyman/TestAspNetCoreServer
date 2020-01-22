using System.Net;
using System.Net.Sockets;

namespace AspNetCoreTestServer.Core
{
    public class PortResolver : IPortResolver
    {
        public int GetAvailableTcpPort()
        {
            using var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            const int randomOpenPort = 0;

            sock.Bind(new IPEndPoint(IPAddress.Loopback, randomOpenPort));

            return ((IPEndPoint) sock.LocalEndPoint).Port;
        }
    }
}