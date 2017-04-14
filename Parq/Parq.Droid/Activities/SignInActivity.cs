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
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Parq.Droid.Services;
using Parq.Helpers;
using Parq.Droid.Helpers;
using Parq.Droid.Behaviors;

using AndroidHUD;
#endregion

namespace Parq.Droid.Activities
{
    [Activity(Label = "LogInActivity", Theme = "@style/ParqTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SignInActivity : BaseActivity
    {
        EditText email;
        EditText password;
        TextView forgotPassword, createAccount;
        Button loginButton, loginButtonFacebook;        
        RelativeLayout relativeLayout;

        protected override int LayoutResource
        {
            get
            {                
                return Resource.Layout.SignInPage;
            }
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            if (Intent.GetBooleanExtra("reset_email", false))
            {
                email.Text = Intent.GetStringExtra("email");
                AndHUD.Shared.ShowSuccess(this, "Password Reset Confirmation Email sent to\n" + email.Text, MaskType.Black, TimeSpan.FromSeconds(3),null, () => AndHUD.Shared.Dismiss(this));
            }            
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ParqApplication.CurrentActivity = this;

            relativeLayout = FindViewById<RelativeLayout>(Resource.Id.sign_in_page);
            loginButton = FindViewById<Button>(Resource.Id.main_sign_in);
            loginButtonFacebook = FindViewById<Button>(Resource.Id.facebook_login_button);
            email = FindViewById<EditText>(Resource.Id.email);
            password = FindViewById<EditText>(Resource.Id.password);

            forgotPassword = FindViewById<TextView>(Resource.Id.forgot_password);
            createAccount = FindViewById<TextView>(Resource.Id.txt_create_account);

            loginButton.Click += LogInActivity_Click;
            loginButtonFacebook.Click += LoginButtonFacebook_Click;

            forgotPassword.Click += ForgotPassword_Click;
            createAccount.Click += CreateAccount_Click;

            //Start Transition on Background
            var trans = new BackgroundTransitions(relativeLayout, 10000);

        }




        /// <summary>
        /// Log in with Facebook Social provider : Auth0.com
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoginButtonFacebook_Click(object sender, EventArgs e)
        {
            //Busy Indicator
            AndHUD.Shared.Show(this, "Redirecting to Facebook...", -1, MaskType.Black);

            // Continue with login process                 
            var result = await Auth0AccountServices.Instance.LoginUserFacebook(this);            

            if (result)
            {
                AndHUD.Shared.Dismiss(this);
                ServiceRegistrar.Startup(FileAccesHelper.GetLocalStoragePath("parqdata.db3"));
                var intent = new Intent(this, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                intent.PutExtra("login", true);
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.slideright, Resource.Animation.slide2);
            }
            else
            {
                AndHUD.Shared.ShowError(this, Settings.ErrorMessage, MaskType.Black, null, null, () => AndHUD.Shared.Dismiss(this));
            }
        }      
       


        /// <summary>
        /// Log in with Auth0 Custom Database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LogInActivity_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(email.Text) | string.IsNullOrWhiteSpace(password.Text))
            {
                AndHUD.Shared.ShowError(this, "No Email/Password provided", MaskType.Black, TimeSpan.FromSeconds(3));
            }            
            else
            {
                //Busy Indicator
                AndHUD.Shared.Show(this, "Signing In...",-1, MaskType.Black);

                // Continue with login process                 
                var result = await Auth0AccountServices.Instance.LoginUserParq(email.Text, password.Text);
                                
                if (result)
                {
                    AndHUD.Shared.Dismiss(this);
                    ServiceRegistrar.Startup(FileAccesHelper.GetLocalStoragePath("parqdata.db3"));
                    var intent = new Intent(this, typeof(MainActivity));
                    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    intent.PutExtra("login", true);
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.slideright, Resource.Animation.slide2);
                }
                else
                {
                    AndHUD.Shared.ShowError(this, Settings.ErrorMessage, MaskType.Black, null, null, () => AndHUD.Shared.Dismiss(this));
                } 
            } 
        }


        /// <summary>
        /// Navigate to activity to create Account for Parq users
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateAccount_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(SignUpActivity));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.slideright, Resource.Animation.slide2);
        }



        /// <summary>
        /// Navigate to forgot password activity to request password reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForgotPassword_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(ResetPasswordActivity));
            intent.PutExtra("email", email.Text); //Pass text entered into email field to password reset activity
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.slideright, Resource.Animation.slide2);
        }        
    }
}