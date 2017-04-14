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
using System.Collections.Generic;
using System.Threading.Tasks;
using Parq.Models;

namespace Parq.Interfaces
{
    public interface IActiveTicketService
    {

        Task<ActiveTicket> GetActiveTicketAsync(string id);
        Task<IEnumerable<ActiveTicket>> GetActiveTicketsAsync();
        Task SyncActiveTicketsAsync();
        Task<ActiveTicket> InsertActiveTicketAsync(ActiveTicket expense);
        Task<string> DeleteActiveTicketAsync(ActiveTicket expense);
        string UserId { get; }

        Task Init();

    }
}
