# ATLAS Advanced Streaming Samples

![Build Status](https://mat-ocs.visualstudio.com/Telemetry%20Analytics%20Platform/_apis/build/status/MAT.OCS.Streaming/Streaming%20Samples?branchName=develop)

Table of Contents
=================
<!--ts-->
* [Introduction](/README.md)
* C# Samples
    * [Writing Data](/docs/CSharp/WritingData.md)
    * [Reading Data](/docs/CSharp/ReadingData.md)
* MATLAB Samples
    * [Introduction to .NET MATLAB integration](/docs/Matlab/IntroToNetMatlabIntegration.md)
    * [Reading data from Kafka](/docs/Matlab/ReadingDataFromKafka.md)
    * [Writing data to Kafka](/docs/Matlab/WritingDataToKafka.md)
        * [ATLAS 10 Configuration](/docs/Matlab/Atlas10Configuration.md)
    * [Reading and writing in pipeline](/docs/Matlab/ReadingAndWritingInPipeline.md)
<!--te-->

# Reading Data

Demonstrates reading from a stream and printing time and parameter data to the Console.

```c#
using System;
using MAT.OCS.Streaming.IO;
using MAT.OCS.Streaming.Kafka;

namespace MAT.OCS.Streaming.Samples.CSharp
{
    public class ReadSample
    {
        public string BrokerList { get; set; } = "localhost:9092";

        public Uri DependenciesUri { get; set; } = new Uri("http://localhost:8180/api/dependencies/");

        public void Run()
        {
            var dataFormatClient = new DataFormatClient(new HttpDependencyClient(DependenciesUri, "dev"));

            Console.WriteLine("Hit ENTER to stop");

            using (var client = new KafkaStreamClient(BrokerList))
            using (client.StreamTopic("demo").Into(streamId => ProcessStream(streamId, dataFormatClient)))
            {
                Console.ReadLine();
            }
        }

        private static IStreamInput ProcessStream(string streamId, DataFormatClient dataFormatClient)
        {
            Console.WriteLine($"New stream: {streamId}");
            var input = new SessionTelemetryDataInput(streamId, dataFormatClient);
            input.DataInput.BindDefaultFeed("vCar:Chassis").DataBuffered += PrintSamples;
            input.StreamFinished += (sender, args) => Console.WriteLine("--------");
            return input;
        }

        private static void PrintSamples(object input, IO.TelemetryData.TelemetryDataFeedEventArgs e)
        {
            var data = e.Buffer.GetData();
            var time = data.TimestampsNanos;
            var vCar = data.Parameters[0].AvgValues;
            for (var i = 0; i < time.Length; i++)
            {
                Console.WriteLine($"{data.EpochNanos + time[i]}, {vCar[i]}");
            }
        }
    }
}
```