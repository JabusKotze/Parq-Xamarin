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



using Parq.PlatformSpecific;
using Parq.Interfaces;
using Parq.ViewModels;
using Refractored.Xam.Settings;
#if __IOS__
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
#elif __ANDROID__
using Microsoft.WindowsAzure.MobileServices;
#elif WINDOWS_PHONE

#endif

namespace Parq.Helpers
{
    public static class ServiceRegistrar
    {
        public static void Startup(string dbpath)
        {
#if __ANDROID__
            CurrentPlatform.Init();
#elif __IOS__
      CurrentPlatform.Init();
      SQLitePCL.CurrentPlatform.Init();
#endif
            Settings.DataBasePath = dbpath;

            var activeTicketService = AzureActiveTicketService.Instance;
            activeTicketService.Init().Wait();

            var historyTicketService = AzureHistoryTicketService.Instance;
            historyTicketService.Init().Wait();

            ServiceContainer.Register<IMessageDialog>(() => new MessageDialog());
            ServiceContainer.Register<IActiveTicketService>(() => activeTicketService);
            ServiceContainer.Register<ActiveTicketsViewModel>();
            ServiceContainer.Register<ActiveTicketViewModel>();

            ServiceContainer.Register<IHistoryTicketService>(() => historyTicketService);
            ServiceContainer.Register<HistoryTicketsViewModel>();            
        }
    }
}