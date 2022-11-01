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

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Search;
using PdfCodeEditor.Editor;
using PdfCodeEditor.Editor.AvalonEdit.AddIn;

namespace PdfCodeEditor.Views
{
    /// <summary>
    /// Interaction logic for PdfDocumentView.xaml
    /// </summary>
    public partial class PdfDocumentView
    {
        #region Fields

        private readonly FoldingStrategy _foldingStrategy = new FoldingStrategy();
        private TextMarkerService _colorTransformer;
        private ITextMarker _objectRefMarker;

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

            Editor.DocumentChanged += (sender, args) =>
            {
                _foldingStrategy.UpdateManager(Editor.TextArea);
                _foldingStrategy.UpdateFoldings(Editor.Document);
                _colorTransformer = new TextMarkerService(Editor.Document);
                Editor.TextArea.TextView.BackgroundRenderers.Add(_colorTransformer);
                Editor.TextArea.TextView.LineTransformers.Add(_colorTransformer);
            };

            Editor.TextArea.TextEntered += TextAreaOnTextEntered;
            Editor.TextArea.PreviewMouseDown += TextAreaOnPreviewMouseDown;
            Editor.TextArea.MouseMove += TextAreaOnMouseMove;
            Editor.TextArea.MouseRightButtonDown += TextAreaOnMouseRightButtonDown;
            Editor.Options.AllowToggleOverstrikeMode = true;
            SearchPanel.Install(Editor);
        }

        #endregion

        #region Private methods
        
        private void HighlightObjectRef(TextViewPosition? position)
        {
            if (position == null)
                return;

            if (_objectRefMarker != null)
                _colorTransformer.Remove(_objectRefMarker);
            var line = Editor.Document.GetLineByNumber(position.Value.Line);
            var regex = new Regex(@"[^a-zA-Z0-9]\d+\s\d+\sR[^a-zA-Z0-9]");
            var matches = regex.Matches(Editor.Document.GetText(line));

            foreach (Match match in matches)
            {
                if (match.Index < position.Value.Location.Column &&
                    match.Index + match.Length > position.Value.Location.Column)
                {
                    _objectRefMarker = _colorTransformer.Create(line.Offset + match.Index + 1,
                        match.Length - 2);
                    _objectRefMarker.MarkerTypes = TextMarkerTypes.NormalUnderline;
                    _objectRefMarker.MarkerColor = Colors.Blue;
                }
            }
        }

        #endregion

        #region Event handlers

        private void TextAreaOnMouseMove(object sender, MouseEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                var position = Editor.GetPositionFromPoint(e.GetPosition(Editor));
                HighlightObjectRef(position);
            }
            else
            {
                if (_objectRefMarker != null)
                    _colorTransformer.Remove(_objectRefMarker);
            }
        }

        private void TextAreaOnTextEntered(object sender, TextCompositionEventArgs e)
        {
            _foldingStrategy.UpdateFoldings(Editor.Document);
        }

        private void TextAreaOnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    if (Keyboard.Modifiers != ModifierKeys.Control)
                        break;

                    var position = Editor.GetPositionFromPoint(e.GetPosition(Editor));
                    if (position == null)
                        return;
                    Editor.CaretOffset = Editor.Document.GetOffset(position.Value.Location);

                    GoToDefinitionMouseBinding.Command.Execute(GoToDefinitionMouseBinding.CommandParameter);

                    e.Handled = true;
                    break;
                case MouseButton.XButton1:
                    BackwardKeyBinding.Command.Execute(null);
                    break;
                case MouseButton.XButton2:
                    ForwardKeyBinding.Command.Execute(null);
                    break;
            }
        }

        private void TextAreaOnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = Editor.GetPositionFromPoint(e.GetPosition(Editor));
            if (position.HasValue)
            {
                Editor.TextArea.Caret.Position = position.Value;
            }
        }

        #endregion

    }
}
