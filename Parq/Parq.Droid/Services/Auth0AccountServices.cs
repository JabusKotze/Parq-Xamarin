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
using Android.Content;
using Auth0.SDK;
using Parq.Helpers;
using Parq.Models;
using Parq.Services;
using AndroidHUD;

namespace Parq.Droid.Services
{
    class Auth0AccountServices
    {

        private static Auth0Client authClient;
        private static Auth0User authUser;
        private static Auth0AccountServices instance;

        public static Auth0Client AuthClient
        {
            get
            {
                if (authClient == null)
                {
                    authClient = new Auth0Client(Keys.Auth0ApplicationNameSpace,Keys.Auth0ApplicationNameClientId);
                }

                return authClient;
            }
        }

        public static Auth0AccountServices Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Auth0AccountServices();
                }

                return instance;
            }
        }

        

        public Auth0AccountServices()
        {
            
        }

        public async Task<bool> LoginUserParq(string email, string password, bool isRegister = false)
        {
            //string message;
            bool result;
            if (await ConnectivityService.IsConnected())
            {
                try
                {
                    authUser = await AuthClient.LoginAsync(Keys.AuthConnectionParq, email, password, withRefreshToken: true);
                    var refreshToken = authUser.RefreshToken;
                    Settings.RefreshToken = refreshToken;
                    var api = await AuthClient.RefreshToken(refreshToken);
                    var token = api["id_token"].ToString();
                    var name = authUser.Profile["user_metadata"]["given_name"].ToString();
                    var lastname = authUser.Profile["user_metadata"]["family_name"].ToString();

                    var user = new User
                    {
                        FirstName = name,
                        LastName = lastname,
                        Email = email,
                        ProfileImage = authUser.Profile["picture"].ToString(),
                        ConnType = "Parq",
                        userId = authUser.Profile["user_id"].ToString(),
                    };

                    
                    result = await AzureAccountService.Instance.Login(token, user, isRegister);
                    if (result)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }

                }
                catch (Exception ex)
                {
                    Settings.ErrorMessage = Strings.invalidUsernamePassword;
                    result = false;
                }
            }
            else
            {
                Settings.ErrorMessage = Strings.noInternet;
                result = false;
            }

            return result;
        }


        //Log user in using facebook
        public async Task<bool> LoginUserFacebook(Context context)
        {
            //string message;
            bool result;
            if (await ConnectivityService.IsConnected())
            {
                try
                {
                    authUser = await AuthClient.LoginAsync(context, Keys.AuthConnectionFacebook, withRefreshToken: true);
                    var refreshToken = authUser.RefreshToken;
                    Settings.RefreshToken = refreshToken;
                    Settings.Connection = "facebook";
                    var api = await AuthClient.RefreshToken(refreshToken);
                    var token = api["id_token"].ToString();
                    var given_name = authUser.Profile["given_name"].ToString();
                    var family_name = authUser.Profile["family_name"].ToString();
                    var email = authUser.Profile["email"].ToString();
                    var cover = authUser.Profile["cover"]["source"].ToString();
                    var profileImage = authUser.Profile["picture"].ToString();

                    Settings.Cover = cover;

                    var user = new User
                    {
                        FirstName = given_name,
                        LastName = family_name,
                        Email = email,
                        ConnType = "facebook",
                        ProfileImage = profileImage,
                        userId = authUser.Profile["user_id"].ToString(),
                    };

                    AndHUD.Shared.Show(context, "we're getting things ready...", -1, MaskType.Black);

                    result = await AzureAccountService.Instance.Login(token, user);
                    if (result)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }

                }
                catch (InvalidOperationException)
                {
                    Settings.ErrorMessage = Strings.invalidUsernamePassword;
                    result = false;
                }
            }
            else
            {
                Settings.ErrorMessage = Strings.noInternet;
                result = false;
            }

            return result;
        }



        public bool Logout()
        {
            bool result;
            try
            {
                Auth0AccountServices.AuthClient.Logout();
                AzureAccountService.Instance.SignOut();
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }    
        
        //Refresh the user token.
        public async Task<bool> RefreshToken()
        {
            var refreshToken = Settings.RefreshToken;
            try
            {                
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    if (!TokenValidator.HasExpired(Settings.AuthToken))
                    {                        
                        return true;
                    }
                    else
                    {
                        var result = await AuthClient.RefreshToken(refreshToken);
                        var newToken = result["id_token"].ToString();
                        AzureAccountService.Instance.SaveAuthenticationToken(newToken);
                        return true;
                    }
                }
                else
                {
                    Settings.ErrorMessage = Strings.couldNotSignIn;
                    return false;
                }
                                
            }
            catch (Exception ex)
            {
                Settings.ErrorMessage = Strings.couldNotSignIn;
                return false;
            }
            
        } 

    }
}