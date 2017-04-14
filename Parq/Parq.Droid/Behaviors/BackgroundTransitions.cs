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
using System.Timers;
using Android.Views;
using Android.Graphics.Drawables;

namespace Parq.Droid.Behaviors
{
    public class BackgroundTransitions
    {
        Timer timer;
        int counter, intervalMS;
        View layout;
        TransitionDrawable transition;

        public BackgroundTransitions(View layout, int intervalMS)
        {
            this.layout = layout;
            this.intervalMS = intervalMS;

            //Set initial background transition
            layout.SetBackgroundResource(Resource.Drawable.transition01);
            transition = (TransitionDrawable)layout.Background;
            transition.CrossFadeEnabled = true;
            transition.StartTransition(10000);

            counter = 1;

            timer = new Timer();
            timer.Interval = intervalMS;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();            
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            int transId;
            int transInterval;

            if((intervalMS-1000) <= 0)
            {
                transInterval = 1000;
            }else
            {
                transInterval = intervalMS - 1000;
            }

            counter++;

            if (counter > 6)
            {
                counter = 1;
            }

            switch (counter)
            {
                case 1:
                    transId = Resource.Drawable.transition01;
                    break;

                case 2:
                    transId = Resource.Drawable.transition02;
                    break;

                case 3:
                    transId = Resource.Drawable.transition03;
                    break;

                case 4:
                    transId = Resource.Drawable.transition04;
                    break;

                case 5:
                    transId = Resource.Drawable.transition05;
                    break;

                case 6:
                    transId = Resource.Drawable.transition06;
                    break;

                default:
                    transId = Resource.Drawable.transition01;
                    break;

            }

            //Set background transition            
            ParqApplication.CurrentActivity.RunOnUiThread(() => {
                layout.SetBackgroundResource(transId);
                transition = (TransitionDrawable)layout.Background;
                transition.CrossFadeEnabled = true;
                transition.StartTransition(transInterval);
            });
        }
    }
}