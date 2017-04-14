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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Parq.Helpers;
using Parq.Interfaces;
using Parq.Models;
using Parq.Data;

namespace Parq.ViewModels
{
    public class HistoryTicketsViewModel : ViewModelBase
    {
        private IHistoryTicketService historyTicketService;
        private DataBase adb;
        private IMessageDialog messageDialog;

        public HistoryTicketsViewModel()
        {
            historyTicketService = ServiceContainer.Resolve<IHistoryTicketService>();
            adb = DataBase.adb;
            messageDialog = ServiceContainer.Resolve<IMessageDialog>();
            NeedsUpdate = true;
            CheckedOut = false;
            CheckedIn = false;
            Scanned = false;
        }

        /// <summary>
        /// Gets or sets if an update is needed
        /// </summary>
        public bool NeedsUpdate { get; set; }

        public bool CheckedOut { get; set; }       

        public bool CheckedIn { get; set; }       

        public bool Scanned { get; set; }


        /// <summary>
        /// Gets or sets if we have loaded alert
        /// </summary>
        public bool LoadedAlert { get; set; }



        private ObservableCollection<HistoryTicket> historyTickets = new ObservableCollection<HistoryTicket>();

        public ObservableCollection<HistoryTicket> HistoryTickets
        {
            get { return historyTickets; }
            set { historyTickets = value; OnPropertyChanged("HistoryTicket"); }
        }


        private async Task SyncHistoryTickets()
        {
            HistoryTickets.Clear();

            try
            {
                var historyTickets = await adb.UpdateAllHistoryTickets();                             
                foreach (var historyTicket in historyTickets)
                {
                    HistoryTickets.Add(historyTicket);
                }
                HistoryTickets.Sort(x => x.EntryTime, false);
                NeedsUpdate = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to query and gather history Tickets");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task UpdateHistoryTickets()
        {
            HistoryTickets.Clear();

            try
            {
                var historyTickets = await adb.GetAllHistoryTickets().ConfigureAwait(continueOnCapturedContext: false);

                foreach (var historyTicket in historyTickets)
                {
                    HistoryTickets.Add(historyTicket);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to update history Tickets");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private RelayCommand loadHistoryTicketsCommand;

        public ICommand LoadHistoryTicketsCommand
        {
            get { return loadHistoryTicketsCommand ?? (loadHistoryTicketsCommand = new RelayCommand(async () => await ExecuteLoadHistoryTicketsCommand())); }
        }

        public async Task ExecuteLoadHistoryTicketsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await SyncHistoryTickets();
        }

        private RelayCommand<HistoryTicket> deleteHistoryTicketsCommand;

        public ICommand DeleteHistoryTicketsCommand
        {
            get { return deleteHistoryTicketsCommand ?? (deleteHistoryTicketsCommand = new RelayCommand<HistoryTicket>(async (item) => await ExecuteDeleteHistoryTicketCommand(item))); }
        }

        public async Task ExecuteDeleteHistoryTicketCommand(HistoryTicket ticket)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                await adb.DeleteHistoryTicket(ticket).ConfigureAwait(continueOnCapturedContext: false);
                HistoryTickets.Remove(HistoryTickets.FirstOrDefault(ex => ex.Id == ticket.Id));                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to delete history ticket");
            }
            finally
            {
                IsBusy = false;
            }
        }


        private RelayCommand<HistoryTicket> insertHistoryTicketsCommand;

        public ICommand InsertHistoryTicketsCommand
        {
            get { return insertHistoryTicketsCommand ?? (insertHistoryTicketsCommand = new RelayCommand<HistoryTicket>(async (item) => await ExecuteInsertHistoryTicketCommand(item))); }
        }

        public async Task ExecuteInsertHistoryTicketCommand(HistoryTicket ticket)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                await adb.InsertHistoryTicket(ticket).ConfigureAwait(continueOnCapturedContext: false);
                HistoryTickets.Add(ticket);
                HistoryTickets.Sort(x => x.EntryTime,false);                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to insert history ticket");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public String TotalPrice
        {
            get
            {
                double total = 0;
                StringBuilder sb = new StringBuilder();
                foreach (var ticket in HistoryTickets)
                {
                    total = total + ticket.Price;
                }

                total = Math.Truncate(total * 100) / 100;
                sb.Append("R ").Append(string.Format("{0:N2}",total));

                return sb.ToString();
            }
        }
    }
}