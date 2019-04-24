# ATLAS Advanced Streaming Samples

![Build Status](https://mat-ocs.visualstudio.com/Telemetry%20Analytics%20Platform/_apis/build/status/MAT.OCS.Streaming/Streaming%20Samples?branchName=develop)

Table of Contents
=================
<!--ts-->
* [Introduction - MAT.OCS.Streaming library](/readme.md)
* [C# Read/Write Samples](../CSharp/readme.md)
* [C# Model sample](../CSharp/models.md)
* [Python Read/Write Samples](/readme.md)
* [Python Model sample](/models.md)

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

# Python Samples
## Basic samples
Basic samples demonstrate the simple usage of Advanced Streams, covering all the bare-minimum steps to implement Telematry Data and Telemetry Samples read and write to and from Kafka or Mqtt streams.

## Read
First of all you need to configure the [dependencies](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L60-L66)
```python
DEPENDENCY_SERVER_URI = 'http://localhost:8180/api/dependencies'
DEPENDENCY_GROUP = 'dev'
KAFKA_IP = 'localhost:9092'
TOPIC_NAME = 'test_topic'
dependency_client = HttpDependencyClient(DEPENDENCY_SERVER_URI, DEPENDENCY_GROUP)
data_format_client = DataFormatClient(dependency_client)
kafka_client = KafkaStreamClient(kafka_address=KAFKA_IP, consumer_group=DEPENDENCY_GROUP)
```

The dependency_client is used to handle requests for AtlasConfigurations and DataFormats. You must provide an URI for this service. 
The data_format_client handles the data formats through the dependency_client for the given group name.

Create a stream pipeline using the kafka_client and the TOPIC_NAME. Stream the messages [.Into your handler method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L68)
```python
pipeline: StreamPipeline = kafka_client.stream_topic(TOPIC_NAME).into(stream_input_handler)
```

Within your stream_input_handler method
```python
def stream_input_handler(stream_id: str) -> StreamInput:
    print("Streaming session: " + stream_id)
```
[Create a SessionTelemetryDataInput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L70) with the actual stream id and the dataFormatClient 
```python
telemetry_input = SessionTelemetryDataInput(stream_id=stream_id, data_format_client=data_format_client)
```

### Read Telemetry Data
In this example we [bind the **data_input** to the handler method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L71) using the default feed and simply [print out some details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L72-L86) about the incoming data.

```python
def print_data(sender, event_args: TelemetryDataFeedEventArgs):
    tdata: TelemetryData = event_args.buffer.get()
    print('data for ' + str(event_args.message_origin.stream_id) + ' with length ' + str(len(tdata.time)))

telemetry_input.data_input.bind_default_feed("").data_buffered += print_data

```

### Read Telemetry Samples
In this example we [bind the **samples_input** to the handler method](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L77) and simply [print out some details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L78-L82) 
```python
def print_samples(sender, event_args: TelemetryEventArgs):
    d: TelemetrySamples = event_args.data
    print('tsamples for ' + str(event_args.message_origin.stream_id) + ' with  ' + str(len(d.parameters.keys())) + ' parameters received')

telemetry_input.samples_input.autobind_feeds += print_samples
```

You can optionally handle the [stream_finished event](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L88).
```python
telemetry_input.stream_finished += lambda x, y: print('Stream finished')
```


## Write
First of all you need to configure the [dependencies](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/update_samples/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L102-L113)
```python
"""Setup details"""
# Populate these constants with the correct values for your project.
DEPENDENCY_SERVER_URI = 'http://localhost:8180/api/dependencies'
DEPENDENCY_GROUP = 'dev'
KAFKA_IP = 'localhost:9092'
TOPIC_NAME = 'test_topic'

frequency = 100
"""Create a dependency client"""
dependency_client = HttpDependencyClient(DEPENDENCY_SERVER_URI, DEPENDENCY_GROUP)

"""Create Atlas configurations"""
atlas_configuration_client = AtlasConfigurationClient(dependency_client)
atlas_configuration = AtlasConfiguration({"Chassis":
    ApplicationGroup(groups={"State":
        ParameterGroup(parameters={"vCar:Chassis":
            AtlasParameter(name="vCar")})})})

atlas_configuration_id = atlas_configuration_client.put_and_identify_atlas_configuration(atlas_configuration)

"""Create Dataformat"""
parameter: DataFeedParameter = DataFeedParameter(identifier="vCar:Chassis", aggregates_enum=[Aggregates.avg])
parameters: List[DataFeedParameter] = [parameter]
feed = DataFeedDescriptor(frequency=frequency, parameters=parameters)
feed_name = ""
data_format = DataFormat({feed_name: feed})

data_format_client = DataFormatClient(dependency_client)
data_format_id = data_format_client.put_and_identify_data_format(data_format)

"""Create a Kafka client"""
client = KafkaStreamClient(kafka_address=KAFKA_IP, consumer_group=DEPENDENCY_GROUP)
```

The dependency_client is used to handle requests for AtlasConfigurations and DataFormats. You must provide an URI for this service. 
The data_format_client handles the data formats through the dependency_client for the given group name.
DataFormat is required when writing to stream, as it is used to define the structre of the data and data_format_id is used to retrieve dataformat from the dataFormatClient.

AtlasConfigurationId is needed only if you want to display your data in Atlas10.

[Open the output topic](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L115) using the preferred client (KafkaStreamClient or MqttStreamClient) and the topicName.
```python
with client.open_output_topic(TOPIC_NAME) as output_topic:
	...
```

[Create a SessionTelemetryDataOutput](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L117) and configure session output [properties](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L118-L123).
```python
try:
    output = SessionTelemetryDataOutput(output_topic=output_topic, data_format_id=data_format_id, data_format_client=data_format_client)

    output.session_output.add_session_dependency(DependencyTypes.atlas_configuration, atlas_configuration_id)
    output.session_output.add_session_dependency(DependencyTypes.data_format, data_format_id)

    output.session_output.session_state = StreamSessionState.Open
    output.session_output.session_start = datetime.utcnow()
    output.session_output.session_identifier = "test_" + str(datetime.utcnow())
    output.session_output.session_details = {"test_session": "tamas"}
    output.session_output.send_session()

	....


except Exception as e:
    print(e)
    if output is not None:
        output.session_output.session_state = StreamSessionState.Truncated
finally:
    if output is not None:
        output.session_output.send_session()
```

Open the session within a Try Except block and handle sesseion status sending as shown above.
You must add data_format_id and atlas_configuration_id to session dependencies to be able to use them during the streaming session.


### Write Telemetry Data

[Bind the feed to **output.data_output**](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L128-L129) by its name. You can use the default feedname or use a custom one.
```python
output_feed: TelemetryDataFeedOutput = output.data_output.bind_default_feed()
```

You will need **TelemetryData** to write to the output. In this example we [generate some random TelemetryData](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/33d410b4555bf3fa7d783db18dc444e1728df6b5/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L125) for the purpose of demonstration.
```python
data: TelemetryData = output_feed.make_telemetry_data(samples=10, epoch=to_telemetry_time(datetime.utcnow()))
data = generate_data(data, frequency)
```

[send](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L131) the telemetry data.
```python
output_feed.send(data)
```

### Write Telemetry Samples
You will need **TelemetrySamples** to write to the output. In this example we [generate some random telemetrySamples](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L123) for the purpose of demonstration.
```python
telemetry_samples = generate_samples(sample_count=10, session_start=datetime.utcnow(), parameter_id="vCar:Chassis", frequency=frequency)
```

[Bind the feed to **output.samples_output**](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L125-L126) by its name. You can use the default feedname or use a custom one.
```python
output_feed: TelemetrySamplesFeedOutput = output.samples_output.bind_feed(feed_name="")
``

[Send Samples](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/f9f66fa96aaa51a4ec24bf921461918b3771d929/src/MAT.OCS.Streaming.Samples/Samples/Basic/TSamples.cs#L128).
```python
output_feed.send(telemetry_samples)
```


Once you sent all your data, don't forget to [set the session state to closed](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L133) 
```python
output.session_output.session_state = StreamSessionState.Closed
```

and [send the session details](https://github.com/McLarenAppliedTechnologies/mat.ocs.streaming.samples/blob/5b7fcb3e763a36f753f3f320ad2484867fcd66e0/src/MAT.OCS.Streaming.Samples/Samples/Basic/TData.cs#L134) or do it in the finally block as recommended above.
```python
output.SessionOutput.SendSession(); // send session
```
