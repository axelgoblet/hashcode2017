using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

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
            // var problem = Parser.Load("../../../Input/me_at_the_zoo.in");
            // var problem = Parser.Load("../../../Input/videos_worth_spreading.in");
            var problem = Parser.Load("../../../Input/trending_today.in");
            // var problem = Parser.Load("../../../Input/kittens.in");

            Console.WriteLine("Hello!");

            foreach (var endpoint in problem.Endpoints)
            {
                endpoint.ConnectedCaches = endpoint.ConnectedCaches.Where(c => c.Latency < endpoint.Latency).ToList();
            }


            foreach (var cacheServer in problem.CacheServers)
            {
                cacheServer.Endpoints =
                    problem.Endpoints.Where(x => x.ConnectedCaches.Any(y => y.Id == cacheServer.Id)).ToList();
            }

            var solution = new Solution();

            foreach (var cacheServer in problem.CacheServers)
            {
                solution.CacheServers.Add(new CacheServer { Id = cacheServer.Id });
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
                        y.Value = y.RequestCount / y.Video.Size;
                        return y;
                    })
                    .OrderByDescending(x => x.Value)
                    .ToList();

            var count = 0;

            while (videoRequests.Count > 0)
            {
                var videoStatistic = videoRequests.First();

                CacheServer bestCache = null;
                var bestScore = 0;
                List<RequestDescription> bestUsedRequestDescriptions = null;
                foreach (var cacheServer in problem.CacheServers)
                {
                    var intersect = videoStatistic.Endpoints.Intersect(cacheServer.Endpoints).Select(e => e.Id).ToList();
                    var usedRequestDescriptions =
                        problem.RequestDescriptions.Where(
                            r => intersect.Contains(r.EndpointId) && r.VideoId == videoStatistic.Video.Id).ToList();
                    var score = usedRequestDescriptions.Select(r => r.NumberOfRequests).Sum();
                    if (bestCache == null || score > bestScore)
                    {
                        var solutionCache = solution.CacheServers.Single(y => y.Id == cacheServer.Id);

                        // cache full
                        if (solutionCache.CurrentSize + videoStatistic.Video.Size > problem.CacheServerCapacity)
                        {
                            continue;
                        }

                        // video already in cache
                        if (solutionCache.VideoIds.Contains(videoStatistic.Video.Id))
                        {
                            continue;
                        }

                        bestCache = cacheServer;
                        bestScore = score;
                        bestUsedRequestDescriptions = usedRequestDescriptions;
                    }
                }

                if (bestCache != null)
                {
                    var cache = solution.CacheServers.Single(y => y.Id == bestCache.Id);
                    cache.VideoIds.Add(videoStatistic.Video.Id);
                    cache.CurrentSize += videoStatistic.Video.Size;

                    // remove requests from ranking and reorder
                    videoStatistic.Value -= bestScore / videoStatistic.Video.Size;
                    videoRequests = videoRequests.OrderByDescending(v => v.Value).ToList();

                    count += bestUsedRequestDescriptions.Count;

                    // remove request descriptions used
                    foreach (var d in bestUsedRequestDescriptions)
                    {
                        problem.RequestDescriptions.Remove(d);
                    }

                    Console.WriteLine("Processed requests {0} / {1}", count, problem.NumberOfRequestDescriptions);
                }
                else
                {
                    // remove video from list (no more caches to fit it in)
                    videoRequests.RemoveAt(0);
                }
            }

            // Parser.Publish(solution, "../../../Output/me_at_the_zoo.out");
            // Parser.Publish(solution, "../../../Output/videos_worth_spreading.out");
            Parser.Publish(solution, "../../../Output/trending_today.out");
            // Parser.Publish(solution, "../../../Output/kittens.out");
        }
    }
}