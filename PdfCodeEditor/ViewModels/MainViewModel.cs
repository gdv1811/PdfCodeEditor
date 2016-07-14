// Copyright (c) 2016 Dmitry Goryachev
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
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace PdfCodeEditor.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        #region Fields

        private PdfDocumentViewModel _currentPdfDocument;
        private ICommand _openCommand;
        private ICommand _dropCommand;

        #endregion

        #region Properties

        public ObservableCollection<PdfDocumentViewModel> Documents { get; }

        public PdfDocumentViewModel CurrentPdfDocument
        {
            get { return _currentPdfDocument; }
            set
            {
                _currentPdfDocument = value;
                OnPropertyChanged(nameof(CurrentPdfDocument));
            }
        }

        #endregion

        #region Properties-commands

        public ICommand OpenCommand
        {
            get { return _openCommand ?? (_openCommand = new RelayCommand(arg => Open(arg as string))); }
        }

        public ICommand DropCommand
        {
            get { return _dropCommand ?? (_dropCommand = new RelayCommand(arg => Drop(arg as DragEventArgs))); }
        }

        #endregion

        #region Constructors

        public MainViewModel()
        {
            Documents = new ObservableCollection<PdfDocumentViewModel>();
        }

        #endregion

        #region Private methods
        
        private void Open(string path)
        {
            if(string.IsNullOrEmpty(path))
                return;

            var doc = new PdfDocumentViewModel();
            doc.Open(path);

            Documents.Add(doc);
            CurrentPdfDocument = doc;
        }

        private void Drop(DragEventArgs arg)
        {
            if (!arg.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var files = (string[])arg.Data.GetData(DataFormats.FileDrop);

            foreach (var file in files)
            {
                Open(file);
            }
        }

        #endregion
    }
}
