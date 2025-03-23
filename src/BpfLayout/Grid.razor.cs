using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BpfLayout
{
    public partial class Grid
    {
        static int _lastJsId = 0;

        readonly TrackDefinitions<GridRowDefinition> _rows = new();
        readonly TrackDefinitions<GridColumnDefinition> _columns = new();
        readonly List<GridSplitter> _splitters = [];
        IJSObjectReference? _jsModule = default!;
        DotNetObjectReference<Grid>? _jsGrid = default!;
        bool _splittersDirty = false;
        readonly int _jsId = ++_lastJsId;

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
        public RenderFragment? GridRowDefinitions
        {
            get;
            set;
        }

        [Parameter]
        public RenderFragment? GridColumnDefinitions
        {
            get;
            set;
        }

        [Parameter]
        public double RowSnapOffset
        {
            get;
            set;
        } = 0.0;

        [Parameter]
        public double ColumnSnapOffset
        {
            get;
            set;
        } = 0.0;

        [Parameter]
        public double RowDragInterval
        {
            get;
            set;
        } = 1.0;

        [Parameter]
        public double ColumnDragInterval
        {
            get;
            set;
        } = 1.0;

        [Parameter]
        public EventCallback<SplitterResizedGridEventArgs> SplitterResizedGrid
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

        internal string StyleCss => $"display: grid; grid-template-rows: {GridTemplateRowsCss}; grid-template-columns: {GridTemplateColumnsCss}; overflow: clip; box-sizing: border-box; width: {RootWidthCss}; height: {RootHeightCss};";

        internal void AddRow(GridRowDefinition row)
        {
            _rows.Add(row);
        }

        internal void AddColumn(GridColumnDefinition column)
        {
            _columns.Add(column);
        }

        internal void RemoveRow(GridRowDefinition row)
        {
            _rows.Remove(row);
        }

        internal void RemoveColumn(GridColumnDefinition column)
        {
            _columns.Remove(column);
        }

        internal void AddSplitter(GridSplitter splitter)
        {
            _splitters.Add(splitter);
            OnSplittersChanged();
        }

        internal void RemoveSplitter(GridSplitter splitter)
        {
            _splitters.Remove(splitter);
            OnSplittersChanged();
        }

        internal bool IsRowSizedToContent(string? rowId, int span) =>
            _rows.IsTrackSizedToContent(rowId, span);

        internal bool IsColumnSizedToContent(string? columnId, int span) =>
            _columns.IsTrackSizedToContent(columnId, span);

        internal int GetGridRowIndex(string? rowId) =>
            _rows.GetTrackIndex(rowId);

        internal int GetGridColumnIndex(string? columnId) =>
            _columns.GetTrackIndex(columnId);

        string GridTemplateRowsCss => _rows.GetGridTemplateCss(_splitters);

        string GridTemplateColumnsCss => _columns.GetGridTemplateCss(_splitters);

        void OnSplittersChanged()
        {
            _splittersDirty = true;
        }

        record SplitterTrackDefinition(int Track, string CssSelector);

        record TrackConstraint(int Track, double Constraint);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_splittersDirty)
            {
                _splittersDirty = false;

                _jsModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BpfLayout/Grid.razor.js");
                _jsGrid ??= DotNetObjectReference.Create(this);

                var rowSplitters = _splitters
                    .Where(splitter => splitter.Row is not null)
                    .Select(splitter => new SplitterTrackDefinition(_rows.GetTrackIndex(splitter.Row), $".{splitter.UniqueClassCssForClipContainer}"))
                    .ToArray();

                var columnSplitters = _splitters
                    .Where(splitter => splitter.Column is not null)
                    .Select(splitter => new SplitterTrackDefinition(_columns.GetTrackIndex(splitter.Column), $".{splitter.UniqueClassCssForClipContainer}"))
                    .ToArray();

                bool hasRowSplitters = rowSplitters.Length > 0;
                bool hasColumnSplitters = columnSplitters.Length > 0;
                var rowMinConstraints = hasRowSplitters ? CreateConstraints(_rows.Ordered, r => r.MinHeight) : [];
                var rowMaxConstraints = hasRowSplitters ? CreateConstraints(_rows.Ordered, r => r.MaxHeight) : [];
                var columnMinConstraints = hasColumnSplitters ? CreateConstraints(_columns.Ordered, c => c.MinWidth) : [];
                var columnMaxConstraints = hasColumnSplitters ? CreateConstraints(_columns.Ordered, c => c.MaxWidth) : [];

                await _jsModule!.InvokeVoidAsync(
                    "updateSplitters",
                    _jsId,
                    _jsGrid,
                    rowSplitters,
                    columnSplitters,
                    rowMinConstraints,
                    rowMaxConstraints,
                    columnMinConstraints,
                    columnMaxConstraints,
                    RowSnapOffset,
                    ColumnSnapOffset,
                    RowDragInterval,
                    ColumnDragInterval);

                // We don't know we have splitters until we render the grid interior, after we've generated
                // the initial grid CSS. Split-Grid works with this initial grid template CSS, but if any of 
                // our splitters are associated with rows that have constraints, we need to rerender so that
                // we can remove the min/max logic from the grid template CSS and let the splitter logic
                // take over the handling of those constraints.
                //
                // NOTE: we don't need to this rerender to re-run the splitter logic. We just need to regenerate
                // the grid template CSS to remove min-max logic that trips the splitter code up.
                if (rowMinConstraints.Length > 0 || rowMaxConstraints.Length > 0
                    || columnMinConstraints.Length > 0 || columnMaxConstraints.Length > 0)
                {
                    StateHasChanged();
                }
            }

            await base.OnAfterRenderAsync(firstRender);

            static TrackConstraint[] CreateConstraints<T>(IEnumerable<T> tracks, Func<T, double?> constraintSelector) => tracks
                        .Zip(Enumerable.Range(0, int.MaxValue), (r, i) => (Constraint: constraintSelector(r), Index: i))
                        .Where(pair => pair.Constraint.HasValue)
                        .Select(pair => new TrackConstraint(pair.Index, pair.Constraint!.Value))
                        .ToArray();
        }

        [JSInvokable]
        public async Task OnSplitterResizedGridAsync(bool isRow, string gridTemplate)
        {
            var overrides = gridTemplate.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (isRow)
            {
                _rows.SetOverrides(overrides);
            }
            else
            {
                _columns.SetOverrides(overrides);
            }

            if (SplitterResizedGrid is var splitterResizedGrid)
            {
                await splitterResizedGrid.InvokeAsync(new SplitterResizedGridEventArgs(
                    Rows: _rows.TracksWithSizeSpecs.Select(e => new RowSizeSpecification(e.Size, e.Track.Name)).ToArray(),
                    Columns: _columns.TracksWithSizeSpecs.Select(e => new ColumnSizeSpecification(e.Size, e.Track.Name)).ToArray()));
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            try
            {
                _jsGrid?.Dispose();

                if (_jsModule is not null)
                {
                    await _jsModule.InvokeVoidAsync("disposeSplitters", _jsId);
                    await _jsModule.DisposeAsync();
                }
            }
            catch
            {

            }
        }
    }
}
