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
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Parq.DataLayer;
using Parq.Models;
using Parq.Factories;
using Parq.Helpers;
using Microsoft.WindowsAzure.MobileServices;
using Refractored.Xam.Settings;
using Parq.Data;

namespace Parq.Services
{
    public class AzureAccountService
	{
		private static AzureAccountService instance;     
        
        public MobileServiceClient client { get; set; }   

        public static MobileServiceUser CurrentUser { get; set; }

		public static AzureAccountService Instance
		{
			get 
			{
				if (instance == null) 
				{
					instance = new AzureAccountService ();
				}

				return instance;
			}
		}

		public string AuthenticationToken { get; set; }
		public Account Account { get; set; }
		public User User { get; set; }

		public bool ReadyToSignIn
		{
			get { return !string.IsNullOrEmpty (AuthenticationToken); }
		}

		private AzureAccountService ()
		{
			FetchAuthenticationToken ();
		}

		void FetchAuthenticationToken ()
		{              
			AuthenticationToken = Settings.AuthToken;
            client = MobileServiceClientFactory.CreateClient();
            var azureUser = new MobileServiceUser(Settings.UserId);
            azureUser.MobileServiceAuthenticationToken = AuthenticationToken;
            client.CurrentUser = azureUser;
        }

		public async Task<User> Register (SignUp signup)
		{

            User result = null;
            var signupRest = new RestUserAccounts();
            var user =  await signupRest.SignUp(signup);

            try
            {
                if (user != null)
                {
                    result = user;
                    var token = await signupRest.SignIn(signup.Email, signup.Password);

                    SaveAuthenticationToken(token);                    

                    var metadata = await signupRest.UserMetadataInsert(signup.FirstName, signup.LastName, "auth0|" + result.userId, token);
                    
                    if (!metadata)
                    {
                        result = null;
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }
		

		public async Task<bool> Login (string token, User user, bool isRegister = true)
		{
			bool result;
            var signupRest = new RestUserAccounts();

            try
			{                                
                SaveAuthenticationToken(token);

                //Save User data
                Settings.ProfileImage = user.ProfileImage;
                Settings.ProfileName = user.FirstName + " " + user.LastName;
                Settings.ProfileEmail = user.Email;
                Settings.UserId = user.userId;

                if (isRegister)
                {
                    result = true;
                    var insertUserAzure = await signupRest.InsertUserAzure(user);

                    if (!insertUserAzure)
                    {
                        result = true;
                    }                                        
                }
                else
                {
                    result = true;
                }  
			}
			catch (Exception ex)
			{
                //Xamarin.Insights.Report (ex);
                Settings.ErrorMessage = Strings.Oops;
                result = false;
			}

			return result;
		}

		public async void SignOut ()
		{
			AzureAccountService.Instance.AuthenticationToken = "";

            DataBase adb = DataBase.adb;
            await adb.clearTables();

            Settings.AuthToken = string.Empty;
            Settings.RefreshToken = string.Empty;
            Settings.TokenExpiration = DateTime.Now;
            Settings.ErrorMessage = string.Empty;
            Settings.SuccessMessage = string.Empty;
            Settings.ProfileImage = string.Empty; 
            Settings.ProfileName = string.Empty;
            Settings.ProfileEmail = string.Empty;
            Settings.UserId = string.Empty;
            Settings.Cover = string.Empty; 
            Settings.Connection = string.Empty;
            Settings.DataBasePath = string.Empty;
            
        }

		public async Task<bool> DeleteAccount ()
		{
			bool result;

			try {
				using (var handler = new AuthenticationHandler ()) {
					using (client = MobileServiceClientFactory.CreateClient (handler)) {
						// Account
						var accountTable = client.GetTable<Account> ();
						await accountTable.DeleteAsync (Account);

						// User
						var userTable = client.GetTable <User> ();
						await userTable.DeleteAsync (User);						
					}
				}

				result = true;
			} catch {
				result = false;
			}

			return result;
		}

		public async Task<bool> InsertUser (User user)
		{
            bool result = false;
            try
            {       
                using (client = MobileServiceClientFactory.CreateClient())
                {
                    var azureUser = new MobileServiceUser(user.userId);                            
                    azureUser.MobileServiceAuthenticationToken = Settings.AuthToken;                                          
                    client.CurrentUser = azureUser;
                    var table = client.GetTable<User>();
                    
                    var currentUser = await table.ReadAsync();
                    var count = currentUser.Count();
                    if (count < 1)
                    {
                        await table.InsertAsync(user);
                    }    
                }                                   

                result = true;

            }catch (Exception ex)
            {
                Settings.ErrorMessage = Strings.Oops;
                result = false;
            }

            return result;
			
		}

		private async Task<Account> GetCurrentAccount (Account account)
		{
			using (var handler = new ZumoAuthHeaderHandler ()) {
				using (client = MobileServiceClientFactory.CreateClient (handler)) {
					var currentAccount = await client.GetTable <Account> ()
						.Where (acct => acct.Email == account.Email)
						.Select (acct => acct).ToListAsync ();

					return currentAccount [0];
				}
			}
		}

		private async Task<User> GetCurrentUser ()
		{
			using (var handler = new ZumoAuthHeaderHandler ()) {
				using (client = MobileServiceClientFactory.CreateClient (handler)) {
					return await client.GetTable <User> ().LookupAsync (Account.UserId);
				}
			}
		}

        private async Task<User> GetUserById(string userId)
        {
            try
            {
                using (client = MobileServiceClientFactory.CreateClient())
                {
                    return await client.GetTable<User>().LookupAsync(userId);
                }
            }
            catch
            {
                return null;
            }            
        }

        public void SaveAuthenticationToken(string token)
        {
            AzureAccountService.Instance.AuthenticationToken = token;
            Settings.AuthToken = token;            
        }
    }
}