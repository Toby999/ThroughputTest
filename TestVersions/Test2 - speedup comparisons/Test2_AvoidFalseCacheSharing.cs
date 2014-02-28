using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PoS_InventoryFilterSlim.SimpleTest.MaxThroughput
{
    public class Test_2 : Test_1
    {
        protected new long NmbrOfRequests { get { return Config.NmbrOfRequests + Config.NbrOfRedos; } }

     
        public override long Run(Stopwatch timer)
        {
            long totalSum = 0;
            var nbrOfrequests = NmbrOfRequests;
            GC.Collect();
            timer.Start();
            Parallel.For(0, Config.NbrOfCores, threadId =>
                {
                    long sum = 0;
                    var intArrayPerThread = _longArray[threadId];
                    for (int redo = 0; redo < Config.NbrOfRedos; redo++)
                        for (long i = redo; i < nbrOfrequests; i += Config.Step)
                        {
                            sum = intArrayPerThread[i] + 1;        //DIFF: avoid the public class variable since it may cause false cache sharing through the cache coherence protocol.
                        }
                    totalSum += sum;
                });
            timer.Stop();

            return totalSum;    //DIFF: return the accumelated number in order to avoid 
        }
    }
}

/*result
Step   10: Throughput:   461,7 MReq/s and       27,5 GB/s (64),   Timetaken/request:      2,2 ns/req, Total TimeTaken:  3119 msec, Total Requests:   1 440 000 120
 
Nope: there seem to be no problem with false cache sharing. Unless the effect of the actual addition out weighs the effect of the false cache sharing...
 * 
 */
