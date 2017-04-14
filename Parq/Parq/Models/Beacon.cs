using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parq.BusinessLayer.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Parq.Models
{
    public class Beacon : BusinessEntityBase
    {
        [JsonProperty("gateId")]
        public string GateId { get; set; }

        [JsonProperty("namespace_id")]
        public string NamespaceId { get; set; }

        [JsonProperty("instance_id")]
        public string InstanceId { get; set; }

        [JsonProperty("ask")]
        public bool Ask { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }
        
    }
}
