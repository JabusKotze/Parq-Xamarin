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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Parq.DataLayer;
using Parq.Droid.Behaviors;
using AndroidHUD;
using Parq.Helpers;

namespace Parq.Droid.Activities
{
    [Activity(Label = "ResetPasswordActivity", NoHistory = true, Theme = "@style/ParqTheme")]
    public class ResetPasswordActivity : BaseActivity
    {
        EditText email;
        Button resetButton;
        TextView requireAssistance;
        RelativeLayout layout;        

        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.ResetPassword;
            }
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            var extra_email = Intent.GetStringExtra("email");
            email.Text = extra_email;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ParqApplication.CurrentActivity = this;

            email = FindViewById<EditText>(Resource.Id.pw_reset_email);
            resetButton = FindViewById<Button>(Resource.Id.pw_reset_submit);
            requireAssistance = FindViewById<TextView>(Resource.Id.need_help);
            layout = FindViewById<RelativeLayout>(Resource.Id.reset_password_page);

            resetButton.Click += ResetButton_Click;
            requireAssistance.Click += RequireAssistance_Click;
            
            //Start Transition on Background
            var trans = new BackgroundTransitions(layout, 10000); 
        }
                
        private void RequireAssistance_Click(object sender, EventArgs e)
        {
            //TODO
        }

        private async void ResetButton_Click(object sender, EventArgs e)
        {
            //Busy Indicator
            AndHUD.Shared.Show(this, "Sending Request", -1, MaskType.Black);

            //
            var signup = new RestUserAccounts();
            var result = await signup.ChangePassword(email.Text);

            if (result)
            {
                AndHUD.Shared.Dismiss(this);
                var intent = new Intent(this, typeof(SignInActivity));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask );
                intent.PutExtra("email", email.Text);
                intent.PutExtra("reset_email", true);
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.slide, Resource.Animation.slideleft);
            }
            else
            {
                AndHUD.Shared.ShowError(this, Settings.ErrorMessage, MaskType.Black, null, null, () => AndHUD.Shared.Dismiss(this));
            }
        }
    }
}