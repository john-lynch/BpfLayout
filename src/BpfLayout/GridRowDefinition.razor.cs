using Microsoft.AspNetCore.Components;

namespace BpfLayout
{
    public partial class GridRowDefinition
    {
        [Parameter]
        public string Height
        {
            get;
            set;
        } = "*";

        [Parameter]
        public double? MinHeight
        {
            get;
            set;
        } = default;

        [Parameter]
        public double? MaxHeight
        {
            get;
            set;
        } = default;

        internal override string Size => Height;

        internal override double? MinSize => MinHeight;

        internal override double? MaxSize => MaxHeight;

        internal override string? GetSplitterTrackId(GridSplitter splitter) => splitter.Row;

        protected override void OnInitialized()
        {
            Parent.AddRow(this);
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
                Parent?.RemoveRow(this);
            }
        }
    }
}
