using System.Collections.Generic;

namespace HashCode2017.Solution
{
    public class Solution
    {
        public List<CacheServer> CacheServers { get; set; }
    }

    public class CacheServer
    {
        public CacheServer()
        {
            VideoIds = new List<int>();
        }

        public int Id { get; set; }

        public List<int> VideoIds { get; set; }
    }
}
