namespace AspNetCoreTestServer.Core
{
    public interface IPortResolver
    {
        int GetAvailableTcpPort();
    }
}