// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using System.CodeDom.Compiler;

#if __UNIFIED__
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

namespace TrackBeamTheme_Sample_iOS.Views.iPhone
{
	[Register ("DetailThemeController")]
	partial class DetailThemeController
	{
		[Outlet]
		UILabel trackLabel { get; set; }

		[Outlet]
		UIScrollView scrollView { get; set; }

		[Outlet]
		UILabel genreLabel { get; set; }

		[Outlet]
		UIImageView bgImageView { get; set; }

		[Outlet]
		UILabel artistLabel { get; set; }

		[Outlet]
		UIImageView albumImageView { get; set; }

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
