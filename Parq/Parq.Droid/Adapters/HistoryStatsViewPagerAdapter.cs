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

using Android.Support.V4.App;

using Parq.Droid.Fragments;

using Java.Lang;

namespace Parq.Droid.Adapters
{
    public class HistoryStatsViewPagerAdapter : FragmentPagerAdapter
    {
        
        public HistoryStatsViewPagerAdapter(FragmentManager fm) : base(fm)
        {            
        }
        public override int Count
        {
            get
            {
                return 2;
            }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0: return GraphFragment.Instance();
                case 1: return GridFragment.Instance();
            }
            return null;
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            switch (position)
            {
                case 0: return new Java.Lang.String("Graph");
                case 1: return new Java.Lang.String("Grid");
            }
            return base.GetPageTitleFormatted(position);
        }
    }
}