using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ScanditSDK;


namespace iOSSampleClassic
{
	public partial class ScanditSDKDemoViewController : UIViewController
	{
		private SIBarcodePicker picker;
		private OverlayControllerDelegate scanDelegate;
		public static string appKey = "---- ENTER YOUR APP KEY HERE - SIGN UP AT WWW.SCANDIT.COM ----";

		public ScanditSDKDemoViewController () : base ("ScanditSDKDemoViewController", null)
		{
			if (appKey.Length != 43) {
				UIAlertView alert = new UIAlertView () { 
					Title = "App key not set", Message = "Please set the app key in the ScanditSDKDemoViewController class."
				};
				alert.AddButton ("OK");
				alert.Show ();
			} else {
				// Prepare the picker such that it starts up faster.
				SIBarcodePicker.Prepare (appKey, SICameraFacingDirection.Back);
			}
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		partial void scanButtonClicked (MonoTouch.Foundation.NSObject sender)
		{
			// Setup the barcode scanner
			picker = new ScanditSDKRotatingBarcodePicker (appKey);
			// Make sure that you create a class variable for the delegate to keep a reference to it 
			// otherwise the garbage collector will collect it when low on memory.
			scanDelegate = new OverlayControllerDelegate(picker, this);
			picker.OverlayController.Delegate = scanDelegate;
			picker.OverlayController.ShowToolBar(true);
			picker.OverlayController.ShowSearchBar(true);

			PresentViewController (picker, true, null);

			picker.StartScanning ();
		}
		
		public class OverlayControllerDelegate : SIOverlayControllerDelegate
		{
			private SIBarcodePicker picker;
			private UIViewController presentingViewController;

			public OverlayControllerDelegate(SIBarcodePicker picker, UIViewController presentingViewController) {
				this.picker = picker;
				this.presentingViewController = presentingViewController;
			}

			public override void DidScanBarcode (SIOverlayController overlayController, NSDictionary barcode) {
				Console.WriteLine ("barcode scanned: {0}, '{1}'", barcode["symbology"], barcode["barcode"]);

				// stop the camera
				picker.StopScanning ();

				UIAlertView alert = new UIAlertView () { 
					Title = barcode["symbology"] + " Barcode Detected", Message = "" + barcode["barcode"]
				};
				alert.AddButton("OK");

				alert.Clicked += (object sender, UIButtonEventArgs e) => {
					picker.StartScanning ();
				};

				alert.Show ();
			}

			public override void DidCancel (SIOverlayController overlayController, NSDictionary status) {
				Console.WriteLine ("Cancel was pressed.");
				presentingViewController.DismissViewController (true, null);
			}

			public override void DidManualSearch (SIOverlayController overlayController, string text) {
				Console.WriteLine ("Search was used.");

				// stop the camera
				picker.StopScanning ();

				UIAlertView alert = new UIAlertView () { 
					Title = "User entered barcode", Message = "" + text
				};
				alert.AddButton("OK");

				alert.Clicked += (object sender, UIButtonEventArgs e) => {
					picker.StartScanning ();
				};

				alert.Show ();
			}
		}
	}
}

