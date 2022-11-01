// Copyright (c) 2022 Dmitry Goryachev
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows;

namespace PdfCodeEditor.ViewModels
{
    internal class ProgressViewModel : ViewModelBase
    {
        private string _message;
        private bool _isIndeterminate;
        private int _percent;

        public string Message
        {
            get => _message;
            set
            {
                if (_message == value) 
                    return;
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set
            {
                if (_isIndeterminate == value)
                    return;
                _isIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
            }
        }

        public int Percent
        {
            get => _percent;
            set
            {
                if(_percent==value) 
                    return;
                _percent = Math.Min(value, 100);
                OnPropertyChanged(nameof(Percent));
            }
        }

        public void ShowMessage(string message, bool isIndeterminate = false)
        {
            ExecuteInDispatcher(() =>
            {
                IsIndeterminate = isIndeterminate;
                Message = message;
            });
        }

        private static void ExecuteInDispatcher(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                action();
            else
                Application.Current.Dispatcher.Invoke(action);
        }
    }
}
