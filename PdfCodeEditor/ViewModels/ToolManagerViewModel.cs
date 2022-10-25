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

using System.Collections.Specialized;
using System.Windows.Input;

namespace PdfCodeEditor.ViewModels
{
    internal class ToolManagerViewModel:ViewModelBase
    {
        private readonly DockManagerViewModel _dockManager;
        private bool _isToolOpen = false;
        private ICommand _openToolCommand;
        private ICommand _closeToolCommand;

        public ToolViewModel Tool { get; }

        public bool IsToolOpen
        {
            get => _isToolOpen;
            private set
            {
                _isToolOpen = value;
                OnPropertyChanged(nameof(IsToolOpen));
            }
        }

        public ICommand OpenToolCommand
        {
            get { return _openToolCommand ??= new RelayCommand(_ => OpenTool()); }
        }

        public ICommand CloseToolCommand
        {
            get { return _closeToolCommand ??= new RelayCommand(_ => CloseTool()); }
        }

        public ToolManagerViewModel(ToolViewModel tool, DockManagerViewModel dockManager)
        {
            _dockManager = dockManager;
            _dockManager.Tools.CollectionChanged+= ToolsOnCollectionChanged;
            Tool = tool;
        }

        private void ToolsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null && e.OldItems.Contains(Tool))
                IsToolOpen = false;
        }

        public void OpenTool()
        {
            _dockManager.Tools.Add(Tool);
            IsToolOpen = true;
        }

        public void CloseTool()
        {
            _dockManager.Tools.Remove(Tool);
            IsToolOpen = false;
        }
    }
}
