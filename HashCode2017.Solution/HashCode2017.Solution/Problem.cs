using System.Collections.Generic;

namespace HashCode2017.Solution
{
    public class Problem
    {
        public Problem()
        {
            Videos = new List<Video>();
            Endpoints = new List<Endpoint>();
            RequestDescriptions = new List<RequestDescription>();
        }

        public int NumberOfVideos { get; set; }

        public int NumberOfEndpoints { get; set; }

        public int NumberOfRequestDescriptions { get; set; }

        public int NumberOfCacheServers { get; set; }

        public int CacheServerCapacity { get; set; }

        public List<Video> Videos { get; set; }

        public List<Endpoint> Endpoints { get; set; }

        public List<RequestDescription> RequestDescriptions { get; set; }
    }

    public class Video
    {
        public int Id { get; set; }

        public int Size { get; set; }
    }

    public class Endpoint
    {
        public Endpoint()
        {
            ConnectedCaches = new List<Cache>();
        }

        public int Latency { get; set; }

        public int ConnectedCacheServers { get; set; }

        public List<Cache> ConnectedCaches { get; set; }
    }

    public class Cache
    {
        public int Id { get; set; }

        public int Latency { get; set; }
    }

    public class RequestDescription
    {
        public int VideoId { get; set; }

        public int EndpointId { get; set; }

        public int NumberOfRequests { get; set; }
    }
}
