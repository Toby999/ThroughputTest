using System;
using System.Diagnostics;
using System.IO;

namespace PoS_InventoryFilterSlim.SimpleTest.MaxThroughput
{
    public partial class MaxThroughput
    {
        public long _result;
        public int[] _resultArray; 


        public void RunTest()
        {
            if (!Config.VerifyDoableConfig()) throw new Exception("config values must match!");

            Fill_IntArray();

            var timer = new Stopwatch();
            string testName;
            var byteSizePerRequest = Test0_1(timer, out testName);

            var timetakenInSec = timer.ElapsedMilliseconds / (double)1000;
            long totalNbrOfRequest = Config.NmbrOfRequests / Config.Step * Config.NbrOfCores * Config.NbrOfRedos;
            var throughput_ReqPerSec = totalNbrOfRequest / timetakenInSec;
            var throughput_BytesPerSec = throughput_ReqPerSec * byteSizePerRequest;
            var timeTakenPerRequestInNanos = Math.Round(1e6 * timer.ElapsedMilliseconds / totalNbrOfRequest, 1);
            string template = "Step {4,4}: Throughput: {0,7} MReq/s and {1,10} GB/s ({5}),   Timetaken/request: {2,8} ns/req, Total TimeTaken: {3,5} msec, Total Requests: {6,15}";

            var resultMReqPerSec = Math.Round(throughput_ReqPerSec / 1e6, 1);
            var resultGBPerSec = Math.Round(throughput_BytesPerSec / 1073741824, 1);
            var resultTimeTakenInSec = Math.Round(timetakenInSec, 1);

            string Output = string.Format(template,
                                          resultMReqPerSec,
                                          resultGBPerSec,
                                          timeTakenPerRequestInNanos,
                                          timer.ElapsedMilliseconds,
                                          Config.Step,
                                          byteSizePerRequest,
                                          totalNbrOfRequest.ToString("### ### ### ###"));
            Console.WriteLine(Output);

            string templateHeader = "Step\tMReq/s\tMB/s({0})\tTimetaken/request (ns/req)\tTotal TimeTaken (sec.)";
            string result = "{0}\t{1}\t{2}\t{3}\t{4}";

            var filePath = @"Result\" + testName + ".txt";

            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine(templateHeader, byteSizePerRequest);
                sw.WriteLine(result, Config.Step,
                                     resultMReqPerSec,
                                     resultGBPerSec,
                                     timeTakenPerRequestInNanos,
                                     resultTimeTakenInSec);
                sw.WriteLine();
                sw.WriteLine(Output);
            }

            Console.ReadLine();
        }
    }
}
