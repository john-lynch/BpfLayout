using System.Data;

namespace BpfLayout
{
    internal class TrackDefinitions<T> where T : GridTrackDefinitionBase
    {
        readonly List<T> _unordered = [];
        readonly Dictionary<string, string> _templateOverrideByName = [];
        readonly List<string?> _templateOverrideByIndex = [];

        IEnumerable<string?> TrackGridTemplateByIndex => _templateOverrideByIndex.Concat(Enumerable.Repeat(default(string?), int.MaxValue));

        IEnumerable<string> GridTemplateCssSizes
        {
            get
            {
                if (!Ordered.Any())
                    return ["1fr"];

                var unnormalized = Ordered
                    .Zip(TrackGridTemplateByIndex, (t, o) => (Track: t, Override: o))
                    .Select(e => GetGridTemplateCssSize(e.Track, e.Override))
                    .ToArray();

                var normalizationFactor = unnormalized
                    .Select(template => template.EndsWith("fr") ? double.Parse(template[0..^2]) : 0.0)
                    .Sum();

                return normalizationFactor < 1.0
                    ? unnormalized
                        .Select(template => template.EndsWith("fr") ? $"{double.Parse(template[0..^2]) / normalizationFactor}fr" : template)
                    : unnormalized;
            }
        }

        public void Add(T definition) => _unordered.Add(definition);

        public void Remove(T definition) => _unordered.Remove(definition);

        // OrderBy is used because it is a stable sort so that columns without explicit sort
        // order will maintain their relative position.
        public IEnumerable<T> Ordered => _unordered.OrderBy(track => track.SortOrder ?? 0);

        public IEnumerable<(T Track, string Size)> TracksWithSizeSpecs =>
            Ordered
                .Zip(GridTemplateCssSizes, (t, s) => (t, GetSizeSpecFromCss(s)));

        public int GetTrackIndex(string? trackId) =>
            int.TryParse(trackId ?? "0", out var index) ? index : Ordered.Count(t => t.Name != trackId);

        public bool IsTrackSizedToContent(string? trackId, int span) =>
            Ordered.Skip(GetTrackIndex(trackId)).Take(span).Any(t => string.Equals(t.Size.Trim(), "auto", StringComparison.OrdinalIgnoreCase));

        public void SetOverrides(string[] templateOverrides)
        {
            int addCount = templateOverrides.Length - _templateOverrideByIndex.Count;
            if (addCount > 0)
            {
                _templateOverrideByIndex.AddRange(Enumerable.Repeat(default(string?), addCount));
            }

            foreach ((string templateOverride, int i, T track) in templateOverrides.Zip(Enumerable.Range(0, int.MaxValue), Ordered))
            {
                _templateOverrideByIndex[i] = templateOverride;
                if (track.Name is not null)
                {
                    _templateOverrideByName[track.Name] = templateOverride;
                }
            }
        }

        public string GetGridTemplateCss(IEnumerable<GridSplitter> splitters) =>
            string.Join(
                ' ',
                Ordered
                    .Zip(Enumerable.Range(0, int.MaxValue), GridTemplateCssSizes)
                    .Select(e => UpdateCssSizeForMinMax(e.First, e.Third, splitters.Any(s => GetTrackIndex(e.First.GetSplitterTrackId(s)) is var i && (i == e.Second - 1 || i == e.Second + 1)))));

        string GetGridTemplateCssSize(T track, string? indexTemplateOverride)
        {
            // If the track has a name, identify the override by name. This makes it easier
            // to persist track sizes across layouts that are changing dynamically.
            var templateOverride = track.Name is not null && _templateOverrideByName.TryGetValue(track.Name, out string? idTemplateOverride)
                ? idTemplateOverride
                : indexTemplateOverride;

            return templateOverride ?? GetGridTemplateElementCss(track.Size);
        }

        static string UpdateCssSizeForMinMax(T track, string size, bool nextToSplitter)
        {
            // If this track is next to a splitter, we have to let the splitter code handle the min
            // and max logic.
            if (nextToSplitter || (track.MinSize is null && track.MaxSize is null))
            {
                return size;
            }

            string? min = track.MinSize is not null ? $"{track.MinSize}px" : null;
            string? max = track.MaxSize is not null ? $"{track.MaxSize}px" : null;

            return $"minmax({min ?? size}, {max ?? size})";
        }

        static string GetGridTemplateElementCss(string sizeSpec)
        {
            var size = sizeSpec.Trim();
            if (string.Equals(size, "auto", StringComparison.OrdinalIgnoreCase))
            {
                return "max-content";
            }

            if (size.EndsWith('*'))
            {
                var frsStr = size[0..^1];
                var frs = frsStr.Length > 0 ? double.Parse(frsStr) : 1.0;
                return $"{frs}fr";
            }

            return $"{double.Parse(size)}px";
        }

        static string GetSizeSpecFromCss(string sizeCss)
        {
            var size = sizeCss.Trim();
            if (string.Equals(size, "max-content", StringComparison.OrdinalIgnoreCase))
            {
                return "auto";
            }

            var numStr = size[0..^2];
            var num = double.Parse(numStr);
            return size.EndsWith("fr")
                ? $"{num}*"
                : (size.EndsWith("px")
                    ? num.ToString()
                    : throw new Exception($"Unexpected CSS grid track size specification: {sizeCss}"));
        }
    }
}
