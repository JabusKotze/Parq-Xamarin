using System;
using System.Collections.Generic;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;

using Gcm.Client;
using WindowsAzure.Messaging;
using Android.Util;
using Android.Support.V4.App;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;

using Parq.Droid.Activities;
using Parq.Helpers;

namespace Parq.Droid.Services
{
    [Service]
    class PushHandlerService : GcmServiceBase
    {
        public static string RegistrationID { get; private set; }

        private NotificationHub Hub { get; set; }

        public PushHandlerService() : base(Helpers.Constants.SENDER_ID)
        {

        }

        public static string RegistrationId { get; set; }

        protected override void OnRegistered(Context context, string registrationId)
        {
            RegistrationID = registrationId;

            this.Hub = new NotificationHub(
                Helpers.Constants.NOTIF_HUB_NAME, 
                Helpers.Constants.DEFAULT_LISTEN_SHARED_ACCESS_SIGNATURE_ENDPOINT, 
                context);

            try
            {
                this.Hub.UnregisterAll(registrationId);
            }
            catch (Exception ex)
            {
                Log.Error(NotificationsBroadcastReceiver.TAG, ex.Message);
            }

            var tags = new List<string>() { Settings.UserId }; // Tags for user related notifications. User can subscribe and unsubscribe from groups            

            try
            {
                var hubRegistration = this.Hub.Register(
                    registrationId, 
                    tags.ToArray());
            }
            catch (Exception ex)
            {
                Log.Error(NotificationsBroadcastReceiver.TAG, ex.Message);
            }
        }

        protected override void OnMessage(Context context, Intent intent)
        {
            var msg = new StringBuilder();

            if(intent != null && intent.Extras != null)
            {
                foreach(var key in intent.Extras.KeySet())
                    msg.AppendLine(key + "=" + intent.Extras.Get(key).ToString());
            }

            string messageText = intent.Extras.GetString("message");
            if (string.IsNullOrWhiteSpace(messageText))
            {
                CreateNotification("Unknown message details", msg.ToString());
            }
            else
            {
                CreateNotification("New hub message!", messageText);
            }
        }

        void CreateNotification(string title, string desc)
        {
            
            //Pass the current Beacon Id to the activity
            Bundle valuesForActivity = new Bundle();
            valuesForActivity.PutString("beaconId", "1234");

            //When the user clicks notification, MainActivity will launch
            var launchIntent = new Intent(this, typeof(SplashActivity));

            //Pass values to Mainactivity
            launchIntent.PutExtras(valuesForActivity);

            //Construct a back stack for cross-task navigation
            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(SplashActivity)));
            stackBuilder.AddNextIntent(launchIntent);

            //launchIntent.SetAction(Intent.ActionMain);
            //launchIntent.AddCategory(Intent.CategoryLauncher);
            //PendingIntent pi = PendingIntent.GetActivity(this, 0,
            //    launchIntent, PendingIntentFlags.OneShot);
            PendingIntent pi = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            //Build the Notification
            NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this)
                .SetAutoCancel(true)                                    //Dismiss from the notif. Area when clicked
                .SetSmallIcon(Resource.Drawable.ic_action_place)        // Display this icon
                .SetContentTitle(title)                                 //Set the title
                .SetContentText(desc)                                   //the message to display
                .SetNumber(1)                                           //Display count in the content info
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(desc))                //Set style
                .SetContentIntent(pi)                                                         //Start Mainactivity when clicked
                .SetDefaults(NotificationCompat.DefaultAll);
                

            //Finally, publish the notification
            NotificationManager notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(2,
                notificationBuilder.Build());

            dialogNotify(title, desc);

        }

        protected void dialogNotify(String title, String message)
        {
            if (MainActivity.Instance != null)
            {
                MainActivity.Instance.RunOnUiThread(() =>
                {
                    AlertDialog.Builder dlg = new AlertDialog.Builder(MainActivity.Instance);
                    AlertDialog alert = dlg.Create();
                    alert.SetTitle(title);
                    alert.SetButton("Ok", delegate
                    {
                        alert.Dismiss();
                    });
                    alert.SetMessage(message);
                    alert.Show();
                });
            }
        }

        protected override void OnError(Context context, string errorId)
        {
            Log.Error(NotificationsBroadcastReceiver.TAG, "GCM Error: " + errorId);
        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {
            Log.Verbose(NotificationsBroadcastReceiver.TAG, "GCM Unregistered: " + registrationId);

            CreateNotification("GCM Unregistered...", "The device has been unregistered!");
        }

        protected override bool OnRecoverableError(Context context, string errorId)
        {
            Log.Warn(NotificationsBroadcastReceiver.TAG, "Recoverable Error: " + errorId);

            return base.OnRecoverableError(context, errorId);
        }

    }
}