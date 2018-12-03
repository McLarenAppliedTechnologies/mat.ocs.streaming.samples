using MAT.OCS.Streaming.IO;
using MAT.OCS.Streaming.IO.TelemetryData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using static MAT.OCS.Streaming.Samples.Samples.Models;

namespace MAT.OCS.Streaming.Samples.Samples
{
    public class Reader : IDisposable
    {
        private readonly TimeSpan connectionTimeoutInSeconds = TimeSpan.FromSeconds(30);
        private DataFormatClient DataFormatClient { get; }
        public IStreamPipelineBuilder StreamPipelineBuilder { get; }
        /// <summary>
        /// Reader constructor to set up the initial connections to Dependency Client, DataFormatClient and the StreamPipelineBuilder for reading the stream
        /// </summary>
        /// <param name="dependencyServiceUri">The URI where the dependency service is running and accessible</param>
        /// <param name="group">The environment specific group name</param>
        /// <param name="pipelineBuilder">The pipeline builder for the stream to read from</param>
        /// <param name="enableCache">Boolean flag to enable dependency caching in <see cref="HttpDependencyClient" /> </param>
        public Reader(Uri dependencyServiceUri, string group, IStreamPipelineBuilder pipelineBuilder, bool enableCache = true)
        {
            var dependenciesClient = new HttpDependencyClient(dependencyServiceUri, group, enableCache);
            this.DataFormatClient = new DataFormatClient(dependenciesClient);
            this.StreamPipelineBuilder = pipelineBuilder;
        }

        /// <summary>
        /// Read telemetry data from the stream for the given list of Atlas configuration parameters and handle the data by the TelemetryDataHandler delegate
        /// </summary>
        /// <param name="parameterIdentifiers">List of Atlas configuration parameter identifiers</param>
        /// <param name="handler">Use this for the streamed data handling, updating etc.</param>
        /// <returns>Pipeline representing the network resource streaming messages to the inputs.</returns>
        public IStreamPipeline Read(List<string> parameterIdentifiers, TelemetryDataHandler handler)
        {
            var pipeline = this.StreamPipelineBuilder.Into(streamId => this.Read(streamId, parameterIdentifiers, handler));

            if (!pipeline.WaitUntilConnected(this.connectionTimeoutInSeconds, CancellationToken.None))
                throw new Exception("Couldn't connect");

            return pipeline;
        }

        /// <summary>
        /// Read telemetry data from the stream for the given Atlas configuration parameter and handle the data by the TelemetryDataHandler delegate
        /// </summary>
        /// <param name="parameterIdentifier">The Atlas configuration parameter identifier</param>
        /// <param name="handler">Use this for the streamed data handling, updating etc.</param>
        /// <returns>Pipeline representing the network resource streaming messages to the inputs.</returns>
        public IStreamPipeline Read(string parameterIdentifier, TelemetryDataHandler handler)
        {
            var pipeline = this.StreamPipelineBuilder.Into(streamId => this.Read(streamId, parameterIdentifier, handler));

            if (!pipeline.WaitUntilConnected(this.connectionTimeoutInSeconds, CancellationToken.None))
                throw new Exception("Couldn't connect");

            return pipeline;
        }

        /// <summary>
        /// Read telemetry samples from the stream for the given Atlas configuration parameter and handle the data by the TelemetryDataHandler delegate
        /// </summary>
        /// <param name="parameterIdentifier">The Atlas configuration parameter identifier</param>
        /// <param name="handler">Use this for the streamed data handling, updating etc.</param>
        /// <returns>Pipeline representing the network resource streaming messages to the inputs.</returns>

        public IStreamPipeline Read(string parameterIdentifier, TelemetrySamplesHandler handler)
        {
            var pipeline = this.StreamPipelineBuilder.Into(streamId => this.ReadTSamples(handler));

            if (!pipeline.WaitUntilConnected(this.connectionTimeoutInSeconds, CancellationToken.None))
                throw new Exception("Couldn't connect");

            return pipeline;
        }

        /// <summary>
        /// Read telemetry data from the stream for the given Atlas configuration parameter and handle the data by the TelemetryDataHandler delegate
        /// and link to another output, using the <paramref name="writer" /> and the <paramref name="outputFeedName" /> 
        /// </summary>
        /// <param name="parameterIdentifier">The Atlas configuration parameter identifier</param>
        /// <param name="handler">Use this for the streamed data handling, updating etc.</param>
        /// <param name="writer">Writer object used for writing the streamed input to another output.</param>
        /// <param name="outputFeedName">The output feed name where the streamed input will be linked to, using the <paramref name="writer" /></param>
        /// <returns>Pipeline representing the network resource streaming messages to the inputs.</returns>
        public IStreamPipeline ReadAndLink(string parameterIdentifier, TelemetryDataHandler handler, Writer writer, string outputFeedName)
        {
            var pipeline = this.StreamPipelineBuilder.Into(new Func<string, IStreamInput>(streamId =>
                this.ReadAndLinkOutput(streamId, parameterIdentifier, writer, outputFeedName, handler)));

            if (!pipeline.WaitUntilConnected(this.connectionTimeoutInSeconds, CancellationToken.None))
                throw new Exception("Couldn't connect");

            return pipeline;
        }

