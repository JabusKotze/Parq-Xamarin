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

using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Square.Picasso;

namespace Parq.Droid.Helpers
{
    public class PicassoBackgroundManagerTargetLinearLayout : Java.Lang.Object, ITarget
    {
        LinearLayout mBackgroundManager;

        public PicassoBackgroundManagerTargetLinearLayout(LinearLayout backgroundManager)
        {
            this.mBackgroundManager = backgroundManager;
        }

        public void OnBitmapLoaded(Bitmap bitmap, Picasso.LoadedFrom loadedFrom)
        {
            this.mBackgroundManager.Background = new BitmapDrawable(bitmap);
        }

        public void OnBitmapFailed(Drawable drawable)
        {
            this.mBackgroundManager.Background = drawable;
        }

        public void OnPrepareLoad(Drawable drawable)
        {
        }

    }
}