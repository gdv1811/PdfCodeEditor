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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PdfCodeEditor.Services;
using Squirrel;

namespace PdfCodeEditor.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        #region Fields

        private string _gitHubProjectPath = "https://github.com/gdv1811/PdfCodeEditor";
        private UpdateManager _updateManager;

        private readonly IDialogService _dialogService;
        private PdfDocumentViewModel _currentPdfDocument;
        private ToolViewModel _currentToolView;
        private ViewModelBase _currentContent;
        private bool _isUpdateAvailable;

        private ICommand _openCommand;
        private ICommand _dropCommand;
        private ICommand _gitHubCommand;
        private ICommand _updateAppCommand;

        #endregion

        #region Properties

        public ObservableCollection<PdfDocumentViewModel> Documents { get; }
        public ObservableCollection<ToolViewModel> Tools { get; }

        public PdfDocumentViewModel CurrentPdfDocument
        {
            get => _currentPdfDocument;
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
            get => _currentToolView;
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
            get => _currentContent;
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

        public bool IsUpdateAvailable
        {
            get => _isUpdateAvailable;
            set
            {
                _isUpdateAvailable = value;
                OnPropertyChanged(nameof(IsUpdateAvailable));
            }
        }

        #endregion

        #region Properties-commands

        public ICommand OpenCommand
        {
            get { return _openCommand ??= new RelayCommand(_ => Open()); }
        }

        public ICommand DropCommand
        {
            get { return _dropCommand ??= new RelayCommand(arg => Drop(arg as DragEventArgs)); }
        }

        public ICommand GitHubCommand
        {
            get { return _gitHubCommand ??= new RelayCommand(_ => OpenGitHubProjectPage()); }
        }

        public ICommand UpdateAppCommand
        {
            get { return _updateAppCommand ??= new RelayCommand(_ => UpdateApp()); }
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

            InitUpdateManager();
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
            doc.DocumentClosing += (o, a) =>
            {
                Documents.Remove(doc);
                while (Tools.Remove(doc.PdfTree))
                { }

                doc.PdfTree = null;
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

        private void OpenGitHubProjectPage()
        {
            Process.Start(new ProcessStartInfo(_gitHubProjectPath) { UseShellExecute = true });
        }

        private async void InitUpdateManager()
        {
            try
            {
                _updateManager = await UpdateManager.GitHubUpdateManager(_gitHubProjectPath);
                var updateInfo = await _updateManager.CheckForUpdate();

                IsUpdateAvailable = updateInfo.ReleasesToApply.Count > 0;
            }
            catch
            {
                //todo add to log
            }
        }

        private async void UpdateApp()
        {
            try
            {
                await _updateManager.UpdateApp();

                IsUpdateAvailable = false;
                _dialogService.ShowMessage("Update completed successfully!\nPlease, restart application.", "Update");
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage(ex.Message, "Update error");
            }
        }

        #endregion
    }
}
