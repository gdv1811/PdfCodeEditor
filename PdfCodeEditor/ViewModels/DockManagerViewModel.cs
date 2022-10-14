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

using System.Collections.ObjectModel;
using NuGet;

namespace PdfCodeEditor.ViewModels
{
    internal class DockManagerViewModel:ViewModelBase
    {
        private PdfDocumentViewModel _currentPdfDocument;
        private readonly ToolManagerViewModel _mainTreeManager;

        public ObservableCollection<PdfDocumentViewModel> Documents { get; }
        public ObservableCollection<ToolViewModel> Tools { get; }
        
        public PdfDocumentViewModel CurrentPdfDocument
        {
            get => _currentPdfDocument;
            set
            {
                _currentPdfDocument = value;
                OnPropertyChanged(nameof(CurrentPdfDocument));
                
                _mainTreeManager.Tool.Content = _currentPdfDocument.PdfTree;
            }
        }

        public ToolManagerViewModel MainTreeManager
        {
            get => _mainTreeManager;
            private init
            {
                _mainTreeManager = value;
                OnPropertyChanged(nameof(MainTreeManager));
            }
        }

        public DockManagerViewModel()
        {
            Documents = new ObservableCollection<PdfDocumentViewModel>();
            Tools = new ObservableCollection<ToolViewModel>();
            MainTreeManager =
                new ToolManagerViewModel(
                    new ToolViewModel(PdfTreeViewModel.Empty, this) { Title = "Object tree" }, 
                    this);
        }

        public void Close()
        {
            Documents.Clear();
            Tools.RemoveAll(item => item != MainTreeManager.Tool);
        }
    }
}
