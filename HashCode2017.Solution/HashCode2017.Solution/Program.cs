using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        }
    }
}
