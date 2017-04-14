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
    public class ActiveTicketsViewModel : ViewModelBase
    {
        private IActiveTicketService activeTicketService;
        private DataBase adb;
        private IMessageDialog messageDialog;

        public ActiveTicketsViewModel()
        {
            activeTicketService = ServiceContainer.Resolve<IActiveTicketService>();
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

        public ActiveTicket CheckedOutTicket { get; set; }

        public bool CheckedIn { get; set; }

        public ActiveTicket CheckedInTicket { get; set; }

        public bool Scanned { get; set; }


        /// <summary>
        /// Gets or sets if we have loaded alert
        /// </summary>
        public bool LoadedAlert { get; set; }



        private ObservableCollection<ActiveTicket> activeTickets = new ObservableCollection<ActiveTicket>();

        public ObservableCollection<ActiveTicket> ActiveTickets
        {
            get { return activeTickets; }
            set { activeTickets = value; OnPropertyChanged("ActiveTicket"); }
        }


        private async Task SyncActiveTickets()
        {
            ActiveTickets.Clear();            
            
            try
            {
                var activeTickets = await adb.UpdateAllActiveTickets();
                var orderedByDateTickets = activeTickets.OrderByDescending(x => x.createdAtTime);
                foreach (var activeTicket in activeTickets)
                {
                    ActiveTickets.Add(activeTicket);                    
                }
                ActiveTickets.Sort(x => x.createdAtTime, false);
                NeedsUpdate = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to query and gather active Tickets");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task UpdateActiveTickets()
        {
            ActiveTickets.Clear();

            try
            {
                var activeTickets = await adb.GetAllActiveTickets().ConfigureAwait(continueOnCapturedContext: false);

                foreach (var activeTicket in activeTickets)
                {
                    ActiveTickets.Add(activeTicket);
                }
                ActiveTickets.Sort(x => x.createdAtTime, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to update active Tickets");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private RelayCommand loadActiveTicketsCommand;

        public ICommand LoadActiveTicketsCommand
        {
            get { return loadActiveTicketsCommand ?? (loadActiveTicketsCommand = new RelayCommand(async () => await ExecuteLoadActiveTicketsCommand())); }
        }

        public async Task ExecuteLoadActiveTicketsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await SyncActiveTickets();
        }

        private RelayCommand<ActiveTicket> deleteActiveTicketsCommand;

        public ICommand DeleteActiveTicketsCommand
        {
            get { return deleteActiveTicketsCommand ?? (deleteActiveTicketsCommand = new RelayCommand<ActiveTicket>(async (item) => await ExecuteDeleteActiveTicketCommand(item))); }
        }

        public async Task ExecuteDeleteActiveTicketCommand(ActiveTicket ticket)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                await adb.DeleteActiveTicket(ticket).ConfigureAwait(continueOnCapturedContext:false);
                ActiveTickets.Remove(ActiveTickets.FirstOrDefault(ex => ex.Id == ticket.Id));                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to delete active ticket");
            }
            finally
            {
                IsBusy = false;
            }
        }


        private RelayCommand<ActiveTicket> insertActiveTicketsCommand;

        public ICommand InsertActiveTicketsCommand
        {
            get { return insertActiveTicketsCommand ?? (insertActiveTicketsCommand = new RelayCommand<ActiveTicket>(async (item) => await ExecuteInsertActiveTicketCommand(item))); }
        }

        public async Task ExecuteInsertActiveTicketCommand(ActiveTicket ticket)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                await adb.InsertActiveTicket(ticket).ConfigureAwait(continueOnCapturedContext:false);
                ActiveTickets.Add(ticket);
                ActiveTickets.Sort(x => x.createdAtTime, false);              
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to insert active ticket");
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
                foreach (var ticket in ActiveTickets)
                {
                    total = total + ticket.ElapsedPrice;
                }

                total = Math.Truncate(total * 100) / 100;
                sb.Append("R ").Append(string.Format("{0:N2}", total));

                return sb.ToString();
            }       
        }
    }
}