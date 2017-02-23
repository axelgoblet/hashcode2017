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

            problem.VideoSizes = lines[1].Split(' ').Select(x => Convert.ToInt32(x)).ToList();

            var nextEndpoint = 2;
            Endpoint latestEndpoint = null;
            var lastIndex = 2;

            for (int i = 2; i < lines.Length; i++)
            {
                lastIndex = i;
                var unparsed = lines[i].Split(' ');
                if (unparsed.Length == 3)
                {
                    break;
                }

                if (nextEndpoint == i)
                {
                    latestEndpoint = new Endpoint
                    {
                        Latency = Convert.ToInt32(unparsed[0]),
                        ConnectedCacheServers = Convert.ToInt32(unparsed[1])
                    };

                    problem.Endpoints.Add(latestEndpoint);
                    nextEndpoint = i + latestEndpoint.ConnectedCacheServers + 1;
                }
                else
                {
                    var cache = new Cache
                    {
                        Id = Convert.ToInt32(unparsed[0]),
                        Latency = Convert.ToInt32(unparsed[1])
                    };

                    latestEndpoint.ConnectedCaches.Add(cache);
                }
            }

            for (int i = lastIndex; i < lines.Length; i++)
            {
                var unparsed = lines[i].Split(' ');

                problem.RequestDescriptions.Add(new RequestDescription
                {
                    VideoId = Convert.ToInt32(unparsed[0]),
                    EndpointId = Convert.ToInt32(unparsed[1]),
                    NumberOfRequests = Convert.ToInt32(unparsed[2])
                });
            }

            return problem;
        }
    }
}