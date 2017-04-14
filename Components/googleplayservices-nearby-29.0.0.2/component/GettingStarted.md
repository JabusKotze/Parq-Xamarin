Nearby exposes simple publish and subscribe methods that rely on proximity. Your app publishes a payload that can be received by nearby subscribers. On top of this foundation, you can build a variety of user experiences to share messages and create real-time connections between nearby devices.



Required Android API Levels
===========================

We recommend setting your app's *Target Framework* and *Target Android version* to **Android 5.0 (API Level 21)** or higher in your app project settings.

This Google Play Service SDK's requires a *Target Framework* of at least Android 4.1 (API Level 16) to compile.

You may still set a lower *Minimum Android version* (as low as Android 2.3 - API Level 9) so your app will run on older versions of Android, however you must ensure you do not use any API's at runtime that are not available on the version of Android your app is running on.




Google Developers Console Setup
=================================

Many of the Google Play Services SDK's require that you create an application inside the [Google Developers Console][1].  Visit the [Google Developers Console][1] to create a project for your application.

Once you have created a project for your Android app, enable the necessary APIs in the developer console for the Google Play Services APIs you will be using in your app.



To use Nearby you must enable the *Nearby Messages API* in the Developers Console.



Credentials
-----------

Some Google Play Services APIs require an *API Key* or an *OAuth 2.0 Client ID* (or both) to be setup to allow your app to make authenticated calls against the API.

In the Developers Console, in your app's Project, under the *APIs & auth* section, go to *Credentials*.





### API Key

If the Google Play Services API you are using requires an API Key:

  1. *Add credentials* button and then *API key*
  2. Choose *Android key*
  3. Click *Add package name and fingerprint*
  4. Enter your android app's package name as found in your *AndroidManifest.xml* file
  5. [Find your SHA-1 fingerprints][2]
  6. Enter your SHA-1 fingerprint of your app's debug keystore
  7. Repeat steps 4-6 with the package name and SHA-1 of the keystore file you will be signing your app's Release builds with
  8. Click *Create*
  9. Note the *API key* value you generated

Once you have your API key value, you will need to add this to your *AndroidManifest.xml* as a metadata value either by directly editing the manifest file, or using an assembly level attribute which will generate the value in the manifest file for you.  The metadata key will be different for each Google Play Services API.  For example, if you are adding it for Maps, you could add this assembly level attribute to your project:

```csharp
[assembly: MetaData ("com.google.android.maps.v2.API_KEY", Value="YOUR-API-KEY")]
```


Once you have created your API key, you must add it as a metadata value in your *AndroidManifest.xml* file either manually or by including an assembly level attribute in your app:

```csharp
[assembly: MetaData ("com.google.android.nearby.messages.API_KEY", Value="YOUR-API-KEY")]
```



Samples
=======

You can find a Sample Application within each Google Play Services component.  The sample will demonstrate the necessary configuration and some basic API usages.






Learn More
==========

You can learn more about the various Google Play Services SDKs & APIs by visiting the official [Google APIs for Android][3] documentation


You can learn more about Google Play Services Nearby by visiting the official [Nearby API](https://developers.google.com/nearby/) documentation.



[1]: https://console.developers.google.com/ "Google Developers Console"
[2]: https://developer.xamarin.com/guides/android/deployment,_testing,_and_metrics/MD5_SHA1/ "Finding your SHA-1 Fingerprints"
[3]: https://developers.google.com/android/ "Google APIs for Android"

