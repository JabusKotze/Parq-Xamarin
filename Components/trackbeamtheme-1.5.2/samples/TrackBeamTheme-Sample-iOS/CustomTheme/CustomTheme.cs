using System;
using Xamarin.Themes.TrackBeam;

#if __UNIFIED__
using UIKit;
#else
using MonoTouch.UIKit;
#endif

namespace TrackBeamTheme_Sample_iOS.CustomTheme
{
	public class CustomTheme : TrackBeamTheme
	{
		public override UIColor BaseTintColor
		{
			get
			{
				return UIColor.Red;
			}
		}
	}
}

