using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Nearby;
using Android.Gms.Nearby.Messages;
using Android.Support.V4.App;
using Android.OS;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;

using Parq.Droid.Activities;

[assembly: MetaData("com.google.android.gms.nearby.connection.SERVICE_ID", Value = "@string/google_api_key")]

namespace Parq.Droid.Services
{
    [Service]
    public class BeaconBackgroundSubscribeService : IntentService
                            
    {
        private const int MESSAGES_NOTIFICATION_ID = 1;

        private readonly BackgroundMessageListener _listener;
                      
        public BeaconBackgroundSubscribeService() : base(typeof(BeaconBackgroundSubscribeService).Name)
        {
            _listener = new BackgroundMessageListener(this);
        }

        public override void OnCreate()
        {
            base.OnCreate();                    
        }

        public class BackgroundMessageListener : MessageListener
        {
            readonly BeaconBackgroundSubscribeService _intentService;

            public BackgroundMessageListener(BeaconBackgroundSubscribeService intentService)
            {
                _intentService = intentService;
            }

            public override void OnFound(Android.Gms.Nearby.Messages.Message message)
            {
                var contents = Encoding.UTF8.GetString(message.GetContent());
                _intentService.UpdateNotification("Found Message", $"Namespace - '{message.Namespace}', Content - '{contents}'.");
            }

            public override void OnLost(Android.Gms.Nearby.Messages.Message message)
            {
                var contents = Encoding.UTF8.GetString(message.GetContent());
                _intentService.UpdateNotification("Lost Message", $"Namespace - '{message.Namespace}', Content - '{contents}'.");
            }
        }


        /// <summary>
        /// Handles Incoming intents
        /// </summary>
        /// <param name="intent">The intent sent by Nearby Services. This Intent is provided to Nearby Services (inside a PendingIntent)</param>
        protected override void OnHandleIntent(Intent intent)
        {
            if (intent != null)                            
                NearbyClass.Messages.HandleIntent(intent, _listener);            
        }


        #region code to send notification

        private void UpdateNotification(string contentTitle, string contentText)
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
                .SetAutoCancel(true)                                                            //Dismiss from the notif. Area when clicked
                .SetSmallIcon(Resource.Drawable.ic_action_place)                                // Display this icon
                .SetContentTitle(contentTitle)                                                  //Set the title
                .SetContentText(contentText)                                                    //the message to display
                .SetNumber(1)                                                                   //Display count in the content info
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(contentText))           //Set style
                .SetContentIntent(pi)                                                           //Start Mainactivity when clicked
                .SetDefaults(NotificationCompat.DefaultAll)
                .SetVisibility(NotificationCompat.VisibilityPrivate);                           //Set visible in private mode


            //Finally, publish the notification
            NotificationManager notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(MESSAGES_NOTIFICATION_ID,
                notificationBuilder.Build());
        }

        #endregion
    }
}