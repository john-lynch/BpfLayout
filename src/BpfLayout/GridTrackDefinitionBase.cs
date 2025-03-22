using Microsoft.AspNetCore.Components;

namespace BpfLayout
{
    public abstract class GridTrackDefinitionBase : ComponentBase
    {
        [CascadingParameter]
        public Grid Parent
        {
            get;
            set;
        } = default!;

        [Parameter]
        public int? SortOrder
        {
            get;
            set;
        } = default;

        [Parameter]
        public string? Name
        {
            get;
            set;
        } = default;

        internal abstract string Size { get; }

        internal abstract double? MinSize { get; }

        internal abstract double? MaxSize { get; }

        internal abstract int GetSplitterIndex(GridSplitter splitter);
    }
}
