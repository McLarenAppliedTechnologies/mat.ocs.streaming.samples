using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MAT.OCS.Streaming;
using MAT.OCS.Streaming.IO;
using MAT.OCS.Streaming.IO.TelemetryData;
using MAT.OCS.Streaming.Kafka;
using MAT.OCS.Streaming.Model;
using MAT.OCS.Streaming.Model.AtlasConfiguration;
using MAT.OCS.Streaming.Model.DataFormat;

namespace MAT.OCS.Streaming.Samples.Models
{
    public class ModelSample
    {
        private const string DependencyUrl = "http://10.228.4.9:8180/api/dependencies/";
        private const string InputTopicName = "ModelsInput";
        private const string OutputTopicName = "ModelsOutput";
        private const string BrokerList = "10.228.4.22";

        private string dataFormatId;
        private string atlasConfId;
        private HttpDependencyClient dependencyClient;
        private DataFormatClient dataFormatClient;

        public ModelSample()
        {
            // TODO: Replace this with dependency service REST API call.
            this.dependencyClient = new HttpDependencyClient(new Uri(DependencyUrl), "dev", false);

            this.dataFormatClient = new DataFormatClient(dependencyClient); // would be a web service in production
        }

        public void Run(CancellationToken cancellationToken = default(CancellationToken))
        {
            var outputDataFormat = DataFormat.DefineFeed()
                            .Parameters(new List<string> { "gTotal:vTag" })
                            .AtFrequency(100)
                            .BuildFormat();

            this.dataFormatId = dataFormatClient.PutAndIdentifyDataFormat(outputDataFormat);

            var acClient = new AtlasConfigurationClient(dependencyClient);

            var atlasConfiguration = this.CreateAtlasConfiguration();
            this.atlasConfId = acClient.PutAndIdentifyAtlasConfiguration(atlasConfiguration);

            using (var client = new KafkaStreamClient(BrokerList))
            {
                using (var outputTopic = client.OpenOutputTopic(OutputTopicName))
                using (var pipeline = client.StreamTopic(InputTopicName).Into(streamId => this.CreateStreamPipeline(streamId, outputTopic)))
                {
                    cancellationToken.WaitHandle.WaitOne();
                    pipeline.Drain();
                    pipeline.WaitUntilStopped(new TimeSpan(0, 0, 1), default(CancellationToken));
                }
            }
        }

        private IStreamInput CreateStreamPipeline(string streamId, IOutputTopic outputTopic)
        {
            var streamModel = new StreamModel(this.dataFormatClient, outputTopic, this.dataFormatId, this.atlasConfId);

            return streamModel.CreateStreamInput(streamId);
        }

        protected AtlasConfiguration CreateAtlasConfiguration()
        {
            return new AtlasConfiguration
            {
                AppGroups = new Dictionary<string, ApplicationGroup>
                {
                    {
                        "Models", new ApplicationGroup
                        {
                            Groups = new Dictionary<string, ParameterGroup>
                            {
                                {
                                    "TractionCircle", new ParameterGroup
                                    {
                                        Parameters = new Dictionary<string, Parameter>
                                        {
                                            {
                                                "gTotal:vTag", new Parameter
                                                {
                                                    Name = "gTotal",
                                                    PhysicalRange = new Range(0,10)
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
            }
                }
            };
        }
    }
}
