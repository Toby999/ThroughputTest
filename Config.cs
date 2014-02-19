using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoS_InventoryFilterSlim.SimpleTest.MaxThroughput
{
    public class Config
    {
        public const long NmbrOfRequests = (long)12e7;
        public const int NbrOfCores = 12;
        public const int NbrOfRedos = 10;
        public const int Step = 10;

        public static bool VerifyDoableConfig()
        {
            return (NmbrOfRequests % NbrOfCores == 0) &&
                   ((NmbrOfRequests / NbrOfCores) % Step == 0) &&
                   (NmbrOfRequests % Step == 0) &&
                   true;
        }
    }
}
