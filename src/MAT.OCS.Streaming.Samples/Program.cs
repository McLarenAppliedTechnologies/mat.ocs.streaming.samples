using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MAT.OCS.Streaming.Samples.Models;
using MAT.OCS.Streaming.Samples.Samples;
using MAT.OCS.Streaming.Samples.Samples.Basic;

using NLog;

namespace MAT.OCS.Streaming.Samples
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Samples show how to read and write Telemetry Data and Telemetry Samples
            // For basic usage please look at the samples in the Samples/Basic folder

            /* Read Telemetry Data

            var tData = new TData();
            tData.ReadTData();
            
             */

            // Write Telemetry Data
            List<Task> tasks = new List<Task>();
            for (var i = 0; i <= 5; i++)
            {
                var counter = i;
                tasks.Add(Task.Run(() =>
                {
                    var tData = new TData();
                    Console.WriteLine($"Sending TData #{counter}");
                    tData.WriteTData(false);
                }
                ).ContinueWith((task) =>
                {
                    Console.WriteLine($"done sending TData #{counter}");
                }));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Done sending TDatas");

            /*   tData = new TData();
               tData.WriteTData(true);*/

            /* Read Telemetry Samples

            var tSamples = new TSamples();
            tSamples.ReadTSamples();
             */


            //  Write Telemetry Samples
         /*   Console.WriteLine("Sending TSamples");
            var tSamples = new TSamples();
            tSamples.WriteTSamples(false);*/

           /* new TSamples();
            tSamples.WriteTSamples(true);*/


            /*
            var model = new ModelSample();
            model.Run();
            */

            // For advanced usage with structured code please look at the samples in the Samples folder

            /* Read/Write/Read and link TDataSingleFeedSingleParameter

            TDataSingleFeedSingleParameter.Read();
            TDataSingleFeedSingleParameter.Write();
            TDataSingleFeedSingleParameter.ReadAndLink();

             */
        }
    }
}
 