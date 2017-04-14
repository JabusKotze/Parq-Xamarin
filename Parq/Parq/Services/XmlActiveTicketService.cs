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
using System.Threading.Tasks;
using Parq.Interfaces;
using Parq.Models;
using Newtonsoft.Json;
using PCLStorage;

namespace Parq.Services
{
    public class XmlActiveTicketService : IActiveTicketService
    {

        private readonly IMessageDialog dialog;

        public XmlActiveTicketService()
        {
        }


        public static Task<T> DeserializeObjectAsync<T>(string value)
        {
            return Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(value));
        }

        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        List<ActiveTicket> ActiveTickets = new List<ActiveTicket>();

        public Task<ActiveTicket> GetActiveTicketAsync(string id)
        {
            return Task.Run(() => ActiveTickets.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<ActiveTicket>> GetActiveTicketsAsync()
        {
            var rootFolder = FileSystem.Current.LocalStorage;

            var folder = await rootFolder.CreateFolderAsync(Folder,
                CreationCollisionOption.OpenIfExists);

            var file = await folder.CreateFileAsync(File,
                CreationCollisionOption.OpenIfExists);

            var json = await file.ReadAllTextAsync();

            if (!string.IsNullOrWhiteSpace(json))
                ActiveTickets = DeserializeObject<List<ActiveTicket>>(json);

            if (ActiveTickets.Count == 0)
            {
                var activeTicket = new ActiveTicket
                {
                    FacilityName = "Test",                    
                };
                await InsertActiveTicketAsync(activeTicket);
                ActiveTickets.Add(activeTicket); ;
            }

            return ActiveTickets;
        }

        public Task SyncActiveTicketsAsync()
        {
            return Task.Run(() => { });
        }

        public async Task<ActiveTicket> InsertActiveTicketAsync(ActiveTicket activeTicket)
        {
            if (string.IsNullOrWhiteSpace(activeTicket.Id))
            {
                activeTicket.Id = DateTime.Now.ToString();
                ActiveTickets.Add(activeTicket);
            }
            else
            {
                var found = ActiveTickets.FirstOrDefault(e => e.Id == activeTicket.Id);
                if (found != null)
                    found.SyncProperties(activeTicket);
            }
            await Save();
            return activeTicket;
        }

        public async Task<string> DeleteActiveTicketAsync(ActiveTicket ticket)
        {
            var id = ticket.Id;
            ActiveTickets.Remove(ticket);
            await Save();
            return id;
        }

        private string Folder = "ActiveTickets";
        private string File = "activeTickets.json";

        private async Task Save()
        {
            var rootFolder = FileSystem.Current.LocalStorage;

            var folder = await rootFolder.CreateFolderAsync(Folder,
                CreationCollisionOption.OpenIfExists);

            var file = await folder.CreateFileAsync(File,
                CreationCollisionOption.ReplaceExisting);

            await file.WriteAllTextAsync(JsonConvert.SerializeObject(ActiveTickets));
        }

        public string UserId
        {
            get { return string.Empty; }
        }

        public Task Init()
        {
            throw new NotImplementedException();
        }
    }
}
