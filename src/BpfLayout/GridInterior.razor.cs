using Microsoft.AspNetCore.Components;

namespace BpfLayout
{
    public partial class GridInterior
    {
        [CascadingParameter]
        public Grid Parent
        {
            get;
            set;
        } = default!;

        [Parameter]
        public RenderFragment? ChildContent
        {
            get;
            set;
        }
    }
}
