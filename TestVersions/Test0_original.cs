using System;
using System.Diagnostics;
using System.Threading.Tasks;
using PoS_InventoryFilterSlim.TestVersions;

namespace PoS_InventoryFilterSlim.SimpleTest.MaxThroughput
{          
    public class Test_0 : BaseTest
    {
        public override long TotalNmbrOfRequests
        {
            get { return Config.NmbrOfRequests / Config.Step * Config.NbrOfCores * Config.NbrOfRedos; }
        }

        public override void Fill()
        {
            _longArray = new long[Config.NbrOfCores][];
            Console.WindowWidth = 100;
            for (int threadId = 0; threadId < Config.NbrOfCores; threadId++)
            {
                _longArray[threadId] = new long[Config.NmbrOfRequests];
                for (int i = 0; i < Config.NmbrOfRequests; i++)
                {
                    _longArray[threadId][i] = i;
                    if (i%(int) 1e7 == 0) Console.Write(".");
                }
            }
            GC.Collect();
            Console.WriteLine(" Done.");
        }
          
  
        public override long Run(Stopwatch timer)
        {
            GC.Collect();
            timer.Start();
            Parallel.For(0, Config.NbrOfCores, threadId =>
                {
                    var intArrayPerThread = _longArray[threadId];
                    for (int redo = 0; redo < Config.NbrOfRedos; redo++)
                        for (long i = 0; i < Config.NmbrOfRequests; i += Config.Step) 
                            _result = intArrayPerThread[i]+1;
                });
            timer.Stop();

            return 0;
        }
    }
}

/*result USING 12 cores:
Step    1: Throughput:  4549,8 MReq/s and      271,2 GB/s (64),   Timetaken/request:      0,2 ns/req, Total TimeTaken:  3165 msec, Total Requests:  14 400 000 000
Step   10: Throughput:   466,2 MReq/s and       27,8 GB/s (64),   Timetaken/request:      2,1 ns/req, Total TimeTaken:  3089 msec, Total Requests:   1 440 000 000
Step  100: Throughput:     375 MReq/s and       22,4 GB/s (64),   Timetaken/request:      2,7 ns/req, Total TimeTaken:   384 msec, Total Requests:     144 000 000
Step 1000: Throughput:   214,9 MReq/s and       12,8 GB/s (64),   Timetaken/request:      4,7 ns/req, Total TimeTaken:    67 msec, Total Requests:      14 400 000
Step 1000: Throughput:   217,2 MReq/s and       12,9 GB/s (64),   Timetaken/request:      4,6 ns/req, Total TimeTaken:   663 msec, Total Requests:     144 000 000
Step 10000:Throughput:   146,9 MReq/s and        8,8 GB/s (64),   Timetaken/request:      6,8 ns/req, Total TimeTaken:    98 msec, Total Requests:      14 400 000

 * Comments: 
 * Step 1 result is obvious. Data is fetched in RAM and with a 64 byte cache block size, 7 out of 8 requests would be able to find in CPU caches instead.
 * Step 10: This should kill the cache block fetch making each iteration do a memory lookup. 
 * Step 100+: The degradation in throughput with step 100, 1000, and 10000 is a bit trickier to explain. 
 *            One explanation could be that the inner loop is looping less so that the overhead of the actual loop structure starts to kick in.  
 
 */
