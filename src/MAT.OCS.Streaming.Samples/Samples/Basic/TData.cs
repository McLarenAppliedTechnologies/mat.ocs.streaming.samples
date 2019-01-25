using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using MAT.OCS.Streaming.IO;
using MAT.OCS.Streaming.Kafka;
using MAT.OCS.Streaming.Model;
using MAT.OCS.Streaming.Model.AtlasConfiguration;
using MAT.OCS.Streaming.Samples.CSharp;

namespace MAT.OCS.Streaming.Samples.Samples
{
    class TData
    {
        // The data tree structure in Atlas is built up by these values, and would look like this:
        // Chassis 
        //        |- State 
        //               |- vCar
        //                      (vCar:Chassis, kmh, ...)
        private const string AppGroupId = "Chassis";
        private const string ParameterGroupId = "State";
        private const string ParameterId = "vCar:Chassis";
        private const string ParameterName = "vCar";
        private const string ParameterUnits = "kmh";

        private const double Frequency = 100; // The frequency used in the DataFormat. The default frequency is 100 anyway.
        private const long Interval = (long)(1000 / Frequency * 1000000L); // interval in nanoseconds, calculated by the given frequency

        // A single group and single parameter Atlas configuration sample.
        public static AtlasConfiguration AtlasConfiguration = new AtlasConfiguration
        {
            AppGroups =
            {
                {
                    AppGroupId, new ApplicationGroup
                    {
                        Groups =
                        {
                            {
                                ParameterGroupId, new ParameterGroup
                                {
                                    Parameters =
                                    {
                                        {
                                            ParameterId,
                                            new Parameter {Name = ParameterName, Units = ParameterUnits}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        public void ReadTData()
        {
            var dependencyServiceUri = new Uri("http://localhost:8180/api/dependencies/"); // The URI where the dependency services are running
            const string brokerList = "localhost:9092"; // The host and port where the Kafka broker is running
            const string groupName = "dev"; // The group name
            const string topicName = "sample_in"; // The existing topic's name in the Kafka broker. The *_annonce topic name must exist too. In this case the sample_in_announce
            var client = new KafkaStreamClient(brokerList); // Create a new KafkaStreamClient for connecting to Kafka broker
            var dataFormatClient = new DataFormatClient(new HttpDependencyClient(dependencyServiceUri, groupName, true)); // Create a new DataFormatClient

            //read
            var pipeline = client.StreamTopic(topicName).Into(streamId => // Stream Kafka topic into the handler method
            {
                var input = new SessionTelemetryDataInput(streamId, dataFormatClient);
                input.DataInput.BindDefaultFeed(ParameterId).DataBuffered += (sender, e) => // Bind the incoming feed and take the data
                {
                    var data = e.Buffer.GetData();
                    // In this sample we consume the incoming data and print it
                    var time = data.TimestampsNanos;
                    for (var i = 0; i < data.Parameters.Length; i++)
                    {
                        Trace.WriteLine($"Parameter[{i}]:");
                        var vCar = data.Parameters[i].AvgValues;
                        for (var j = 0; j < time.Length; j++)
                        {
                            var fromMilliseconds = TimeSpan.FromMilliseconds(time[j].NanosToMillis());
                            Trace.WriteLine($"{fromMilliseconds:hh\\:mm\\:ss\\.fff}, { NumberToBarString.Convert(vCar[j]) }");
                        }
                    }
                };

                input.StreamFinished += (sender, e) => Trace.WriteLine("Finished"); // Handle the steam finished event
                return input;
            });

            if (!pipeline.WaitUntilConnected(TimeSpan.FromSeconds(30), CancellationToken.None)) // Wait until the connection is established
                throw new Exception("Couldn't connect");
            pipeline.WaitUntilFirstStream(TimeSpan.FromMinutes(1), CancellationToken.None); // Wait until the first stream is ready to read.

        }
    }
}
