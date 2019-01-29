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

# C# Samples
## Basic samples
Basic samples demonstrate the simple usage of Advanced Streams, covering all the bare minimal and necessarry steps to implement Telematry Data and Telemetry Sample read and write to and from Kafka streams.

### Read Telemetry Data
First of all you need to create or use an [AtlasConfiguration](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/446ff3b07aa2c8e1a2df8138e74c537666803948/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L32-L57)
You need to set specify [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/446ff3b07aa2c8e1a2df8138e74c537666803948/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L17-L26) what AppGroupId, ParameterGroupId, ParameterID you would use.

Once you have your AtlasConfiguration design, you need to set [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/446ff3b07aa2c8e1a2df8138e74c537666803948/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L61-L66) for the DependencyService URI, the stream broker address, the group name and the topic name that you want to read. 
The DependencyService is used to handle requests for AtlasConfigurations and DataFormats, you must provide an URI for this service. 
The DataFormatClient handles the data formats through the DependencyService for the given group name.

Create a stream pipeline using the KafkaStreamClient and the topicName and stream the messages [.Into your handler method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/61cedae614653f4e1526d61b09518312edc47401/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L68-L69).

[Create a SessionTelemetryDataInput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/61cedae614653f4e1526d61b09518312edc47401/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L70) with the actual stream id and the dataFormatClient, and [bind the data input to your handler method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/61cedae614653f4e1526d61b09518312edc47401/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L71-L73). In this example we bind the default feed and simply [print out some details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/61cedae614653f4e1526d61b09518312edc47401/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L75-L85) about the incoming data.

You can optionally handle the [StreamFinished event](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/61cedae614653f4e1526d61b09518312edc47401/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L88).

A few important things to successfully read and consume the stream is to make sure to [wait until connected](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/61cedae614653f4e1526d61b09518312edc47401/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L92-L93) and [wait for the first stream](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L94). Optionally you can tell the pipeline to wait for a specific time [while the stream is being idle](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L95), before exiting from the process.

### Write Telemetry Data
First of all you need to create or use an [AtlasConfiguration](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/446ff3b07aa2c8e1a2df8138e74c537666803948/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L32-L57)
You need to set specify [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/446ff3b07aa2c8e1a2df8138e74c537666803948/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L17-L26) what AppGroupId, ParameterGroupId, ParameterID you would use.

Once you have your AtlasConfiguration design, you need to set [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/446ff3b07aa2c8e1a2df8138e74c537666803948/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L102-L108) for the DependencyService URI, the stream broker address, the group name and the topic name that you want to read. 
The DependencyService is used to handle requests for AtlasConfigurations and DataFormats, you must provide an URI for this service. 
The DataFormatClient handles the data formats through the DependencyService for the given group name.

[Open the output topic](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L110) using the KafkaStreamClient and the topicName.

[Retrieve the atlas configuration id](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L112) for your AtlasConfiguration. 
[Identify the dataFormatId](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L113-L114) for your dataformat, using your parameterIds.

[Create a SessionTelemetryDataOutput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L116) and configure session output [properties](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L117-L122). Once it is done, [send the session](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L123) details to the output.

You will need TelemetryData to write to the output. In this example we [generate some random telemetryData](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L125) for the purpose of demonstration.

[Bind your feed to DataOutput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L127-L128) by its name to the output. You can use the default feedname or use a custom one.

[Enqueue and send Data](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L130) your telemetry data.

Once you sent all your data, don't forget to [set the session state to closed](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L132) and [send the session details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L133).


### Read Telemetry Sample
First of all you need to create or use an [AtlasConfiguration](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L34-L59)
You need to set specify [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L19-L28) what AppGroupId, ParameterGroupId, ParameterID you would use.

Once you have your AtlasConfiguration design, you need to set [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L65-L70) for the DependencyService URI, the stream broker address, the group name and the topic name that you want to read. 
The DependencyService is used to handle requests for AtlasConfigurations and DataFormats, you must provide an URI for this service. 
The DataFormatClient handles the data formats through the DependencyService for the given group name.

[Set ProtobufCodes](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L63) if the data you want to read was written to the stream using Protobuf encoding.

Create a stream pipeline using the KafkaStreamClient and the topicName and stream the messages [.Into your handler method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L72).

