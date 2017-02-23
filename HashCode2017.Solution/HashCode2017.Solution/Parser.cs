using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode2017.Solution
{
    public static class Parser
    {
        public static Problem Load(string path)
        {
            var lines = File.ReadAllLines(path);
            var metaData = lines[0].Split(' ');

            var problem = new Problem
            {
                NumberOfVideos = Convert.ToInt32(metaData[0]),
                NumberOfEndpoints = Convert.ToInt32(metaData[1]),
                NumberOfRequestDescriptions = Convert.ToInt32(metaData[2]),
                NumberOfCacheServers = Convert.ToInt32(metaData[3]),
                CacheServerCapacity = Convert.ToInt32(metaData[4])
            };

            return problem;
        }
    }
}
