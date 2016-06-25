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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Folding;
using PdfCodeEditor.Editor;
using PdfCodeEditor.Models;

namespace PdfCodeEditor.Views
{
    /// <summary>
    /// Interaction logic for PdfDocumentView.xaml
    /// </summary>
    public partial class PdfDocumentView
    {
        #region Fields

        private readonly FoldingStrategy _foldingStrategy = new FoldingStrategy();
        //private readonly Timer _timerOfUpdateFoldings;
        
        #endregion

        #region Constructor

        public PdfDocumentView()
        {
            InitializeComponent();
            
            _foldingStrategy.FoldingTemplates = new List<FoldingTemplate>
            {
                new FoldingTemplate
                {
                    OpeningPhrase = "[^a-zA-Z0-9]stream[^a-zA-Z0-9]",
                    ClosingPhrase = "[^a-zA-Z0-9]endstream[^a-zA-Z0-9]",
                    IsDefaultFolded = true,
                    Name = "stream"
                }
            };

            Editor.DocumentChanged += (sender, args) => UpdateFoldings();

            //_timerOfUpdateFoldings = new Timer(1000) { AutoReset = false };
            //_timerOfUpdateFoldings.Elapsed += (sender, args) => UpdateFoldings();

            Editor.TextArea.TextEntered += TextAreaOnTextEntered;
        }
        
        #endregion

        #region Private methods

        private void UpdateFoldings()
        {
            _foldingStrategy.FoldingManager = _foldingStrategy.FoldingManager ?? FoldingManager.Install(Editor.TextArea);
            _foldingStrategy.UpdateFoldings(Editor.Document);
        }
        

        #endregion

        #region Event handlers

        private void TextAreaOnTextEntered(object sender, TextCompositionEventArgs e)
        {
            UpdateFoldings();
            //_timerOfUpdateFoldings.Start();
        }

        //private void TextAreaOnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        //{
        //    if (keyEventArgs.Key != Key.F12)
        //        return;
        //    var reference = GetReference(Editor.CaretOffset);
        //    if (reference == null)
        //        return;
        //    AddPositionInHistory(Editor.CaretOffset);
        //    GoToDefinition(reference);
        //}
        
        //private void TextAreaOnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ChangedButton != MouseButton.Left || Keyboard.Modifiers != ModifierKeys.Control)
        //        return;

        //    var position = Editor.GetPositionFromPoint(e.GetPosition(Editor));
        //    if (position == null)
        //        return;
        //    var offset = Editor.Document.GetOffset(position.Value.Location);
        //    var reference = GetReference(offset);
        //    if (reference == null)
        //        return;

        //    AddPositionInHistory(offset);
        //    GoToDefinition(reference);
        //    e.Handled = true;
        //}
        
        #endregion
        
    }
}
