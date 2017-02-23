using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace HashCode2017.Solution
{
    class Program
    {
        public class VideoStatistics
        {
            private Video _video;
            private List<Endpoint> _endpoints;
            private int _requestCount;
            private int _value;

            public Video Video
            {
                get { return _video; }
            }

            public List<Endpoint> Endpoints
            {
                get { return _endpoints; }
            }

            public int RequestCount
            {
                get { return _requestCount; }
            }

            public int Value
            {
                get { return _value; }
                set { _value = value; }
            }

            public VideoStatistics(Video video, List<Endpoint> endpoints, int requestCount, int value)
            {
                _video = video;
                _endpoints = endpoints;
                _requestCount = requestCount;
                _value = value;
            }
        }

        static void Main(string[] args)
        {
            var problem = Parser.Load("../../../Input/me_at_the_zoo.in");

            problem.Endpoints = problem.Endpoints.Where(e => e.ConnectedCacheServers > 0).ToList();
            foreach (var endpoint in problem.Endpoints)
            {
                endpoint.ConnectedCaches = endpoint.ConnectedCaches.Where(c => c.Latency < endpoint.Latency).ToList();
            }


            foreach (var cacheServer in problem.CacheServers)
            {
                cacheServer.Endpoints =
                    problem.Endpoints.Where(x => x.ConnectedCaches.Any(y => y.Id == cacheServer.Id)).ToList();
            }

            var videoRequests =
                problem.RequestDescriptions.GroupBy(x => x.VideoId)
                    .Select(
                        grouping =>
                            new VideoStatistics(problem.Videos.Single(y => y.Id == grouping.Key),
                                grouping.Select(x => problem.Endpoints.Single(y => x.EndpointId == y.Id))
                                    .Distinct()
                                    .ToList(), grouping.Sum(y => y.NumberOfRequests), 0))
                    .Select(y =>
                    {
                        y.Value = y.RequestCount/y.Video.Size;
                        return y;
                    })
                    .OrderByDescending(x => x.Value)
                    .ToList();

            foreach (var videoStatistic in videoRequests)
            {
                var result = videoStatistic.Endpoints
                    .Select(endpoint => problem.CacheServers
                        .Count(cache => cache.Endpoints.Any(z => z.Id == endpoint.Id)))
                    .ToList();
            }

            var solution = Solve();
            Parser.Publish(solution, "../../../Output/me_at_the_zoo.out");
        }

        private static Solution Solve()
        {
            return new Solution
            {
                CacheServers = new List<CacheServer>
                {
                    new CacheServer
                    {
                        Id = 0,
                        VideoIds = new List<int> {1, 2}
                    },
                    new CacheServer
                    {
                        Id = 1,
                        VideoIds = new List<int> {3, 4, 5}
                    },
                }
            };
        }
    }
}