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
        private readonly PdfObjectViewModel _parent;
        private readonly IPdfObjectProvider _provider;
        private readonly PdfObject _pdfObject;

        private PdfObjectType _type;
        private string _name;
        private string _value;
        private string _valueReference;

        private bool _isExpanded;

        public PdfObjectType Type
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

        public string ValueRef
        {
            get { return _valueReference; }
            set
            {
                _valueReference = value;
                OnPropertyChanged(nameof(ValueRef));
            }
        }

        public ObservableCollection<PdfObjectViewModel> ValuesCollection { get; private set; } 

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

                if (ValuesCollection == null)
                    return;

                // Load child items
                foreach (var item in ValuesCollection)
                    item.Load();
            }
        }

        public bool IsLoaded { get; private set; }

        public PdfObjectViewModel(PdfObject obj, IPdfObjectProvider provider, PdfObjectViewModel parent = null)
        {
            _pdfObject = obj;
            _provider = provider;
            _parent = parent;

            Name = obj.Name;
            InitFromPdfObject(obj, _provider);
        }

        public void Load()
        {
            if (IsLoaded)
                return;

            if (Type == PdfObjectType.Reference)
            {
                var obj = _provider.GetPdfObject(_pdfObject.ValueReference);
                InitFromPdfObject(obj, _provider);
            }

            IsLoaded = true;
        }

        private void InitFromPdfObject(PdfObject obj, IPdfObjectProvider provider)
        {
            Type = obj.Type;
            Value = obj.Value;
            if (obj.ValueReference != null)
                ValueRef = obj.ValueReference.ToString();

            if (obj.ValuesCollection != null)
            {
                ValuesCollection = new ObservableCollection<PdfObjectViewModel>();
                foreach (var item in obj.ValuesCollection)
                    ValuesCollection.Add(new PdfObjectViewModel(item, provider, this));
                if (Value == null) 
                    Value = $"{obj.ValuesCollection.Count} entries";
            }
        }
    }
}
