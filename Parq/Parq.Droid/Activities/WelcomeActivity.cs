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
#region USINGS
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Content.PM;
using Parq.Helpers;
using Parq.Droid.Behaviors;
using HockeyApp;
using HockeyApp.Metrics;
#endregion

namespace Parq.Droid.Activities
{
    [Activity(Label = "@string/app_name" ,Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleTop, Theme = "@style/ParqTheme",ScreenOrientation = ScreenOrientation.Portrait)]
    public class WelcomeActivity : BaseActivity
    {
        RelativeLayout layout; 

        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.welcomePage;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ParqApplication.CurrentActivity = this;

            //Register HockeyApp CrashManager
            CrashManager.Register(this, Keys.HockeyAppID);

            //Register HockeyApp Metrics Manager
            MetricsManager.Register(this, Application, Keys.HockeyAppID);

            SetContentView(Resource.Layout.welcomePage);
            layout = FindViewById<RelativeLayout>(Resource.Id.welcome_screen);
            FindViewById<Button>(Resource.Id.home_sign_in).Click += OnSignInClicked;
            FindViewById<Button>(Resource.Id.home_join_now).Click += OnJoinNowClicked;

            //Start Transition on Background
            var trans = new BackgroundTransitions(layout, 10000);

        }



        /// <summary>
        /// Redirects to signup activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnJoinNowClicked(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(SignUpActivity));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.slideright, Resource.Animation.slide2);
        }


        /// <summary>
        /// Redirects to sign in activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="a"></param>
        private void OnSignInClicked(object sender, EventArgs a)
        {
            var intent = new Intent(this, typeof(SignInActivity));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.slideright, Resource.Animation.slide2);
        }
    }
}