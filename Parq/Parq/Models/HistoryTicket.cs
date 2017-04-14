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
using Parq.BusinessLayer.Contracts;
using Newtonsoft.Json;
using SQLite;

namespace Parq.Models
{
    public class HistoryTicket : BusinessEntityBase
    {
        public HistoryTicket()
        {
        }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("ticketId")]
        public string TicketId { get; set; }

        [JsonProperty("facilityId")]
        public string FacilityId { get; set; }

        [JsonProperty("exitGateId")]
        public string ExitGateId { get; set; }

        [JsonProperty("entryGateId")]
        public string EntryGateId { get; set; }

        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        [JsonProperty("couponId")]
        public string CouponId { get; set; }

        [JsonProperty("cardId")]
        public string CardId { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("discount")]
        public double Discount { get; set; }

        [JsonProperty("paymentType")]
        public string PaymentType { get; set; }

        [JsonProperty("curency")]
        public string Curency { get; set; }

        [JsonProperty("vehicleLat")]
        public double VehicleLat { get; set; }

        [JsonProperty("vehicleLon")]
        public double VehicleLon { get; set; }

        [JsonProperty("vehicleRegistration")]
        public string VehicleRegistration { get; set; }

        [JsonProperty("facilityName")]
        public string FacilityName { get; set; }

        [JsonProperty("entryTime")]
        public DateTime EntryTime { get; set; }

        [JsonIgnore,Ignore]
        public string FacilityCoverImage
        {
            get { return "http://www.parq.co.za/appimages/facility/" + FacilityName.Replace(" ", "") + ".jpg"; }
        }

        [JsonIgnore]
        public string EntryDateLongDisplay
        {
            get { return EntryTime.ToLocalTime().ToString("D"); }
        }

        [JsonIgnore]
        public string EntryDateShortDisplay
        {
            get { return EntryTime.ToLocalTime().ToString("d"); }
        }

        [JsonIgnore]
        public string ExitDateLongDisplay
        {
            get { return createdAtTime.ToLocalTime().ToString("D"); }
        }

        [JsonIgnore]
        public string ExitDateShortDisplay
        {
            get { return createdAtTime.ToLocalTime().ToString("d"); }
        }

        [JsonIgnore]
        public DateTime LastRefreshTime { get; set; }

        [JsonIgnore, Ignore]
        public string formatedElapsedPrice
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("R ").Append(string.Format("{0:N2}",Price));
                return sb.ToString();
            }
        }

        public string formatElapsedTime
        {
            get
            {
                long now = Duration;
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

        public string formatElapsedTimeSingleLine
        {
            get
            {
                return formatElapsedTime.Replace("\n", " ");
            }
        }

        private string formatDigits(long num)
        {
            return (num < 10) ? "0" + num : num.ToString();
        }
    }
}
