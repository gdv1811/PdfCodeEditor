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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private bool _isUpdateAvailable;

        private readonly IDialogService _dialogService;
        private DockManagerViewModel _dockManager;
        private ProgressViewModel _progress;

        private ICommand _openCommand;
        private ICommand _dropCommand;
        private ICommand _gitHubCommand;
        private ICommand _updateAppCommand;
        private ICommand _closeCommand;

        #endregion

        #region Properties
        
        public DockManagerViewModel DockManager
        {
            get => _dockManager;
            set
            {
                _dockManager = value;
                OnPropertyChanged(nameof(DockManager));
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

        public ProgressViewModel Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
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

        public ICommand CloseCommand
        {
            get { return _closeCommand ??= new RelayCommand(_ => Close()); }
        }

        #endregion

        #region Constructors

        public MainViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            _dockManager = new DockManagerViewModel();
            _progress = new ProgressViewModel();

            Task.Run(InitUpdateManager);

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

            var doc = new PdfDocumentViewModel(_dialogService, _dockManager, Progress);
            doc.OpenAsync(path);
            _dockManager.Documents.Add(doc);
            _dockManager.CurrentPdfDocument = doc;
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
                Progress.ShowMessage("Unable to check for update");
            }
        }

        private async void UpdateApp()
        {
            try
            {
                _progress.ShowMessage("Updating...");
                await _updateManager.UpdateApp(progress => { _progress.Percent = progress; });

                IsUpdateAvailable = false;

                _dialogService.ShowMessage("Update completed successfully!\nPlease, restart application.", "Update");
                _progress.ShowMessage("Update completed successfully!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage(ex.Message, "Update error");
                _progress.ShowMessage("Update error");
            }
        }

        private void Close()
        {
            _dockManager.Close();
        }

        #endregion
    }
}
