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
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace PdfCodeEditor.ViewModels
{
    internal class PdfTreeViewModel : ToolViewModel
    {
        #region Fields

        private ICommand _refreshCommand;
        private ICommand _openInNewTabCommand;

        private NavigatorViewModel _navigator;
        private IPdfObjectProvider _objectProvider;

        #endregion

        #region Events

        public event EventHandler<EventArgs> NewTabRequired;

        #endregion

        #region Properties

        public ObservableCollection<PdfObjectViewModel> PdfTree { get; set; }

        public override string Title => Path.GetFileName(FilePath) + " - Object tree";

        #endregion

        #region Properties-commands

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(arg => Refresh())); }
        }

        public ICommand OpenInNewTabCommand
        {
            get { return _openInNewTabCommand ?? (_openInNewTabCommand = new RelayCommand(arg => OnNewTabRequired())); }
        }

        #endregion

        #region Constructors

        public PdfTreeViewModel(IPdfObjectProvider provider, NavigatorViewModel navigator)
        {
            _objectProvider = provider;
            _navigator = navigator;
            Init(provider, navigator);
        }

        private void Init(IPdfObjectProvider provider, NavigatorViewModel navigator)
        {
            var version = provider.GetPdfVersion();
            var trailer = provider.GetTrailer();
            version.ValuesCollection = trailer.ValuesCollection;

            if (PdfTree == null)
                PdfTree = new ObservableCollection<PdfObjectViewModel>();
            else
                PdfTree.Clear();
            PdfTree.Add(new PdfObjectViewModel(version, provider, navigator) { IsExpanded = true });
        }

        #endregion

        #region Private methods

        private void Refresh()
        {
            _objectProvider.Reset();
            Init(_objectProvider, _navigator);
        }

        private void OnNewTabRequired() 
        {
            NewTabRequired?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
