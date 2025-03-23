using Microsoft.AspNetCore.Components;

namespace BpfLayout
{
    public partial class ScrollViewer
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
        public RenderFragment? ChildContent
        {
            get;
            set;
        }

        [Parameter]
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get;
            set;
        } = ScrollBarVisibility.Disabled;

        [Parameter]
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get;
            set;
        } = ScrollBarVisibility.Auto;

        string StyleCss => $"display: grid; grid-template-rows: 100%; grid-template-columns: 100%; overflow-x: {OverflowXCss}; overflow-y: {OverflowYCss}; box-sizing: border-box; width: {RootWidthCss}; height: {RootHeightCss};";

        string OverflowXCss => GetOverflowCss(HorizontalScrollBarVisibility);

        string OverflowYCss => GetOverflowCss(VerticalScrollBarVisibility);

        static string GetOverflowCss(ScrollBarVisibility visibility)
        {
            return visibility switch
            {
                ScrollBarVisibility.Disabled => "clip",
                ScrollBarVisibility.Auto => "auto",
                ScrollBarVisibility.Hidden => "hidden",
                ScrollBarVisibility.Visible => "scroll",
                _ => throw new ArgumentOutOfRangeException(nameof(visibility))
            };
        }
    }
}
