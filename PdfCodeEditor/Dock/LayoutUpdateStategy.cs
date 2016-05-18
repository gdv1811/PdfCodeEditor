using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PdfCodeEditor.ViewModels;
using Xceed.Wpf.AvalonDock.Layout;

namespace PdfCodeEditor.Dock
{
    internal class LayoutUpdateStrategy : ILayoutUpdateStrategy
    {
        private bool BeforeInsertContent(LayoutRoot layout, LayoutContent anchorableToShow)
        {
            var viewModel = (ViewModelBase) anchorableToShow.Content;
            var layoutContent =
                layout.Descendents().OfType<LayoutContent>().FirstOrDefault(x => x.ContentId == viewModel.ContextId);
            if (layoutContent == null)
                return false;
            layoutContent.Content = anchorableToShow.Content;
            // Add layoutContent to it's previous container
            var layoutContainer =
                layoutContent.GetType()
                    .GetProperty("PreviousContainer", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(layoutContent, null) as ILayoutContainer;
            if (layoutContainer is LayoutAnchorablePane)
                (layoutContainer as LayoutAnchorablePane).Children.Add(layoutContent as LayoutAnchorable);
            else if (layoutContainer is LayoutDocumentPane)
                (layoutContainer as LayoutDocumentPane).Children.Add(layoutContent);
            else
                throw new NotSupportedException();
            return true;
        }

        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            return BeforeInsertContent(layout, anchorableToShow);
        }
        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown) { }
        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return BeforeInsertContent(layout, anchorableToShow);
        }
        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown) { }
    }
}
