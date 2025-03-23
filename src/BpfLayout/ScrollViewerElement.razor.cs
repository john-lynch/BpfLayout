namespace BpfLayout
{
    public partial class ScrollViewerElement
    {
        protected override bool HorizontalStretchForImplicitWidth => HorizontalAlignment == HorizontalAlignment.Stretch && Parent.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled;

        protected override bool VerticalStretchForImplicitHeight => VerticalAlignment == VerticalAlignment.Stretch && Parent.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled;

        protected override string HorizontalAlignmentCss => Parent.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled ? base.HorizontalAlignmentCss : "start";

        protected override string VerticalAlignmentCss => Parent.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled ? base.VerticalAlignmentCss : "start";

        string MinWidthCss => HorizontalStretch ? "100%" : "0px";

        string MinHeightCss => VerticalStretch ? "100%" : "0px";
    }
}
