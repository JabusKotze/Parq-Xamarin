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

namespace Parq.Helpers
{
    public static class Strings
	{
        //Auth0 Url strings
        public static readonly string SignupURL = "https://parq.auth0.com/dbconnections/signup";
        public static readonly string SignInURL = "https://parq.auth0.com/oauth/ro";
        public static readonly string ChangePasswordURL = "https://parq.auth0.com/dbconnections/change_password";
        public static readonly string metadata = "https://parq.auth0.com/api/v2/users";


        //Azure API URLs        
        public static readonly string activeTicket = "https://javusparqapp.azurewebsites.net/api/activeTicket";
        public static readonly string historyTicket = "https://javusparqapp.azurewebsites.net/api/historyTicket";
        public static readonly string facility = "https://javusparqapp.azurewebsites.net/api/facility";
        public static readonly string gate = "https://javusparqapp.azurewebsites.net/api/gate";
        public static readonly string signup = "https://javusparqapp.azurewebsites.net/api/signup";

        /*
         * API - SCAN 
         */
        public static readonly string scanBeaconAPI = "https://javusparqapp.azurewebsites.net/api/v1/scan/beacon/"; //Add {beaconId} as params
        public static readonly string scanQRAPI = "https://javusparqapp.azurewebsites.net/api/v1/scan/qr/"; //Add {qrId} as params
        public static readonly string scanNFCAPI = "https://javusparqapp.azurewebsites.net/api/v1/scan/nfc/"; //Add {nfcId} as params


        //Error Messages
        public static readonly string invalidUsernamePassword = "Incorrect Username Password";
        public static readonly string userAlreadyExists = "Account already exists";
        public static readonly string noInternet = "No Internet Connection";
        public static readonly string Oops = "Oops! Something went wrong \nPlease try again";
        public static readonly string couldNotSignIn = "Oops! Could not sign in \nPlease sign in again";
        public static readonly string noAccountFound = "This Account does not exist";
        public static readonly string couldNotSignOut = "Oops! Could not sign out\nPlease try again";
        public static readonly string requiredFieldsAreMissing = "Required fields are missing";
        public static readonly string passwordStrength = "Password must contain at least:\n1 Uppercase letter \n1 Numerical Value \n1 Lowercase letter\n and contain 8 characters or more";
        public static readonly string notValidBarcode = "Barcode Not Valid";

        public static readonly string onYourMarks = "On Your Marks";
        public static readonly string getSet = "Get Set";
        public static readonly string go = "Go!";
    }
}