        /// <summary>
        /// Read telemetry data from the stream for the given Atlas configuration parameter and handle the data by the TelemetryDataHandler delegate
        /// and link to another output, using the <paramref name="writer" /> and the <paramref name="outputFeedNames" /> 
        /// </summary>
        /// <param name="parameterIdentifier">The Atlas configuration parameter identifier</param>
        /// <param name="handler">Use this for the streamed data handling, updating etc.</param>
        /// <param name="writer">Writer object used for writing the streamed input to another output.</param>
        /// <param name="outputFeedNames">List of output feed names where the streamed input will be linked to, using the <paramref name="writer" /></param>
        /// <returns>Pipeline representing the network resource streaming messages to the inputs.</returns>
        public IStreamPipeline ReadAndLink(string parameterIdentifier, TelemetryDataHandler handler, Writer writer, List<string> outputFeedNames)
        {
            var pipeline = this.StreamPipelineBuilder.Into(new Func<string, IStreamInput>(streamId =>
                this.ReadAndLinkOutput(streamId, parameterIdentifier, writer, outputFeedNames, handler)));

            if (!pipeline.WaitUntilConnected(this.connectionTimeoutInSeconds, CancellationToken.None))
                throw new Exception("Couldn't connect");

            return pipeline;
        }

        /// <summary>
        /// Read telemetry data from the stream for the given Atlas configuration parameter and handle the data by the TelemetryDataHandler delegate
        /// and link to another output, using the <paramref name="writer" /> and the <paramref name="outputFeedName" /> 
        /// </summary>
        /// <param name="parameterIdentifiers">List of Atlas configuration parameter identifiers</param>
        /// <param name="handler">Use this for the streamed data handling, updating etc.</param>
        /// <param name="writer">Writer object used for writing the streamed input to another output.</param>
        /// <param name="outputFeedName">The output feed name where the streamed input will be linked to, using the <paramref name="writer" /></param>
        /// <returns>Pipeline representing the network resource streaming messages to the inputs.</returns>
        public IStreamPipeline ReadAndLink(List<string> parameterIdentifiers, TelemetryDataHandler handler, Writer writer, string outputFeedName)
        {
            var pipeline = this.StreamPipelineBuilder.Into(
                new Func<string, IStreamInput>(streamId => this.ReadAndLinkOutput(streamId, parameterIdentifiers, writer, outputFeedName, handler)));

            if (!pipeline.WaitUntilConnected(this.connectionTimeoutInSeconds, CancellationToken.None))
                throw new Exception("Couldn't connect");

            return pipeline;
        }

        /// <summary>
        /// Read telemetry data from the stream for the given Atlas configuration parameter and handle the data by the TelemetryDataHandler delegate
        /// and link to another output, using the <paramref name="writer" /> and the <paramref name="outputFeedNames" /> 
        /// </summary>
        /// <param name="parameterIdentifiers">List of Atlas configuration parameter identifiers</param>
        /// <param name="handler">Use this for the streamed data handling, updating etc.</param>
        /// <param name="writer">Writer object used for writing the streamed input to another output.</param>
        /// <param name="outputFeedNames">List of output feed names where the streamed input will be linked to, using the <paramref name="writer" /></param>
        /// <returns>Pipeline representing the network resource streaming messages to the inputs.</returns>

        public IStreamPipeline ReadAndLink(List<string> parameterIdentifiers, TelemetryDataHandler handler, Writer writer, List<string> outputFeedNames)
        {
            var pipeline = this.StreamPipelineBuilder.Into(
                new Func<string, IStreamInput>(streamId => this.ReadAndLinkOutput(streamId, parameterIdentifiers, writer, outputFeedNames, handler)));

            if (!pipeline.WaitUntilConnected(this.connectionTimeoutInSeconds, CancellationToken.None))
                throw new Exception("Couldn't connect");

            return pipeline;
        }

        private IStreamInput Read(
            string streamId,
            List<string> parameterIdentifiers,
            TelemetryDataHandler handler)
        {
            var input = new SessionTelemetryDataInput(streamId, this.DataFormatClient);
            input.DataInput.BindDefaultFeed(parameterIdentifiers).DataBuffered += (sender, e) => //Subscribing to data event
            {
                handler(e.Buffer.GetData());
            };

            input.StreamFinished += this.HandleStreamFinished;
            return input;
        }

