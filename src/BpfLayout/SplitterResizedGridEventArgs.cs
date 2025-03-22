namespace BpfLayout
{
    public record SplitterResizedGridEventArgs(
        IReadOnlyCollection<RowSizeSpecification> Rows,
        IReadOnlyCollection<ColumnSizeSpecification> Columns);

    public record RowSizeSpecification(string Height, string? Name);

    public record ColumnSizeSpecification(string Width, string? Name);
}
