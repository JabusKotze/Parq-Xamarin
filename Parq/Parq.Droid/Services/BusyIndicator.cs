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

using Android.Content;
using Android.Graphics;
using Com.Syncfusion.Sfbusyindicator;
using Com.Syncfusion.Sfbusyindicator.Enums;
using Parq.Helpers;

namespace Parq.Droid.Services
{
    public class BusyIndicator
    {
        SfBusyIndicator sfBusyIndicator;
        
        public SfBusyIndicator Instance
        {
            get
            {
                return sfBusyIndicator;
            }
        }

        public BusyIndicator(string Label, Context con)
        {
            sfBusyIndicator = new SfBusyIndicator(con);            
            sfBusyIndicator.IsBusy = true;
            sfBusyIndicator.TextColor = Color.ParseColor(Colors.AccentSTR);            
            sfBusyIndicator.AnimationType = AnimationTypes.DoubleCircle;
            sfBusyIndicator.ViewBoxWidth = 130;
            sfBusyIndicator.ViewBoxHeight = 130;
            sfBusyIndicator.TextSize = 40;
            sfBusyIndicator.Title = Label;
            sfBusyIndicator.SetBackgroundColor(Color.ParseColor(Colors.TransparentSTR));            
        }

        public void StopIndicator()
        {
            sfBusyIndicator.IsBusy = false;
        }

        public void changeTitle(string Label)
        {
            sfBusyIndicator.Title = Label;
        }
    }
}