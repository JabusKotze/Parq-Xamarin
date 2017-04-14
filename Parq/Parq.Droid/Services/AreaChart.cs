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
using Android.Views;
using Android.Graphics;

using Parq.Helpers;

using Com.Syncfusion.Charts;
using Com.Syncfusion.Charts.Enums;

namespace Parq.Droid.Services
{
    public class AreaChart
    {
        SfChart chart;

        public SfChart Instance
        {
            get
            {
                return chart;
            }
        }

        public AreaChart(Context context, ObservableArrayList data, string chartTitle, string secondaryAxisLabelFormat, double secondaryMax, int interval)
        {
            chart = new SfChart(context);
            chart.SetBackgroundColor(Color.White);
            chart.Title.Text = chartTitle;
            chart.Title.TextSize = 12;
            chart.Title.TextAlignment = TextAlignment.ViewStart;
            chart.Title.SetTextColor(Color.ParseColor(Colors.PrimaryCardViewSTR));

            var primaryAxis = new CategoryAxis { LabelPlacement = LabelPlacement.BetweenTicks, Interval = (interval-1) };            
            primaryAxis.ShowMajorGridLines = false;
            primaryAxis.EdgeLabelsDrawingMode = EdgeLabelsDrawingMode.Shift;
            primaryAxis.MaximumLabels = 1;         
            chart.PrimaryAxis = primaryAxis;

            var secondaryAxis = new NumericalAxis();            
            secondaryAxis.LabelStyle.LabelFormat = secondaryAxisLabelFormat;
            secondaryAxis.ShowMajorGridLines = false;
            secondaryAxis.ShowMinorGridLines = false;
            //secondaryAxis.Maximum = secondaryMax;
            secondaryAxis.MaximumLabels = 1;            
            secondaryAxis.OpposedPosition = true;
            secondaryAxis.MajorTickStyle.TickSize = 0;
            secondaryAxis.MajorTickStyle.StrokeWidth = 0;
            secondaryAxis.MinorTickStyle.TickSize = 0;
            secondaryAxis.MinorTickStyle.StrokeWidth = 0;
            secondaryAxis.RangePadding = NumericalPadding.Round;
            chart.SecondaryAxis = secondaryAxis;

            var areaSeries = new AreaSeries
            {
                StrokeWidth = 5,
                Color = Color.ParseColor(Colors.PrimarySTR),
                Alpha = 0.5f,
                StrokeColor = Color.ParseColor(Colors.PrimaryDarkSTR),
                DataSource = data,                
            };
            
            areaSeries.DataMarker.MarkerType = DataMarkerType.Ellipse;
            areaSeries.DataMarker.ShowLabel = false;
            areaSeries.DataMarker.ShowMarker = true;
            areaSeries.DataMarker.MarkerColor = Color.ParseColor(Colors.PrimaryDarkSTR);            

            chart.Series.Add(areaSeries);

        }

        public static Color ConvertHexaToColor(uint hex)
        {
            var alpha = (hex & 0xFF000000) >> 24;
            var red = (hex & 0xFF0000) >> 16;
            var green = (hex & 0xFF00) >> 8;
            var blue = (hex & 0xFF);
            return Color.Argb((int)alpha, (int)red, (int)green, (int)blue);
        }
    }
}