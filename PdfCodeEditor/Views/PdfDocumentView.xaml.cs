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

namespace PdfCodeEditor.Views
{
    /// <summary>
    /// Interaction logic for PdfDocumentView.xaml
    /// </summary>
    public partial class PdfDocumentView
    {
        private readonly FoldingStrategy _foldingStrategy = new FoldingStrategy();
        //private readonly Timer _timerOfUpdateFoldings;

        #region Constructor

        public PdfDocumentView()
        {
            InitializeComponent();

            TextEditor.TextArea.Caret.PositionChanged += CaretOnPositionChanged;
            TextEditor.TextArea.PreviewMouseDown += TextAreaOnPreviewMouseDown;
            TextEditor.TextArea.PreviewKeyDown += TextAreaOnPreviewKeyDown;
            TextEditor.TextArea.SelectionChanged += TextAreaOnSelectionChanged;

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

            TextEditor.DocumentChanged += (sender, args) => UpdateFoldings();

            //_timerOfUpdateFoldings = new Timer(1000) { AutoReset = false };
            //_timerOfUpdateFoldings.Elapsed += (sender, args) => UpdateFoldings();

            TextEditor.TextArea.TextEntered += TextAreaOnTextEntered;
        }

        private void TextAreaOnTextEntered(object sender, TextCompositionEventArgs e)
        {
            UpdateFoldings();
            //_timerOfUpdateFoldings.Start();
        }

        private void UpdateFoldings()
        {
            _foldingStrategy.FoldingManager = _foldingStrategy.FoldingManager ?? FoldingManager.Install(TextEditor.TextArea);
            _foldingStrategy.UpdateFoldings(TextEditor.Document);
        }

        #endregion

        #region Private methods

        private string GetReference(int carreteOffset)
        {
            var spacesCount = 0;
            var token = "";
            var step = 1;
            for (var currentOffset = carreteOffset; ; currentOffset += step)
            {
                var currentChar = TextEditor.Document.GetCharAt(currentOffset);
                if (currentChar == 'R')
                {
                    token += currentChar;
                    currentOffset = carreteOffset;
                    step = -1;
                    continue;
                }
                if (currentChar == ' ')
                {
                    spacesCount++;
                    if (spacesCount == 3)
                        break;
                }
                else if (!char.IsDigit(currentChar))
                    break;

                if (step == 1)
                    token += currentChar;
                else
                    token = currentChar + token;
            }

            return spacesCount > 1 ? token : null;
        }

        private void GoToDefinition(string reference)
        {
            var definition = reference.Replace("R", "obj");
            var regex = new Regex("[^0-9]" + definition);
            var match = regex.Match(TextEditor.Text);

            if (!match.Success)
                return;
            TextEditor.Select(match.Index + 1, definition.Length);
            TextEditor.TextArea.Caret.BringCaretToView();
        }

        #endregion

        #region Event handlers

        private void TextAreaOnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            Offset.Content = TextEditor.CaretOffset;
            if (keyEventArgs.Key != Key.F12)
                return;
            var reference = GetReference(TextEditor.CaretOffset);
            if (reference == null)
                return;
            GoToDefinition(reference);
        }

        private void CaretOnPositionChanged(object sender, EventArgs eventArgs)
        {
            Offset.Content = TextEditor.CaretOffset;
        }

        private void TextAreaOnSelectionChanged(object sender, EventArgs e)
        {
            Selected.Content = TextEditor.TextArea.Selection.Length;
        }

        private void TextAreaOnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || Keyboard.Modifiers != ModifierKeys.Control)
                return;

            var position = TextEditor.GetPositionFromPoint(e.GetPosition(TextEditor));
            if (position == null)
                return;
            var offset = TextEditor.Document.GetOffset(position.Value.Location);
            var reference = GetReference(offset);
            if (reference == null)
                return;

            GoToDefinition(reference);
            e.Handled = true;
        }

        #endregion
    }
}
