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
using Parq.Services;

// Source: http://thirteendaysaweek.com/2013/12/13/xamarin-ios-and-authentication-in-windows-azure-mobile-services-part-iii-custom-authentication/
namespace Parq.Helpers
{
	public class ZumoAuthHeaderHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(AzureAccountService.Instance.AuthenticationToken))
			{
				throw new InvalidOperationException("User is not currently logged in");
			}

			request.Headers.Add("X-ZUMO-AUTH", AzureAccountService.Instance.AuthenticationToken);
            request.Headers.Add("ZUMO-API-VERSION", "2.0.1.");

            return base.SendAsync(request, cancellationToken);
		}
	}
}