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
using System;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

using Parq.Models;
using Parq.ViewModels;
using Parq.Droid.Services;
using Parq.Droid.Helpers;

using Square.Picasso;
using Refractored.Controls;
using Parq.Helpers;
#endregion

namespace Parq.Droid.Adapters
{
    #region VIEWHOLDER
    public class HistoryTicketHolder : RecyclerView.ViewHolder
    {
        public TextView Facility { get; private set; }
        public TextView Timer { get; private set; }
        public TextView Price { get; private set; }
        public TextView CreatedAtTime { get; private set; }
        public LinearLayout Background { get; private set; }
        public CircleImageView ProfilePic { get; private set; }

        public HistoryTicketHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Facility = itemView.FindViewById<TextView>(Resource.Id.his_ticket_facility_name);
            Timer = itemView.FindViewById<TextView>(Resource.Id.his_ticket_timer);
            Price = itemView.FindViewById<TextView>(Resource.Id.his_ticket_list_price);
            CreatedAtTime = itemView.FindViewById<TextView>(Resource.Id.his_ticket_created_at_time);
            Background = itemView.FindViewById<LinearLayout>(Resource.Id.his_ticket_linear_layout);
            ProfilePic = itemView.FindViewById<CircleImageView>(Resource.Id.his_ticket_pro_image);

            itemView.Click += (sender, e) => listener(base.AdapterPosition);
        }
    }
    #endregion

    #region ADAPTER
    public class HistoryTicketAdapter : RecyclerView.Adapter
    {
        private HistoryTicketsViewModel viewModel;
        private Context context;
        private Activity activity;
        private string connection, profileUri;
        private int image;

        public event EventHandler<int> ItemClick;
        private HistoryTicketHolder viewHolder;

        public System.Timers.Timer timer;
        public TimeSpan ticks;

        private StopWatchService m_stopwatchService;

        public ActiveTicket mActiveTickets;
        public HistoryTicketAdapter(Context context, HistoryTicketsViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.context = context;
            this.activity = (Activity)context;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_history_ticket_mod, parent, false);
            viewHolder = new HistoryTicketHolder(itemView, OnClick);

            connection = Settings.Connection;
            profileUri = Settings.ProfileImage;
            image = Resource.Drawable.profile_avatar;

            if (connection == "facebook")
            {
                PicassoLoadImage.LoadProfilePic(activity, profileUri, viewHolder.ProfilePic, image, false, true);
            }
            else
            {
                PicassoLoadImage.LoadProfilePic(activity, profileUri, viewHolder.ProfilePic, image);
            }

            return viewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                viewHolder = holder as HistoryTicketHolder;

                // Set the data in this ViewHolder's CardView 
                // from this position in the history ticket viewModel:
                var ticket = viewModel.HistoryTickets[position];

                viewHolder.Facility.Text = ticket.FacilityName;
                viewHolder.CreatedAtTime.Text = ticket.EntryDateLongDisplay;

                Picasso.With(context)
                       .Load(ticket.FacilityCoverImage)
                       .Placeholder(Resource.Drawable.transparent_circle_background_blk)
                       .Error(Resource.Drawable.transparent_circle_background_blk)
                       .Resize(360, 126)
                       .CenterCrop()
                       .Into(new PicassoBackgroundManagerTargetLinearLayout(viewHolder.Background));

                updateElapsedTime(ticket);

                updateElapsedPrice(ticket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void updateElapsedTime(HistoryTicket ticket)
        {
            if (viewModel.HistoryTickets.Count > 0)
            {
                activity.RunOnUiThread(() => viewHolder.Timer.Text = ticket.formatElapsedTime);
            }
        }

        public void updateElapsedPrice(HistoryTicket ticket)
        {
            if (viewModel.HistoryTickets.Count > 0)
            {
                activity.RunOnUiThread(() => viewHolder.Price.Text = ticket.formatedElapsedPrice);
            }
        }

        // Return the number of history tickets in the viewModel:
        public override int ItemCount
        {
            get
            {
                return viewModel.HistoryTickets.Count;
            }
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
    #endregion
}