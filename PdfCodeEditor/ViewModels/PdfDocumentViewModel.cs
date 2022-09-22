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
        private string _filePath;
        private TextDocument _document;
        private bool _isModified;
        private ICommand _saveCommand;
        private ICommand _saveAsCommand;
        private NavigatorViewModel _navigator;
        private PdfTreeViewModel _pdfTree;

        #endregion

        #region Properties

        public string FilePath
        {
            get { return _filePath; }
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
            get { return _document; }
            set
            {
                _document = value;
                OnPropertyChanged(nameof(Document));
            }
        }

        public bool IsModified
        {
            get { return _isModified; }
            set
            {
                _isModified = value;
                OnPropertyChanged(nameof(IsModified));
                OnPropertyChanged(nameof(Title));
            }
        }

        public NavigatorViewModel Navigator
        {
            get { return _navigator; }
            set
            {
                _navigator = value;
                OnPropertyChanged(nameof(Navigator));
            }
        }

        public PdfTreeViewModel PdfTree
        {
            get { return _pdfTree; }
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
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(arg => Save(FilePath))); }
        }

        public ICommand SaveAsCommand
        {
            get
            {
                return _saveAsCommand ??
                       (_saveAsCommand =
                           new RelayCommand(arg => Save(_dialogService.ShowSaveDialog("PDF|*.pdf|No extension|*.*"))));
            }
        }

        #endregion

        #region Constructors

        public PdfDocumentViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            Document = new TextDocument();
            Navigator = new NavigatorViewModel(Document);
        }

        #endregion

        #region Public methods

        public void Open(string filePath)
        {
            FilePath = filePath;

            if (File.Exists(filePath))
            {
                _document = new TextDocument(FileManager.ReadTextFile(_filePath));
                Navigator.Document = _document;

                var stm = new TextDocumentStream(_document);
                IPdfObjectProvider provider = new PdfObjectiTextProvider(stm);
                PdfTree = new PdfTreeViewModel(provider, Navigator);
            }
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

        #endregion
    }
}
