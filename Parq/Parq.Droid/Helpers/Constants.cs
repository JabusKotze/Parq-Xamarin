using System;

namespace Parq.Droid.Helpers
{
    public static class Constants
    {
        public const String TAG = "ParqAPP";

        // Request code to attempt to resolve Google Play services connection failures.
        public const int CONNECTION_FAILURE_RESOLUTION_REQUEST = 9000;
        // Timeout for making a connection to GoogleApiClient (in milliseconds).
        public const long CONNECTION_TIME_OUT_MS = 100;

        /*
         * PUSH NOTIFICATION IDs
         */
        public const String GCM_API_KEY = "AIzaSyDhZCwlPFlomqm1CrEskM0IMKrtZP4Kjcc";
        //Default Listen Shared Access Signature Endpoint
        public const String DEFAULT_LISTEN_SHARED_ACCESS_SIGNATURE_ENDPOINT = "Endpoint=sb://parqnotificationsnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=GJh2RkUSPCTnyaFcOI2BHEVFqtaMX32ORxnbGl7sjTU=";
        //Default Full Shared Access Signature Endpoint
        public const String DEFAULT_FULL_SHARED_ACCESS_SIGNATURE_ENDPOINT = "Endpoint=sb://parqnotificationsnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=rv29nC9/s2FOcffVX5Q1eGiLuZ66f9fbnuKLqe2a/Y8=";
        //Google Sender ID
        public const String SENDER_ID = "833040369127";
        //Notification Hub Name
        public const String NOTIF_HUB_NAME = "ParqNotificationsHub";

    }
}