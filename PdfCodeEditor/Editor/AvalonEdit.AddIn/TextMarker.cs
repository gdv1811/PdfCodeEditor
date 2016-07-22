// Copied from
// https://github.com/icsharpcode/SharpDevelop/blob/master/src/AddIns/DisplayBindings/AvalonEdit.AddIn/Src/TextMarkerService.cs

using System;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;

namespace PdfCodeEditor.Editor.AvalonEdit.AddIn
{
    public sealed class TextMarker : TextSegment, ITextMarker
    {
        readonly TextMarkerService service;

        public TextMarker(TextMarkerService service, int startOffset, int length)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            this.service = service;
            this.StartOffset = startOffset;
            this.Length = length;
            this.markerTypes = TextMarkerTypes.None;
        }

        public event EventHandler Deleted;

        public bool IsDeleted
        {
            get { return !this.IsConnectedToCollection; }
        }

        public void Delete()
        {
            service.Remove(this);
        }

        internal void OnDeleted()
        {
            if (Deleted != null)
                Deleted(this, EventArgs.Empty);
        }

        void Redraw()
        {
            service.Redraw(this);
        }

        Color? backgroundColor;

        public Color? BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                if (backgroundColor != value)
                {
                    backgroundColor = value;
                    Redraw();
                }
            }
        }

        Color? foregroundColor;

        public Color? ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                if (foregroundColor != value)
                {
                    foregroundColor = value;
                    Redraw();
                }
            }
        }

        FontWeight? fontWeight;

        public FontWeight? FontWeight
        {
            get { return fontWeight; }
            set
            {
                if (fontWeight != value)
                {
                    fontWeight = value;
                    Redraw();
                }
            }
        }

        FontStyle? fontStyle;

        public FontStyle? FontStyle
        {
            get { return fontStyle; }
            set
            {
                if (fontStyle != value)
                {
                    fontStyle = value;
                    Redraw();
                }
            }
        }

        public object Tag { get; set; }

        TextMarkerTypes markerTypes;

        public TextMarkerTypes MarkerTypes
        {
            get { return markerTypes; }
            set
            {
                if (markerTypes != value)
                {
                    markerTypes = value;
                    Redraw();
                }
            }
        }

        Color markerColor;

        public Color MarkerColor
        {
            get { return markerColor; }
            set
            {
                if (markerColor != value)
                {
                    markerColor = value;
                    Redraw();
                }
            }
        }

        public object ToolTip { get; set; }
    }
}