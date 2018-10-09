using MqttLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTManagerLib
{
    public class MQTTMessage {

        public string ClientId;
        public string Message;
        public string Topic;
        public static MQTTMessage Parse(string rawMessage, string topic)
        {
            var m = Newtonsoft.Json.JsonConvert.DeserializeObject<MQTTMessage>(rawMessage);
            m.Topic = topic;
            return m;
        }
        public string ToJSON()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this, 
                new JsonSerializerSettings { 
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver() 
                }
            );
            return json;
        }
    }

}