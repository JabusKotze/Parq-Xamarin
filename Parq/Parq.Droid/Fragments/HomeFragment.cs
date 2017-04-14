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
#region USINGS
using System.Linq;
using System.Timers;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Parq.Droid.Adapters;
using Parq.ViewModels;
using Parq.Helpers;
using Parq.Droid.Services;

using RecyclerViewAnimators.Adapters;
using RecyclerViewAnimators.Animators;
#endregion


namespace Parq.Droid.Fragments
{
    public class HomeFragment : Fragment
    {
        public static ActiveTicketsViewModel ticketsViewModel;
        private static ActiveTicketAdapter activeTicketAdapter;        
        private RecyclerView mRecyclerView;
        private RecyclerView.LayoutManager mLayoutManager;
        public static TextView totalTickets, totalPrice;
        public BusyIndicator sfBusyIndicater;
        private FrameLayout busyLayoutFrame;

        //Recyclerview Animators
        private static ScaleInAnimationAdapter alphaAdapter;
        //private static ScaleInAnimationAdapter alphaAdapter;
        private static FadeInUpAnimator fadeInUpAnimator;

        //Timer to update the elapsedTime display
        private static readonly long mFrequency = 1000;
        private static readonly int TICK_WHAT = 2;

        //message handler for ticket timer updates
        private messageHandler mHandler;

        /// <summary>
        /// Message Handler to update timers and adapters
        /// </summary>
        class messageHandler : Handler
        {
            public override void HandleMessage(Message message)
            {
                switch (message.What)
                {
                    case 2:
                        isBusy = true;
                        UpdateTotals();
                        alphaAdapter.NotifyDataSetChanged();                        
                        SendMessageDelayed(Message.Obtain(this, TICK_WHAT), mFrequency);
                        isBusy = false;
                        if(LastCount != ticketsViewModel.ActiveTickets.Count())
                        {
                            UpdateTotals(true);
                        }
                        break;
                }
            }
        }

        public static bool isBusy { get; set; }

        public static int LastCount { get; set; }

        public HomeFragment()
        {
            this.RetainInstance = true;
            isBusy = false;
            LastCount = 0;                    
        }


        #region OVERRIDE METHODS
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.Fragment_Home, container, false);

            ticketsViewModel = ServiceContainer.Resolve<ActiveTicketsViewModel>();

            //Find Labels
            totalTickets = rootView.FindViewById<TextView>(Resource.Id.home_total_tickets);
            totalPrice = rootView.FindViewById<TextView>(Resource.Id.home_total_price);

            //Set the adapter
            activeTicketAdapter = new ActiveTicketAdapter(base.Activity, ticketsViewModel);

            activeTicketAdapter.ItemClick += ActiveTicketAdapter_ItemClick;

            //Get the RecyclerView
            mRecyclerView = rootView.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            //Layout manager Setup:
            //mLayoutManager = new WrappingLinearLayoutManager(this.Activity);
            mLayoutManager = new LinearLayoutManager(this.Activity);

            //Plug the layout manager into the recyclerView:
            mRecyclerView.SetLayoutManager(mLayoutManager);
            mRecyclerView.NestedScrollingEnabled = false;
            mRecyclerView.HasFixedSize = false;

            //Plug the adapter into the RecyclerView
            mRecyclerView.SetItemAnimator(new FadeInAnimator());
            alphaAdapter = new ScaleInAnimationAdapter(activeTicketAdapter);
            alphaAdapter.SetFirstOnly(true);
            alphaAdapter.SetDuration(1000);
            alphaAdapter.SetInterpolator(new OvershootInterpolator(.5f));
            //alphaAdapter = new ScaleInAnimationAdapter(alphaAdapter);
            //fadeInUpAnimator = new FadeInUpAnimator(new OvershootInterpolator(.5f));            
            mRecyclerView.SetAdapter(alphaAdapter);

            mHandler = new messageHandler();
            mHandler.SendMessageDelayed(Message.Obtain(mHandler, TICK_WHAT), mFrequency);

            return rootView;
        }

        public async override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            busyLayoutFrame = view.FindViewById<FrameLayout>(Resource.Id.home_frame_busy);

            //Start Busy Indicator
            sfBusyIndicater = new BusyIndicator("", Context);            
            busyLayoutFrame.Visibility = ViewStates.Visible;        
            busyLayoutFrame.AddView(sfBusyIndicater.Instance);

            await ticketsViewModel.ExecuteLoadActiveTicketsCommand();

            //End Busy Indicator            
            sfBusyIndicater.Instance.IsBusy = false;
            busyLayoutFrame.RemoveView(sfBusyIndicater.Instance);
            busyLayoutFrame.Visibility = ViewStates.Gone;

            UpdateTotals(true);

            alphaAdapter.NotifyDataSetChanged();            
        }

        public async override void OnStart()
        {
            base.OnStart();

            ParqApplication.CurrentActivity = this.Activity;

            if (ticketsViewModel.NeedsUpdate)
            {
                await ticketsViewModel.ExecuteLoadActiveTicketsCommand();
                alphaAdapter.NotifyDataSetChanged();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();                     
        }
        #endregion

        #region CLICK EVENTS
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            base.Activity.RunOnUiThread(() => activeTicketAdapter.NotifyDataSetChanged());
        }

        private void ActiveTicketAdapter_ItemClick(object sender, int position)
        {
            int photoNum = position + 1;
            Toast.MakeText(this.Activity, "You Clicked Ticket " + photoNum.ToString(), ToastLength.Short).Show();
        }
        #endregion

        #region CLASS SPECIFIC METHODS/FUNCTIONS
        public static void UpdateTotals(bool forceUpdate = false)
        {
            if (ticketsViewModel.Scanned || forceUpdate)
            {
                if (!isBusy)
                {
                    LastCount = ticketsViewModel.ActiveTickets.Count();
                    totalTickets.Text = ticketsViewModel.ActiveTickets.Count().ToString();
                    totalPrice.Text = ticketsViewModel.TotalPrice;
                    ticketsViewModel.Scanned = false;
                }                
            }
        }
        #endregion
    }
}