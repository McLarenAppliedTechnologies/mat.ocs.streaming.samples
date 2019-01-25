using MAT.OCS.Streaming.IO.TelemetryData;
using MAT.OCS.Streaming.Model;
using MAT.OCS.Streaming.Model.AtlasConfiguration;
using MAT.OCS.Streaming.Model.DataFormat;
using System;
using System.Collections.Generic;
using MAT.OCS.Streaming.IO.TelemetrySamples;
using System.Threading.Tasks;
using MAT.OCS.Streaming.IO;
using MAT.OCS.Streaming.IO.Session;

namespace MAT.OCS.Streaming.Samples.Samples
{
    public class Writer : IDisposable
    {
        private readonly Dictionary<string, TelemetryDataFeedOutput> dataOutputs = new Dictionary<string, TelemetryDataFeedOutput>();
        private readonly Dictionary<string, TelemetrySamplesFeedOutput> sampleOutputs = new Dictionary<string, TelemetrySamplesFeedOutput>();
        public string TopicName { get; }
        public DateTime? SessionStart => this.session?.SessionOutput?.SessionStart;

        public readonly SessionOutput SessionOutput;

        private readonly SessionTelemetryDataOutput session;
        public Writer(Uri dependencyServiceUri, AtlasConfiguration atlasConfiguration, DataFormat dataFormat, string group, IOutputTopic topic, bool enableCache = true)
        {
            var httpDependencyClient = new HttpDependencyClient(dependencyServiceUri, group, enableCache); // DependencyClient stores the Data format, Atlas Configuration
            var dataFormatClient = new DataFormatClient(httpDependencyClient); 
            var atlasConfigurationClient = new AtlasConfigurationClient(httpDependencyClient);
            var atlasConfigurationId = atlasConfigurationClient.PutAndIdentifyAtlasConfiguration(atlasConfiguration); // Uniq ID created for the AtlasConfiguration
            var dataFormatId = dataFormatClient.PutAndIdentifyDataFormat(dataFormat); // Uniq ID created for the Data Format
            this.TopicName = topic.TopicName;
            
            //Init Session
            this.session = new SessionTelemetryDataOutput(topic, dataFormatId, dataFormatClient);
            this.session.SessionOutput.AddSessionDependency(DependencyTypes.DataFormat, dataFormatId);
            this.session.SessionOutput.AddSessionDependency(DependencyTypes.AtlasConfiguration, atlasConfigurationId);
            this.SessionOutput = this.session.SessionOutput;
        }

        public void OpenSession(string sessionIdentifier)
        {
            if (this.session.SessionOutput.SessionState == StreamSessionState.Open) return;
            this.session.SessionOutput.SessionState = StreamSessionState.Open;
            this.session.SessionOutput.SessionStart = DateTime.Now;
            this.session.SessionOutput.SessionIdentifier = sessionIdentifier;
            this.session.SessionOutput.SendSession();
        }

        public void CloseSession()
        {
            if (this.session.SessionOutput.SessionState == StreamSessionState.Closed) return;
            // should be closed not truncated
            this.session.SessionOutput.SessionState = StreamSessionState.Closed;
            this.session.SessionOutput.SendSession();
        }

        public void Write(string feedName, TelemetryData telemetryData)
        {
            if (!this.dataOutputs.ContainsKey(feedName) || this.dataOutputs[feedName] == null)
                this.dataOutputs[feedName] = this.session.DataOutput.BindFeed(feedName);

            //get the feed from data feed outputs
            if (!this.dataOutputs.TryGetValue(feedName, out var outputFeed))
            {
                throw new Exception("Feed not found");
            }

            Task.WaitAll(outputFeed.EnqueueAndSendData(telemetryData));
            // Set earliest epochnano
            // set latest timestamp offset by its epoch
            // Figure out Session duration based on the difference of the above 2
           // output.SessionOutput.SessionDurationNanos =  output.SessionOutput.Sess
        }

        public void Write(List<string> feedNames, TelemetryData telemetryData)
        {
            foreach (var feedName in feedNames)
            {
                this.Write(feedName, telemetryData);
            }
        }

        public void Write(string feedName, TelemetrySamples telemetrySamples)
        {
            if (!this.sampleOutputs.ContainsKey(feedName) || this.sampleOutputs[feedName] == null)
                this.sampleOutputs[feedName] = this.session.SamplesOutput.BindFeed(feedName);

            //get the feed from data feed outputs
            if (!this.sampleOutputs.TryGetValue(feedName, out var outputFeed))
            {
                throw new Exception("Feed not found");
            }

            Task.WaitAll(outputFeed.SendSamples(telemetrySamples));
            // Set earliest epochnano
            // set latest timestamp offset by its epoch
            // Figure out Session duration based on the difference of the above 2
            // output.SessionOutput.SessionDurationNanos =  output.SessionOutput.Sess
        }


        public void Dispose()
        {
            if (this.session.SessionOutput.SessionState == StreamSessionState.Closed) return;
            this.session.SessionOutput.SessionState = StreamSessionState.Truncated;
            this.session.SessionOutput.SendSession();
        }
    }
}
