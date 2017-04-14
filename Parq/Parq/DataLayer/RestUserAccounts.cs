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
using System.Text;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Parq.Models;
using Parq.Helpers;
using Parq.Services;

namespace Parq.DataLayer
{    


    public class RestUserAccounts
    {
        HttpClient client;

        public HttpClient GetClient()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            return client;
        }

        public async Task<User> SignUp(SignUp signup)
        {
            User user = null;
            var uri = new Uri(Strings.SignupURL);        
            
            signup.Connection = Keys.AuthConnectionParq;
            client = GetClient();

            try
            {                

                var json = JsonConvert.SerializeObject(signup);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;

                response = await client.PostAsync(uri, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);
                    var userId = jsonObject["_id"].ToString();
                    user = new User
                    {
                        FirstName = signup.FirstName,
                        LastName = signup.LastName,
                        Email = signup.Email,
                        ProfileImage = "",
                        ConnType = Keys.AuthConnectionParq,
                        userId = userId
                    };                   

                }
                else
                {
                    var res = response.StatusCode.ToString();                
                    
                }


            } catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                
            }

            return user;
        }



        public async Task<string> SignIn(string email, string password)
        {
            string token = "";
            var uri = new Uri(Strings.SignInURL);

            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("client_id", Keys.Auth0ApplicationNameClientId);
            data.Add("username", email);
            data.Add("password", password);            
            data.Add("connection", Keys.AuthConnectionParq);
            data.Add("grant_type", "password");
            data.Add("scope", "openid");

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
                    token = jsonObject["id_token"].ToString();
                }
                else
                {
                    var res = response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return token;
        }



        public async Task<bool> UserMetadataInsert(string name, string surname, string userId, string token)
        {
            bool result = false;
            var method = new HttpMethod("PATCH");
            var uri = new Uri(Strings.metadata + "/" + userId);

            var updateUserMetadata = new UserMetaData
            {
                GivenName = name,
                FamilyName = surname,
            };

            Dictionary<string, UserMetaData> data = new Dictionary<string, UserMetaData>();
            data.Add("user_metadata", updateUserMetadata);            
            
            client = GetClient();

            try
            {

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");                

                var request = new HttpRequestMessage(method, uri)
                {
                    Content = content,                  
                    
                };

                HttpResponseMessage response = new HttpResponseMessage();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                response = await client.SendAsync(request);                

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);
                    result = true;
                }
                else
                {
                    var res = response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }

            return result;

        }


        /*
        * Change Password request when user forgot password
        */
        public async Task<bool> ChangePassword(string email)
        {

            if (await ConnectivityService.IsConnected())
            {
                var uri = new Uri(Strings.ChangePasswordURL);
                SignUp signup = new SignUp()
                {
                    Email = email,
                    Connection = Keys.AuthConnectionParq,
                };

                client = GetClient();

                try
                {

                    var json = JsonConvert.SerializeObject(signup);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;

                    response = await client.PostAsync(uri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        Settings.ErrorMessage = Strings.noAccountFound;
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    Settings.ErrorMessage = Strings.Oops;
                    return false;
                }
            }
            else
            {
                Settings.ErrorMessage = Strings.noInternet;
                return false;
            }

        }


        /// <summary>
        /// Insert User Data at Signup into Azure Database
        /// </summary>
        /// <param name="user">Passing User Data after registering with Auth0</param>
        /// <returns></returns>
        public async Task<bool> InsertUserAzure(User user)
        {
            bool result = false;
            var uri = new Uri(Strings.signup);           
            
            client = Client.GetClient();

            try
            {

                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;

                response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);
                    result = true; 
                }
                else
                {
                    var res = response.StatusCode.ToString();
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }

            return result;
        }
    }
}