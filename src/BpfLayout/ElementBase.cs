using Microsoft.AspNetCore.Components;

namespace BpfLayout
{
    public abstract class ElementBase<T> : ComponentBase
    {
        Guid _uid = Guid.NewGuid();

        internal string UniqueClassCssForClipContainer => $"bpf-layout-clip-{_uid}";

        internal string UniqueClassCssForMarginContainer => $"bpf-layout-margin-{_uid}";

        [Parameter]
        public double? Width
        {
            get;
            set;
        }

        [Parameter]
        public double? Height
        {
            get;
            set;
        }

        [Parameter]
        public Thickness Margin
        {
            get;
            set;
        } = new();

        [Parameter]
        public HorizontalAlignment HorizontalAlignment
        {
            get;
            set;
        } = HorizontalAlignment.Stretch;

        [Parameter]
        public VerticalAlignment VerticalAlignment
        {
            get;
            set;
        } = VerticalAlignment.Stretch;

        [CascadingParameter]
        public T Parent
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

        protected bool ImplicitWidth => Width is null;

        protected bool ImplicitHeight => Height is null;

        protected abstract bool HorizontalStretchForImplicitWidth { get; }

        protected abstract bool VerticalStretchForImplicitHeight { get; }

        protected string ElementWidthCss => Width is double w
            ? $"{w}px"
            : (HorizontalStretchForImplicitWidth ? "100%" : "max-content");

        protected string ElementHeightCss => Height is double h
            ? $"{h}px"
            : (VerticalStretchForImplicitHeight ? "100%" : "max-content");

        protected bool HorizontalStretch => ImplicitWidth && HorizontalStretchForImplicitWidth;

        protected bool VerticalStretch => ImplicitHeight && VerticalStretchForImplicitHeight;

        protected string ContainerWidthCss => HorizontalStretch ? "100%" : "max-content";

        protected string ContainerHeightCss => VerticalStretch ? "100%" : "max-content";

        protected virtual string HorizontalAlignmentCss => HorizontalAlignment switch
        {
            HorizontalAlignment.Left => "start",
            HorizontalAlignment.Center => "center",
            HorizontalAlignment.Right => "end",
            HorizontalAlignment.Stretch when Width is not null => "center",
            HorizontalAlignment.Stretch => "stretch",
            _ => throw new ArgumentOutOfRangeException(nameof(HorizontalAlignment))
        };

        protected virtual string VerticalAlignmentCss => VerticalAlignment switch
        {
            VerticalAlignment.Top => "start",
            VerticalAlignment.Center => "center",
            VerticalAlignment.Bottom => "end",
            VerticalAlignment.Stretch when Height is not null => "center",
            VerticalAlignment.Stretch => "stretch",
            _ => throw new ArgumentOutOfRangeException(nameof(VerticalAlignment))
        };

        protected string MarginCss => $"{Margin.Top}px {Margin.Right}px {Margin.Bottom}px {Margin.Left}px";
    }
}
