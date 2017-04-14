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
using Android.Content;
using Android.Views;
using Android.Support.V7.Widget;

namespace Parq.Droid.LayoutManagers
{
    public class WrappingLinearLayoutManager : LinearLayoutManager
    {
        public Context context;
        public WrappingLinearLayoutManager(Context context) : base(context)
        {
            this.context = context;
        }

        private int[] mMeasuredDimension = new int[2];

        public override bool CanScrollVertically()
        {
            return false;
        }

        public override void OnMeasure(RecyclerView.Recycler recycler, RecyclerView.State state, int widthSpec, int heightSpec)
        {
            base.OnMeasure(recycler, state, widthSpec, heightSpec);        

            int widthMode = (int)View.MeasureSpec.GetMode(widthSpec);
            int heightMode = (int)View.MeasureSpec.GetMode(heightSpec);

            int widthSize = View.MeasureSpec.GetSize(widthSpec);
            int heightSize = View.MeasureSpec.GetSize(heightSpec);

            int width = 0;
            int height = 0;

            for (int i=0; i < ItemCount; i++)
            {
                if(Orientation == Horizontal)
                {
                    measureScrapChild(recycler, i, View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified), heightSpec, mMeasuredDimension);

                    width = width + mMeasuredDimension[0];
                    if(i == 0)
                    {
                        height = mMeasuredDimension[1];
                    }
                }
                else
                {
                    measureScrapChild(recycler, i, widthSpec, View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified), mMeasuredDimension);

                    height = height + mMeasuredDimension[1];
                    if(i == 0)
                    {
                        width = mMeasuredDimension[0];
                    }
                }
            }

            switch (widthMode)
            {
                case (int)MeasureSpecMode.Exactly:
                    width = widthSize;
                    break;
                case (int)MeasureSpecMode.AtMost:
                    break;
                case (int)MeasureSpecMode.Unspecified:
                    break;
            }

            switch (heightMode)
            {
                case (int)MeasureSpecMode.Exactly:
                    height = heightSize;
                    break;
                case (int)MeasureSpecMode.AtMost:
                    break;
                case (int)MeasureSpecMode.Unspecified:
                    break;
            }

            SetMeasuredDimension(width, height);
        }

        private void measureScrapChild(RecyclerView.Recycler recycler, int position, int widthSpec,
            int heightSpec, int[] measuredDimension)
        {
            try
            {

                View view = recycler.GetViewForPosition(position);
                if (view.Visibility == ViewStates.Gone)
                {
                    measuredDimension[0] = 0;
                    measuredDimension[1] = 0;
                    return;
                }
                // For adding Item Decor Insets to view
                //super.measureChildWithMargins(view, 0, 0);
                this.MeasureChildWithMargins(view, 0, 0);
                RecyclerView.LayoutParams p = (RecyclerView.LayoutParams)view.LayoutParameters;
                int childWidthSpec = ViewGroup.GetChildMeasureSpec(
                        widthSpec,
                        PaddingLeft + PaddingRight + GetDecoratedLeft(view) + GetDecoratedRight(view),
                        p.Width);
                int childHeightSpec = ViewGroup.GetChildMeasureSpec(
                        heightSpec,
                        PaddingTop + PaddingBottom + GetDecoratedTop(view) + GetDecoratedBottom(view),
                        p.Height);
                view.Measure(childWidthSpec, childHeightSpec);

                // Get decorated measurements
                measuredDimension[0] = GetDecoratedMeasuredWidth(view) + p.LeftMargin + p.RightMargin;
                measuredDimension[1] = GetDecoratedMeasuredHeight(view) + p.BottomMargin + p.TopMargin;
                recycler.RecycleView(view);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}