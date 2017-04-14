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
using System.Drawing;
using UIKit;

namespace Parq.iOS.ViewControllers
{
    public partial class SideMenuController : BaseController
    {
        public SideMenuController() : base(null, null)
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
            View.BackgroundColor = UIColor.FromRGB(.9f, .9f, .9f);

            var title = new UILabel(new RectangleF(0, 50, 230, 20));
            title.Font = UIFont.SystemFontOfSize(24.0f);
            title.TextAlignment = UITextAlignment.Center;
            title.TextColor = UIColor.Blue;
            title.Text = "Menu";

            var body = new UILabel(new RectangleF(30, 70, 170, 100));
            body.Font = UIFont.SystemFontOfSize(12.0f);
            body.TextAlignment = UITextAlignment.Center;
            body.Lines = 0;
            body.Text = @"This is the side menu. You can use any UIViewController to put whatever you want here!";

            var introButton = new UIButton(UIButtonType.System);
            introButton.Frame = new RectangleF(0, 180, 230, 20);
            introButton.SetTitle("Intro", UIControlState.Normal);
            introButton.TouchUpInside += (sender, e) => {
                NavController.PopToRootViewController(false);
                sidebarController.CloseMenu();
            };

            var contentButton = new UIButton(UIButtonType.System);
            contentButton.Frame = new RectangleF(0, 220, 230, 20);
            contentButton.SetTitle("Content", UIControlState.Normal);
            contentButton.TouchUpInside += (sender, e) => {
                NavController.PushViewController(new ContentController(), false);
                sidebarController.CloseMenu();
            };

            View.Add(title);
            View.Add(body);
            View.Add(introButton);
            View.Add(contentButton);

            // Perform any additional setup after loading the view, typically from a nib.
        }
    }
}