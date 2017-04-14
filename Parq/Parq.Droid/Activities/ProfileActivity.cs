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
using Android.Content.PM;
using Microsoft.WindowsAzure.MobileServices;
using Android.Views;
using Android.OS;
using Android.Support.Design.Widget;
using Parq.Helpers;
using Android.Widget;
using Android.Content;
using Refractored.Controls;
using Parq.Droid.Helpers;
using Parq.Droid.Services;
using AndroidHUD;


namespace Parq.Droid.Activities
{
    [Activity(Label = "Parq", Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleTop, Theme = "@style/ParqTheme")]
    public class ProfileActivity : BaseActivity
    {

        ImageView cover;
        TextView profileNameTV, profileEmailTV;
        CircleImageView profilePic;
        CollapsingToolbarLayout collapsingToolbarLayout;
        string profileName, profileEmail, connection, profileImageUri, coverURL;
        int image;

        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.Activity_Profile;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CurrentPlatform.Init();

            ParqApplication.CurrentActivity = this;

            cover = FindViewById<ImageView>(Resource.Id.profile_backdrop);
            profileNameTV = FindViewById<TextView>(Resource.Id.profile_name);
            profileEmailTV = FindViewById<TextView>(Resource.Id.profile_email);
            profilePic = FindViewById<CircleImageView>(Resource.Id.prof_profile_image);
            collapsingToolbarLayout = FindViewById<CollapsingToolbarLayout>(Resource.Id.profile_collapsing_toolbar);
            image = Resource.Drawable.profile_avatar;

            profileName = Settings.ProfileName;
            profileEmail = Settings.ProfileEmail; 
            connection = Settings.Connection; 
            profileImageUri = Settings.ProfileImage; 

            profileNameTV.Text = Settings.ProfileName; 
            profileEmailTV.Text = Settings.ProfileEmail; 

            collapsingToolbarLayout.SetTitle(profileName);      
            

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24dp);
            SupportActionBar.SetIcon(Resource.Drawable.icon);            

            if (connection == "facebook")
            {
                coverURL = Settings.Cover;
                PicassoLoadImage.LoadPic(this, coverURL, cover, image);
                PicassoLoadImage.LoadProfilePic(this, profileImageUri, profilePic, image, true, true);
            }
            else
            {
                PicassoLoadImage.LoadProfilePic(this, profileImageUri, profilePic, image);
            }

        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.profile_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:                    
                    Finish();
                    OverridePendingTransition(Resource.Animation.slide, Resource.Animation.slideleft);
                    return true;

                case Resource.Id.prof_menu_locate:
                    return true;

                case Resource.Id.prof_menu_logout:
                    try
                    {
                        if (Auth0AccountServices.Instance.Logout())
                        {
                            var intent = new Intent(this, typeof(WelcomeActivity));
                            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                            StartActivity(intent);
                        }
                    }
                    catch (Exception ex)
                    {
                        AndHUD.Shared.ShowError(this, Strings.couldNotSignOut, MaskType.Black, TimeSpan.FromSeconds(3));
                    }
                    return true;

                case Resource.Id.prof_menu_scanner:                    
                    return true;

                case Resource.Id.prof_menu_refresh:
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}