using System;
using PoS_InventoryFilterSlim.SimpleTest.MaxThroughput;

namespace PoS_InventoryFilterSlim
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowHeight = 80;
            new MaxThroughput().RunTest();

        }
    }
}
