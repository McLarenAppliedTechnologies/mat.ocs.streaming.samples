using System;
using System.Collections.Generic;
using System.Threading;
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

            /* Write Telemetry Data

            var tData = new TData();
            tData.WriteTData();

            */

            /* Read Telemetry Samples

            var tSamples = new TSamples();
            tSamples.ReadTSamples();

             */

            /* Write Telemetry Samples

            var tSamples = new TSamples();
            tSamples.WriteTSamples();

             */

             var model = new ModelSample();
             model.Run();

            // For advanced usage with structured code please look at the samples in the Samples folder

            /* Read/Write/Read and link TDataSingleFeedSingleParameter

            TDataSingleFeedSingleParameter.Read();
            TDataSingleFeedSingleParameter.Write();
            TDataSingleFeedSingleParameter.ReadAndLink();

             */
        }
    }
}
 