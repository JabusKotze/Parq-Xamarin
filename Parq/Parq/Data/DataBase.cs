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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using Parq.Models;
using Parq.Helpers;
using Refractored.Xam.Settings;
using Parq.DataLayer;
using Parq.ViewModels;
using Parq.Services;

namespace Parq.Data
{
    public class DataBase 
    {
        private readonly string dbpath;
        private readonly SQLiteAsyncConnection conn;        

        public string StatusMessage { get; set; }

        static DataBase database;

        public static DataBase adb
        {
            get
            {
                var path = Settings.DataBasePath; 
                return database == null ? (database = new DataBase(path)) : database;
            }
        }

        public DataBase (string dbpath)
        {
            this.dbpath = dbpath;
            
            if (conn == null)
            {
                conn = new SQLiteAsyncConnection(dbpath);                
                conn.CreateTableAsync<ActiveTicket>().Wait();
                conn.CreateTableAsync<HistoryTicket>().Wait();
            }            
        }

        #region ACTIVE TICKET TABLE
        public async Task<int> TableCountActiveTickets()
        {
            return await conn.Table<ActiveTicket>().CountAsync();
        }

       
        public async Task<bool> InsertActiveTicket(ActiveTicket ticket)
        {
            bool result = false;
            List<ActiveTicket> tickets = new List<ActiveTicket>();
            tickets.Add(ticket);
            try
            {
                if (ticket == null)
                {
                    result = false;
                    StatusMessage = string.Format("No Ticket");
                }
                else
                {
                    await conn.InsertAsync(ticket).ContinueWith(t => {                    
                        StatusMessage = string.Format("Welcome to: {0}", ticket.FacilityName);
                        result = true;
                    });
                }
            }
            catch(Exception ex)
            {
                StatusMessage = string.Format("Failed to Add Ticket. Error: {0}", ex.Message);
            }

            return result;
        }

        public async Task<bool> DeleteActiveTicket(ActiveTicket ticket)
        {
            bool result = false;
            try
            {
                if (ticket == null)
                {
                    result = false;
                    StatusMessage = string.Format("No Ticket");
                }
                else
                {
                    await conn.DeleteAsync(ticket).ContinueWith(t => { 
                        StatusMessage = string.Format("Welcome to: {0}", ticket.FacilityName);
                        result = true;
                    });
                }
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to Add Ticket. Error: {0}", ex.Message);
            }

            return result;
        }

        public Task<List<ActiveTicket>> GetActiveTicket(string id)
        {
            return conn.Table<ActiveTicket>().Where(s => s.Id == id).ToListAsync();
        }

        public Task<List<ActiveTicket>> GetAllActiveTickets()
        {            
                return conn.Table<ActiveTicket>().ToListAsync();                        
        }

        public async Task<bool> ClearActiveTickets()
        {
            int count;
            List<ActiveTicket> tickets;
            bool result = false;
            try
            {
                tickets = await GetAllActiveTickets().ConfigureAwait(continueOnCapturedContext: false);
                count = tickets.Count();
                if(count > 0)
                {
                    foreach(var ticket in tickets)
                    {
                        await conn.DeleteAsync(ticket).ConfigureAwait(continueOnCapturedContext:false);
                    }
                }
                result = true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);                
            }

            return result;         
        }

        public async Task<List<ActiveTicket>> UpdateAllActiveTickets()
        {
            List<ActiveTicket> activeTickets;
            IEnumerable<ActiveTicket> tickets;
            if (await ConnectivityService.IsConnected())  //await ConnectivityService.IsConnected()
            {
                try
                {
                    var ApiActiveTickets = new APIActiveTickets();
                    tickets = await ApiActiveTickets.GetAllActiveTickets().ConfigureAwait(continueOnCapturedContext: false);
                    activeTickets = await Task.Run(() => tickets.ToList()).ConfigureAwait(continueOnCapturedContext: false);
                    var clear = await ClearActiveTickets().ConfigureAwait(continueOnCapturedContext: false);
                    if (clear)
                    {
                        var i = await conn.InsertAllAsync(tickets).ConfigureAwait(continueOnCapturedContext: false);
                        ServiceContainer.Resolve<ActiveTicketsViewModel>().NeedsUpdate = false;
                        activeTickets = await GetAllActiveTickets().ConfigureAwait(continueOnCapturedContext: false);
                    }
                    else
                    {
                        activeTickets = await GetAllActiveTickets().ConfigureAwait(continueOnCapturedContext: false);
                    }
                }
                catch(Exception ex)
                {
                    activeTickets = await conn.Table<ActiveTicket>().ToListAsync();                    
                }
            }else
            {
                activeTickets = await conn.Table<ActiveTicket>().ToListAsync();                
            }

            var count = await conn.Table<ActiveTicket>().CountAsync();

            return activeTickets;
        }

