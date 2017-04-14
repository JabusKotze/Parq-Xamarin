using System.Text;

using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Nearby.Messages;
using NotificationCompat = Android.Support.V4.App.NotificationCompat;

using Parq.Droid.Activities;

namespace Parq.Droid.Services
{

    public class BeaconMessageListener : MessageListener
    {
        //Unique IDs for our notifications
        private static readonly int FoundBeaconNotificationId = 1000;
        private readonly MainActivity _activity;

        public BeaconMessageListener(MainActivity activity){
            _activity = activity;
        }                

        public override void OnFound(Android.Gms.Nearby.Messages.Message message)
        {
            var contents = Encoding.UTF8.GetString(message.GetContent());
            //Log message results
            Log.Debug(Helpers.Constants.TAG, "Message Found: " + message);
            Log.Debug(Helpers.Constants.TAG, "Message String: " + message.ToString());
            Log.Debug(Helpers.Constants.TAG, "Message namespaced type: " + message.Namespace + " /" + message.Type);

            //Build notification
            NotificationCompat.Builder builder = new NotificationCompat.Builder(ParqApplication.CurrentActivity)
                .SetAutoCancel(true)
                .SetContentTitle("Beacon Found")
                .SetSmallIcon(Resource.Drawable.ic_action_place)
                .SetContentText("Beacon was found")
                .SetDefaults(NotificationCompat.DefaultAll);

            //Finally, publish the notification:
            NotificationManager notificationManager = (NotificationManager)ParqApplication.CurrentActivity.GetSystemService(Context.NotificationService);
            notificationManager.Notify(FoundBeaconNotificationId, builder.Build());
                       
        }

        public override void OnLost(Android.Gms.Nearby.Messages.Message message)
        {
            var contents = Encoding.UTF8.GetString(message.GetContent());
            //Log message results
            Log.Debug(Helpers.Constants.TAG, "Message Found: " + message);
            Log.Debug(Helpers.Constants.TAG, "Message String: " + message.ToString());
            Log.Debug(Helpers.Constants.TAG, "Message namespaced type: " + message.Namespace + " /" + message.Type);
        }        
    }
}