        private IStreamInput Read(
            string streamId, 
            string parameterIdentifier,
            TelemetryDataHandler handler)
        {
            var input = new SessionTelemetryDataInput(streamId, this.DataFormatClient);
            input.DataInput.BindDefaultFeed(parameterIdentifier).DataBuffered += (sender, e) =>
            {
                handler(e.Buffer.GetData());
            };

            input.StreamFinished += this.HandleStreamFinished;
            return input;
        }

        private IStreamInput ReadTSamples(TelemetrySamplesHandler handler)
        {
            var input = new TelemetrySamplesInput(this.DataFormatClient);
            input.AutoBindFeeds((s,e) =>
            {
                handler(e.Data);
            });

            return null;
        }
        
        private IStreamInput ReadAndLinkOutput(
            string streamId,
            string parameterIdentifier,
            Writer writer,
            string outputFeedName,
            TelemetryDataHandler handler)
        {
            var input = new SessionTelemetryDataInput(streamId, this.DataFormatClient);
            Debug.WriteLine($"Linking stream {streamId}");
            // automatically propagate session metadata and lifecycle
            input.LinkToOutput(writer.SessionOutput, identifier => identifier + "_" + writer.TopicName);

            // react to data
            input.DataInput.BindDefaultFeed(parameterIdentifier).DataBuffered += (sender, e) =>
            {
                Debug.WriteLine($"Parameter for {streamId} received {parameterIdentifier}");
                var data = e.Buffer.GetData();
                handler(data);
                writer.Write(outputFeedName, data);
            };

            input.StreamFinished += this.HandleStreamFinished;
            return input;
        }

        private IStreamInput ReadAndLinkOutput(
            string streamId,
            string parameterIdentifier,
            Writer writer,
            List<string> outputFeedNames,
            TelemetryDataHandler handler)
        {
            var input = new SessionTelemetryDataInput(streamId, this.DataFormatClient);
            Debug.WriteLine($"Linking stream {streamId}");
            // automatically propagate session metadata and lifecycle
            input.LinkToOutput(writer.SessionOutput, identifier => identifier + "_" + writer.TopicName);

            // react to data
            input.DataInput.BindDefaultFeed(parameterIdentifier).DataBuffered += (sender, e) =>
            {
                Debug.WriteLine($"Parameter for {streamId} received {parameterIdentifier}");
                var data = e.Buffer.GetData();
                handler(data);
                foreach (var outputFeedName in outputFeedNames)
                {
                    writer.Write(outputFeedName, data);
                }
            };

            input.StreamFinished += this.HandleStreamFinished;
            return input;
        }

        private IStreamInput ReadAndLinkOutput(
            string streamId,
            List<string> parameterIdentifiers,
            Writer writer,
            string outputFeedName,
            TelemetryDataHandler handler)
        {
            var input = new SessionTelemetryDataInput(streamId, this.DataFormatClient);
            Debug.WriteLine($"Linking stream {streamId}");
            // automatically propagate session metadata and lifecycle
            input.LinkToOutput(writer.SessionOutput, identifier => identifier + "_" + writer.TopicName);

            // react to data
            input.DataInput.BindDefaultFeed(parameterIdentifiers).DataBuffered += (sender, e) =>
            {
                var data = e.Buffer.GetData();
                handler(data);
                writer.Write(outputFeedName, data);
            };

            input.StreamFinished += this.HandleStreamFinished;
            return input;
        }

        private IStreamInput ReadAndLinkOutput(
            string streamId,
            List<string> parameterIdentifiers,
            Writer writer,
            List<string> outputFeedNames,
            TelemetryDataHandler handler)
        {
            var input = new SessionTelemetryDataInput(streamId, this.DataFormatClient);
            Debug.WriteLine($"Linking stream {streamId}");
            // automatically propagate session metadata and lifecycle
            input.LinkToOutput(writer.SessionOutput, identifier => identifier + "_" + writer.TopicName);

            // react to data
            input.DataInput.BindDefaultFeed(parameterIdentifiers).DataBuffered += (sender, e) =>
            {
                var data = e.Buffer.GetData();
                handler(data);
                foreach (var outputFeedName in outputFeedNames)
                {
                    writer.Write(outputFeedName, data);
                }
            };

            input.StreamFinished += this.HandleStreamFinished;
            return input;
        }

        private void HandleStreamFinished(object sender, StreamEventArgs e)
        {
            //Dispose objects, close sessions if needed.
        }

        public void Dispose()
        {
        }
    }
}
