using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Parq.Data;
using Refractored.Xam.Settings;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using Parq.Helpers;
using Parq.Services;

namespace Parq.DataLayer
{
    public class APIScanHelper
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


        
        public async Task<int> ScanBeacon(string beaconUID)
        {
            int action = 0;
            string parameters = "";
            string barcode = "";
            string message;
            var uri = new Uri(Strings.activeTicket);

            if (await ConnectivityService.IsConnected() == false)
            {
                Settings.ErrorMessage = Strings.noInternet;
                return action;
            }

            if (!GeneralChecks.isBarcodeValid(""))
            {
                Settings.ErrorMessage = Strings.notValidBarcode;
                return action;
            }

            barcode = GeneralChecks.stripBarcode("");

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
    }
}
