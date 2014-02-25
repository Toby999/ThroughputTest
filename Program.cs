using System;
using PoS_InventoryFilterSlim.SimpleTest.MaxThroughput;

namespace PoS_InventoryFilterSlim
{
    class Program
    {
        static void Main(string[] args)
        {
           Console.WindowHeight = 80;

           var test = new Test_2();

           new TestEngine().RunTest(test);
        }
    }

    public class Config
    {
        public const int Step = 10;
        public const int NbrOfRedos = 10;
        public const long NmbrOfRequests = (long)12e7;

        public const int NbrOfCores = 12;

        public static bool VerifyDoableConfig()
        {
            return (NmbrOfRequests % NbrOfCores == 0) &&
                   ((NmbrOfRequests / NbrOfCores) % Step == 0) &&
                   (NmbrOfRequests % Step == 0) &&
                   true;    //just fo Resharper purpose...
        }
    }

}
