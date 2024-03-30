namespace YarpGateway.Helpers
{
    public class YarpConfigs
    {
        public Dictionary<string, Route> Routes { get; set; }
        public Dictionary<string, Cluster> Clusters { get; set; }
    }

    public class Match
    {
        public string Path { get; set; }
    }

    public class TransformsItem
    {
        public string PathPattern { get; set; }
    }

    public class Route
    {
        public string ClusterId { get; set; }
        public Match Match { get; set; }
        public List<TransformsItem> Transforms { get; set; }
        public string RateLimiterPolicy { get; set; }
    }

    public class Destination1
    {
        public string Address { get; set; }
    }

    public class Destinations
    {
        public Destination1 destination1 { get; set; }
    }

    public class Cluster
    {
        public Destinations Destinations { get; set; }
    }
}
