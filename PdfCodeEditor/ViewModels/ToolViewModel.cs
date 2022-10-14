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

using System.Windows.Input;

namespace PdfCodeEditor.ViewModels
{
    internal class ToolViewModel : ViewModelBase
    {
        private string _toolTip;
        private string _title;
        private bool _isSelected;
        private ViewModelBase _content;
        private DockManagerViewModel _dockManager;

        private ICommand _closeCommand;

        public string ToolTip
        {
            get => _toolTip;
            set
            {
                _toolTip = value;
                OnPropertyChanged(nameof(ToolTip));
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public ViewModelBase Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }
        
        public ICommand CloseCommand
        {
            get { return _closeCommand ??= new RelayCommand(arg => Close()); }
        }

        public ToolViewModel(ViewModelBase content, DockManagerViewModel dockManager)
        {
            _content = content;
            _dockManager = dockManager;
        }

        public void Close()
        {
            if (_dockManager.MainTreeManager.Tool == this)
                _dockManager.MainTreeManager.CloseTool();
            else
                _dockManager.Tools.Remove(this);
        }
    }
}
