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

// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
//using Refractored.Xam.Settings;

namespace Parq.Helpers
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
  public static class Settings
  {
        private static ISettings AppSettings
        {
          get
          {
            return CrossSettings.Current;
          }
        }

        public enum Platform { AndroidPlatform, iOSPlatform, WindowsPlatform, AllPlatforms }

        #region Setting Constants

        private const string ErrorMessageKey = "error_message";
        private static readonly string ErrorMessageDefault = string.Empty;

        private const string SuccessMessageKey = "success_message";
        private static readonly string SuccessMessageDefault = string.Empty;

        private static readonly string AuthTokenKey = "auth_key";
        private static readonly string AuthTokenDefault = string.Empty;

        private static readonly string ConnectionKey = "connection";
        private static readonly string ConnectionDefault = string.Empty;

        private static readonly string ProfileNameKey = "profile_image";
        private static readonly string ProfileNameDefault = string.Empty;

        private static readonly string ProfileEmailKey = "profile_email";
        private static readonly string ProfileEmailDefault = string.Empty;

        private static readonly string ProfileImageKey = "profile_image";
        private static readonly string ProfileImageDefault = string.Empty;

        private static readonly string CoverKey = "cover";
        private static readonly string CoverDefault = string.Empty;

        private static readonly string RefreshTokenKey = "refresh_token";
        private static readonly string RefreshTokenDefault = string.Empty;

        private static readonly string UserIdKey = "userId";
        private static readonly string UserIdDefault = string.Empty;

        private static readonly string TokenExpirationKey = "token_expiration";
        private static readonly DateTime TokenExpirationDefault = DateTime.Now;

        private static readonly string DataBasePathKey = "database_path";
        private static readonly string DataBasePathDefault = string.Empty;

        private static readonly string CurrentPlatformKey = "database_path";
        private static readonly int CurrentPlatformDefault = 0;

        #endregion

        #region GETTERS and SETTERS
        public static string ErrorMessage
        {
          get
          {
            return AppSettings.GetValueOrDefault<string>(ErrorMessageKey, ErrorMessageDefault);
          }
          set
          {
            AppSettings.AddOrUpdateValue<string>(ErrorMessageKey, value);
          }
        }

        public static string SuccessMessage
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(SuccessMessageKey, SuccessMessageDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(SuccessMessageKey, value);
            }
        }

        public static string AuthToken
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(AuthTokenKey, AuthTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(AuthTokenKey, value);
            }
        }


        public static string Connection
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(ConnectionKey, ConnectionDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(ConnectionKey, value);
            }
        }

        public static string ProfileName
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(ProfileNameKey, ProfileNameDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(ProfileNameKey, value);
            }
        }


        public static string ProfileEmail
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(ProfileEmailKey, ProfileEmailDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(ProfileEmailKey, value);
            }
        }


        public static string ProfileImage
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(ProfileImageKey, ProfileImageDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(ProfileImageKey, value);
            }
        }

        public static string Cover
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(CoverKey, CoverDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(CoverKey, value);
            }
        }

        public static string RefreshToken
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(RefreshTokenKey, RefreshTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(RefreshTokenKey, value);
            }
        }

        public static string UserId
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(UserIdKey, UserIdDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(UserIdKey, value);
            }
        }

        public static DateTime TokenExpiration
        {
            get
            {
                return AppSettings.GetValueOrDefault<DateTime>(TokenExpirationKey, TokenExpirationDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<DateTime>(TokenExpirationKey, value);
            }
        }

        public static string DataBasePath
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(DataBasePathKey, DataBasePathDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(DataBasePathKey, value);
            }
        }

        public static int CurrentPlatform
        {
            get
            {
                return AppSettings.GetValueOrDefault<int>(CurrentPlatformKey, CurrentPlatformDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<int>(CurrentPlatformKey, value);
            }
        }
        #endregion


                

    }
}