using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Parq.BusinessLayer.Contracts;

namespace Parq.Models
{
    public class Client : BusinessEntityBase
    {
        [JsonProperty("client_name")]
        public string ClientName { get; set; }

        [JsonProperty("is_active")]
        public bool isActive { get; set; }

        [JsonProperty("is_public")]
        public bool isPublic { get; set; }
    }
}
