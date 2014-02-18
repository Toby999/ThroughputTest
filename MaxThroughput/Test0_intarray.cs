using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace PoS_InventoryFilterSlim.SimpleTest.MaxThroughput
{          
    public partial class MaxThroughput
    {
        public long[][] _longArray;

        private void Fill_IntArray()
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

        private int Test0_1(Stopwatch timer,  out string testName)
        {
            var byteSize = 64;//72B -->read: object pointer lookup to object + object: 8 + 24: 32 B -> 64 bytes cache line.
            var testNethod = MethodBase.GetCurrentMethod().Name;
            testName = string.Format("{0}_step{1}_adsize{2}", testNethod, Config.Step, byteSize);
            Console.WriteLine(testName);

GC.Collect();
timer.Start();
Parallel.For(0, Config.NbrOfCores, threadId =>
    {
        var intArrayPerThread = _longArray[threadId];
        for (int redo = 0; redo < Config.NbrOfRedos; redo++)
            for (long i = 0; i < Config.NmbrOfRequests; i += Config.Step) 
                _result = intArrayPerThread[i];                        
    });
timer.Stop();

/*result
Step   10: Throughput:   570,3 MReq/s and         34 GB/s (64),   Timetaken/request:      1,8 ns/req, Total TimeTaken: 12624 msec, Total Requests:   7 200 000 000
Step  100: Throughput:   462,0 MReq/s and       27,5 GB/s (64),   Timetaken/request:      2,2 ns/req, Total TimeTaken: 15586 msec, Total Requests:   7 200 000 000
Step 1000: Throughput:   236,6 MReq/s and       14,1 GB/s (64),   Timetaken/request:      4,2 ns/req, Total TimeTaken: 30430 msec, Total Requests:   7 200 000 000
 * 
 * USING 12 cores:
Step   10: Throughput:   551,6 MReq/s and       32,9 GB/s (64),   Timetaken/request:      1,8 ns/req, Total TimeTaken: 26107 msec, Total Requests:  14 400 000 000
Step  100: Throughput:   506,6 MReq/s and       30,2 GB/s (64),   Timetaken/request:        2 ns/req, Total TimeTaken: 28425 msec, Total Requests:  14 400 000 000
Step 1000: Throughput:   259,4 MReq/s and       15,5 GB/s (64),   Timetaken/request:      3,9 ns/req, Total TimeTaken: 55507 msec, Total Requests:  14 400 000 000
 */

return byteSize;  
        }
    }
}
