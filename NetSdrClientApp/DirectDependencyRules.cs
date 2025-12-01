using EchoTspServer;

namespace NetSdrClientApp
{
    public class DirectDependencyRules
    {
        public class DirectDependencyImplementation
        {
            public void CreateServer()
            {
                var server = new EchoServer(5000);
            }
        }
    }
}
