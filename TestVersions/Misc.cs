using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PoS_InventoryFilterSlim.TestVersions
{
    public class Misc
    {
        /// <summary>
        /// Sets the processor affinity of the current thread. see http://stackoverflow.com/questions/12328751/set-thread-processor-affinity-in-microsoft-net
        /// </summary>
        /// <param name="cpus">A list of CPU numbers. The values should be
        /// between 0 and <see cref="Environment.ProcessorCount"/>.</param>
        public static void SetThreadProcessorAffinity(params int[] cpus)
        {
            if (cpus == null)
                throw new ArgumentNullException("cpus");
            if (cpus.Length == 0)
                throw new ArgumentException("You must specify at least one CPU.", "cpus");

            // Supports up to 64 processors
            long cpuMask = 0;
            foreach (int cpu in cpus)
            {
                if (cpu < 0 || cpu >= Environment.ProcessorCount)
                    throw new ArgumentException("Invalid CPU number.");

                cpuMask |= 1L << cpu;
            }

            // Ensure managed thread is linked to OS thread; does nothing on default host in current .Net versions
            Thread.BeginThreadAffinity();

#pragma warning disable 618
            // The call to BeginThreadAffinity guarantees stable results for GetCurrentThreadId,
            // so we ignore the obsolete warning
            int osThreadId = AppDomain.GetCurrentThreadId();
#pragma warning restore 618

            // Find the ProcessThread for this thread.
            ProcessThread thread = Process.GetCurrentProcess().Threads.Cast<ProcessThread>()
                                       .Where(t => t.Id == osThreadId).Single();
            // Set the thread's processor affinity
            thread.ProcessorAffinity = new IntPtr(cpuMask);
        }
    }
}
