using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Parq.Models
{
    public class OtherModels
    {
    }

    /// <summary>
    /// Model to upload user meta data to Auth0
    /// </summary>
    public class UserMetaData
    {
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }
    }

    /// <summary>
    /// Custom Pricing Model for zone. 
    /// Calculates price based on elapsed time interval.
    /// </summary>
    public class CustomPricing
    {
        [JsonProperty("from")]
        public long GivenName { get; set; }

        [JsonProperty("to")]
        public long FamilyName { get; set; }

        [JsonProperty("friendly_name")]
        public string FriendlyName { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }
    }

    /// <summary>
    /// Interval Pricing Model for zone. 
    /// If user needs to pay based on linear increment. 
    /// Or when the elapsed time is outside of range based on custom pricing array
    /// </summary>
    public class IntervalPricing
    {
        [JsonProperty("grace_period")]
        public long GracePeriod { get; set; }

        [JsonProperty("interval")]
        public long GivenName { get; set; }

        [JsonProperty("price")]
        public long FamilyName { get; set; }
    }
}
