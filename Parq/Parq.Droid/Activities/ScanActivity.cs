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
using Android.Content;
using Android.OS;
using Scandit;
using AndroidHUD;
using Parq.Helpers;

namespace Parq.Droid.Activities
{
    [Activity(Label = "ScanActivity")]
    public class ScanActivity : Activity, Scandit.Interfaces.IScanditSDKListener
    {
        private ScanditSDKBarcodePicker picker;        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Setup barcode scanner
            picker = new ScanditSDKBarcodePicker(this, Keys.ScanditAppKey);
            picker.OverlayView.AddListener(this);            

            //Setup Camera switch button
            picker.OverlayView.SetCameraSwitchVisibility(Scandit.Interfaces.ScanditSDKOverlay.CameraSwitchAlways);

            //Show toolbar
            picker.OverlayView.ShowToolBar(true);

            //Set title on toolbar
            picker.OverlayView.SetTitleMessage("Parq - Scan");

            //Camera switch back content description
            picker.OverlayView.SetTextForBarcodeDecodingInProgress("Scanning...");

            //Camera switch back content description


            //Set GUI Style to laser
            picker.OverlayView.SetGuiStyle(Scandit.Interfaces.ScanditSDKOverlay.GuiStyleLaser);

            //Setup Scan Settings
            ScanditSDKScanSettings settings = ScanditSDKScanSettings.DefaultSettings;
            
            //Only allow QR Codes
            settings.EnableSymbology(Scandit.Interfaces.ScanditSDKSymbology.Qr);
            
            //Start Scanning            
            picker.StartScanning(settings);

            //Show Scan Userinterface
            SetContentView(picker); 
            
        }

        public void DidCancel()
        {
            
        }

        public void DidManualSearch(string text)
        {
            AndHUD.Shared.ShowSuccess(this, string.Format("Search Was used: {0}", text), MaskType.Black, TimeSpan.FromSeconds(3), null, () => AndHUD.Shared.Dismiss(this));
        }


        /// <summary>
        /// Return barcode to MainActivity after scanned
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="symbology"></param>
        public void DidScanBarcode(string barcode, string symbology)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("barcode", barcode);
            SetResult(Result.Ok, intent);
            picker.StopScanning();
            Finish();     
            //AndHUD.Shared.ShowSuccess(this, string.Format("Barcode Scanned: {0}, '{1}'",barcode,symbology), MaskType.Black, TimeSpan.FromSeconds(3), null, () => AndHUD.Shared.Dismiss(this));
        }
    }
}