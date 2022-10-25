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
using System.IO;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using NuGet;
using PdfCodeEditor.Editor;
using PdfCodeEditor.Models;
using PdfCodeEditor.Models.Pdf;
using PdfCodeEditor.Services;

namespace PdfCodeEditor.ViewModels
{
    internal class PdfDocumentViewModel : ViewModelBase
    {
        #region Fields

        private readonly IDialogService _dialogService;
        private readonly DockManagerViewModel _dockManager;
        private string _filePath;
        private TextDocument _document;
        private bool _isModified;
        private ICommand _saveCommand;
        private ICommand _saveAsCommand;
        private ICommand _closeCommand;
        private NavigatorViewModel _navigator;
        private PdfTreeViewModel _pdfTree;

        #endregion

        #region Properties

        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath == value)
                    return;
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
                OnPropertyChanged(nameof(FileName));
                OnPropertyChanged(nameof(Title));
            }
        }

        public string FileName => Path.GetFileName(_filePath);

        public string Title => Path.GetFileName(_filePath) + (IsModified ? " *" : "");

        public TextDocument Document
        {
            get => _document;
            set
            {
                _document = value;
                OnPropertyChanged(nameof(Document));
            }
        }

        public bool IsModified
        {
            get => _isModified;
            set
            {
                _isModified = value;
                OnPropertyChanged(nameof(IsModified));
                OnPropertyChanged(nameof(Title));
            }
        }

        public NavigatorViewModel Navigator
        {
            get => _navigator;
            set
            {
                _navigator = value;
                OnPropertyChanged(nameof(Navigator));
            }
        }

        public PdfTreeViewModel PdfTree
        {
            get => _pdfTree;
            set
            {
                _pdfTree = value;
                OnPropertyChanged(nameof(PdfTree));
            }
        }

        #endregion
        
        #region Properties-commands

        public ICommand SaveCommand
        {
            get { return _saveCommand ??= new RelayCommand(arg => Save(FilePath)); }
        }

        public ICommand SaveAsCommand
        {
            get
            {
                return _saveAsCommand ??= new RelayCommand(arg => Save(_dialogService.ShowSaveDialog("PDF|*.pdf|No extension|*.*")));
            }
        }
        public ICommand CloseCommand
        {
            get { return _closeCommand ??= new RelayCommand(arg => Close()); }
        }

        #endregion

        #region Constructors

        public PdfDocumentViewModel(IDialogService dialogService, DockManagerViewModel dockManager)
        {
            _dialogService = dialogService;
            _dockManager = dockManager;
            Document = new TextDocument();
            Navigator = new NavigatorViewModel(Document);
        }

        #endregion

        #region Public methods

        public void Open(string filePath)
        {
            FilePath = filePath;

            if (!File.Exists(filePath)) 
                return;

            _document = new TextDocument(FileManager.ReadTextFile(_filePath));
            Navigator.Document = _document;

            var stm = new TextDocumentStream(_document);
            IPdfObjectProvider provider = new PdfObjectiTextProvider(stm);
            PdfTree = new PdfTreeViewModel(provider, Navigator, _dockManager)
            {
                FilePath = filePath
            };
        }

        public void Save(string path)
        {
            if (path == null)
                return;
            FilePath = path;

            try
            {
                FileManager.WriteTextFile(Document.Text, FilePath);

                IsModified = false;
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage(ex.Message, "Save error");
            }
        }

        public void Close()
        {
            _dockManager.Documents.Remove(this);
            if (_dockManager.MainTreeManager.Tool.Content == PdfTree)
                _dockManager.MainTreeManager.Tool.Content = PdfTreeViewModel.Empty;
            _dockManager.Tools.RemoveAll(item => item.Content == PdfTree);
        }
        
        #endregion
    }
}
