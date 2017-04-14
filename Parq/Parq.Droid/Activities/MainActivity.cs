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
using System.Threading.Tasks;

using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Graphics;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Gms.Common.Apis;
using Android.Gms.Nearby;
using Android.Gms.Nearby.Messages;
using Android.Gms.Common;
using Android.Util;

using Microsoft.WindowsAzure.MobileServices;
using Refractored.Controls;
using AndroidHUD;
using Card.IO;
using Square.Picasso;
using HockeyApp;
using HockeyApp.Metrics;
using Gcm.Client;

using Parq.Droid.Helpers;
using Parq.DataLayer;
using Parq.Helpers;
using Parq.ViewModels;
using Parq.Droid.Fragments;
using Parq.Droid.Services;
using Parq.Droid.Behaviors;
#endregion

namespace Parq.Droid.Activities
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleTop, Theme ="@style/ParqTheme")]
    public class MainActivity : BaseActivity,
        GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, IResultCallback
    {
        DrawerLayout drawerLayout;
        RelativeLayout navRelativeLayout;
        View navigationHeader;        
        NavigationView navigationView;
        CollapsingToolbarLayout collapsingToolbarLayout;
        CoordinatorLayout mainCoordinator;
        CircleImageView profilePic;
        TextView profileNameTV, profileEmailTV;
        int image, currentFragmentPosition;
        string fragmentTag, connection, coverURL, profileUri, profileName, profileEmail;
        Android.Support.V4.App.Fragment fragment;
        HomeFragment homeFragment;
        FloatingActionButton fab;
        APIActiveTickets AzureAPIActiveTicket;

        //Notification Frame Attributes
        FrameLayout notificationFrameCheckOut;
        TextView titleCheckout, titleCheckin, captionCheckout, captionCheckin;
        CoordinatorLayout.LayoutParams notifLP, notifLPOriginal, fabLP, fabLPOriginal;
        CoordinatorLayout.Behavior notifCBOriginal, fabCBOriginal;
        NotificationBehavior nb;
        FabMoveBehavior fb;     

       
        GoogleApiClient mGoogleApiClient;
        //Stores the PendingIntent used to request beacon monitoring
        PendingIntent mBeaconRequestIntent;
        BeaconMessageListener beaconMessageListener;
        BeaconSubscribeCallback _callback = new BeaconSubscribeCallback();
        SubscribeOptions options;
        MessageFilter filter;


        //Flag that indicates if a request is underway
        bool mInProgress;

        /// <summary>
        /// Defines the allowable request types
        /// </summary>
        enum RequestType { SubscribeMessages }
        RequestType mRequestType;

        /// <summary>
        /// Check connectivity to google play services
        /// </summary>
        bool IsGooglePlayServicesAvailable
        {
            get
            {
                int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
                if (resultCode == ConnectionResult.Success)
                {
                    if (Log.IsLoggable(Helpers.Constants.TAG, LogPriority.Debug))
                    {
                        Log.Debug(Helpers.Constants.TAG, "Google Play Services is available");
                    }
                    return true;
                }
                else
                {
                    Log.Error(Helpers.Constants.TAG, "Google Play Services is unavailable");
                    return false;
                }
            }
        }

        PendingIntent BeaconPendingIntent
        {
            get
            {
                return PendingIntent.GetService(Application.Context, 0, BeaconIntent, PendingIntentFlags.UpdateCurrent);                
            }
        }

        Intent BeaconIntent
        {
            get
            {
                return new Intent(Application.Context, typeof(BeaconBackgroundSubscribeService));
            }
        }


        /// <summary>
        /// Get Instance of Main Activity from anywhere within Application
        /// </summary>
        public static MainActivity Instance { get; private set; }

        
        #region OVERRIDE METHODS
        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.Main;
            }
        }        

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            if (Intent.GetBooleanExtra("login", false))
            {
                ServiceContainer.Resolve<ActiveTicketsViewModel>().NeedsUpdate = true;
                ServiceContainer.Resolve<HistoryTicketsViewModel>().NeedsUpdate = true;
                AndHUD.Shared.ShowSuccess(this, "Go Park!", MaskType.Black, TimeSpan.FromSeconds(3));
            }
            if (Intent.GetBooleanExtra("signup", false))
            {
                ServiceContainer.Resolve<ActiveTicketsViewModel>().NeedsUpdate = true;
                ServiceContainer.Resolve<HistoryTicketsViewModel>().NeedsUpdate = true;
                AndHUD.Shared.ShowSuccess(this, "Welcome to Parq", MaskType.Black, TimeSpan.FromSeconds(3));
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            ParqApplication.CurrentActivity = this;
        }        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
                        
            CurrentPlatform.Init();

            //Set Currentactivity Context
            ParqApplication.CurrentActivity = this;

            //Set MainActivity Instance
            MainActivity.Instance = this;           

            //Register HockeyApp CrashManager
            CrashManager.Register(this, Keys.HockeyAppID);

            //Register HockeyApp Metrics Manager
            MetricsManager.Register(this, Application, Keys.HockeyAppID);

            //Register Google Cloud Messaging
            this.RegisterGCM();

            //Get Resources
            mainCoordinator = FindViewById<CoordinatorLayout>(Resource.Id.main_content);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            collapsingToolbarLayout = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationHeader = navigationView.InflateHeaderView(Resource.Layout.nav_header);
            profileNameTV = navigationHeader.FindViewById<TextView>(Resource.Id.nav_name);
            profileEmailTV = navigationHeader.FindViewById<TextView>(Resource.Id.nav_email);
            profilePic = navigationHeader.FindViewById<CircleImageView>(Resource.Id.profile_image);
            navRelativeLayout = navigationHeader.FindViewById<RelativeLayout>(Resource.Id.nav_rel_layout);
            fab = FindViewById<FloatingActionButton>(Resource.Id.main_fab);            
            image = Resource.Drawable.profile_avatar;

            //SupportActionBar Setup
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            SupportActionBar.SetIcon(Resource.Drawable.icon);
                         
            //Get user info           
            connection = Settings.Connection;
            profileName = Settings.ProfileName;
            profileEmail = Settings.ProfileEmail;
            profileUri = Settings.ProfileImage;

            //Set view Text
            profileNameTV.Text = profileName;
            profileEmailTV.Text = profileEmail;            

            //Load Images
            if (connection == "facebook")
            {
                coverURL = Settings.Cover;
                Picasso.With(this)
                       .Load(coverURL)
                       .Into(new PicassoBackgroundManagerTarget(navRelativeLayout));

                PicassoLoadImage.LoadProfilePic(this, profileUri, profilePic, image, true, true);
            }else
            {
                PicassoLoadImage.LoadProfilePic(this, profileUri, profilePic, image);
            }

            //SETUP NOTIFICATION FRAME FOR CHECKOUT
            notificationFrameCheckOut = FindViewById<FrameLayout>(Resource.Id.notifFrame_CheckOut);
            notificationFrameCheckOut.Visibility = ViewStates.Invisible;

            titleCheckout = notificationFrameCheckOut.FindViewById<TextView>(Resource.Id.notifTitle_CheckOut);
            captionCheckout = notificationFrameCheckOut.FindViewById<TextView>(Resource.Id.notifCaption_CheckOut);
            titleCheckout.Typeface = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueLTStd_UltLtCn.otf");

            /*
             *Retain instance of original LayoutParameters and Coordinator Behaviors
             */
            notifLPOriginal = (CoordinatorLayout.LayoutParams)notificationFrameCheckOut.LayoutParameters;
            notifCBOriginal = notifLPOriginal.Behavior;
            
            fabLPOriginal = (CoordinatorLayout.LayoutParams)fab.LayoutParameters;
            fabCBOriginal = fabLPOriginal.Behavior;           

            notifLP = (CoordinatorLayout.LayoutParams)notificationFrameCheckOut.LayoutParameters;
            fabLP = (CoordinatorLayout.LayoutParams)fab.LayoutParameters;

            /* Craft curved motion into FAB
             */
            fb = new FabMoveBehavior(Resource.Id.notifFrame_CheckOut, Resource.Id.fabPlaceholder_CheckOut);
            fabLP.Behavior = fb;
            fab.LayoutParameters = fabLP;

            //Setup NavigationView Click Event Handler
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);

                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_home:
                        ListItemClicked(0);                        
                        collapsingToolbarLayout.SetTitle(GetString(Resource.String.home));
                        SupportActionBar.SetIcon(Resource.Drawable.icon);
                        break;

                    case Resource.Id.nav_history:
                        ListItemClicked(1);                        
                        collapsingToolbarLayout.SetTitle(GetString(Resource.String.history));
                        SupportActionBar.SetIcon(Resource.Drawable.ic_history_black_24dp);
                        break;

                    case Resource.Id.nav_profile:
                        ListItemClicked(2); 
                        break;

                    case Resource.Id.nav_wallet:
                        ListItemClicked(3);                        
                        collapsingToolbarLayout.SetTitle(GetString(Resource.String.wallet));
                        SupportActionBar.SetIcon(Resource.Drawable.ic_wallet);                        
                        break;

                    case Resource.Id.nav_vehicles:
                        ListItemClicked(4);                        
                        collapsingToolbarLayout.SetTitle(GetString(Resource.String.vehicles));
                        SupportActionBar.SetIcon(Resource.Drawable.ic_directions_car_black_24dp);
                        break;

                    case Resource.Id.nav_coupon:
                        ListItemClicked(5);                        
                        collapsingToolbarLayout.SetTitle(GetString(Resource.String.coupon));
                        SupportActionBar.SetIcon(Resource.Drawable.ic_coupon);
                        break;

                    case Resource.Id.nav_share:
                        ListItemClicked(6);                        
                        collapsingToolbarLayout.SetTitle(GetString(Resource.String.share));
                        SupportActionBar.SetIcon(Resource.Drawable.ic_share_black_24dp);
                        break;

                    case Resource.Id.nav_settings:
                        ListItemClicked(7);                        
                        collapsingToolbarLayout.SetTitle(GetString(Resource.String.settings));
                        SupportActionBar.SetIcon(Resource.Drawable.ic_settings);
                        break;

                    case Resource.Id.nav_about:
                        ListItemClicked(8);                        
                        collapsingToolbarLayout.SetTitle(GetString(Resource.String.about));
                        SupportActionBar.SetIcon(Resource.Drawable.ic_about);
                        break;
                }

                drawerLayout.CloseDrawers();
            };    
            
            //if first time you will want to go ahead and click first item.
            if(savedInstanceState == null)
            {
                ListItemClicked(0);                
                drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);                
            }
        }

        protected override async void OnResume()
        {
            base.OnResume();

            //Start with the request flag set to false
            mInProgress = false;
            //Start Google API Client
            buildGoogleApiClient();

            await Task.Run(() => mGoogleApiClient.BlockingConnect());

            var status = await NearbyClass.Messages.GetPermissionStatusAsync(mGoogleApiClient);

            if (status.IsSuccess)
                Subscribe();
            else
                status.StartResolutionForResult(this, ConnectionResult.ResolutionRequired);
        }

        protected override void OnStop()
        {
            base.OnStop();
            NearbyClass.Messages.Unsubscribe(mGoogleApiClient, beaconMessageListener);
            //Subscribe in the background
            NearbyClass.Messages.Subscribe(mGoogleApiClient, BeaconPendingIntent, options).SetResultCallback(this);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.home_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;

                case Resource.Id.menu_locate:
                    return true;

                case Resource.Id.menu_logout:
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

                case Resource.Id.menu_scanner:
                    StartScanQR();
                    return true;

                case Resource.Id.menu_refresh:
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        /**********************
        * ACTIVITY FOR RESULTS
        **********************/
        protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (data != null)
            {
                switch (requestCode)
                {
                    case 101:   //Card.IO 
                        var card = data.GetParcelableExtra(CardIOActivity.ExtraScanResult).JavaCast<CreditCard>();

                        
                        AndHUD.Shared.ShowSuccess(this, string.Format("Card Number: {0}", card.FormattedCardNumber), MaskType.Black, TimeSpan.FromSeconds(3), null, () => AndHUD.Shared.Dismiss(this));

                        break;

                    case 102:   //Scandit barcode number
                        var barcode = data.GetStringExtra("barcode");
                        AzureAPIActiveTicket = new APIActiveTickets();
                        bool result = false;
                        await Task.Run(async () =>
                        {
                            AndHUD.Shared.Show(this, Strings.onYourMarks);
                            var action = await AzureAPIActiveTicket.GateScanned(barcode);                            
                            switch (action)
                            {
                                case 101:
                                    AndHUD.Shared.Show(this, Strings.getSet);
                                    result = await AzureAPIActiveTicket.GateCheckOut(barcode);
                                    if (result)
                                    {
                                        //Animate NotificationFrameCheckout                                        
                                        AndHUD.Shared.Dismiss(this);                                        
                                        RunOnUiThread(() => {                                            
                                            fab.Click += OnFabNotifClick;
                                            nb = new NotificationBehavior();
                                            nb.Dismissed += Nb_Dismissed;
                                            notifLP.Behavior = nb;
                                            notificationFrameCheckOut.LayoutParameters = notifLP;

                                            titleCheckout.Text = "Thank You, Please Visit Us Again";
                                            captionCheckout.Text = Settings.SuccessMessage;

                                            fab.SetImageResource(Resource.Drawable.ic_close_black_24dp);                                                                                      

                                            notificationFrameCheckOut.Visibility = ViewStates.Visible;
                                            float initialX = -(notificationFrameCheckOut.Left + notificationFrameCheckOut.Width + notificationFrameCheckOut.PaddingLeft);
                                            notificationFrameCheckOut.TranslationX = initialX;
                                            ViewCompat.Animate(notificationFrameCheckOut)
                                                      .TranslationX(0)
                                                      .SetDuration(600)
                                                      .SetStartDelay(100)
                                                      .SetInterpolator(new Android.Support.V4.View.Animation.LinearOutSlowInInterpolator())
                                                      .Start();
                                        });
                                    }
                                    else
                                    {
                                        AndHUD.Shared.ShowError(this, Settings.ErrorMessage, MaskType.Black, null, null, () => AndHUD.Shared.Dismiss(this));
                                    }
                                    break;
                                case 102:
                                    AndHUD.Shared.Show(this, Strings.getSet);
                                    result = await AzureAPIActiveTicket.GateCheckIn(barcode);
                                    if (result)
                                    {
                                        //Animate NotificationFrameCheckout                                        
                                        AndHUD.Shared.Dismiss(this);
                                        RunOnUiThread(() => {                                            
                                            fab.Click += OnFabNotifClick;
                                            nb = new NotificationBehavior();
                                            nb.Dismissed += Nb_Dismissed;
                                            notifLP.Behavior = nb;
                                            notificationFrameCheckOut.LayoutParameters = notifLP;

                                            titleCheckout.Text = Settings.SuccessMessage;
                                            captionCheckout.Text = ("Swipe to dismiss");

                                            fab.SetImageResource(Resource.Drawable.ic_close_black_24dp);                                            

                                            notificationFrameCheckOut.Visibility = ViewStates.Visible;
                                            float initialX = -(notificationFrameCheckOut.Left + notificationFrameCheckOut.Width + notificationFrameCheckOut.PaddingLeft);
                                            notificationFrameCheckOut.TranslationX = initialX;
                                            ViewCompat.Animate(notificationFrameCheckOut)
                                                      .TranslationX(0)
                                                      .SetDuration(600)
                                                      .SetStartDelay(100)
                                                      .SetInterpolator(new Android.Support.V4.View.Animation.LinearOutSlowInInterpolator())
                                                      .Start();
                                        });
                                    }
                                    else
                                    {
                                        AndHUD.Shared.ShowError(this, Settings.ErrorMessage, MaskType.Black, null, null, () => AndHUD.Shared.Dismiss(this));
                                    }
                                    break;
                                default:
                                    AndHUD.Shared.ShowError(this, Settings.ErrorMessage, MaskType.Black, null, null, () => AndHUD.Shared.Dismiss(this));
                                    break;
                            }
                            if (result)
                            {
                                fab.Click -= FabClickScandit;
                            }
                            return;
                        });
                        break;
                }
            }
        }
        #endregion

        #region CLICK EVENT HANDLERS
        private void OnFabNotifClick(object sender, EventArgs e)
        {
            fab.Click -= OnFabNotifClick;
            RunOnUiThread(() => {
                notifLPOriginal.Behavior = notifCBOriginal;
                notificationFrameCheckOut.LayoutParameters = notifLPOriginal;
            });
            if (notificationFrameCheckOut.Visibility == ViewStates.Visible)
            {
                notificationFrameCheckOut.Visibility = ViewStates.Invisible;
                notificationFrameCheckOut.TranslationX = 1;
            }
            if (currentFragmentPosition != 0)
            {
                ListItemClicked(0);
                collapsingToolbarLayout.SetTitle(GetString(Resource.String.home));
                SupportActionBar.SetIcon(Resource.Drawable.icon);
            }
            else
            {               
                fab.SetImageResource(Resource.Drawable.ic_barcode_scan);
            }
            fab.Click += FabClickScandit;
        }

        private void Nb_Dismissed(object sender, EventArgs e)
        {
            fab.Click -= OnFabNotifClick;
            RunOnUiThread(() => { 
                notifLPOriginal.Behavior = notifCBOriginal;
                notificationFrameCheckOut.LayoutParameters = notifLPOriginal;                
            });
            if (currentFragmentPosition != 0)
            {
                ListItemClicked(0);
                collapsingToolbarLayout.SetTitle(GetString(Resource.String.home));
                SupportActionBar.SetIcon(Resource.Drawable.icon);
            }else
            {
                fab.SetImageResource(Resource.Drawable.ic_barcode_scan);
            }
            fab.Click += FabClickScandit;
        }

        
        private void FabClickScandit(object sender, EventArgs e)
        {
            StartScanQR();
        }

        private void StartScanQR()
        {
            var intent = new Intent(this, typeof(ScanActivity));
            intent.SetFlags(ActivityFlags.NoHistory);
            StartActivityForResult(intent, 102);
        }

        private void FabClickCardIO(object sender, EventArgs e)
        {
            StartScanCard();
        }

        private void StartScanCard()
        {
            var intent = new Intent(this, typeof(CardIOActivity));
            intent.PutExtra(CardIOActivity.ExtraRequireExpiry, true);
            intent.PutExtra(CardIOActivity.ExtraRequireCvv, true);
            intent.PutExtra(CardIOActivity.ExtraRequirePostalCode, false);
            intent.PutExtra(CardIOActivity.ExtraUseCardioLogo, true);
            intent.PutExtra(CardIOActivity.ExtraRequireCardholderName, true);
            intent.PutExtra(CardIOActivity.ExtraCapturedCardImage, true);
            intent.SetFlags(ActivityFlags.NoHistory);
            StartActivityForResult(intent, 101);
        }  

        private void ListItemClicked (int position)
        {
            fragment = null;
            fragmentTag = "";
            currentFragmentPosition = position;
            switch (position)
            {
                case 0:
                    //fab.Hide();
                    fab.Click += FabClickScandit;
                    fab.SetImageResource(Resource.Drawable.ic_barcode_scan);
                    fragment = new HomeFragment();
                    fragmentTag = "HomeFragment";
                    homeFragment = (HomeFragment)fragment;
                    break;

                case 1:
                    fab.Click += FabHistoryStatsActivity;                    
                    fab.SetImageResource(Resource.Drawable.ic_trending_up_black_24dp);
                    fragment = new HistoryFragment();
                    fragmentTag = "HistoryFragment";
                    break;

                case 2:                                      
                    var intent = new Intent(this, typeof(ProfileActivity));
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.slideright, Resource.Animation.slide2);
                    break;

                case 3:                    
                    fab.Click += FabClickCardIO;
                    fab.SetImageResource(Resource.Drawable.ic_action_content_add);
                    fragment = new WalletFragment();
                    fragmentTag = "WalletFragment";
                    break;

                case 4:                    
                    fab.SetImageResource(Resource.Drawable.ic_action_content_add);
                    fragment = new VehiclesFragment();
                    fragmentTag = "VehiclesFragment";
                    break;

                case 5:                    
                    fab.Click -= FabClickCardIO;
                    fab.Click += FabClickScandit;
                    fab.SetImageResource(Resource.Drawable.ic_barcode_scan);
                    fragment = new CouponFragment();
                    fragmentTag = "CouponFragment";
                    break;

                case 6:                    
                    fragment = new ShareFragment();
                    fragmentTag = "ShareFragment";
                    break;

                case 7:                    
                    fragment = new SettingsFragment();
                    fragmentTag = "SettingsFragment";
                    break;

                case 8:
                    fragment = new AboutFragment();
                    fragmentTag = "AboutFragment";
                    break;
            }

            if(fragment != null)
            {
                SupportFragmentManager.BeginTransaction()
               .Replace(Resource.Id.content_frame, fragment,fragmentTag)
               .Commit();
            }           
        }

        private void FabHistoryStatsActivity(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(HistoryStatsActivity));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.slideright, Resource.Animation.slide2);
        }
        #endregion

        #region PUSH NOTIFICATIONS METHODS

        void RegisterGCM()
        {
            GcmClient.CheckDevice(this);
            GcmClient.CheckManifest(this);

            GcmClient.Register(this, Helpers.Constants.SENDER_ID);
        }

        #endregion

        #region BEACON CLASSES
        public void buildGoogleApiClient()
        {
            //Start a request to listen to BLE Beacon Messages
            mRequestType = RequestType.SubscribeMessages;

            //Test for Google Play Services after setting the request type
            if (!IsGooglePlayServicesAvailable)
            {
                Log.Error(Helpers.Constants.TAG, "Google Play services unavailable");
                return;
            }
            if (mGoogleApiClient != null)
            {
                return;
            }
            

            mGoogleApiClient = new GoogleApiClient.Builder(this)
                .AddApi(NearbyClass.MessagesApi)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .Build();
        }

        private void Subscribe()
        {
            try
            {
                if (mInProgress)
                {
                    return;
                }


                filter = new MessageFilter.Builder()
                    .IncludeNamespacedType("parq-beacon", "string")
                    .Build();

                options = new SubscribeOptions.Builder()
                    .SetStrategy(Strategy.BleOnly)
                    .SetCallback(_callback)
                    .Build();                

                

                //Subscribe in the foreground
                beaconMessageListener = new BeaconMessageListener(this);
                NearbyClass.Messages.Subscribe(mGoogleApiClient, beaconMessageListener, options).SetResultCallback(this);

            }
            catch (System.Exception ex)
            {
                Log.Error(Helpers.Constants.TAG, "Exception while subscribing", ex);
            }
        }

        public void OnConnected(Bundle connectionHint)
        {
            mInProgress = false;
            Log.Debug(Helpers.Constants.TAG, "Connected to API");            
            //Use mRequestType to determine what action to take. Only Subscribe to messages added
            if (mRequestType == RequestType.SubscribeMessages)
            {
                mBeaconRequestIntent = BeaconPendingIntent;                
                //Subscribe();
            }
        }

        public void OnConnectionSuspended(int cause)
        {
            Log.Debug(Helpers.Constants.TAG, "Connection Suspended");
        }


        public void OnConnectionFailed(ConnectionResult result)
        {
            mInProgress = false;
            //If the error has a resolution, start a Google Play services activity to resolve it
            if (result.HasResolution)
            {
                try
                {
                    result.StartResolutionForResult(ParqApplication.CurrentActivity, Helpers.Constants.CONNECTION_FAILURE_RESOLUTION_REQUEST);
                }
                catch (Exception ex)
                {
                    Log.Error(Helpers.Constants.TAG, "Exception while resolving connection error.", ex);
                }
            }
            else
            {
                int errorCode = result.ErrorCode;
                Log.Error(Helpers.Constants.TAG, "Connection to Google Play services failed with error code " + errorCode);
            }
        }

        public void OnDisconnected()
        {
            //Turn of the request flag
            mInProgress = false;
            //Destroy the current nearby messages client
            mGoogleApiClient = null;
        }

        public void OnResult(Java.Lang.Object status)
        {
            bool bp = true;
        }
        #endregion

        #region CLASS SPECIFIC FUNCTIONS/METHODS

        #endregion

    }
}

