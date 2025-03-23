using Microsoft.AspNetCore.Components;

namespace BpfLayout
{
    public partial class StackPanel
    {
        [Parameter]
        public string RootWidthCss
        {
            get;
            set;
        } = "100%";

        [Parameter]
        public string RootHeightCss
        {
            get;
            set;
        } = "100%";

        [Parameter]
        public Orientation Orientation
        {
            get;
            set;
        }

        [Parameter]
        public RenderFragment? ChildContent
        {
            get;
            set;
        }

        string StyleCss => $"display: flex; flex-direction: {FlexDirectionCss}; flex-wrap: nowrap; overflow: clip; box-sizing: border-box; width: {RootWidthCss}; height: {RootHeightCss}";

        string FlexDirectionCss => Orientation switch
        {
            Orientation.Horizontal => "row",
            Orientation.Vertical => "column",
            _ => throw new ArgumentOutOfRangeException(nameof(Orientation))
        };
    }
}
