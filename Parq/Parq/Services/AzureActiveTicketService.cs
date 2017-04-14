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
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Parq.Models;
using Parq.Interfaces;
using Parq.Services;
using Parq.Data;


namespace Parq.PlatformSpecific
{
    public class AzureActiveTicketService : IActiveTicketService
    {
        private IMobileServiceSyncTable<ActiveTicket> activeTicketTable;

        private DataBase db;

        private static AzureActiveTicketService instance;

        public MobileServiceClient MobileService { get; set; }

        public AzureActiveTicketService()
        {            
            if(MobileService == null)
            {
                MobileService = AzureAccountService.Instance.client;
            }
        }
        
        public static AzureActiveTicketService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureActiveTicketService();
                }

                return instance;
            }
        }

        public async Task Init()
        {
            db = DataBase.adb;
            //string path = "syncstore3.db";
            //var store = new MobileServiceSQLiteStore(path);
            //store.DefineTable<ActiveTicket>();            
            //await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //activeTicketTable = MobileService.GetSyncTable<ActiveTicket>();
        }

        public async Task<ActiveTicket> InsertActiveTicketAsync(ActiveTicket ticket)
        {
            if (string.IsNullOrWhiteSpace(ticket.Id))
                await db.InsertActiveTicket(ticket);
            else
                await db.InsertActiveTicket(ticket);
            return ticket;
        }


        public async Task<string> DeleteActiveTicketAsync(ActiveTicket ticket)
        {
            if (db == null)
                return null;
            await db.DeleteActiveTicket(ticket);
            return ticket.Id;
        }


        public async Task<IEnumerable<ActiveTicket>> GetActiveTicketsAsync()
        {
            if (db == null)
                return new List<ActiveTicket>();
                        
            return await db.GetAllActiveTickets();
        }

        public async Task<ActiveTicket> GetActiveTicketAsync(string id)
        {
            if (db == null)
                return null;

            var tickets = await db.GetActiveTicket(id);
            return tickets.Count() == 0 ? null : tickets.ElementAt(0);
        }

        public async Task SyncActiveTicketsAsync()
        {
            try
            {
                await MobileService.SyncContext.PushAsync();

                await activeTicketTable.PullAsync("allItems", activeTicketTable.CreateQuery());
            }catch(MobileServiceInvalidOperationException ex)
            {
                Debug.WriteLine(@"Sync Failed: {0}", ex.Message);
            }catch(Exception ex)
            {
                Debug.WriteLine(@"Sync Failed: {0}", ex.Message);
            }
        }

        public string UserId
        {
            get
            {
                if (MobileService == null || MobileService.CurrentUser == null)
                    return string.Empty;

                return MobileService.CurrentUser.UserId;
            }
        }
    }
}
