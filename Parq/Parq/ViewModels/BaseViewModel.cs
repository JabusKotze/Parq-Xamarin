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
using System.Collections.Generic;
using System.ComponentModel;

namespace Parq.ViewModels
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		private string title = string.Empty;
		private string subTitle = string.Empty;
		private string icon = null;
		private bool isBusy;

		public const string TitlePropertyName = "Title";
		public const string SubtitlePropertyName = "Subtitle";
		public const string IconPropertyName = "Icon";
		public const string IsBusyPropertyName = "IsBusy";
		
		public event PropertyChangedEventHandler PropertyChanged;

		public BaseViewModel ()
		{
		}

		public string Title
		{
			get { return title; }
			set { SetProperty (ref title, value, TitlePropertyName);}
		}
		
		public string Subtitle
		{
			get { return subTitle; }
			set { SetProperty (ref subTitle, value, SubtitlePropertyName);}
		}

		public string Icon
		{
			get { return icon; }
			set { SetProperty (ref icon, value, IconPropertyName);}
		}

		public bool IsBusy 
		{
			get { return isBusy; }
			set { SetProperty (ref isBusy, value, IsBusyPropertyName);}
		}
			
		protected void SetProperty<T> (ref T backingStore, T value, string propertyName, Action onChanged = null, Action<T> onChanging = null) 
		{
			if (EqualityComparer<T>.Default.Equals(backingStore, value)) 
				return;

			if (onChanging != null) 
				onChanging(value);

			//OnPropertyChanging(propertyName);

			backingStore = value;

			if (onChanged != null) 
				onChanged();

			OnPropertyChanged(propertyName);
		}
		
		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
		}
	}
}