using System;
using System.Collections.Generic;
using System.Text;
using MAT.OCS.Streaming.Kafka;
using MAT.OCS.Streaming.Mqtt;

namespace MAT.OCS.Streaming.Samples.Adapters
{
    class MqttStreamAdapter : StreamAdapter
    {
        private MqttStreamClient client;

        public MqttStreamAdapter(MqttConnectionConfig config)
        {
            client = new MqttStreamClient(config);
        }

        public override IOutputTopic OpenOutputTopic(string topicName)
        {
            return client.OpenOutputTopic(topicName);
        }

        public override IStreamPipelineBuilder OpenStreamTopic(string topicName)
        {
            return client.StreamTopic(topicName);
        }
    }
}
