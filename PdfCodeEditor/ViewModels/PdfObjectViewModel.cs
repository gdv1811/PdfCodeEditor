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

namespace PdfCodeEditor.ViewModels
{
    internal class PdfObjectViewModel: ViewModelBase
    {
        //private static readonly ObservableCollection<PdfObjectViewModel> DummyChild = new ObservableCollection<PdfObjectViewModel>();

        private readonly PdfObjectViewModel _parent;
        private readonly IPdfObjectProvider _provider;
        private readonly PdfObject _pdfObject;

        private string _type;
        private string _name;
        private string _value;
        private string _reference;

        private bool _isExpanded;

        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public string Reference
        {
            get { return _reference; }
            set
            {
                _reference = value;
                OnPropertyChanged(nameof(Reference));
            }
        }

        public ObservableCollection<PdfObjectViewModel> ValuesCollection { get; } 

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (!IsLoaded)
                {
                    //ValuesCollection.Remove(DummyChild);
                    Load();
                }
            }
        }

        public bool IsLoaded { get; private set; }

        public PdfObjectViewModel(PdfObject obj, IPdfObjectProvider provider, PdfObjectViewModel parent = null)
        {
            _pdfObject = obj;

            Type = obj.Type.ToString();
            Name = obj.Name;
            Value = obj.Value;
            Reference = obj.Reference;
            if (obj.ValuesCollection != null)
                ValuesCollection = new ObservableCollection<PdfObjectViewModel>();

            _parent = parent;
            _provider = provider;
        }

        public void Load()
        {

        }
    }
}
