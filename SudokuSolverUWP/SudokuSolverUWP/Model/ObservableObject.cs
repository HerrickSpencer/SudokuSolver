using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;

namespace SudokuSolverUWP.Model
{
    public class ObservableObject : INotifyPropertyChanged
    {
        protected CoreDispatcher dispatcher;
        public ObservableObject(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected async void RaisepropertyChanged(string propertyName)
        {
            if (dispatcher.HasThreadAccess)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }

        protected bool Set<T>(ref T field, T value,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            RaisepropertyChanged(propertyName);
            return true;
        }
    }
}
