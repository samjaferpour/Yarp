using Microsoft.Extensions.Primitives;
using System.Collections.ObjectModel;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;

namespace YarpGateway.Helpers
{
    public class CustomProxyConfigProvider : IProxyConfigProvider
    {
        private CustomMemoryConfig _config;

        public CustomProxyConfigProvider()
        {
            // Load a basic configuration
            // Should be based on your application needs.
            var routeConfig = new RouteConfig
            {
                RouteId = "tag-route",
                ClusterId = "tag-cluster",
                Match = new RouteMatch
                {
                    Path = "/tags/{**catch-all}"
                },
                Transforms = new List<IReadOnlyDictionary<string, string>>
                {
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
                    {
                        { "PathPattern", "{**catch-all}" }
                    })
                }
            };

            var routeConfigs = new[] { routeConfig };

            var clusterConfigs = new[]
            {
                new ClusterConfig
                {
                    ClusterId = "tag-cluster",
                    LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        { "destination1", new DestinationConfig { Address = "http://localhost:5247/api/v1/Tag" } },
                    }
                }
            };

            _config = new CustomMemoryConfig(routeConfigs, clusterConfigs);
        }

        public IProxyConfig GetConfig() => _config;

        /// <summary>
        /// By calling this method from the source we can dynamically adjust the proxy configuration.
        /// Since our provider is registered in DI mechanism it can be injected via constructors anywhere.
        /// </summary>
        public void Update(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            var oldConfig = _config;
            _config = new CustomMemoryConfig(routes, clusters);
            oldConfig.SignalChange();
        }

        private class CustomMemoryConfig : IProxyConfig
        {
            private readonly CancellationTokenSource _cts = new CancellationTokenSource();

            public CustomMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
            {
                Routes = routes;
                Clusters = clusters;
                ChangeToken = new CancellationChangeToken(_cts.Token);
            }

            public IReadOnlyList<RouteConfig> Routes { get; }

            public IReadOnlyList<ClusterConfig> Clusters { get; }

            public IChangeToken ChangeToken { get; }

            internal void SignalChange()
            {
                _cts.Cancel();
            }
        }
    }
}
