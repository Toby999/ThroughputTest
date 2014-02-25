This project is mererly examining the possibility of reaching max throughput of DDR3 memories. 
More information why here: http://stackoverflow.com/questions/20554123/how-to-maximize-ddr3-memory-data-transfer-rate

----- Project test structure:
The test structure is rather simple. It consists of a partial class that have tests defined in the Test0_xxx, Test1_xxx 
that tries out different versions of the test. This file tells the story of what changes exists:

--- Test0 - original
This is the original test as posted to the stackoverflow forum.


--- Test1 - Better usage of test data.
The previous version did not read all the data in the array. This does better at this by using the redo variable as the start. 
This way, when the next redo iteration occurs, we do not risk cache hit in L2 cache. 

--- Test2 - Avoid false sharing
Previous test assign the result to a public class field varaible. An assignment of some sort to something public is needed so the compiler does not optimize the code and simply remove the loop as unneccessary..
However, the hypotothesis was that since this public field variable is exposed to all threads (the compiler can't tell the difference), 
the field would be invalidated each loop iteration and cause a false sharing cache symptom since the cache coherence protocol is obliged to update all values in L1 and L2 caches.

No improved throughput.

--- Test3 - Remove .NET parallel construct.

