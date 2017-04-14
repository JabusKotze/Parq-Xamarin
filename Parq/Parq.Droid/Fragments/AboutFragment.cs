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

using Android.Support.V4.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using Parq.Helpers;

using HockeyApp;

namespace Parq.Droid.Fragments
{
    class AboutFragment : Fragment
    {
        public AboutFragment()
        {
            this.RetainInstance = true;            
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.Fragment_About, container, false);

            FeedbackManager.Register(this.Context, Keys.HockeyAppID);

            Button feedbackButton = rootView.FindViewById<Button>(Resource.Id.about_feedbackButton);
            Button crashButton = rootView.FindViewById<Button>(Resource.Id.about_forceCrash);
            feedbackButton.Click += delegate
            {
                FeedbackManager.ShowFeedbackActivity(this.Activity.ApplicationContext);
            };

            crashButton.Click += CrashButton_Click;
            return rootView;
        }

        private void CrashButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}