using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Scandit;
using Scandit.Interfaces;
using ScanditBarcodePicker.Android;
using ScanditBarcodePicker.Android.Recognition;

namespace XamarinScanditSDKDemoAndroid
{
	[Activity (Label = "ScanActivity")]			
	public class ScanActivity : Activity, IOnScanListener, IDialogInterfaceOnCancelListener
	{
		private BarcodePicker picker;
		public static string appKey = "---- ENTER YOUR APP KEY HERE - SIGN UP AT WWW.SCANDIT.COM ----";		

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			RequestWindowFeature (WindowFeatures.NoTitle);
			Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

			// Set the app key before instantiating the picker.
			ScanditLicense.AppKey = appKey;
			
			// The scanning behavior of the barcode picker is configured through scan
			// settings. We start with empty scan settings and enable a very generous
			// set of symbologies. In your own apps, only enable the symbologies you
			// actually need.
			ScanSettings settings = ScanSettings.Create ();
			int[] symbologiesToEnable = new int[] {
				Barcode.SymbologyEan13,
				Barcode.SymbologyEan8,
				Barcode.SymbologyUpca,
				Barcode.SymbologyDataMatrix,
				Barcode.SymbologyQr,
				Barcode.SymbologyCode39,
				Barcode.SymbologyCode128,
				Barcode.SymbologyInterleaved2Of5,
				Barcode.SymbologyUpce
			};
			
			for (int sym = 0; sym < symbologiesToEnable.Length; sym++) {
				settings.SetSymbologyEnabled (symbologiesToEnable[sym], true);
			}	
			
			// Some 1d barcode symbologies allow you to encode variable-length data. By default, the
			// Scandit BarcodeScanner SDK only scans barcodes in a certain length range. If your
			// application requires scanning of one of these symbologies, and the length is falling
			// outside the default range, you may need to adjust the "active symbol counts" for this
			// symbology. This is shown in the following few lines of code.

			SymbologySettings symSettings = settings.GetSymbologySettings(Barcode.SymbologyCode128);
			short[] activeSymbolCounts = new short[] {
				7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20
			};
			symSettings.SetActiveSymbolCounts(activeSymbolCounts);
			// For details on defaults and how to calculate the symbol counts for each symbology, take
			// a look at http://docs.scandit.com/stable/c_api/symbologies.html.

			picker = new BarcodePicker (this, settings);

			// Set listener for the scan event.
			picker.SetOnScanListener (this);
			
			// Show the scan user interface
			SetContentView (picker);
		}

		public void DidScan(IScanSession session) 
		{
			if (session.NewlyRecognizedCodes.Count > 0) {
				Barcode code = session.NewlyRecognizedCodes [0];
				Console.WriteLine ("barcode scanned: {0}, '{1}'", code.SymbologyName, code.Data);

				// Call GC.Collect() before stopping the scanner as the garbage collector for some reason does not 
				// collect objects without references asap but waits for a long time until finally collecting them.
				GC.Collect ();

				// Stop the scanner directly on the session.
				session.StopScanning ();

				// If you want to edit something in the view hierarchy make sure to run it on the UI thread.
				RunOnUiThread (() => {
					AlertDialog alert = new AlertDialog.Builder (this)
						.SetTitle (code.SymbologyName + " Barcode Detected")
						.SetMessage (code.Data)
						.SetPositiveButton("OK", delegate {
							picker.StartScanning ();
						})
						.SetOnCancelListener(this)
						.Create ();

					alert.Show ();
				});
			}
		}

		public void OnCancel(IDialogInterface dialog) {
			picker.StartScanning ();
		}

		protected override void OnResume () 
		{
			picker.StartScanning ();
			base.OnResume ();
		}

		protected override void OnPause () 
		{
			// Call GC.Collect() before stopping the scanner as the garbage collector for some reason does not 
			// collect objects without references asap but waits for a long time until finally collecting them.
			GC.Collect ();
			picker.StopScanning ();
			base.OnPause ();
		}

		public override void OnBackPressed () 
		{
			base.OnBackPressed ();
			Finish ();
		}
	}
}

