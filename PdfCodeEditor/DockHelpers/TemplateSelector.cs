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

using AvalonDock.Layout;
using System.Windows.Controls;
using System.Windows;
using PdfCodeEditor.ViewModels;

namespace PdfCodeEditor.DockHelpers
{
    internal class TemplateSelector : DataTemplateSelector
    {
        public TemplateSelector()
        {
        }


        public DataTemplate PdfDocumentViewTemplate
        {
            get;
            set;
        }

        public DataTemplate PdfTreeViewTemplate
        {
            get;
            set;
        }

        public DataTemplate ContentToolViewTemplate
        {
            get;
            set;
        }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is PdfDocumentViewModel)
                return PdfDocumentViewTemplate;

            if (item is PdfTreeViewModel)
                return PdfTreeViewTemplate;

            if (item is ContentToolViewModel)
                return ContentToolViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
