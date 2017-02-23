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

            problem.Videos =
                lines[1].Split(' ')
                    .Select((value, index) => new Video {Id = index, Size = Convert.ToInt32(value)})
                    .ToList();

            var nextEndpoint = 2;
            Endpoint latestEndpoint = null;
            var lastIndex = 2;
            var cacheServers = new List<CacheServer>();
            var endpointId = 0;

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
                        Id = endpointId,
                        Latency = Convert.ToInt32(unparsed[0]),
                        ConnectedCacheServers = Convert.ToInt32(unparsed[1])
                    };

                    problem.Endpoints.Add(latestEndpoint);
                    nextEndpoint = i + latestEndpoint.ConnectedCacheServers + 1;
                    endpointId++;
                }
                else
                {
                    var cache = new Cache
                    {
                        Id = Convert.ToInt32(unparsed[0]),
                        Latency = Convert.ToInt32(unparsed[1])
                    };

                    if (!cacheServers.Any(y => y.Id == cache.Id))
                    {
                        cacheServers.Add(new CacheServer {Id = cache.Id});
                    }

                    latestEndpoint.ConnectedCaches.Add(cache);
                }
            }

            problem.CacheServers = cacheServers;

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

        public static void Publish(Solution solution, string path)
        {
            using (var stream = File.OpenWrite(path))
            using (var writeStream = new StreamWriter(stream))
            {
                writeStream.WriteLine(solution.CacheServers.Count);

                foreach (var cacheServer in solution.CacheServers)
                {
                    writeStream.WriteLine("{0} {1}", cacheServer.Id,
                        string.Join(" ", cacheServer.VideoIds.Select(x => x.ToString())));
                }
            }
        }
    }
}