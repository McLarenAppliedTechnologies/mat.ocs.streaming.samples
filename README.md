# ATLAS Advanced Streaming Samples

![Build Status](https://mat-ocs.visualstudio.com/Telemetry%20Analytics%20Platform/_apis/build/status/MAT.OCS.Streaming/Streaming%20Samples?branchName=develop)

Table of Contents
=================
<!--ts-->
* [Introduction - MAT.OCS.Streaming library](/README.md)
* [C# Samples](/README.md)
<!--te-->

# Introduction - MAT.OCS.Streaming Library

This API provides infrastructure for streaming data around the ATLAS technology platform. 

Using this API, you can: 
* Subscribe to streams of engineering values - no ATLAS recorder required 
* Inject parameters and aggregates back into the ATLAS ecosystem 
* Build up complex processing pipelines that automatically process new sessions as they are generated 

With support for Apache Kafka, the streaming API also offers: 
* Late-join capability - clients can stream from the start of a live session 
* Replay of historic streams 
* Fault-tolerant, scalable broker architecture 

## API Documentation
See [v1.8 documentation](https://mclarenappliedtechnologies.github.io/mat.atlas.advancedstreams-docs/1.8/)

## Knowledgebase
Be sure to look at our support knowledgebase on Zendesk: https://mclarenappliedtechnologies.zendesk.com/

## Scope
This pre-release version of the API demonstrates the event-based messaging approach, for sessions and simple telemetry data. 

Future versions will model all ATLAS entities, and likely offer better support for aggregates and predictive models. 

## Requirements
You need to install the following Nuget packages from [MAT source](https://mat-ocs.pkgs.visualstudio.com/_packaging/MAT/nuget/v3/index.json)

* MAT.OCS.Streaming
* MAT.OCS.Streaming.Kafka
* MAT.OCS.Streaming.Mqtt
* MAT.OCS.Streaming.Codecs.Protobuf

# C# Samples
## Basic samples
Basic samples demonstrate the simple usage of Advanced Streams, covering all the bare-minimum steps to implement Telematry Data and Telemetry Sample read and write to and from Kafka streams.

### Read Telemetry Data
First of all you need to configure the [dependencies](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L60-L66)
```cs
const string brokerList = "localhost:9092"; // The host and port where the Kafka broker is running
const string groupName = "dev"; // The group name
const string topicName = "data_in"; // The existing topic's name in the Kafka broker. The *_annonce topic name must exist too. In this case the data_in_announce
var dependencyServiceUri = new Uri("http://localhost:8180/api/dependencies/"); // The URI where the dependency services are running

var client = new KafkaStreamClient(brokerList); // Create a new KafkaStreamClient for connecting to Kafka broker
var dataFormatClient = new DataFormatClient(new HttpDependencyClient(dependencyServiceUri, groupName)); // Create a new DataFormatClient
```

The DependencyService is used to handle requests for AtlasConfigurations and DataFormats. You must provide an URI for this service. 
The DataFormatClient handles the data formats through the DependencyService for the given group name.

If you want to connect to MQTT, create an MqttStream client instead of KafkaStreamClient:
```cs
var client = new MqttStreamClient(new MqttConnectionConfig(brokerList, "userName", "password"));
```

Create a stream pipeline using the KafkaStreamClient and the topicName. Stream the messages [.Into your handler method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L68)
```cs
var pipeline = client.StreamTopic(topicName).Into(streamId => // Stream Kafka topic into the handler method
```

[Create a SessionTelemetryDataInput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L70) with the actual stream id and the dataFormatClient, and [bind the data input to your handler method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L71). 
```cs
var input = new SessionTelemetryDataInput(streamId, dataFormatClient);
input.DataInput.BindDefaultFeed(ParameterId).DataBuffered += (sender, e) => // Bind the incoming feed and take the data
{
}
```

In this example we bind the default feed and simply [print out some details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L72-L86) about the incoming data.
```cs
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
          Trace.WriteLine($"{fromMilliseconds:hh\\:mm\\:ss\\.fff}, {  new string('.', (int)(50 * vCar[j])) }");
      }
  }
};
```

You can optionally handle the [StreamFinished event](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L88).
```cs
input.StreamFinished += (sender, e) => Trace.WriteLine("Finished"); // Handle the steam finished event
```

In order to successfully read and consume the stream, make sure to [wait until connected](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L92-L93) and [wait for the first stream](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L94). Optionally you can tell the pipeline to wait for a specific time [while the stream is being idle](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L95), before exiting from the process.
```cs
if (!pipeline.WaitUntilConnected(TimeSpan.FromSeconds(30), CancellationToken.None)) // Wait until the connection is established
     throw new Exception("Couldn't connect");
pipeline.WaitUntilFirstStream(TimeSpan.FromMinutes(1), CancellationToken.None); // Wait until the first stream is ready to read.
pipeline.WaitUntilIdle(TimeSpan.FromMinutes(5), CancellationToken.None); // Wait for 5 minutes of the pipeline being idle before exit.
```

### Write Telemetry Data
First of all you need to configure the [dependencies](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/update_samples/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L102-L113)
```cs
const string brokerList = "localhost:9092"; // The host and port where the Kafka broker is running
const string groupName = "dev"; // The group name
const string topicName = "data_in"; // The existing topic's name in the Kafka broker. The *_annonce topic name must exist too. In this case the data_in_announce
var dependencyServiceUri = new Uri("http://localhost:8180/api/dependencies/"); // The URI where the dependency services are running

var client = new KafkaStreamClient(brokerList); // Create a new KafkaStreamClient for connecting to Kafka broker
var dataFormatClient = new DataFormatClient(new HttpDependencyClient(dependencyServiceUri, groupName)); // Create a new DataFormatClient
var httpDependencyClient = new HttpDependencyClient(dependencyServiceUri, groupName); // DependencyClient stores the Data format, Atlas Configuration

var atlasConfigurationId = new AtlasConfigurationClient(httpDependencyClient).PutAndIdentifyAtlasConfiguration(AtlasConfiguration); // Uniq ID created for the AtlasConfiguration
var dataFormat = DataFormat.DefineFeed().Parameter(ParameterId).BuildFormat(); // Create a dataformat based on the parameters, using the parameter id
var dataFormatId = dataFormatClient.PutAndIdentifyDataFormat(dataFormat); // Uniq ID created for the Data Format
```

The DependencyService is used to handle requests for AtlasConfigurations and DataFormats. You must provide an URI for this service. 
The DataFormatClient handles the data formats through the DependencyService for the given group name.
DataFormat is required when writing to stream, as it is used to define the structre of the data and dataFormatId is used to retrieve dataformat from the dataFormatClient.

AtlasConfigurationId is needed only if you want to display your data in Atlas10.

If you want to connect to MQTT, create an MqttStream client instead of KafkaStreamClient:
```cs
var client = new MqttStreamClient(new MqttConnectionConfig(brokerList, "userName", "password"));
```

[Open the output topic](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L115) using the preferred client (KafkaStreamClient or MqttStreamClient) and the topicName.
```cs
using (var outputTopic = client.OpenOutputTopic(topicName)) // Open a KafkaOutputTopic
{
}
```

[Create a SessionTelemetryDataOutput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L117) and configure session output [properties](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L118-L123).
```cs
var output = new SessionTelemetryDataOutput(outputTopic, dataFormatId, dataFormatClient);
output.SessionOutput.AddSessionDependency(DependencyTypes.DataFormat, dataFormatId); // Add session dependencies to the output
output.SessionOutput.AddSessionDependency(DependencyTypes.AtlasConfiguration, atlasConfigurationId);

output.SessionOutput.SessionState = StreamSessionState.Open; // set the sessions state to open
output.SessionOutput.SessionStart = DateTime.Now; // set the session start to current time
output.SessionOutput.SessionIdentifier = "data_" + DateTime.Now; // set a custom session identifier
```

You must add dataFormatId and atlasConfigurationId to session dependencies to be able to use them during the streaming session.

Once it is done, [send the session](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L126) details to the output.
```cs
output.SessionOutput.SendSession();
```

You will need TelemetryData to write to the output. In this example we [generate some random telemetryData](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L125) for the purpose of demonstration.
```cs
var telemetryData = GenerateData(10, (DateTime)output.SessionOutput.SessionStart); // Generate some telemetry data
```

[Bind the feed to DataOutput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L128-L129) by its name. You can use the default feedname or use a custom one.
```cs
const string feedName = ""; // As sample DataFormat uses default feed, we will leave this empty.
var outputFeed = output.DataOutput.BindFeed(feedName); // bind your feed by its name to the Data Output
```

[Enqueue and send](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L131) the telemetry data.
```cs
Task.WaitAll(outputFeed.EnqueueAndSendData(telemetryData)); // enqueue and send the data to the output through the outputFeed
```

Once you sent all your data, don't forget to [set the session state to closed](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L133) 
```cs
output.SessionOutput.SessionState = StreamSessionState.Closed; // set session state to closed. In case of any unintended session close, set state to Truncated
```

and [send the session details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L134).
```cs
output.SessionOutput.SendSession(); // send session
```

### Read Telemetry Samples
First of all you need to configure the [dependencies](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L63-L71)
```cs
ProtobufCodecs.RegisterCodecs(true); // Enable Protobuff codec if the streamed data is Protobuff encoded

const string brokerList = "localhost:9092"; // The host and port where the Kafka broker is running
const string groupName = "dev"; // The group name
const string topicName = "sample_in"; // The existing topic's name in the Kafka broker. The *_annonce topic name must exist too. In this case the sample_in_announce

var dependencyServiceUri = new Uri("http://localhost:8180/api/dependencies/"); // The URI where the dependency services are running
var client = new KafkaStreamClient(brokerList); // Create a new KafkaStreamClient for connecting to Kafka broker
var dataFormatClient = new DataFormatClient(new HttpDependencyClient(dependencyServiceUri, groupName)); // Create a new DataFormatClient
```
Set ProtobufCodes if the data you want to read was written to the stream using Protobuf encoding.

The DependencyService is used to handle requests for AtlasConfigurations and DataFormats. You must provide an URI for this service. 
The DataFormatClient handles the data formats through the DependencyService for the given group name.

If you want to connect to MQTT, create an MqttStream client instead of KafkaStreamClient:
```cs
var client = new MqttStreamClient(new MqttConnectionConfig(brokerList, "userName", "password"));
```

Create a stream pipeline using the KafkaStreamClient and the topicName. Stream the messages [.Into your handler method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L73)
```cs
var pipeline = client.StreamTopic(topicName).Into(streamId => // Stream Kafka topic into the handler method
```

[Create a SessionTelemetryDataInput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L75) with the actual stream id and the dataFormatClient, and [bind the samples input to your handler method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L77-L82). 
```cs
var input = new SessionTelemetryDataInput(streamId, dataFormatClient);

input.SamplesInput.AutoBindFeeds((s, e) => // Take the input and bind feed to an event handler
{
    var data = e.Data;// The event handler here only takes the samples data 
    Trace.WriteLine(data.Parameters.First().Key); // and prints some information to the debug console
    Trace.WriteLine(data.Parameters.Count);
});
```

In this example we bind the default feed and simply print out some details.

You can optionally handle the [StreamFinished event](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L84).
```cs
input.StreamFinished += (sender, e) => Trace.WriteLine("Finished"); // Handle the steam finished event
```

In order to successfully read and consume the stream, make sure to [wait until connected](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L88-L89) and [wait for the first stream](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L91). Optionally you can tell the pipeline to wait for a specific time [while the stream is being idle](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L92), before exiting from the process.
```cs
if (!pipeline.WaitUntilConnected(TimeSpan.FromSeconds(30), CancellationToken.None)) // Wait until the connection is established
     throw new Exception("Couldn't connect");
pipeline.WaitUntilFirstStream(TimeSpan.FromMinutes(1), CancellationToken.None); // Wait until the first stream is ready to read.
pipeline.WaitUntilIdle(TimeSpan.FromMinutes(5), CancellationToken.None); // Wait for 5 minutes of the pipeline being idle before exit.
```

### Write Telemetry Samples
First of all you need to configure the [dependencies](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L99-L110)
```cs
const string brokerList = "localhost:9092"; // The host and port where the Kafka broker is running
const string groupName = "dev"; // The group name
const string topicName = "sample_in"; // The existing topic's name in the Kafka broker. The *_annonce topic name must exist too. In this case the data_in_announce
var dependencyServiceUri = new Uri("http://localhost:8180/api/dependencies/"); // The URI where the dependency services are running

var client = new KafkaStreamClient(brokerList); // Create a new KafkaStreamClient for connecting to Kafka broker
var dataFormatClient = new DataFormatClient(new HttpDependencyClient(dependencyServiceUri, groupName)); // Create a new DataFormatClient
var httpDependencyClient = new HttpDependencyClient(dependencyServiceUri, groupName); // DependencyClient stores the Data format, Atlas Configuration

var atlasConfigurationId = new AtlasConfigurationClient(httpDependencyClient).PutAndIdentifyAtlasConfiguration(AtlasConfiguration); // Uniq ID created for the AtlasConfiguration
var dataFormat = DataFormat.DefineFeed().Parameter(ParameterId).BuildFormat(); // Create a dataformat based on the parameters, using the parameter id
var dataFormatId = dataFormatClient.PutAndIdentifyDataFormat(dataFormat); // Uniq ID created for the Data Format
```

The DependencyService is used to handle requests for AtlasConfigurations and DataFormats. You must provide an URI for this service. 
The DataFormatClient handles the data formats through the DependencyService for the given group name.
DataFormat is required when writing to stream, as it is used to define the structre of the data and dataFormatId is used to retrieve dataformat from the dataFormatClient.

AtlasConfigurationId is needed only if you want to display your data in Atlas10.

If you want to connect to MQTT, create an MqttStream client instead of KafkaStreamClient:
```cs
var client = new MqttStreamClient(new MqttConnectionConfig(brokerList, "userName", "password"));
```

[Open the output topic](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L112) using the preferred client (KafkaStreamClient or MqttStreamClient) and the topicName.
```cs
using (var outputTopic = client.OpenOutputTopic(topicName)) // Open a KafkaOutputTopic
```

[Create a SessionTelemetryDataOutput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L114) and configure session output [properties](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L114-L120).
```cs
var output = new SessionTelemetryDataOutput(outputTopic, dataFormatId, dataFormatClient);
output.SessionOutput.AddSessionDependency(DependencyTypes.DataFormat, dataFormatId); // Add session dependencies to the output
output.SessionOutput.AddSessionDependency(DependencyTypes.AtlasConfiguration, atlasConfigurationId);

output.SessionOutput.SessionState = StreamSessionState.Open; // set the sessions state to open
output.SessionOutput.SessionStart = DateTime.Now; // set the session start to current time
output.SessionOutput.SessionIdentifier = "sample_" + DateTime.Now; // set a custom session identifier
```

You must add dataFormatId and atlasConfigurationId to session dependencies to be able to use them during the streaming session.

Once it is done, [send the session](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L121) details to the output.
```cs
output.SessionOutput.SendSession();
```

You will need TelemetrySamples to write to the output. In this example we [generate some random telemetrySamples](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L123) for the purpose of demonstration.
```cs
var telemetryData = GenerateSamples(10, (DateTime)output.SessionOutput.SessionStart); // Generate some telemetry samples
```

[Bind the feed to SamplesOutput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L125-L126) by its name. You can use the default feedname or use a custom one.
```cs
const string feedName = ""; // As sample DataFormat uses default feed, we will leave this empty.
var outputFeed = output.SamplesOutput.BindFeed(feedName); // bind your feed by its name to the SamplesOutput
```

[Send Samples](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L128).
```cs
Task.WaitAll(outputFeed.SendSamples(telemetrySamples)); // send the samples to the output through the outputFeed
```

Once you sent all your data, don't forget to [set the session state to closed](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L130) 
```cs
output.SessionOutput.SessionState = StreamSessionState.Closed; // set session state to closed. In case of any unintended session close, set state to Truncated
```

and [send the session details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L131).
```cs
output.SessionOutput.SendSession(); // send session details
```



## Advances samples
Advanced samples cover the usual use cases for reading, writing and reading and linking telemetry data in an structured and organized code.
According to that each sample .cs file has a Write(), Read() and ReadAndLink() methods and all of the sample files rely on the same structure. You can use them as working samples copying to your application code.
The .cs files in the [Samples folder](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/tree/update_samples/src/MAT.OCS.Streaming.Samples/Samples) have documenting and descriptive comments, but lets take a look at a simple and a more complex sample in depth.

## Writing Telemetry Data with a parameter and default feed name to a Kafka topic.

First of all you need to create or use an [AtlasConfiguration](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L27-L53). You need to set the [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L13-L22) what AppGroupId, ParameterGroupId, ParameterID you want to use.

Once you have your AtlasConfiguration design, you need to set [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L117-L120) for the DependencyService URI, the stream broker address, the group name and the output topic name where you want to write. 
The DependencyService is used to handle requests for AtlasConfigurations and DataFormats, you must provide an URI for this service. 

A [KafkaStreamAdapter](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L121) is used to manage Kafka streams.

Using the KafkaStreamAdapter you must [open and output topic](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L122) in Kafka.

You need to persist your configuration, setup your DataFormat and setup a streaming session to be able to write to your broker. The [Writer.cs](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/update_samples/src/MAT.OCS.Streaming.Samples/Samples/Writer.cs) code sample covers all of these, you only need to [use a Writer object](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L124) with the required parameters.

In this example we are usign the [default feed name](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L126), which is the empty string.
You must take care about [opening](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L127) and [closing](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L134) the session, otherwise it would mark the session as [truncated](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/Writer.cs#L52), just as if any error would occur during session usage.

In the samples we are working with random [generated data](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L79-L113), while in real scenarios they would come as real data from external systems.

To write to the output topic you only need to [invoke the Write method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L132) on your writer object, passing in the feed name and the telemetry data. The Writer object already "knows" your AtlasConfiguration, the DataFormat and the output topic name.


## Reading Telemetry Data for a parameter from a Kafka stream.

First of all you need to create or use an [AtlasConfiguration](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L27-L53). You need to set the [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L13-L22) what AppGroupId, ParameterGroupId, ParameterID you want to use.

Once you have your AtlasConfiguration design, you need to set [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L117-L120) for the DependencyService URI, the stream broker address, the group name and the output topic name where you want to write. 
The DependencyService is used to handle requests for AtlasConfigurations and DataFormats, you must provide an URI for this service. 

A [KafkaStreamAdapter](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L145) is used to manage Kafka streams.

Using the KafkaStreamAdapter you must [open and output stream](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L145) in Kafka.

You need connect to the DependencyClient and the DataFormatClient and perist the StreamPipelineBuilder that was created by the KafkaStreamAdapter. Using a [Reader object](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L147) takes care about it.

You can start reading a stream with the [Read method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L174) on your reader object. This takes 2 arguments, the first one is more trivial, it is the parameter id. The second must be a user specified method, aligning to the [TelemetryDataHandler](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/Models.cs#L12) delegate. With the help of this you can handle the streamed Telemetry Data as you would like to. In another example you will see how can you link it directly to another output topic.

Lastly, in our sample code we [invoke the Write()](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L151) method while the streaming session is live, to have some input to see that the streaming is working. Our sample delegates, called as Models are in the [Models.cs](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/update_samples/src/MAT.OCS.Streaming.Samples/Samples/Models.cs) and in this example we use the [TraceData method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/Models.cs#L14-L27) to trace the streamed telemetry data deatils.
