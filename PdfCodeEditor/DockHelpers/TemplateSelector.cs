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

        //public DataTemplate FileStatsViewTemplate
        //{
        //    get;
        //    set;
        //}

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is PdfDocumentViewModel)
                return PdfDocumentViewTemplate;

            //if (item is FileStatsViewModel)
            //    return FileStatsViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
