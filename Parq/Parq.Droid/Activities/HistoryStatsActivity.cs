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
using Android.OS;
using Android.Views;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;

using Parq.Droid.Adapters;

using Refractored.Controls;

namespace Parq.Droid.Activities
{
    [Activity(Label = "HistoryStatsActivity", NoHistory = true, LaunchMode = LaunchMode.SingleTop, Theme = "@style/ParqTheme")]
    public class HistoryStatsActivity : AppCompatActivity, AppBarLayout.IOnOffsetChangedListener, View.IOnClickListener
    {

        private static readonly int PERCENTAGE_TO_ANIMATE_AVATAR = 20;
        private bool mIsAvatarShown = true;

        private CircleImageView profileAvatar;
        private int mMaxScrollSize;

        TabLayout tabLayout;
        ViewPager viewPager;
        AppBarLayout appBarLayout;
        Android.Support.V7.Widget.Toolbar toolbar;


        protected int LayoutResource
        {
            get
            {
                return Resource.Layout.Activity_History_Stats;
            }
        }       

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(LayoutResource);

            tabLayout = FindViewById<TabLayout>(Resource.Id.history_stats_tabs);
            viewPager = FindViewById<ViewPager>(Resource.Id.history_stats_viewpager);
            appBarLayout = FindViewById<AppBarLayout>(Resource.Id.history_stats_appbar);
            profileAvatar = FindViewById<CircleImageView>(Resource.Id.history_Stats_Circle_image);

            
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.history_stats_toolbar);
            toolbar.SetNavigationOnClickListener(this);

            appBarLayout.AddOnOffsetChangedListener(this);
            mMaxScrollSize = appBarLayout.TotalScrollRange;

            viewPager.Adapter = new HistoryStatsViewPagerAdapter(SupportFragmentManager);
            tabLayout.SetupWithViewPager(viewPager);
        }


        public void OnOffsetChanged(AppBarLayout layout, int verticalOffset)
        {
            if (mMaxScrollSize == 0)
                mMaxScrollSize = appBarLayout.TotalScrollRange;

            int percentage = (Math.Abs(verticalOffset)) * 100 / mMaxScrollSize;

            if (percentage >= PERCENTAGE_TO_ANIMATE_AVATAR && mIsAvatarShown)
            {
                mIsAvatarShown = false;
                profileAvatar.Animate().ScaleY(0).ScaleX(0).SetDuration(200).Start();
            }

            if(percentage <= PERCENTAGE_TO_ANIMATE_AVATAR && !mIsAvatarShown)
            {
                mIsAvatarShown = true;
                profileAvatar.Animate().ScaleY(1).ScaleX(1).Start();
            }
        }

        public void OnClick(View v)
        {
            OnBackPressed();
        }
    }
}