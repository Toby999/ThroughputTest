using System;
using System.Diagnostics;
using System.Threading;
using PoS_InventoryFilterSlim.TestVersions;


namespace PoS_InventoryFilterSlim.SimpleTest.MaxThroughput
{
    public class Test_6 : Test_5
    {
        private Stopwatch _timer;
        private readonly object _lock = new object();
        private readonly object _lockSum = new object();

        private static long _totalSum;

        public override long Run(Stopwatch timer)
        {
            _timer = timer;
            _totalSum = 0;
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
            var loopUnrooledStep = Config.Step * 100;

            var a = _longArray[threadId];

            Misc.SetThreadProcessorAffinity(threadId);
            unchecked
            {                
                WaitForAllThreadsThenStartTimer();
                for (int redo = 0; redo < Config.NbrOfRedos; redo++)
                {
                    for (long i = redo; i < nbrOfrequests; i += loopUnrooledStep)
                        sum &= a[i] & a[i + 10] & a[i + 20] & a[i + 3] & a[i + 40] & a[i + 50] & a[i + 60] & a[i + 70] & a[i + 80] & a[i + 90] +
                               a[i + 100] & a[i + 110] & a[i + 120] & a[i + 130] & a[i + 140] & a[i + 150] & a[i + 160] & a[i + 170] & a[i + 180] & a[i + 190] +
                               a[i + 200] & a[i + 210] & a[i + 220] & a[i + 230] & a[i + 240] & a[i + 250] & a[i + 260] & a[i + 270] & a[i + 280] & a[i + 290] +
                               a[i + 300] & a[i + 310] & a[i + 320] & a[i + 330] & a[i + 340] & a[i + 350] & a[i + 360] & a[i + 370] & a[i + 380] & a[i + 390] +
                               a[i + 400] & a[i + 410] & a[i + 420] & a[i + 430] & a[i + 440] & a[i + 450] & a[i + 460] & a[i + 470] & a[i + 480] & a[i + 490] +
                               a[i + 500] & a[i + 510] & a[i + 520] & a[i + 530] & a[i + 540] & a[i + 550] & a[i + 560] & a[i + 570] & a[i + 580] & a[i + 590] +
                               a[i + 600] & a[i + 610] & a[i + 620] & a[i + 630] & a[i + 640] & a[i + 650] & a[i + 660] & a[i + 670] & a[i + 680] & a[i + 690] +
                               a[i + 700] & a[i + 710] & a[i + 720] & a[i + 730] & a[i + 740] & a[i + 750] & a[i + 760] & a[i + 770] & a[i + 780] & a[i + 790] +
                               a[i + 800] & a[i + 810] & a[i + 820] & a[i + 830] & a[i + 840] & a[i + 850] & a[i + 860] & a[i + 870] & a[i + 880] & a[i + 890] +
                               a[i + 900] & a[i + 910] & a[i + 920] & a[i + 930] & a[i + 940] & a[i + 950] & a[i + 960] & a[i + 970] & a[i + 980] & a[i + 990];
                }
                StopWallClockIfLast();
           }
            lock (_lockSum) _totalSum += sum;
        }

        private static int _threadStartCount = 0, _threadEndCount = 0;
        private void WaitForAllThreadsThenStartTimer()
        {                                
            var localThreadId = Interlocked.Increment(ref _threadStartCount);

            SpinWait.SpinUntil(() => _threadStartCount == Config.NbrOfCores);

            if (localThreadId == 1)    //all threads should run in parallel and be started so just start on one of the threads.
            {
                _timer.Start();
            }
        }

        private void StopWallClockIfLast()
        {
            var localThreadId = Interlocked.Increment(ref _threadEndCount);                 
            if (localThreadId == Config.NbrOfCores)
                {
                    _timer.Stop();
                }
            }
        } 
    }

/*result (3 runs)
Step   10: Throughput:   463,8 MReq/s and       27,6 GB/s (64),   Timetaken/request:      2,2 ns/req, Total TimeTaken:  3105 msec, Total Requests:   1 440 000 120
Step   10: Throughput:   464,2 MReq/s and       27,7 GB/s (64),   Timetaken/request:      2,2 ns/req, Total TimeTaken:  3102 msec, Total Requests:   1 440 000 120
Step   10: Throughput:   464,5 MReq/s and       27,7 GB/s (64),   Timetaken/request:      2,2 ns/req, Total TimeTaken:  3100 msec, Total Requests:   1 440 000 120

 * * 
 * 
Nope: there seem to be no problem with false cache sharing. Unless the effect of the actual addition out weighs the effect of the false cache sharing...
 * 
 */
