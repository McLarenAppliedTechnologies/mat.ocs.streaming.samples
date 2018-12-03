using MAT.OCS.Streaming.Kafka;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAT.OCS.Streaming.Samples.Adapters
{
    public class KafkaStreamAdapter : StreamAdapter
    {
        private KafkaStreamClient client;

        public KafkaStreamAdapter(string brokerList, string consumerGroup)
        {
            client = new KafkaStreamClient(brokerList);
            client.ConsumerGroup = consumerGroup;
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
