using System;
using System.Diagnostics;
using System.Threading.Tasks;
using PoS_InventoryFilterSlim.TestVersions;

namespace PoS_InventoryFilterSlim.SimpleTest.MaxThroughput
{
    public class Test_1 : BaseTest
    {
        public override long TotalNmbrOfRequests
        {
            get { return NmbrOfRequests / Config.Step * Config.NbrOfCores * Config.NbrOfRedos; }  //NOTE: this is slightly erronous since i does not always start from 0. But the error should be small enough to not matter.  
        }

        protected long NmbrOfRequests { get { return Config.NmbrOfRequests + Config.NbrOfRedos; } }

        public override void Fill()
        {
            _longArray = new long[Config.NbrOfCores][];
            Console.WindowWidth = 100;

            for (int threadId = 0; threadId < Config.NbrOfCores; threadId++)
            {
                _longArray[threadId] = new long[NmbrOfRequests + 1000000];
                for (int i = 0; i < NmbrOfRequests + 1000000; i++)
                {
                    _longArray[threadId][i] = i % 10;               //DIFF: changed so that the value is between 0 and 9 instead.
                    if (i % (int)1e7 == 0) Console.Write(".");
                }
            }

            Console.WriteLine(" Fill done1");
        }

        public override long Run(Stopwatch timer)
        {
            var nbrOfrequests = NmbrOfRequests;

GC.Collect();
timer.Start();
Parallel.For(0, Config.NbrOfCores, threadId =>
    {
        var intArrayPerThread = _longArray[threadId];
        for (int redo = 0; redo < Config.NbrOfRedos; redo++)
            for (long i = redo; i < nbrOfrequests; i += Config.Step) 
                _result &= intArrayPerThread[i];
    });
timer.Stop();

            return 0;
        }
    }
}

/*result
Step   10: Throughput:   452,4 MReq/s and         27 GB/s (64),   Timetaken/request:      2,2 ns/req, Total TimeTaken:  3183 msec, Total Requests:   1 440 000 120
Step   10: Throughput:   463,2 MReq/s and       27,6 GB/s (64),   Timetaken/request:      2,2 ns/req, Total TimeTaken:  3109 msec, Total Requests:   1 440 000 120
 


Step    1: Throughput:  1211,1 MReq/s and       72,2 GB/s (64),   Timetaken/request:      0,8 ns/req, Total TimeTaken:  1189 msec, Total Requests:   1 440 000 012 */
