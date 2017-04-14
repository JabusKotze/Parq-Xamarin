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
using System.Diagnostics;
using Parq.Data;
using Parq.Models;
using Parq.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refractored.Xam.Settings;


namespace Parq.DataLayer
{
    public class APIHistoryTickets
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


        public async Task<IEnumerable<HistoryTicket>> GetAllHistoryTickets()
        {

            string parameters = "";
            string message;
            IEnumerable<HistoryTicket> historyTickets = null;
            var uri = new Uri(Strings.historyTicket);

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
                    var tickets = jsonObject["historyTickets"].ToString();
                    historyTickets = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<HistoryTicket>>(tickets)).ConfigureAwait(continueOnCapturedContext: false);


                    for (int i = 0; i < historyTickets.Count(); i++)
                    {
                        historyTickets.ElementAt(i).LastRefreshTime = DateTime.Now;
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

            return historyTickets;
        }
    }
}
