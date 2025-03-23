using Microsoft.AspNetCore.Components;

namespace BpfLayout
{
    public partial class GridSplitter
    {
        static string GetDefaultGripStyle(string imageName) => $"background-color: rgb(229, 231, 235); background-repeat: no-repeat; background-position: 50%; background-image: url(./_content/BpfLayout/{imageName}.png);";

        bool IsHorizontal => Row is not null;

        string CursorMode => IsHorizontal ? "row-resize" : "col-resize";

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            ChildContent ??= IsHorizontal ? DefaultHorizontalGrip : DefaultVerticalGrip;
        }

        protected override void OnInitialized()
        {
            Parent.AddSplitter(this);
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
                Parent?.RemoveSplitter(this);
            }
        }
    }
}
