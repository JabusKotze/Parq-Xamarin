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
    public class AzureHistoryTicketService : IHistoryTicketService
    {
        private IMobileServiceSyncTable<HistoryTicket> historyTicketTable;

        public DataBase db;

        private static AzureHistoryTicketService instance;

        public MobileServiceClient MobileService { get; set; }

        public AzureHistoryTicketService()
        {
            if (MobileService == null)
            {
                MobileService = AzureAccountService.Instance.client;
            }            
        }

        public static AzureHistoryTicketService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureHistoryTicketService();
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

            //historyTicketTable = MobileService.GetSyncTable<HistoryTicket>();
        }

        public async Task<HistoryTicket> InsertHistoryTicketAsync(HistoryTicket ticket)
        {
            if (string.IsNullOrWhiteSpace(ticket.Id))
                await db.InsertHistoryTicket(ticket);
            else
                await db.InsertHistoryTicket(ticket);
            return ticket;
        }


        public async Task<string> DeleteHistoryTicketAsync(HistoryTicket ticket)
        {
            if (db == null)
                return null;
            await db.DeleteHistoryTicket(ticket);
            return ticket.Id;
        }


        public async Task<IEnumerable<HistoryTicket>> GetHistoryTicketsAsync()
        {
            if (db == null)
                return new List<HistoryTicket>();

            return await db.GetAllHistoryTickets();
        }

        public async Task<HistoryTicket> GetHistoryTicketAsync(string id)
        {
            if (db == null)
                return null;

            var tickets = await db.GetHistoryTicket(id);
            return tickets.Count() == 0 ? null : tickets.ElementAt(0);
        }

        public async Task SyncHistoryTicketsAsync()
        {
            try
            {
                await MobileService.SyncContext.PushAsync();

                await historyTicketTable.PullAsync("allItems", historyTicketTable.CreateQuery());
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                Debug.WriteLine(@"Sync Failed: {0}", ex.Message);
            }
            catch (Exception ex)
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
