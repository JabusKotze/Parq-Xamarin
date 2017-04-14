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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Refractored.Xam.Settings;
using Parq.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Parq.Models;
using Parq.ViewModels;
using Parq.Data;
using Parq.Services;

namespace Parq.DataLayer
{
    public class Checkout
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("action")]
        public int Action { get; set; }
    }

    public class Checkin
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("action")]
        public int Action { get; set; }
    }
    public class APIActiveTickets
    {
        HttpClient client;             
        private DataBase adb;

        /// <summary>
        /// Initialize Http Client
        /// </summary>
        /// <returns></returns>
        private HttpClient GetClient()
        {
            client = Client.GetClient();
            adb = DataBase.adb;
            return client;
        }

        
       /// <summary>
       /// 
       /// </summary>
       /// <param name="QRcode"></param>
       /// <returns></returns>
        public async Task<int> GateScanned(string QRcode)
        {
            int action = 0;
            string parameters = "";
            string barcode = "";
            string message;            
            var uri = new Uri(Strings.activeTicket);

            if(await ConnectivityService.IsConnected() == false)
            {
                Settings.ErrorMessage = Strings.noInternet;
                return action;
            }

            if (!GeneralChecks.isBarcodeValid(QRcode))
            {
                Settings.ErrorMessage = Strings.notValidBarcode;
                return action;                
            }

            barcode = GeneralChecks.stripBarcode(QRcode);

            if (string.IsNullOrWhiteSpace(barcode))
            {
                Settings.ErrorMessage = Strings.notValidBarcode;
                return action;
            }

            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("type", "gateId");
            data.Add("id", barcode);

            parameters = GeneralChecks.BuildURLParametersString(data);

            client = GetClient();

            try
            {

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");                
                HttpResponseMessage response = null;

                response = await client.GetAsync(uri + parameters);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);
                    action = Int32.Parse(jsonObject["action"].ToString());                    
                }
                else
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);
                    message = jsonObject["message"].ToString();
                    Settings.ErrorMessage = message;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return action;
        }


        /// <summary>
        /// Checkout after verifying tickets, gate and qrcode (action = 101)
        /// </summary>
        /// <param name="QRcode"></param>
        /// <returns>True if successfull checkin</returns>
        public async Task<bool> GateCheckOut(string QRcode)
        {

            bool result = false;            
            string barcode = "";
            string message;
            var uri = new Uri(Strings.activeTicket);
            barcode = GeneralChecks.stripBarcode(QRcode);

            var data = new Checkout
            {
                Id = barcode,
                Action = 101
            };

            client = GetClient();

            try
            {
                var json = JsonConvert.SerializeObject(data);

                var content = new StringContent(json, Encoding.UTF8, "application/json");                
                HttpResponseMessage response = null;

                response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);
                    
                    var tickets = jsonObject["activeTicket"].ToString();
                    var htickets = jsonObject["historyTicket"].ToString();
                    var activeTicket = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<ActiveTicket>>(tickets).ToList()).ConfigureAwait(continueOnCapturedContext:false);
                    var historyTicket = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<HistoryTicket>>(tickets).ToList()).ConfigureAwait(continueOnCapturedContext: false);

                    activeTicket[0].LastRefreshTime = DateTime.Now;
                    message = jsonObject["message"].ToString() + "\n" + "Price: R " + jsonObject["price"].ToString() + "\nDuration: " + activeTicket[0].formatElapsedTimeSingleLine;
                    Settings.SuccessMessage = message;

                    ServiceContainer.Resolve<ActiveTicketsViewModel>().CheckedOut = true;
                    ServiceContainer.Resolve<ActiveTicketsViewModel>().Scanned = true;
                    ServiceContainer.Resolve<ActiveTicketsViewModel>().CheckedOutTicket = activeTicket[0];

                    await ServiceContainer.Resolve<ActiveTicketsViewModel>().ExecuteDeleteActiveTicketCommand(activeTicket[0]);
                    await ServiceContainer.Resolve<HistoryTicketsViewModel>().ExecuteInsertHistoryTicketCommand(historyTicket[0]);

                    result = true;
                }
                else
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);
                    message = jsonObject["message"].ToString();
                    Settings.ErrorMessage = message;
                }
            }
            catch (Exception ex)
            {
                Settings.ErrorMessage = Strings.Oops;
            }

            return result;
        }



        /// <summary>
        /// Checkout after verifying tickets, gate and qrcode (action = 101)
        /// </summary>
        /// <param name="QRcode"></param>
        /// <returns>True if successfull checkin</returns>
        public async Task<bool> GateCheckIn(string QRcode)
        {

            bool result = false;
            string barcode = "";
            string message;
            var uri = new Uri(Strings.activeTicket);
            barcode = GeneralChecks.stripBarcode(QRcode);

            var data = new Checkin
            {
                Id = barcode,
                Action = 102
            };

            client = GetClient();

            try
            {
                var json = JsonConvert.SerializeObject(data);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = null;

                response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);
                    message = jsonObject["message"].ToString();
                    var tickets = jsonObject["activeTicket"].ToString();
                    var activeTicket = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<ActiveTicket>>(tickets).ToList()).ConfigureAwait(continueOnCapturedContext:false);

                    Settings.SuccessMessage = message;

                    ServiceContainer.Resolve<ActiveTicketsViewModel>().CheckedIn = true;
                    ServiceContainer.Resolve<ActiveTicketsViewModel>().Scanned = true;
                    ServiceContainer.Resolve<ActiveTicketsViewModel>().CheckedInTicket = activeTicket[0];

                    activeTicket[0].LastRefreshTime = DateTime.Now;

                    await ServiceContainer.Resolve<ActiveTicketsViewModel>().ExecuteInsertActiveTicketCommand(activeTicket[0]);

                    result = true;
                }
                else
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);
                    message = jsonObject["message"].ToString();
                    Settings.ErrorMessage = message;
                }
            }
            catch (Exception ex)
            {
                Settings.ErrorMessage = Strings.Oops;
            }

            return result;
        }


        public async Task<IEnumerable<ActiveTicket>> GetAllActiveTickets()
        {
            
            string parameters = "";            
            string message;
            IEnumerable<ActiveTicket> activeTickets = null;
            var uri = new Uri(Strings.activeTicket);

            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("type", "all");            

            parameters = GeneralChecks.BuildURLParametersString(data);

            client = GetClient();

            try
            {

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = null;

                response = await client.GetAsync(uri + parameters);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);
                    var tickets = jsonObject["activeTickets"].ToString();
                    activeTickets = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<ActiveTicket>>(tickets)).ConfigureAwait(continueOnCapturedContext:false);

                    
                    for(int i = 0; i < activeTickets.Count(); i++)
                    {
                        activeTickets.ElementAt(i).LastRefreshTime = DateTime.Now;                                              
                    }                  
                }
                else
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);
                    message = jsonObject["message"].ToString();
                    Settings.ErrorMessage = message;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return activeTickets;
        }
    }
}
