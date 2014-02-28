using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using PoS_InventoryFilterSlim.TestVersions;

/*Remaining TODO for this version: Pin each thread to a core.
 * 
 * 
 */ 
namespace PoS_InventoryFilterSlim.SimpleTest.MaxThroughput
{
    public class Test_3: Test_2
    {
        private Stopwatch _timer;
        private readonly object _lock = new object();
        private readonly object _lockSum = new object();
        
        private static long _totalSum;
        private int _threadFinishedCount;

        public override long Run(Stopwatch timer)
        {
            _timer = timer;
            _totalSum = 0;
            _threadFinishedCount = 0;
            var threads = new Thread[Config.NbrOfCores];
            var threadCount = Config.NbrOfCores;
            for (int threadId = 0; threadId < threadCount; threadId++)
            {
                var thread = new Thread(RunLoops)
                {
                    Name = threadId.ToString(),
                    IsBackground = true
                };
                threads[threadId] = thread;
            }

            GC.Collect();

            for (int i = 0; i < threadCount; i++) threads[i].Start();            
            for (int i = 0; i < threadCount; i++) threads[i].Join();

            return _totalSum;    
        }

        private void RunLoops()
        {
            long sum = 0;
            var nbrOfrequests = NmbrOfRequests;
            var threadId = Convert.ToInt32(Thread.CurrentThread.Name);
            var intArrayPerThread = _longArray[threadId];

            Misc.SetThreadProcessorAffinity(threadId);

            StartWallClockIfFirst();
            for (int redo = 0; redo < Config.NbrOfRedos; redo++)
            {
                for (long i = redo; i < nbrOfrequests; i += Config.Step)
                {
                    sum = intArrayPerThread[i] + 1;        
                }
            }
            StopWallClockIfLast();

            lock (_lockSum) _totalSum += sum;
        }

        private void StopWallClockIfLast()
        {
            //NOTE: a barrier could have been used instead. But this will be do.
            lock (_lock)
            {
                _threadFinishedCount++;
                if (_threadFinishedCount == Config.NbrOfCores)
                {
                    _timer.Stop();
                }
            }
        }


        private void StartWallClockIfFirst()
        {
            if (_timer.IsRunning) return;
            lock(_lock)
            {
                if (!_timer.IsRunning)
                {
                    _timer.Start();
                }
            }
        }
      
    }


}

/*result (3 runs)
Step   10: Throughput:   463,8 MReq/s and       27,6 GB/s (64),   Timetaken/request:      2,2 ns/req, Total TimeTaken:  3105 msec, Total Requests:   1 440 000 120
Step   10: Throughput:   462,1 MReq/s and       27,5 GB/s (64),   Timetaken/request:      2,2 ns/req, Total TimeTaken:  3116 msec, Total Requests:   1 440 000 120
Step   10: Throughput:   462,9 MReq/s and       27,6 GB/s (64),   Timetaken/request:      2,2 ns/req, Total TimeTaken:  3111 msec, Total Requests:   1 440 000 120

 * 
 * 
Nope: there seem to be no problem with false cache sharing. Unless the effect of the actual addition out weighs the effect of the false cache sharing...
 * 
 */
