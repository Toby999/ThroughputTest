using System;
using System.Diagnostics;
using System.IO;
using PoS_InventoryFilterSlim.TestVersions;

namespace PoS_InventoryFilterSlim.SimpleTest.MaxThroughput
{
    public class TestEngine
    {
        public long _result;
        public int[] _resultArray;

        public void RunTest(BaseTest test)
        {
            if (!Config.VerifyDoableConfig()) throw new Exception("config values must match!");

            var timer = new Stopwatch();

            test.Fill();
            Console.WriteLine(test.TestName);
            test.Run(timer);

            WriteTestResult(test, timer);

            Console.ReadLine();
        }

        private static void WriteTestResult(BaseTest test, Stopwatch timer)
        {
            var timetakenInSec = timer.ElapsedMilliseconds/(double) 1000;
            var throughput_ReqPerSec = test.TotalNmbrOfRequests / timetakenInSec;
            var throughput_BytesPerSec = throughput_ReqPerSec* test.ByteSizePerRequest;
            var timeTakenPerRequestInNanos = Math.Round(1e6 * timer.ElapsedMilliseconds / test.TotalNmbrOfRequests, 1);
            string template =
                "Step {4,4}: Throughput: {0,7} MReq/s and {1,10} GB/s ({5}),   Timetaken/request: {2,8} ns/req, Total TimeTaken: {3,5} msec, Total Requests: {6,15}";

            var resultMReqPerSec = Math.Round(throughput_ReqPerSec/1e6, 1);
            var resultGBPerSec = Math.Round(throughput_BytesPerSec/1073741824, 1);
            var resultTimeTakenInSec = Math.Round(timetakenInSec, 1);

            string output = string.Format(template,
                                          resultMReqPerSec,
                                          resultGBPerSec,
                                          timeTakenPerRequestInNanos,
                                          timer.ElapsedMilliseconds,
                                          Config.Step,
                                          test.ByteSizePerRequest,
                                          test.TotalNmbrOfRequests.ToString("### ### ### ###"));
            Console.WriteLine(output);

            //write to file. Both Excel and notepad friendly version for copy paste to whatever.

            WriteTofile(test, resultMReqPerSec, resultGBPerSec, timeTakenPerRequestInNanos, resultTimeTakenInSec, output);
        }

        private static void WriteTofile(BaseTest test, double resultMReqPerSec, double resultGBPerSec,
                                        double timeTakenPerRequestInNanos, double resultTimeTakenInSec, string Output)
        {
            string templateHeader = "Step\tMReq/s\tMB/s({0})\tTimetaken/request (ns/req)\tTotal TimeTaken (sec.)";
            string result = "{0}\t{1}\t{2}\t{3}\t{4}";

            var filePath = @"Result\" + test.TestName + ".txt";

            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine(templateHeader, test.ByteSizePerRequest);
                sw.WriteLine(result, Config.Step,
                             resultMReqPerSec,
                             resultGBPerSec,
                             timeTakenPerRequestInNanos,
                             resultTimeTakenInSec);
                sw.WriteLine();
                sw.WriteLine(Output);
            }
        }
    }
}
