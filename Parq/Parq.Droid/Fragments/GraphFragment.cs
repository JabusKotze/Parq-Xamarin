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
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using Android.Support.V4.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using Com.Syncfusion.Charts;

using Parq.Droid.Services;
using Parq.Data;
using Parq.Models;

namespace Parq.Droid.Fragments
{
    public class GraphFragment : Fragment
    {
        AreaChart sfAreaChartPrice, sfAreaChartCount;
        FrameLayout graphFramePrice, graphFrameCount;
        DataBase adb;
        List<HistoryTicket> tickets;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            adb = DataBase.adb;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.Fragment_Graph, container, false);

            graphFramePrice = rootView.FindViewById<FrameLayout>(Resource.Id.graph_layout_price);
            graphFrameCount = rootView.FindViewById<FrameLayout>(Resource.Id.graph_layout_count);

            return rootView;
        }

        public async override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            await DrawGraphs();         
        }
        public static Fragment Instance()
        {
            return new GraphFragment();
        }

        private async Task DrawGraphs()
        {
            var collectionPrice = new ObservableArrayList();
            var collectionCount = new ObservableArrayList();
            try
            {                
                Dictionary<string, long> hashMapPrice = new Dictionary<string, long>();
                Dictionary<string, int> hashMapCount = new Dictionary<string, int>();

                DateTime firstDayOfWeek;
                DateTime lastDayOfWeek;
                DateTime currentTime = DateTime.Now;
                tickets = await adb.GetAllHistoryTickets().ConfigureAwait(false);
                int thisWeekNumber;
                int thisDateYear, thisDateDay;
                string thisDateMonth, lastDayOfWeekMonth;
                string graphKey = "";
                long hashOut = 0, currentPrice, grandTotalPrice = 0;                

                foreach (var ticket in tickets)
                {
                    if (ticket.createdAtTime >= currentTime.AddDays(-90))
                    {
                        thisWeekNumber = GetIso8601WeekOfYear(ticket.createdAtTime);
                        thisDateMonth = getMonthFormattedText(ticket.createdAtTime.Month);
                        thisDateDay = ticket.createdAtTime.Day;
                        thisDateYear = ticket.createdAtTime.Year;
                        firstDayOfWeek = GetFirstDayOfWeek(ticket.createdAtTime);//FirstDateOfWeek(thisDateYear, thisWeekNumber, CultureInfo.CurrentCulture);
                        lastDayOfWeek = firstDayOfWeek.AddDays(6);
                        lastDayOfWeekMonth = getMonthFormattedText(lastDayOfWeek.Month);                        
                        currentPrice = (long)ticket.Price;
                        graphKey = firstDayOfWeek.Day.ToString() + " - " + lastDayOfWeek.Day.ToString() + " " + lastDayOfWeekMonth;

                        if (hashMapPrice.TryGetValue(graphKey, out hashOut))
                        {
                            hashMapPrice[graphKey] += currentPrice;
                            hashMapCount[graphKey] += 1;
                        }
                        else
                        {  
                            hashMapPrice.Add(graphKey, currentPrice);
                            hashMapCount.Add(graphKey, 1);
                        }

                        grandTotalPrice = grandTotalPrice + currentPrice;
                    }                   
                }

                var maxPrice = 0.0;
                //Populate collection for total price graph
                foreach (var hash in hashMapPrice)
                {
                    if (hash.Value > maxPrice)
                    {
                        maxPrice = hash.Value;
                    }
                    collectionPrice.Add(new ChartDataPoint(hash.Key, hash.Value));
                }

                var maxCount = 0.0;
                //Populate collection for total count graph
                foreach(var hash in hashMapCount)
                {
                    if(hash.Value > maxCount)
                    {
                        maxCount = hash.Value;
                    }
                    collectionCount.Add(new ChartDataPoint(hash.Key, hash.Value));
                }

                sfAreaChartPrice = new AreaChart(this.Context, collectionPrice, "R " +grandTotalPrice.ToString() + " payed in last 90 days", "R ##.##", maxPrice, hashMapCount.Count);
                sfAreaChartCount = new AreaChart(this.Context, collectionCount, tickets.Count.ToString() + " exits in last 90 days", "##.##", maxCount, hashMapCount.Count);
                this.Activity.RunOnUiThread(() =>
                {
                    graphFramePrice.AddView(sfAreaChartPrice.Instance);
                    graphFrameCount.AddView(sfAreaChartCount.Instance);
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }          
        }


        public static int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime FirstDateOfWeek(int year, int weekOfYear, CultureInfo ci)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if (firstWeek <= 1 || firstWeek > 50)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }

        public static DateTime GetFirstDayOfWeek(DateTime date)
        {
            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            while (date.DayOfWeek != firstDayOfWeek)
            {
                date = date.AddDays(-1);
            }

            return date;
        }

        public static string getMonthFormattedText(int month)
        {
            switch (month)
            {
                case 1: return "Jan";
                case 2: return "Feb";
                case 3: return "Mrt";
                case 4: return "Apr";
                case 5: return "May";
                case 6: return "Jun";
                case 7: return "Jul";
                case 8: return "Aug";
                case 9: return "Sep";
                case 10: return "Oct";
                case 11: return "Nov";
                case 12: return "Dec"; 
            }
            return "";
        }
    }
}