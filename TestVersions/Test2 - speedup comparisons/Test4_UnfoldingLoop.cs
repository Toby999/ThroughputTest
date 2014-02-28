using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace PoS_InventoryFilterSlim.SimpleTest.MaxThroughput
{
    public class Test_4 : Test_2
    {
        private Stopwatch _timer;
        private readonly object _lock = new object();
        private readonly object _lockSum = new object();

        private static long _totalSum;
        private int _threadFinishedCount;

        public override long Run(Stopwatch timer)
        {
            var nbrOfrequests = NmbrOfRequests;
            var loopUnrlooedStep = Config.Step * 100;
            long sum = 0;
            GC.Collect();
            timer.Start();
            Parallel.For(0, Config.NbrOfCores, threadId =>
            {
                var a = _longArray[threadId];
                for (int redo = 0; redo < Config.NbrOfRedos; redo++)
                {
                    for (long i = redo; i < nbrOfrequests; i += loopUnrlooedStep)
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

                    _totalSum &= sum;

                }
            });
            timer.Stop();

            return _totalSum;
        }
    }


}

/*result (3 runs)
Step   10: Throughput:   454,8 MReq/s and       27,1 GB/s (64),   Timetaken/request:      2,2 ns/req, Total TimeTaken:  3166 msec, Total Requests:   1 440 000 120

 * 
 * 
Nope: there seem to be no problem with false cache sharing. Unless the effect of the actual addition out weighs the effect of the false cache sharing...
 * 
 */
