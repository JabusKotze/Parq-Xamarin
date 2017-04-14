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
using System.Timers;

using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.OS;
using Android.Gms.Common.Apis;

using Parq.Droid.Services;
using Parq.Services;
using Parq.Helpers;
using Parq.Droid.Helpers;

using AndroidHUD;
using HockeyApp;

namespace Parq.Droid.Activities
{
    [Activity(Label = "@string/app_name",Icon = "@drawable/icon", MainLauncher = true,NoHistory = true, LaunchMode = LaunchMode.SingleTop, Theme = "@style/ParqTheme.Splash", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : Activity        
    {

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashScreen);

            //Set Current Activity of application
            ParqApplication.CurrentActivity = this;           

            //Check for updates from hockey app store
            CheckForUpdates();                    

            //Start timer for launch screen
            Timer timer = new Timer();
            timer.Interval = 3000;
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed; //Signin flow
            timer.Start();                                            
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
        }

        protected override void OnPause()
        {
            base.OnPause();
            UnregisterManagers();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterManagers();
        }

        /// <summary>
        /// Check if user is signed in, or request new auth token from refresh token and navigate to main activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            AndHUD.Shared.Show(this);
            if (AzureAccountService.Instance.ReadyToSignIn)
            {
                if (await Auth0AccountServices.Instance.RefreshToken())
                {
                    AndHUD.Shared.Dismiss(this);                    
                    
                    //Start Services
                    ServiceRegistrar.Startup(FileAccesHelper.GetLocalStoragePath("parqdata.db3"));                   

                    var intent = new Intent(this, typeof(MainActivity));
                    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask );
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.slideright, Resource.Animation.slide2);
                }
                else
                {
                    AndHUD.Shared.Dismiss(this);
                    var intent = new Intent(this, typeof(WelcomeActivity));
                    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask);
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.slideright, Resource.Animation.slide2);
                }
            }
            else
            {
                AndHUD.Shared.Dismiss(this);
                var intent = new Intent(this, typeof(WelcomeActivity));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask);
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.slideright, Resource.Animation.slide2);
            }
        }



        /// <summary>
        /// HockeyApp Check for updates on APK. IMPORTANT!!! Uncomment content before App Store Builds
        /// </summary>
        private void CheckForUpdates()
        {
            //Remove this for store Builds!!!
            UpdateManager.Register(this, Keys.HockeyAppID);
        }



        /// <summary>
        /// HockeyApp Unregister the Manager for Update Checks
        /// </summary>
        private void UnregisterManagers()
        {
            UpdateManager.Unregister();
        }        
    }
}