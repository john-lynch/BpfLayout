using Microsoft.AspNetCore.Components;

namespace BpfLayout
{
    public partial class GridColumnDefinition
    {
        [Parameter]
        public string Width
        {
            get;
            set;
        } = "*";

        [Parameter]
        public double? MinWidth
        {
            get;
            set;
        } = default;

        [Parameter]
        public double? MaxWidth
        {
            get;
            set;
        } = default;

        internal override string Size => Width;

        internal override double? MinSize => MinWidth;

        internal override double? MaxSize => MaxWidth;

        internal override string? GetSplitterTrackId(GridSplitter splitter) => splitter.Column;

        protected override void OnInitialized()
        {
            Parent.AddColumn(this);
            base.OnInitialized();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Parent?.RemoveColumn(this);
            }
        }
    }
}
