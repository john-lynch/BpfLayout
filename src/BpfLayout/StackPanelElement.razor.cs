namespace BpfLayout
{
    public partial class StackPanelElement
    {
        protected override bool HorizontalStretchForImplicitWidth => HorizontalAlignment == HorizontalAlignment.Stretch && Parent.Orientation != Orientation.Horizontal;

        protected override bool VerticalStretchForImplicitHeight => VerticalAlignment == VerticalAlignment.Stretch && Parent.Orientation != Orientation.Vertical;

        string AlignmentCss => Parent.Orientation == Orientation.Horizontal
            ? VerticalAlignmentCss
            : HorizontalAlignmentCss;
    }
}
