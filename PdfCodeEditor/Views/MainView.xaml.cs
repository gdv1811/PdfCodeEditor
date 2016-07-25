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
using System.Windows.Controls;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;

namespace PdfCodeEditor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView
    {
        public MainView()
        {
            // Load our custom highlighting definition
            IHighlightingDefinition pdfHighlighting;
            using (var s = typeof(MainView).Assembly.GetManifestResourceStream("PdfCodeEditor.Editor.PdfHighlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    pdfHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Pdf", new[] { ".pdf" }, pdfHighlighting);

            InitializeComponent();
        }

        private void OpenMenuItemOnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "PDF|*.pdf",
                CheckFileExists = true
            };
            // todo: fix hack
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                if (dialog.ShowDialog() ?? false)
                    menuItem.CommandParameter = dialog.FileName;
                else
                    menuItem.CommandParameter = null;
                return;
            }

            var button = sender as Button;
            if (button == null)
                return;

            if (dialog.ShowDialog() ?? false)
                button.CommandParameter = dialog.FileName;
            else
                button.CommandParameter = null;
        }

        private void SaveAsMenuItemOnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Filter = "PDF|*.pdf|No extension|*.*"
            };

            // todo: fix hack
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                menuItem.CommandParameter = dlg.ShowDialog().GetValueOrDefault() ? dlg.FileName : null;
                return;
            }

            var button = sender as Button;
            if (button == null)
                return;
            button.CommandParameter = dlg.ShowDialog().GetValueOrDefault() ? dlg.FileName : null;

        }
    }
}
