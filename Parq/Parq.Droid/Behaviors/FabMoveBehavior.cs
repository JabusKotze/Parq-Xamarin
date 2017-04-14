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


using Android.Runtime;
using Android.Views;
using Android.Views.Animations;

using Android.Support.Design.Widget;
using Android.Support.V4.View.Animation;

namespace Parq.Droid.Behaviors
{
    public class FabMoveBehavior : CoordinatorLayout.Behavior
    {
        bool previousVisibility;
        float minX;
        float originalX;
        int distanceX, distanceY;
        float scale;
        int notifFrame, fabPlaceholder;

        IInterpolator interpolator;

        public FabMoveBehavior(int notifFrame,int fabPlaceholder)
        {
            this.notifFrame = notifFrame;
            this.fabPlaceholder = fabPlaceholder;
        }

        public override bool LayoutDependsOn(CoordinatorLayout parent, Java.Lang.Object child, View dependency)
        {
            if (dependency.Id == notifFrame)
                return true;
            return base.LayoutDependsOn(parent, child, dependency);
        }

        public override bool OnDependentViewChanged(CoordinatorLayout parent, Java.Lang.Object child, View dependency)
        {
            if (dependency.Id == notifFrame)
            {
                var fab = child.JavaCast<FloatingActionButton>();
                bool isNowVisibility = dependency.Visibility == ViewStates.Visible;
                
                // If the notification is still invisible, the changes are simply positioning.
                // Initialize everything by recalculating distances
                if (!previousVisibility && !isNowVisibility)
                {
                    var placeholder = dependency.FindViewById(fabPlaceholder);
                    var pos = new int[2];
                    placeholder.GetLocationOnScreen(pos);
                    var targetX = pos[0] + placeholder.Width / 2;
                    var targetY = pos[1] + placeholder.Height / 2;
                    fab.GetLocationOnScreen(pos);
                    distanceX = targetX - (pos[0] + fab.Width / 2);
                    distanceY = targetY - (pos[1] + fab.Height / 2);
                    scale = placeholder.Width / (float)fab.Width;
                    originalX = dependency.GetX();
                    return false;
                }

                // The notification frame is now gone, erase all changes done to FAB
                if (previousVisibility && !isNowVisibility)
                {
                    previousVisibility = false;
                    fab.TranslationY = fab.TranslationX = 0;
                    fab.ScaleY = fab.ScaleX = 1;
                    fab.Alpha = 1;
                    fab.Visibility = ViewStates.Invisible;
                    fab.Show();
                    return true;
                }

                // We start the moving process
                if (isNowVisibility ^ previousVisibility)
                {
                    previousVisibility = isNowVisibility;
                    minX = System.Math.Abs(dependency.TranslationX);
                }

                // Notification is being dragged out to the right, FAB should follow suite
                if (dependency.GetX() > originalX)
                {
                    fab.TranslationX = distanceX + dependency.GetX() - originalX;
                    fab.Alpha = dependency.Alpha;
                    return true;
                }

                // Carry out the initial curved motion in
                /* HACK: since path-based object animators are not yet available
				 * in support, we use the fact that PathInterpolator is to craft
				 * something similar. Think of it as creating the following cubic
				 * bezier (http://cubic-bezier.com/#0,.47,.47,1) and rotating the
				 * graph 90° to the left. X axis becomes the graph and Y axis is
				 * simply a vertical line.
				 */
                if (interpolator == null)
                    interpolator = PathInterpolatorCompat.Create(0, .47f, .47f, 1);

                var currentTranslation = System.Math.Abs(dependency.TranslationX);
                var ratio = (minX - currentTranslation) / (float)minX;
                fab.TranslationY = distanceY * ratio;
                fab.TranslationX = distanceX * interpolator.GetInterpolation(ratio);
                fab.ScaleX = fab.ScaleY = 1 + (scale - 1) * ratio;

                return true;
            }
            else if (dependency.Id == Resource.Id.main_appbar)
            {
                this.updateFabVisibility(parent, (AppBarLayout)dependency, child.JavaCast<FloatingActionButton>());
                return true;
            }
            return base.OnDependentViewChanged(parent, child, dependency);
        }

        public bool updateFabVisibility(CoordinatorLayout parent, AppBarLayout appBarLayout, FloatingActionButton child)
        {
            CoordinatorLayout.LayoutParams lp = (CoordinatorLayout.LayoutParams)child.LayoutParameters;
            if (lp.AnchorId != appBarLayout.Id)
            {
                return false;
            }
            else
            {
                ViewGroup.MarginLayoutParams param = (ViewGroup.MarginLayoutParams)child.LayoutParameters;
                int point = child.Top - param.TopMargin;
                try
                {

                    //Method method = Class.GetDeclaredMethod("getMinimumHeightForVisibleOverlappingContent");
                    //method.Accessible = true;
                    if (point <= 240)
                    {
                        //child.Hide();
                        child.Visibility = ViewStates.Invisible;
                    }
                    else
                    {
                        child.Visibility = ViewStates.Visible;
                        //child.Show();
                    }
                    return true;
                }
                catch (System.Exception ex)
                {
                    return true;
                }
            }
        }
    }
}

