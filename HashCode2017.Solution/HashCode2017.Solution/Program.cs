﻿using System.Collections.Generic;

namespace HashCode2017.Solution
{
    class Program
    {
        static void Main(string[] args)
        {
            var problem = Parser.Load("../../../Input/me_at_the_zoo.in");

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