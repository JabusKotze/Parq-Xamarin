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
using Refractored.Xam.Settings;

namespace Parq.Helpers
{
    public sealed class GeneralChecks
    {
        public GeneralChecks()
        {

        }
        /// <summary>
        /// Checks if the scanned barcode is valid
        /// </summary>
        /// <param name="barcode">Provide QR Barcode</param>
        /// <returns>boolean</returns>
        public static bool isBarcodeValid(string barcode)
        {
            bool result = false;
            if (barcode.StartsWith("Parq"))
            {
                result = true;
            }else
            {
                Settings.ErrorMessage = Strings.notValidBarcode;
                result = false;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public static string stripBarcode(string barcode)
        {
            string result = "";
            try
            {
                result = barcode.Substring(5, barcode.Length - 5);
            }catch(Exception ex)
            {
                Settings.ErrorMessage = Strings.notValidBarcode;
                result = "";
            }

            return result;
        }


        /// <summary>
        /// function Builds the URI Parameter string
        /// </summary>
        /// <param name="parameters">Request Params</param>
        /// <returns>Returns URI Parameters String</returns>
        public static String BuildURLParametersString(Dictionary<string, string> parameters)
        {
            UriBuilder uriBuilder = new UriBuilder();
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var urlParameter in parameters)
            {
                query[urlParameter.Key] = urlParameter.Value;
            }
            uriBuilder.Query = query.ToString();
            return uriBuilder.Query;
        }
    }
}
