﻿using System.Collections.Generic;
using System.Linq;

namespace HashCode2017.Solution
{
    class Program
    {
        static void Main(string[] args)
        {
            var problem = Parser.Load("../../../Input/me_at_the_zoo.in");

            problem.Endpoints = problem.Endpoints.Where(e => e.ConnectedCacheServers > 0).ToList();
            foreach (var endpoint in problem.Endpoints)
            {
                endpoint.ConnectedCaches = endpoint.ConnectedCaches.Where(c => c.Latency < endpoint.Latency).ToList();
            }

            var videoRequests =
                problem.RequestDescriptions.GroupBy(x => x.VideoId)
                    .Select(
                        grouping =>
                            new
                            {
                                VideoId = grouping.Key,
                                Endpoints = grouping.Select(x => problem.Endpoints.Single(y => x.EndpointId == y.Id)).Distinct().ToList(),
                                RequestCount = grouping.Sum(y => y.NumberOfRequests)
                            })
                    .OrderByDescending(x => x.RequestCount)
                    .ToList();

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