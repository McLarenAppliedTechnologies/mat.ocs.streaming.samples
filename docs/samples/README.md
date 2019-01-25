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
Samples cover the usual use cases for reading, writing and reading and linking telemetry data.
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
