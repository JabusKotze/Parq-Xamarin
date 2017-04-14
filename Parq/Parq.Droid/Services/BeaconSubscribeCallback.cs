using Android.Gms.Nearby.Messages;

namespace Parq.Droid.Services
{
    class BeaconSubscribeCallback : SubscribeCallback
    {
        public override void OnExpired()
        {
            base.OnExpired();
            System.Diagnostics.Debug.WriteLine("Subscribed expired");
        }
    }
}