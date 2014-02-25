using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoS_InventoryFilterSlim.TestVersions
{
    public abstract class BaseTest
    {
        public long _result;
        public long[][] _longArray;

        public abstract long TotalNmbrOfRequests { get; }

        public abstract void Fill();
        public abstract long Run(Stopwatch timer);

        public string TestName
        {
            get { return string.Format("{0}_step{1}_cachesize{2}", GetType().Name, Config.Step, 64); } 
        }

        public int ByteSizePerRequest
        {
            get { return 64; } //can be overriden if some test is testing something bigger...
        }
    }
}
