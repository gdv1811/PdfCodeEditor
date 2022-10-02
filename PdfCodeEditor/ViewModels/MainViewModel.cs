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

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PdfCodeEditor.Services;

namespace PdfCodeEditor.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        #region Fields

        private readonly IDialogService _dialogService;
        private PdfDocumentViewModel _currentPdfDocument;
        private ToolViewModel _currentToolView;
        private ViewModelBase _currentContent;
        private ICommand _openCommand;
        private ICommand _dropCommand;

        #endregion

        #region Properties

        public ObservableCollection<PdfDocumentViewModel> Documents { get; }
        public ObservableCollection<ToolViewModel> Tools { get; }

        public PdfDocumentViewModel CurrentPdfDocument
        {
            get { return _currentPdfDocument; }
            set
            {
                _currentPdfDocument = value;
                _currentContent = _currentPdfDocument;
                OnPropertyChanged(nameof(CurrentPdfDocument));
                OnPropertyChanged(nameof(CurrentContent));
            }
        }

        public ToolViewModel CurrentToolView
        {
            get { return _currentToolView; }
            set
            {
                _currentToolView = value;
                _currentContent = _currentToolView;
                OnPropertyChanged(nameof(CurrentToolView));
                OnPropertyChanged(nameof(CurrentContent));
            }
        }

        public ViewModelBase CurrentContent
        {
            get { return _currentContent; }
            set
            {
                _currentContent = value;
                switch (_currentContent)
                {
                    case PdfDocumentViewModel doc:
                        CurrentPdfDocument = doc;
                        break;
                    case ToolViewModel tool:
                        CurrentToolView = tool;
                        break;
                }
            }
        }

        #endregion

        #region Properties-commands

        public ICommand OpenCommand
        {
            get { return _openCommand ?? (_openCommand = new RelayCommand(arg => Open())); }
        }

        public ICommand DropCommand
        {
            get { return _dropCommand ?? (_dropCommand = new RelayCommand(arg => Drop(arg as DragEventArgs))); }
        }

        #endregion

        #region Constructors

        public MainViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            Documents = new ObservableCollection<PdfDocumentViewModel>();
            Tools = new ObservableCollection<ToolViewModel>();

            var args = Environment.GetCommandLineArgs();
            foreach (var path in args.Where(File.Exists).Where(path => Path.GetExtension(path) == ".pdf"))
            {
                Open(path);
            }
        }

        #endregion

        #region Private methods

        private void Open()
        {
            Open(_dialogService.ShowOpenDialog("PDF|*.pdf"));
        }

        private void Open(string path)
        {
            if(string.IsNullOrEmpty(path))
                return;

            var doc = new PdfDocumentViewModel(_dialogService);
            doc.Open(path);
            doc.PdfTree.NewTabRequired += (o, a) => 
            { 
                Tools.Add(doc.PdfTree);
                CurrentToolView = doc.PdfTree;
                doc.PdfTree.IsSelected = true;
            };
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
