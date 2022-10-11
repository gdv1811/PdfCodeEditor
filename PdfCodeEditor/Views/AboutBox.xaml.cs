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
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace PdfCodeEditor.Views
{
    public partial class AboutBox : Window
    {
        private bool _isClosing;

        public AboutBox()
        {
            InitializeComponent();
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var infoDict = GetInfoDict(fvi.ProductName, fvi.ProductVersion, fvi.LegalCopyright);
            AboutEditor.Text = "%PDF-1.5\n" +
                               "%µí®û\n" +
                               "\n" +
                               infoDict +
                               "xref\n" +
                               "1811 1\n" +
                               "0000000016 65535 f\n" +
                               "trailer\n" +
                               "<< /Info 1811 0 R /Size 1 >>\n" +
                               "startxref\n" +
                               $"{infoDict.Length + 16}\n" +
                               "%%EOF";

            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => AboutEditor.TextArea.Caret.Hide()));
        }

        private string GetInfoDict(string tool, string vers, string copyright)
        {
            return "1811 0 obj\n" +
                   "<<\n" +
                   $"   /Tool          ({tool})\n" +
                   $"   /Version       ({vers})\n" +
                   $"   /License       /MIT\n" +
                   $"   /Copyright     ({copyright})\n" +
                   $"   /Source        (https://github.com/gdv1811/PdfCodeEditor)" +
                   ">>\n" +
                   "endobj\n\n";
        }

        private void OkButtonOnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AboutBoxOnDeactivated(object sender, EventArgs e)
        {
            if (!_isClosing)
                Close();
        }

        private void AboutBoxOnClosing(object sender, CancelEventArgs e)
        {
            _isClosing = true;
        }
    }
}
