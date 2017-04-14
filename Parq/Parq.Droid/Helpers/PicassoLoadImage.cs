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

using Android.Content;
using Android.Widget;

using Square.Picasso;
using Refractored.Controls;

using Parq.Helpers;

namespace Parq.Droid.Helpers
{
    public class PicassoLoadImage
    {
        public static void LoadProfilePic(Context context, string imageUri, CircleImageView imageView, int defaultImage, bool isLarge = false, bool isFacebook = false, bool ExcludeGravatar = true)
        {
            if (ExcludeGravatar)
            {
                if (imageUri.Contains("gravatar"))
                {
                    imageUri = null;
                }
            }

            if (isLarge & isFacebook)
            {
                var userid = Settings.UserId;
                imageUri = FaceBook.LargeProfilePicCustomBuilder(userid.Replace("facebook|", ""));                
            }

            Picasso.With(context)
                   .Load(imageUri)
                   .Placeholder(defaultImage)                   
                   .Error(defaultImage)
                   .Into(imageView);
        }

        public static void LoadPic(Context context, string imageUri, ImageView imageView, int defaultImage)
        {            
            Picasso.With(context)
                   .Load(imageUri)
                   .Placeholder(defaultImage)
                   .Error(defaultImage)
                   .Into(imageView);
        }

        public static void LoadPic(Context context, string imageUri, CircleImageView imageView, int defaultImage)
        {
            Picasso.With(context)
                   .Load(imageUri)
                   .Placeholder(defaultImage)
                   .Error(defaultImage)
                   .Into(imageView);
        }
    }
}