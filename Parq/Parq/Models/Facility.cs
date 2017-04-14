#region Copyright
/*Copyright (c) 2016 Javus Software (Pty) Ltd

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
#endregion
using Newtonsoft.Json;
using Parq.BusinessLayer.Contracts;

namespace Parq.Models
{
    public class Facility : BusinessEntityBase
    {
        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("geofenceId")]
        public string GeofenceId { get; set; }

        [JsonProperty("facility_name")]
        public string FacilityName { get; set; }

        [JsonProperty("client_facility_code")]
        public string ClientFacilityCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("is_active")]
        public bool isActive { get; set; }

        [JsonProperty("is_public")]
        public bool isPublic { get; set; }

        [JsonProperty("ask")]
        public bool Ask { get; set; }

        [JsonProperty("logo_url")]
        public string LogoUrl { get; set; }

        [JsonProperty("cover_url")]
        public string CoverUrl { get; set; }        

        [JsonProperty("client")]
        public Client Client { get; set; }

    }
}
