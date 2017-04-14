﻿#region Copyright
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
using System.Windows.Input;

namespace Parq.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action handler;
        private bool isEnabled;
        private readonly Func<bool> canExecute;

        public RelayCommand(Action handler, Func<bool> canExecute = null)
        {
            this.handler = handler;
            this.canExecute = canExecute;
            if (canExecute == null)
                isEnabled = true;
        }


        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;
                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
                IsEnabled = canExecute();

            return IsEnabled;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            handler();
        }

        /// <summary>
        /// Method used to raise the <see cref="CanExecuteChanged"/> event
        /// to indicate that the return value of the <see cref="CanExecute"/>
        /// method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> handler;
        private bool isEnabled = true;

        private readonly Func<T, bool> canExecute;

        public RelayCommand(Action<T> handler, Func<T, bool> canExecute = null)
        {
            this.handler = handler;
            this.canExecute = canExecute;
            if (canExecute == null)
                isEnabled = true;
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;
                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
                IsEnabled = canExecute((T)parameter);

            return IsEnabled;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            handler((T)parameter);
        }

        /// <summary>
        /// Method used to raise the <see cref="CanExecuteChanged"/> event
        /// to indicate that the return value of the <see cref="CanExecute"/>
        /// method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
