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
using System;
using System.Text;
using Newtonsoft.Json;
using Parq.BusinessLayer.Contracts;
using SQLite;

namespace Parq.Models
{
    public class ActiveTicket : BusinessEntityBase
    {
        
        public ActiveTicket()
        {            
        }

        [JsonProperty("facilityId")]
        public string FacilityId { get; set; }        

        [JsonProperty("userId")]
        public string userId { get; set; }

        [JsonProperty("zoneId")]
        public string zoneId { get; set; }

        [JsonProperty("clientId")]
        public string clientId { get; set; }

        [JsonProperty("gateId")]
        public string GateId { get; set; }

        [JsonProperty("scan_type")]
        public string ScanType { get; set; }

        [JsonProperty("scan_id")]
        public string ScanId { get; set; }

        [JsonProperty("facility_name")]
        public string FacilityName { get; set; }        

        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }        

        [JsonProperty("vehicle_lat")]
        public double VehicleLat { get; set; }

        [JsonProperty("vehicle_lon")]
        public double VehicleLon { get; set; }

        [JsonProperty("couponId")]
        public string CouponId { get; set; }

        [JsonProperty("elapsed_time")]
        public long ElapsedTime { get; set; }

        [JsonProperty("client")]
        public Client Client { get; set; }

        [JsonProperty("facility")]
        public Facility Facility { get; set; }

        [JsonProperty("zone")]
        public Zone Zone { get; set; }
        
        [JsonIgnore]
        public long getElapsedTime
        {
            get {                
                DateTime dateStart, dateCurrent;
                dateStart = createdAtTime;
                dateCurrent = createdAtTime.AddMilliseconds(ElapsedTime);
                dateStart.Millisecond.ToString("yyyy-MM-dd HH:mm:ss");
                dateCurrent.Millisecond.ToString("yyyy-MM-dd HH:mm:ss");
                
                return dateCurrent.Millisecond - dateStart.Millisecond;
            }
        }        
        
        [JsonIgnore]
        public string TotalDisplay
        {
            get { return "R" + "0.00"; }
        }

        [JsonIgnore]
        public string InsertDateLongDisplay
        {
            get { return createdAtTime.ToLocalTime().ToString("D"); }            
        }

        [JsonIgnore]
        public string InsertDateShortDisplay
        {
            get { return createdAtTime.ToLocalTime().ToString("d"); }
        }

                
        [JsonIgnore]
        public DateTime LastRefreshTime { get; set; } 
        
        [JsonIgnore,Ignore]
        public long Offset
        {
            get { return (long)DateTime.Now.Subtract(LastRefreshTime).TotalMilliseconds; }            
        }

        [JsonIgnore, Ignore]
        public long ElapsedPrice
        {
            get { return CalculateElapsedPrice(); }
        }

        [JsonIgnore, Ignore]
        public String formatedElapsedPrice
        {
            get {
                StringBuilder sb = new StringBuilder();
                sb.Append("R ").Append(ElapsedPrice).Append(".00");
                return sb.ToString();
                }
        }

       
        public void SyncProperties(ActiveTicket ticket)
        {
            this.FacilityName = ticket.FacilityName;            
            this.userId = ticket.userId;
        }

        public String formatElapsedTime
        {
            get{
                long now = ElapsedTime + Offset;
                long days = 0, hours = 0, minutes = 0, seconds = 0, tenths = 0;
                StringBuilder sb = new StringBuilder();

                if (now < 1000)
                {
                    tenths = now / 100;
                }
                else if (now < 60000)
                {
                    seconds = now / 1000;
                    now -= seconds * 1000;
                    tenths = (now / 100);
                }
                else if (now < 3600000)
                {
                    hours = now / 3600000;
                    now -= hours * 3600000;
                    minutes = now / 60000;
                    now -= minutes * 60000;
                    seconds = now / 1000;
                    now -= seconds * 1000;
                    tenths = (now / 100);
                }
                else if (now > 3600000)
                {
                    days = now / 86400000;
                    now -= days * 86400000;
                    hours = now / 3600000;
                    now -= hours * 3600000;
                    minutes = now / 60000;
                    now -= minutes * 60000;
                    seconds = now / 1000;
                    now -= seconds * 1000;
                    tenths = (now / 100);
                }


                if (days > 1)
                {

                    sb.Append(days).Append(" Days\n").Append(formatDigits(hours)).Append(":")
                            .Append(formatDigits(minutes)).Append(":")
                            .Append(formatDigits(seconds));
                            
                }
                else if (days > 0)
                {
                    sb.Append(days).Append(" Day\n").Append(formatDigits(hours)).Append(":")
                            .Append(formatDigits(minutes)).Append(":")
                            .Append(formatDigits(seconds));
                            
                }
                else if (hours > 0)
                {
                    sb.Append(formatDigits(hours)).Append(":")
                            .Append(formatDigits(minutes)).Append(":")
                            .Append(formatDigits(seconds));
                            
                }
                else
                {
                    sb.Append(formatDigits(minutes)).Append(":")
                            .Append(formatDigits(seconds));
                }

                return sb.ToString();
            }
        }

        public String formatElapsedTimeSingleLine
        {
            get
            {
                return formatElapsedTime.Replace("\n", " ");
            }
        }

        private String formatDigits(long num)
        {
            return (num < 10) ? "0" + num : num.ToString();
        }

        private long CalculateElapsedPrice()
        {
            long hours, price, hoursUnitPrice = 10, minuteUnitPrice = 5;

            hours = (ElapsedTime + Offset) / 3600000;

            if (hours > 0)
            {
                price = hours * hoursUnitPrice;
            }else
            {
                price = minuteUnitPrice;
            }

            return price;
        }
    }
}
