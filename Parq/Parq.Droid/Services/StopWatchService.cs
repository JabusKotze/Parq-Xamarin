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
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Parq.Services;

namespace Parq.Droid.Services
{
    public class StopWatchService : Service
    {
        public static readonly String TAG = "StopWatchService";
        public static readonly int NOTIFICATION_ID = 1;
        IBinder m_binder = null;
        public override IBinder OnBind(Intent intent)
        {
            m_binder = new StopWatchServiceBinder(this);
            return m_binder;
        }

        private MyTicketStopwatch m_stopwatch;        
        private NotificationManager m_notificationMgr;
        private Notification m_notification;
        public long ticketElapsedTime = 0;

        //Timer to update the ongoing notification
        public readonly long mFrequency = 200; //milliseconds
        public readonly int TICK_WHAT = 2;

        public StopWatchService(long ticketElapsedTime)
        {
            this.ticketElapsedTime = ticketElapsedTime;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            m_stopwatch = new MyTicketStopwatch(ticketElapsedTime);
            m_notificationMgr = (NotificationManager)GetSystemService(Context.NotificationService);

            createNotification();
        }

        
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }
        
        public void createNotification()
        {
            int icon = Resource.Drawable.ic_refresh_white;
            Java.Lang.ICharSequence tickerText = new Java.Lang.String("Stopwatch");

            long when = System.DateTime.Now.Millisecond;

            m_notification = new Notification(icon, tickerText, when);
            m_notification.Flags |= NotificationFlags.OngoingEvent;
            m_notification.Flags |= NotificationFlags.NoClear;
        }    
        
        public void start(MyTicketStopwatch stopwatch, long ticketElapsedTime)
        {
            if(stopwatch == null)
            {
                stopwatch = new MyTicketStopwatch(ticketElapsedTime);
            }
            m_stopwatch = stopwatch;
            m_stopwatch.start(ticketElapsedTime);
        }   

        public void pause()
        {
            m_stopwatch.pause();
        }

        public void lap()
        {
            m_stopwatch.lap();
        }

        public void reset()
        {
            m_stopwatch.reset();
        }

        public long getElapsedTime()
        {
            return m_stopwatch.getElapsedTime();
        }

        public String getFormattedElapsedTime()
        {
            return formatElapsedTime(getElapsedTime());
        }

        public bool isStopwatchRunning()
        {
            return m_stopwatch.isRunning();
        }

        public String formatElapsedTime(long now)
        {
            long days = 0, hours = 0, minutes = 0, seconds = 0, tenths = 0;
            StringBuilder sb = new StringBuilder();

            if (now < 1000)
            {
                tenths = now / 100;
            }
            else if (now < 60000)
            {
                seconds = now / 1000;
                now -= seconds * 1000;
                tenths = (now / 100);
            }
            else if (now < 3600000)
            {
                hours = now / 3600000;
                now -= hours * 3600000;
                minutes = now / 60000;
                now -= minutes * 60000;
                seconds = now / 1000;
                now -= seconds * 1000;
                tenths = (now / 100);
            }
            else if (now > 3600000)
            {
                days = now / 86400000;
                now -= days * 86400000;
                hours = now / 3600000;
                now -= hours * 3600000;
                minutes = now / 60000;
                now -= minutes * 60000;
                seconds = now / 1000;
                now -= seconds * 1000;
                tenths = (now / 100);
            }


            if (days > 1)
            {
                
                sb.Append(days).Append(" Days\n").Append(formatDigits(hours)).Append(":")
                        .Append(formatDigits(minutes)).Append(":")
                        .Append(formatDigits(seconds));
            }
            else if (days > 0)
            {
                sb.Append(days).Append(" Day\n").Append(formatDigits(hours)).Append(":")
                        .Append(formatDigits(minutes)).Append(":")
                        .Append(formatDigits(seconds));
            }
            else if (hours > 0)
            {
                sb.Append(formatDigits(hours)).Append(":")
                        .Append(formatDigits(minutes)).Append(":")
                        .Append(formatDigits(seconds));
            }
            else
            {
                sb.Append(formatDigits(minutes)).Append(":")
                        .Append(formatDigits(seconds));
            }

            return sb.ToString();
        }

        private String formatDigits(long num)
        {
            return (num < 10) ? "0" + num : num.ToString();
        }

    }
}