        public async Task<bool> InsertAllActiveTickets(IEnumerable<ActiveTicket> tickets)
        {
            bool result = false;
            try
            {
                await conn.InsertAllAsync(tickets);                
                result = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Could not insert tickets: {0}",ex.Message));
            }

            return result;
        }
        #endregion

        #region HISTORY TICKET TABLE
        public async Task<int> TableCountHistoryTickets()
        {
            return await conn.Table<HistoryTicket>().CountAsync();
        }

        public async Task<bool> InsertHistoryTicket(HistoryTicket ticket)
        {
            bool result = false;
            List<HistoryTicket> tickets = new List<HistoryTicket>();
            tickets.Add(ticket);
            try
            {
                if (ticket == null)
                {
                    result = false;
                    StatusMessage = string.Format("No Ticket");
                }
                else
                {
                    await conn.InsertAsync(ticket).ContinueWith(t => {
                        StatusMessage = string.Format("Welcome to: {0}", ticket.FacilityName);
                        result = true;
                    });
                }
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to Add Ticket. Error: {0}", ex.Message);
            }

            return result;
        }

        public async Task<bool> DeleteHistoryTicket(HistoryTicket ticket)
        {
            bool result = false;
            try
            {
                if (ticket == null)
                {
                    result = false;
                    StatusMessage = string.Format("No Ticket to delete");
                }
                else
                {
                    await conn.DeleteAsync(ticket).ContinueWith(t => {
                        StatusMessage = string.Format("Deleted ticket: {0}", ticket.Id);
                        result = true;
                    });
                }
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to Delete History Ticket. Error: {0}", ex.Message);
            }

            return result;
        }

        public Task<List<HistoryTicket>> GetHistoryTicket(string id)
        {
            return conn.Table<HistoryTicket>().Where(s => s.Id == id).ToListAsync();
        }

        public Task<List<HistoryTicket>> GetAllHistoryTickets()
        {
            return conn.Table<HistoryTicket>().ToListAsync();
        }

        public async Task<bool> ClearHistoryTickets()
        {
            int count;
            List<HistoryTicket> tickets;
            bool result = false;
            try
            {
                tickets = await GetAllHistoryTickets().ConfigureAwait(continueOnCapturedContext: false);
                count = tickets.Count();
                if (count > 0)
                {
                    foreach (var ticket in tickets)
                    {
                        await conn.DeleteAsync(ticket).ConfigureAwait(continueOnCapturedContext: false);
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return result;
        }

        public async Task<List<HistoryTicket>> UpdateAllHistoryTickets()
        {
            List<HistoryTicket> historyTickets;
            IEnumerable<HistoryTicket> tickets;
            if (await ConnectivityService.IsConnected() && ServiceContainer.Resolve<HistoryTicketsViewModel>().NeedsUpdate)  //await ConnectivityService.IsConnected()
            {
                try
                {
                    var ApiHistoryTickets = new APIHistoryTickets();
                    tickets = await ApiHistoryTickets.GetAllHistoryTickets().ConfigureAwait(continueOnCapturedContext: false);
                    historyTickets = await Task.Run(() => tickets.ToList()).ConfigureAwait(continueOnCapturedContext: false);
                    var clear = await ClearHistoryTickets().ConfigureAwait(continueOnCapturedContext: false);
                    if (clear)
                    {
                        var i = await conn.InsertAllAsync(tickets).ConfigureAwait(continueOnCapturedContext: false);
                        ServiceContainer.Resolve<HistoryTicketsViewModel>().NeedsUpdate = false;
                        historyTickets = await GetAllHistoryTickets().ConfigureAwait(continueOnCapturedContext: false);
                    }
                    else
                    {
                        historyTickets = await GetAllHistoryTickets().ConfigureAwait(continueOnCapturedContext: false);
                    }
                }
                catch (Exception ex)
                {
                    historyTickets = await conn.Table<HistoryTicket>().ToListAsync();
                }
            }
            else
            {
                historyTickets = await conn.Table<HistoryTicket>().ToListAsync();
            }

            var count = await conn.Table<HistoryTicket>().CountAsync();

            return historyTickets;
        }

        public async Task<bool> InsertAllHistoryTickets(IEnumerable<HistoryTicket> tickets)
        {
            bool result = false;
            try
            {
                await conn.InsertAllAsync(tickets);
                result = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Could not insert tickets: {0}", ex.Message));
            }

            return result;
        }
        #endregion


        #region GENERAL FUNCTIONS
        public async Task clearTables()
        {
            await conn.DropTableAsync<ActiveTicket>().ConfigureAwait(continueOnCapturedContext: false);
            await conn.DropTableAsync<HistoryTicket>().ConfigureAwait(continueOnCapturedContext: false);
        }
        #endregion

    }
}
