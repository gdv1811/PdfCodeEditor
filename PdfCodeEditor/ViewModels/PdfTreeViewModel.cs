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

using PdfCodeEditor.Models.Pdf;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace PdfCodeEditor.ViewModels
{
    internal class PdfTreeViewModel : ViewModelBase
    {
        #region Fields

        private ICommand _refreshCommand;
        private ICommand _openInNewTabCommand;

        private NavigatorViewModel _navigator;
        private DockManagerViewModel _dockManager;
        private IPdfObjectProvider _objectProvider;
        private string _filePath;

        #endregion

        #region Properties

        public static PdfTreeViewModel Empty = new();

        public ObservableCollection<PdfObjectViewModel> PdfTree { get; set; }

        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath == value)
                    return;
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        #endregion

        #region Properties-commands

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ??= new RelayCommand(_ => Refresh()); }
        }

        public ICommand OpenInNewTabCommand
        {
            get { return _openInNewTabCommand ??= new RelayCommand(_ => OpenInNewTab()); }
        }

        #endregion

        #region Constructors

        private PdfTreeViewModel()
        {
        }

        public PdfTreeViewModel(IPdfObjectProvider provider, NavigatorViewModel navigator, DockManagerViewModel dockManager)
        {
            _objectProvider = provider;
            _navigator = navigator;
            _dockManager = dockManager;

            Init(provider, navigator);
        }

        private void Init(IPdfObjectProvider provider, NavigatorViewModel navigator)
        {
            if (provider == null || navigator == null)
                return;
            PdfObjectViewModel rootObj;
            if (provider.TryInit(out PdfExceptionObject ex))
            {
                var version = provider.GetPdfVersion();
                var trailer = provider.GetTrailer();
                version.ValuesCollection = trailer.ValuesCollection;
                rootObj = new PdfObjectViewModel(version, provider, navigator) { IsExpanded = true };
            }
            else
                rootObj = new PdfObjectViewModel(ex, provider, navigator);

            if (PdfTree == null)
                PdfTree = new ObservableCollection<PdfObjectViewModel>();
            else
                PdfTree.Clear();
            PdfTree.Add(rootObj);
        }

        #endregion

        #region Private methods

        private void Refresh()
        {
            Init(_objectProvider, _navigator);
        }

        private void OpenInNewTab()
        {
            if (_dockManager == null)
                return;

            var tool = new ToolViewModel(this, _dockManager)
            {
                Title = Path.GetFileName(FilePath) + " - Object tree",
                ToolTip = FilePath
            };

            _dockManager.Tools.Add(tool);
            tool.IsSelected = true;
        }

        #endregion
    }
}
