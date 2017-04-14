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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Refractored.Xam.Settings;
using Parq.Services;

// Source: http://thirteendaysaweek.com/2013/12/13/xamarin-ios-and-authentication-in-windows-azure-mobile-services-part-iii-custom-authentication/
namespace Parq.Helpers
{
    public class AuthenticationHandler : DelegatingHandler
	{
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
            //request.Headers.Add("ZUMO-API-VERSION", "2.0.0.");
            var response = await base.SendAsync(request, cancellationToken);
            //response.EnsureSuccessStatusCode();
            
            var jsonString = await response.Content.ReadAsStringAsync();
			var jsonObject = JObject.Parse(jsonString);
			var token = jsonObject["token"].ToString();
			SaveAuthenticationToken (token);

			return response;
		}

		private void SaveAuthenticationToken (string token)
		{
			AzureAccountService.Instance.AuthenticationToken = token;
            Settings.AuthToken = token;
			Settings.TokenExpiration = DateTime.Now.AddDays (30);
		}
	}
}