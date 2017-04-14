using Parq.BusinessLayer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Parq.Models
{
    public class Zone : BusinessEntityBase
    {
        [JsonProperty("facilityId")]
        public string FacilityId { get; set; }

        [JsonProperty("zone_name")]
        public string ZoneName { get; set; }

        [JsonProperty("has_parent")]
        public bool HasParent { get; set; }

        [JsonProperty("parent")]
        public Zone Parent { get; set; }


        /// <summary>
        /// The facility coupled with this Zone
        /// </summary>
        [JsonProperty("facility")]
        public Facility Facility { get; set; }
        
        [JsonProperty("parent_zone_id")]
        public string ParentZoneId { get; set; }

        [JsonProperty("parent_transit_time")]
        public long ParentTransitTime { get; set; }

        [JsonProperty("is_active")]
        public bool isActive { get; set; }

        [JsonProperty("is_public")]
        public bool isPublic { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }


        /// <summary>
        /// Pricing model either "custom" or "interval"
        /// </summary>
        [JsonProperty("pricing_model")]
        public string PricingModel { get; set; }


        /// <summary>
        /// Custom Pricing Array, indicating the non-linear pricing structure.
        /// If Elapsed time exceeds this range, then the pricing will continue with the linear pricing model (IntervalPricing)
        /// </summary>
        [JsonProperty("custom_pricing")]
        public CustomPricing[] CustomPricing { get; set; }


        /// <summary>
        /// Interval Pricing based on linear pricing
        /// </summary>
        [JsonProperty("interval_pricing")]
        public IntervalPricing IntervalPricing { get; set; }
    }
}
