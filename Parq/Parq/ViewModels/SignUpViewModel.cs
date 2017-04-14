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
using System.Threading.Tasks;
using Parq.Models;
using Parq.Services;

namespace Parq.ViewModels
{
    public class SignUpViewModel : BaseViewModel
	{
		public async Task<User> ExecuteSignUpUserCommand (string Name, string Surname, string Email, string Password, bool isActive)
		{
            User user = null;

			if (IsBusy) {
				return user;
			}

			IsBusy = true;
			
            var signup = new SignUp
            {
                FirstName = Name,
                LastName = Surname,
                Email = Email,
                Password = Password
            };  
		
			try
			{				
				if (await ConnectivityService.IsConnected ()) {
					user = await CreateAccount (signup);
				}
			}
			catch (Exception ex) 
			{
				//Insights.Report (ex);
			}

			IsBusy = false;
            return user;
		}

		private async Task<User> CreateAccount (SignUp signup)
		{
			return await AzureAccountService.Instance.Register (signup);
		}		
	}
}