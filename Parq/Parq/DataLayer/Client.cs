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
using System.Net.Http;
using Refractored.Xam.Settings;
using System.Net.Http.Headers;
using ModernHttpClient;
using Parq.Helpers;

namespace Parq.DataLayer
{
    public static class Client
    {         
        /// <summary>
        /// Generates Http Client 
        /// </summary>
        /// <returns>Authorized Http Client</returns>
        public static HttpClient GetClient()
        {
            string authorizationKey = "";
            HttpClient client = new HttpClient(new NativeMessageHandler());
            client.MaxResponseContentBufferSize = 256000;
            if (string.IsNullOrEmpty(authorizationKey))
            {
                authorizationKey = Settings.AuthToken;
            }

            client.DefaultRequestHeaders.Add("x-zumo-auth", authorizationKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));           

            return client;
        }
    }
}
