using System.Collections.Generic;

namespace HashCode2017.Solution
{
    public class Solution
    {
        public Solution()
        {
            CacheServers = new List<CacheServer>();
        }

        public List<CacheServer> CacheServers { get; set; }
    }

    public class CacheServer
    {
        public CacheServer()
        {
            VideoIds = new List<int>();
            Endpoints = new List<Endpoint>();
        }

        public int Id { get; set; }

        public int CurrentSize { get; set; }

        public List<int> VideoIds { get; set; }

        public List<Endpoint> Endpoints { get; set; }
    }
}
