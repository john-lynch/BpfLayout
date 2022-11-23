using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BpfLayout
{
	public abstract class ElementBase<T> : ComponentBase
    {
        Guid _uid = Guid.NewGuid();

        protected string UniqueClassCss => $"bpf-layout-uid-{_uid}";

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

        protected abstract bool HorizontalStretch { get; }

        protected abstract bool VerticalStretch { get; }

        protected string ChildWidthCss => Width is not null || HorizontalStretch
            ? "100%"
            : "fit-content";

        protected string ChildHeightCss => Height is not null || VerticalStretch
            ? "100%"
            : "fit-content";

        protected string WidthCss => Width is double w
            ? $"{w}px"
            : (HorizontalStretch ? "100%" : "fit-content");

        protected string HeightCss => Height is double h
            ? $"{h}px"
            : (VerticalStretch ? "100%" : "fit-content");

        protected string HorizontalAlignmentCss => HorizontalAlignment switch
        {
            HorizontalAlignment.Left => "flex-start",
            HorizontalAlignment.Center => "center",
            HorizontalAlignment.Right => "flex-end",
            HorizontalAlignment.Stretch when Width is not null => "center",
            HorizontalAlignment.Stretch => "stretch",
            _ => throw new ArgumentOutOfRangeException(nameof(HorizontalAlignment))
        };

        protected string VerticalAlignmentCss => VerticalAlignment switch
        {
            VerticalAlignment.Top => "flex-start",
            VerticalAlignment.Center => "center",
            VerticalAlignment.Bottom => "flex-end",
            VerticalAlignment.Stretch when Height is not null => "center",
            VerticalAlignment.Stretch => "stretch",
            _ => throw new ArgumentOutOfRangeException(nameof(VerticalAlignment))
        };

        protected string MarginCss => $"{Margin.Top} {Margin.Right} {Margin.Bottom} {Margin.Left}";
    }
}
