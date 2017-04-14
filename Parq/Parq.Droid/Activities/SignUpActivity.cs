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
using Parq.ViewModels;
using Parq.Helpers;
using Parq.Droid.Helpers;
using Parq.Droid.Behaviors;

using AndroidHUD;
#endregion

namespace Parq.Droid.Activities
{
    [Activity(Label = "SignUpActivity" , Theme = "@style/ParqTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SignUpActivity : BaseActivity
    {
        RelativeLayout relativeLayout;
        EditText name, surname, email, password;
        Button signUpButton, loginButtonFacebook;
        SignUpViewModel suVM;
        TextView alreadyHaveAccount;

        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.SignUpPage;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ParqApplication.CurrentActivity = this;

            relativeLayout = FindViewById<RelativeLayout>(Resource.Id.sign_up_page);
            name = FindViewById<EditText>(Resource.Id.reg_Name);
            surname = FindViewById<EditText>(Resource.Id.reg_Surname);
            email = FindViewById<EditText>(Resource.Id.reg_Email);            
            password = FindViewById<EditText>(Resource.Id.reg_Password);
            signUpButton = FindViewById<Button>(Resource.Id.join_now_button);
            alreadyHaveAccount = FindViewById<TextView>(Resource.Id.alreadyHaveAccount);
            loginButtonFacebook = FindViewById<Button>(Resource.Id.signup_facebook_login_button);

            signUpButton.Click += SignUpButton_Click;
            alreadyHaveAccount.Click += AlreadyHaveAccount_Click;
            loginButtonFacebook.Click += LoginButtonFacebook_Click;

            //Start Transition on Background
            var trans = new BackgroundTransitions(relativeLayout, 10000);
        }

        /// <summary>
        /// Login with facebook credentials
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

        private void AlreadyHaveAccount_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(SignInActivity));
            StartActivity(intent);
            //OverridePendingTransition(Resource.Animation.slideright, Resource.Animation.slide2);
        }

        private async void SignUpButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(name.Text) | string.IsNullOrWhiteSpace(surname.Text) | string.IsNullOrWhiteSpace(email.Text) | string.IsNullOrWhiteSpace(password.Text))
            {
                //Busy Indicator
                AndHUD.Shared.ShowError(this, Strings.requiredFieldsAreMissing, MaskType.Black,null,null,()=> AndHUD.Shared.Dismiss(this));                
            }               
            else
            {
                if(password.Text.Length < 8)
                {
                    AndHUD.Shared.ShowError(this, Strings.passwordStrength, MaskType.Black, null, null, () => AndHUD.Shared.Dismiss(this));
                }
                else
                {                    
                    //Busy Indicator
                    AndHUD.Shared.Show(this, "Creating Account...", -1, MaskType.Black);

                    // Continue with signup process                         
                    suVM = new SignUpViewModel();
                    var user = await suVM.ExecuteSignUpUserCommand(name.Text, surname.Text, email.Text, password.Text, false);
                        

                        if (user != null)
                        {
                            // Continue with login process after signup
                            AndHUD.Shared.Show(this, "Signing In...", -1, MaskType.Black);
                            var signInResult = await Auth0AccountServices.Instance.LoginUserParq(email.Text, password.Text,true);

                            if (signInResult)
                            {
                                AndHUD.Shared.Dismiss(this);
                                ServiceRegistrar.Startup(FileAccesHelper.GetLocalStoragePath("parqdata.db3"));                            
                                var intent = new Intent(this, typeof(MainActivity));
                                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                                intent.PutExtra("signup", true);
                                StartActivity(intent);
                                OverridePendingTransition(Resource.Animation.slideright, Resource.Animation.slide2);
                            }
                            else
                            {
                                AndHUD.Shared.ShowError(this, "Error Signing In, Please try again", MaskType.Black, null, null, () => AndHUD.Shared.Dismiss(this));
                            }                            
                        }
                        else
                        {
                        AndHUD.Shared.ShowError(this, "Error Creating Account, Please try again", MaskType.Black, null, null, () => AndHUD.Shared.Dismiss(this));
                    }                                        
                }
            }
        }
    }
}