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
using Foundation;
using Parq.iOS;

namespace Parq.iOS.ViewControllers
{
    public partial class BaseController : UIViewController
    {
        // provide access to the sidebar controller to all inheriting controllers
        protected SidebarController sidebarController
        {
            get
            {
                return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.sideBarController;
            }
        }

        // provide access to the sidebar controller to all inheriting controllers
        protected NavController NavController
        {
            get
            {
                return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.navController;
            }
        }

        public BaseController(string nibName, NSBundle bundle) : base(nibName, bundle)
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

            NavigationItem.SetRightBarButtonItem(
                new UIBarButtonItem(UIImage.FromBundle("threelines")
                    , UIBarButtonItemStyle.Plain
                    , (sender, args) => {
                        sidebarController.ToggleMenu();
                    }), true);
        }
    }
}