[Create a SessionTelemetryDataInput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L74) with the actual stream id and the dataFormatClient, and [bind the data input to your handler method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L76). In this example we simply [print out some details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L78-L80) about the incoming data.

You can optionally handle the [StreamFinished event](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/c1815bc1f5ff6fb3d431a6c8476178bb1be80dcd/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L83).

A few important things to successfully read and consume the stream is to make sure to [wait until connected](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L86-L87) and [wait for the first stream](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L89). Optionally you can tell the pipeline to wait for a specific time [while the stream is being idle](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L90), before exiting from the process.

### Write Telemetry Data
First of all you need to create or use an [AtlasConfiguration](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L34-L59)
You need to set specify [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L19-L28) what AppGroupId, ParameterGroupId, ParameterID you would use.

Once you have your AtlasConfiguration design, you need to set [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L65-L70) for the DependencyService URI, the stream broker address, the group name and the topic name that you want to read. 
The DependencyService is used to handle requests for AtlasConfigurations and DataFormats, you must provide an URI for this service. 
The DataFormatClient handles the data formats through the DependencyService for the given group name.

[Open the output topic](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L106) using the KafkaStreamClient and the topicName.
[Retrieve the atlas configuration id](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L108) for your AtlasConfiguration. 
[Identify the dataFormatId](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L109-L110) for your dataformat, using your parameterIds.

[Create a SessionTelemetryDataOutput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L112) and configure session output [properties](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L113-L118). Once it is done, [send the session](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L119) details to the output.

You will need TelemetrySamples to write to the output. In this example we [generate some random telemetrySamples](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L121) for the purpose of demonstration.

[Bind you feed to SamplesOutput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L123-L124) by its name to the output. You can use the default feedname or use a custom one.

[Send Samples](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L126) your telemetry data.

Once you sent all your data, don't forget to [set the session state to closed](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L128) and [send the session details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/bd8ba36cd397c0d9d371391829a7224611a051ab/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSample.cs#L129).

## Advances samples
Advanced samples cover the usual use cases for reading, writing and reading and linking telemetry data in an structured and organized code.
According to that each sample .cs file has a Write(), Read() and ReadAndLink() methods and all of the sample files rely on the same structure. You can use them as working samples copying to your application code.
The .cs files in the [Samples folder](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/tree/update_samples/src/MAT.OCS.Streaming.Samples/Samples) have documenting and descriptive comments, but lets take a look at a simple and a more complex sample in depth.

## Writing Telemetry Data with a parameter and default feed name to a Kafka topic.

First of all you need to create or use an [AtlasConfiguration](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L27-L53). You need to set specify [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L13-L22) what AppGroupId, ParameterGroupId, ParameterID you would use.

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

First of all you need to create or use an [AtlasConfiguration](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L27-L53). You need to set specify [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L13-L22) what AppGroupId, ParameterGroupId, ParameterID you would use.

Once you have your AtlasConfiguration design, you need to set [details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L117-L120) for the DependencyService URI, the stream broker address, the group name and the output topic name where you want to write. 
The DependencyService is used to handle requests for AtlasConfigurations and DataFormats, you must provide an URI for this service. 

A [KafkaStreamAdapter](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L145) is used to manage Kafka streams.

Using the KafkaStreamAdapter you must [open and output stream](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L145) in Kafka.

You need connect to the DependencyClient and the DataFormatClient and perist the StreamPipelineBuilder that was created by the KafkaStreamAdapter. Using a [Reader object](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L147) takes care about it.

You can start reading a stream with the [Read method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L174) on your reader object. This takes 2 arguments, the first one is more trivial, it is the parameter id. The second must be a user specified method, aligning to the [TelemetryDataHandler](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/Models.cs#L12) delegate. With the help of this you can handle the streamed Telemetry Data as you would like to. In another example you will see how can you link it directly to another output topic.

Lastly, in our sample code we [invoke the Write()](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/TDataSingleFeedSingleParameter.cs#L151) method while the streaming session is live, to have some input to see that the streaming is working. Our sample delegates, called as Models are in the [Models.cs](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/update_samples/src/MAT.OCS.Streaming.Samples/Samples/Models.cs) and in this example we use the [TraceData method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5d85309e4f0ac20fcc836e273b031075ec3aaa35/src/MAT.OCS.Streaming.Samples/Samples/Models.cs#L14-L27) to trace the streamed telemetry data deatils.
