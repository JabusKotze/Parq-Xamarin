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
using SidebarNavigation;
using UIKit;

namespace Parq.iOS.ViewControllers
{
    public partial class RootViewController : UIViewController
    {
        // the sidebar controller for the app
        public SidebarController sideBarController { get; private set; }

        public NavController navController { get; private set; }

        public RootViewController() : base(null, null)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            navController = new NavController();
            navController.PushViewController(new IntroController(), false);
            sideBarController = new SidebarController(this, navController, new SideMenuController());
            sideBarController.MenuWidth = 220;
            sideBarController.ReopenOnRotate = false;

            // Perform any additional setup after loading the view, typically from a nib.
        }
    }
}