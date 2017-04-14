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
using System.Threading.Tasks;
using System.Diagnostics;
using Parq.Helpers;
using Parq.Interfaces;
using Parq.Models;

namespace Parq.ViewModels
{
    public class ActiveTicketViewModel : ViewModelBase
    {
        public ActiveTicketViewModel()
        {
            activeTicketService = ServiceContainer.Resolve<IActiveTicketService>();
        }

        public bool CanNavigate { get; set; }

        private IActiveTicketService activeTicketService;
        private IMessageDialog dialog;

        public ActiveTicketViewModel(IActiveTicketService activeTicketService)
        {
            this.activeTicketService = activeTicketService;
            Title = "New Ticket";
        }

        private ActiveTicket currentTicket;
        public async Task Init(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
                currentTicket = await activeTicketService.GetActiveTicketAsync(id);
            else
                currentTicket = null;
            Init();
        }

        public void Init(ActiveTicket activeTicket)
        {
            currentTicket = activeTicket;
            Init();
        }

        private void Init()
        {
            dialog = ServiceContainer.Resolve<IMessageDialog>();
            CanNavigate = true;
            if (currentTicket == null)
            {                
                Facility = string.Empty;
                CreatedAtTime = DateTime.Now;
                CurrentTime = DateTime.Now;
                Title = "New Ticket";
                return;
            }

            Facility = currentTicket.FacilityName;
            CreatedAtTime = currentTicket.createdAtTime;
            CurrentTime = DateTime.Now;            
        }

        private string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged("Title"); }
        }

        private string facility = string.Empty;
        public string Facility
        {
            get { return facility; }
            set { facility = value; OnPropertyChanged("Facility"); }
        }

        private DateTime createdAtTime = DateTime.Now;
        public DateTime CreatedAtTime
        {
            get { return createdAtTime; }
            set { createdAtTime = value; OnPropertyChanged("createdAtTime"); }
        }

        private DateTime currentTime = DateTime.Now;
        public DateTime CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; OnPropertyChanged("currentTime"); }
        } 

        
        public async Task ExecuteInsertActiveTicketCommand(ActiveTicket ticket)
        {
            if (IsBusy)
                return;

            CanNavigate = false;
            if (currentTicket == null)
                currentTicket = ticket;
                      
            try
            {
                IsBusy = true;
                await activeTicketService.InsertActiveTicketAsync(ticket);
                await activeTicketService.SyncActiveTicketsAsync();
                ServiceContainer.Resolve<ActiveTicketsViewModel>().NeedsUpdate = true;
                CanNavigate = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

