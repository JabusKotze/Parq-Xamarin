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
using Android.Runtime;
using Android.Views;
using Android.Graphics;

using Android.Support.Design.Widget;
using IOnDismissListener = Android.Support.Design.Widget.SwipeDismissBehavior.IOnDismissListener;

namespace Parq.Droid.Behaviors
{
    public class NotificationBehavior : SwipeDismissBehavior, IOnDismissListener
    {
        public event EventHandler Dismissed;

        public NotificationBehavior()
        {
            this.SetSwipeDirection(SwipeDismissBehavior.SwipeDirectionStartToEnd);
            this.SetDragDismissDistance(.98f);
            this.SetStartAlphaSwipeDistance(.1f);
            this.SetListener(this);
        }

        void IOnDismissListener.OnDismiss(View view)
        {
            
                view.Alpha = 1;
                view.Visibility = ViewStates.Invisible;
                view.RequestLayout();

                Dismissed?.Invoke(this, EventArgs.Empty);
            
        }

        void IOnDismissListener.OnDragStateChanged(int state)
        {
        }

        public override bool CanSwipeDismissView(View view)
        {
            return view.Visibility == ViewStates.Visible && view.TranslationX == 0;
        }

        public override bool BlocksInteractionBelow(CoordinatorLayout parent, Java.Lang.Object child)
        {            
            return child.JavaCast<View>().Visibility == ViewStates.Visible;
        }

        public override int GetScrimColor(CoordinatorLayout parent, Java.Lang.Object child)
        {
            return new Color(0, 0, 0, 128).ToArgb();
        }

        public override float GetScrimOpacity(CoordinatorLayout parent, Java.Lang.Object child)
        {
            return child.JavaCast<View>().Visibility == ViewStates.Visible ? 1f : 0f;
        }
    }
}

