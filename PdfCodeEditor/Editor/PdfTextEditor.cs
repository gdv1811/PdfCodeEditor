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
using System.Windows;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;

namespace PdfCodeEditor.Editor
{
    internal class PdfTextEditor: TextEditor
    {
        #region Dependecy Propeties

        public static DependencyProperty CaretOffsetProperty = DependencyProperty.Register(nameof(CaretOffset),
            typeof(int), typeof(PdfTextEditor), new PropertyMetadata(OnCaretOffsetPropertyChanged));

        private static void OnCaretOffsetPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PdfTextEditor target = (PdfTextEditor)obj;
            target.CaretOffset = (int)args.NewValue;
        }

        public static DependencyProperty BindableSelectionLengthProperty = DependencyProperty.Register(nameof(BindableSelectionLength),
            typeof(int), typeof(PdfTextEditor), new PropertyMetadata(OnBindableSelectionLengthPropertyChanged));

        private static void OnBindableSelectionLengthPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var target = (PdfTextEditor)obj;
            target.BindableSelectionLength = (int)args.NewValue;
        }

        #endregion

        #region Properties

        public new int CaretOffset
        {
            get => base.CaretOffset;
            set
            {
                if (value < 0)
                    return;
                SetValue(CaretOffsetProperty, value);
                base.CaretOffset = value;
                TextArea.Caret.BringCaretToView();
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => Focus()));
            }
        }

        private int _bindableSelectionLength;
        public int BindableSelectionLength
        {
            get => _bindableSelectionLength;
            set
            {
                _bindableSelectionLength = value;
                SetValue(BindableSelectionLengthProperty, _bindableSelectionLength);
            }
        }

        #endregion

        #region Constructor

        public PdfTextEditor()
        {
            TextArea.Caret.PositionChanged += CaretOnPositionChanged;
            TextArea.SelectionChanged += TextAreaOnSelectionChanged;
            Focusable = true;
        }

        #endregion

        #region Event handlers

        private void CaretOnPositionChanged(object sender, EventArgs e)
        {
            SetValue(CaretOffsetProperty, base.CaretOffset);
        }

        private void TextAreaOnSelectionChanged(object sender, EventArgs e)
        {
            SetValue(BindableSelectionLengthProperty, SelectionLength);
        } 

        #endregion
    }
}
