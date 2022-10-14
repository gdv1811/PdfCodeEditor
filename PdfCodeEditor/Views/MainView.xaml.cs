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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;

namespace PdfCodeEditor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView
    {
        private string _dockSettingsPath = @".\dockSettings.config";
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

            this.Loaded += MainViewOnLoaded;
            this.Closed += MainViewOnClosed;
        }

        private void MainViewOnClosed(object sender, EventArgs e)
        {
            var serializer = new AvalonDock.Layout.Serialization.XmlLayoutSerializer(DockManager);
            serializer.Serialize(_dockSettingsPath);
        }

        private void MainViewOnLoaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(_dockSettingsPath))
                return;

            var serializer = new AvalonDock.Layout.Serialization.XmlLayoutSerializer(DockManager);
            serializer.LayoutSerializationCallback += (s, args) =>
            {
                args.Content = args.Content;
                if (args.Model.ContentId == "Object tree")
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        new Action(() => ObjectTreeButton.IsChecked = true));
            };

            serializer.Deserialize(_dockSettingsPath);
        }

        private void TextBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OffsetTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                e.Handled = true;
            }
        }

        private void AboutMenuItemOnClick(object sender, RoutedEventArgs e)
        {
            var aboutBox = new AboutBox();
            aboutBox.Show();
        }
    }
}
