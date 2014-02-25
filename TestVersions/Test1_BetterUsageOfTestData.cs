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
                _longArray[threadId] = new long[NmbrOfRequests];
                for (int i = 0; i < NmbrOfRequests; i++)
                {
                    _longArray[threadId][i] = i;
                    if (i%(int) 1e7 == 0) Console.Write(".");
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
                _result = intArrayPerThread[i] + 1;
    });
timer.Stop();

            return 0;
        }
    }
}

/*result
Step    1: Throughput:  4519,8 MReq/s and      269,4 GB/s (64),   Timetaken/request:      0,2 ns/req, Total TimeTaken:  3186 msec, Total Requests:  14 400 001 200, Redo=10
Step   10: Throughput:   463,0 MReq/s and       27,6 GB/s (64),   Timetaken/request:      2,2 ns/req, Total TimeTaken:  3110 msec, Total Requests:   1 440 000 120, Redo=10
Step  100: Throughput:   432,4 MReq/s and       25,8 GB/s (64),   Timetaken/request:      2,3 ns/req, Total TimeTaken:   333 msec, Total Requests:     144 000 000, Redo=10
Step  100: Throughput:   438,1 MReq/s and       26,1 GB/s (64),   Timetaken/request:      2,3 ns/req, Total TimeTaken:  3287 msec, Total Requests:   1 440 001 200, Redo=100
Step 1000: Throughput:   194,6 MReq/s and       11,6 GB/s (64),   Timetaken/request:      5,1 ns/req, Total TimeTaken:    74 msec, Total Requests:      14 400 000, Redo=10
Step 1000: Throughput:   228,5 MReq/s and       13,6 GB/s (64),   Timetaken/request:      4,4 ns/req, Total TimeTaken:  6303 msec, Total Requests:   1 440 012 000, Redo=1000
 

 */
