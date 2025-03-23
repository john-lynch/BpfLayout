using Microsoft.AspNetCore.Components;

namespace BpfLayout
{
    public partial class GridElement
    {
        [Parameter]
        public string? Row
        {
            get;
            set;
        } = default;

        [Parameter]
        public string? Column
        {
            get;
            set;
        } = default;

        [Parameter]
        public int RowSpan
        {
            get;
            set;
        } = 1;

        [Parameter]
        public int ColumnSpan
        {
            get;
            set;
        } = 1;

        protected override bool HorizontalStretchForImplicitWidth => HorizontalAlignment == HorizontalAlignment.Stretch && !Parent.IsColumnSizedToContent(Column, ColumnSpan);

        protected override bool VerticalStretchForImplicitHeight => VerticalAlignment == VerticalAlignment.Stretch && !Parent.IsRowSizedToContent(Row, RowSpan);

        int GridRowStartCss => Parent.GetGridRowIndex(Row) + 1;

        int GridRowSpanCss => RowSpan;

        int GridColumnStartCss => Parent.GetGridColumnIndex(Column) + 1;

        int GridColumnSpanCss => ColumnSpan;
    }
}
