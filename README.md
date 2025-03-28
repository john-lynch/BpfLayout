# BpfLayout
A Blazor component library attempting to replicate WPF layout panels like Grid, StackPanel, ScrollViewer, and GridSplitter in Blazor.

## Why?

As a programmer familiar with how layout works in WPF, moving to Blazor with its HTML/CSS-based layout was incredibly frustrating. Elements were constantly resizing themselves when I didn't expect them to, things wouldn't scroll as I expected, etc. I wanted something that could give me the control I had with WPF in Blazor.

## Goals
* Replicate as closely as possible the layout elements in WPF so that their use in Blazor would be intuitive to a WPF programmer.
* Work out-of-the-box with arbitrary child elements, especially in other component libraries like MudBlazor.
* Make it obvious why a layout is behaving the way that it is. Minimize the need to go hunting around in browser debugging tools.

## Challenges

There are a couple of key challenges, beyond simply mapping WPF layout behavior to HTML/CSS, when trying to replicate WPF functionality in HTML/CSS:
1. Blazor components are very weakly opinionated. They can contain effectively arbitrary HTML/CSS, and do not provide anything like the base functionality that `FrameworkElement` provides in WPF. As a result, getting a bunch of disparate components to play by similar layout rules is challenging.
2. Blazor has no notion of attached properties like WPF does. You cannot natively attach, for example, `Grid` properties to an arbitrary component or piece of HTML/CSS in Blazor.

## Solutions

To solve the above problems, BpfLayout adopts the following convention:

Each layout panel has two parts: the panel itself, that provides the properties that the equivalent panel in WPF provides, and a wrapper "element" component used to host a single arbitrary Blazor component or HTML/CSS. This wrapper component provides the parameters that `FrameworkElement` provides in WPF and attempts to impart them (using some internal CSS) to the hosted Blazor component or HTML/CSS.

## Examples

For example, here is how a `Grid` works in BpfLayout ([source](tests/BpfLayoutCoreTests/Pages/IntroExample.razor)):

```
<Grid RootWidthCss="800px" RootHeightCss="600px">
    <GridRowDefinitions>
        <GridRowDefinition Height="100" />
        <GridRowDefinition Height="*" />
    </GridRowDefinitions>
    <GridColumnDefinitions>
        <GridColumnDefinition Width="auto" />
        <GridColumnDefinition Width="*" />
    </GridColumnDefinitions>
    <ChildContent>
        <GridElement Row="0" Column="0" HorizontalAlignment="HorizontalAlignment.Center" VerticalAlignment="VerticalAlignment.Center">
            <div>
                BpfLayout
            </div>
        </GridElement>
        <GridElement Row="0" Column="1" HorizontalAlignment="HorizontalAlignment.Center" VerticalAlignment="VerticalAlignment.Center">
            <div>
                Some nonsense text to show layout examples
            </div>
        </GridElement>
        <GridElement Row="1" Column="0">
            <StackPanel Orientation="Orientation.Vertical">
                <StackPanelElement HorizontalAlignment="HorizontalAlignment.Left" Margin="@(new Thickness(4.0, 10.0))">
                    <a href="https://github.com/john-lynch/BpfLayout">BpfLayout on Github</a>
                </StackPanelElement>
                <StackPanelElement HorizontalAlignment="HorizontalAlignment.Left" Margin="@(new Thickness(4.0, 10.0))">
                    <a href="https://www.nuget.org/packages/BpfLayout">BpfLayout on Nuget</a>
                </StackPanelElement>
            </StackPanel>
        </GridElement>
        <GridElement Row="1" Column="1">
            <ScrollViewer>
                <ScrollViewerElement>
                    <div>
                        <p>
                            Lorem ipsum dolor sit amet, consectetur adipiscing elit...
                        </p>
                        <p>
                            Nullam dignissim eros id tempor ornare...
                        </p>
                        <p>
                            Phasellus vel auctor justo...
                        </p>
                        <p>
                            Donec ipsum nunc, placerat dictum egestas ultrices, ornare sit amet elit...
                        </p>
                        <p>
                            Morbi ut volutpat velit, tincidunt ultrices massa...
                        </p>
                    </div>
                </ScrollViewerElement>
            </ScrollViewer>
        </GridElement>
    </ChildContent>
</Grid>
```
Here we see a `Grid` with two rows and two columns, demonstrating the various sizing options for rows and columns. How does this differ from a WPF `Grid`?
* Note the presence of the `RootWidthCss` and `RootHeightCss` properties on the `Grid` component. This lets the user specify a CSS-based width and height for a panel component that is not nested inside other BpfLayout panels, then turns layout over to the BPF layout system. If `RootWidthCss` or `RootHeightCss` are specified on a panel nested inside another panel, the panel's element width and height properties will override it. `RootWidthCss` and `RootHeightCss` default to `100%` so that in most contexts, the user will not even need to specify root sizes, the panel will simply consume all available space and turn subsequent layout over to the BpfLayout system.
* The `Grid` itself does not have the standard `FrameworkElement` properties for dimension or alignment. All `FrameworkElement` properties are present on the `*Element` classes, like `StackPanelElement` or `GridElement`. Because the `Grid` here is a root, the `RootWidthCss` and `RootHeightCss` properties effectively provide the initial size for this layout hierarchy. If we were to nest a `Grid` inside a `Grid` or a `StackPanel`, the `FrameworkElement` properties for the nested `Grid` would come from the `GridElement` or `StackPanelElement` that hosts it. This keeps layout consistent between internal and external components in the BpfLayout system.
* The row and column definitions aren't direct properties of the `Grid`. In WPF, we would see `Grid.RowDefinitions`, using XAML syntax to specify a direct value for the `Grid` property `RowDefinitions`. We don't have that option in Blazor, so instead, the `RenderFragment` parameters `GridRowDefinitions` and `GridColumnDefinitions` are used to supply this information.
* Child elements of the `Grid` are not placed directly under the `Grid` itself. Instead, they are placed under the `ChildContent` `RenderFragment` property. Again, this is simply how Blazor works.
* Elements are not placed directly under `ChildContent`. Instead, all child elements are wrapped in an element component. For a `Grid`, this is `GridElement`. These element components supply the equivalent `FrameworkElement` properties from WPF and attempt to transform their child elements to honor these properties. Subsequent BpfLayout components can be nested inside these element components. In this example, the `div` tags take all their sizing and alignment information from the element component in which they are nested.
* The `Row` and `Column` properites aren't attached properties as in WPF, but are rather properties of the `GridElement` class. 

## Frequently Asked Questions
 
### How does margin work in BpfLayout?

Margin is modeled after the WPF concept of margin: it is a mandatory amount of space around the elements bounding box. An element that has a fixed size in pixels or that is using auto sizing will size itself first, and then its bounding box for layout will be extended by that margin value. An element that is stretching to fill space will stretch so that it leaves its margin between itself and the bounding box of the next element.

Note also that the margin is additive between elements. If element A has a right margin of 20 and is next to element B with a left margin of 30, the distance between them will be 50 pixels.

### Why doesn't it work to set min and max size for a row or column at the same time?

Because I haven't figured out how to get CSS to let me specify an initial size, min size, and max size at the same time. The `minmax` CSS function only has two parameters. To get behavior similar to WPF you'd need three.

If you're using a `GridSplitter`, min size and max size will work better (but not perfectly) because the splitter code will handle enforcing the min and max outside of the CSS logic.

## Reference

### Grid

A `Grid` models a layout with distinct rows and columns. Rows and columns can specify an exact size, take their size from their child elements, or expand to fill space. `Grid` elements are defined by a series of `GridRowDefinition` and/or `GridColumnDefinition` components. They are populated by `GridElement` and `GridSplitter` components.

Grid row and column sizes are specified using the same format as WPF row and column definitions. A row or column size can take one of three forms:
* `"auto"`: The element will take its size from the size of its child elements. This is `max-content` in CSS terms.
* `"<n>"`: The element will have a fixed size of `<n>` pixels. This is `<n>px` in CSS terms.
* `"<m>*"`: The element will stretch to fill the leftover space not accounted for by `"auto"` and `"<n>"` rows/columns. The amount of space a row/column will take up is proportional to `<m>` over the sum of all `<m>` for all `"<m>*"` elements in the same row/column definition set. If `"<m>"` is not specified, it defaults to `1`. This is similar, but not identical, to `<m>fr` in CSS terms; the difference is that BpfLayout always normalizes the specified row/column weights so that they sum to 1, whereas CSS does not normalize if the sum falls below zero.  

#### Example ([source](tests/BpfLayoutCoreTests/Pages/GridExample.razor))

```
@*
    Because this grid is the root element on the page, we must specify its dimensions in CSS terms.
    Here we choose 100% (the default) for height and 600px for width. How that 100% will be interpreted
    is up to the HTML/CSS layout that this grid finds itself in.
*@
<Grid RootWidthCss="600px" RootHeightCss="100%">
    <GridRowDefinitions>
        <GridRowDefinition Height="auto" />
        <GridRowDefinition Height="*" />
    </GridRowDefinitions>
    <GridColumnDefinitions>
        <GridColumnDefinition Width="200" />
        <GridColumnDefinition Width="*" />
    </GridColumnDefinitions>
    <ChildContent>
        <GridElement Row="0" Column="0">
            <div>Top Left Corner</div>
        </GridElement>
        <GridElement Row="1" Column="0" VerticalAlignment="@VerticalAlignment.Bottom">
            <div>Side Bar</div>
        </GridElement>
        <GridElement Row="0" Column="1" HorizontalAlignment="@HorizontalAlignment.Center">
            <div>Top Bar with auto height</div>
        </GridElement>
        <GridElement Row="1" Column="1">
            <Grid>
                @*
                    This grid does not need RootWidthCss or RootHeightCss because it is placed inside another grid 
                    and so it will take its width and height from the grid element encloding it. If RootWidthCss or
                    RootHeightCss were specified here, the GridElement would override them by design.
                *@
            </Grid>
        </GridElement>
    </ChildContent>
</Grid>
```

#### Parameters
* `string RootWidthCss` (default: `"100%"`): The width of the grid element itself in CSS units. Once rooted, nested BpfLayout elements take their widths from `*Element` components, so this parameters is not necessary when the grid is nested directly under another BpfLayout element.
* `string RootHeightCss` (default: `"100%"`): The height of the grid element itself in CSS units. Once rooted, nested BpfLayout elements take their heights from `*Element` components, so this parameters is not necessary when the grid is nested directly under another BpfLayout element.
* `double RowSnapOffset` (default: `0.0`): When using a `GridSplitter`, indicates the number of pixels from a neighboring row at which a row splitter will "snap" shut.
* `double ColumnSnapOffset` (default: `0.0`): When using a `GridSplitter`, indicates the number of pixels from a neighboring column at which a column splitter will "snap" shut.
* `double RowDragInterval` (default: `1.0`): When using a `GridSplitter`, indicates the granularity in pixels of row splitter movement. Values over 1.0 will result in "chunkier" movement.
* `double ColumnDragInterval` (default: `1.0`): When using a `GridSplitter`, indicates the granularity in pixels of column splitter movement. Values over 1.0 will result in "chunkier" movement.
* `EventCallback<SplitterResizedGridEventArgs> SplitterResizedGrid`: Callback issue whenever a splitter resizes the grid, providing the new row and column definitions for the current grid. Clients can use this to save grid layout across sessions.
* `RenderFragment? GridRowDefinitions`: Home for child `GridRowDefinition` components.
* `RenderFragment? GridColumnDefinitions`: Home for child `GridColumnDefinition` components.
* `ChildContent`: Home for child `GridElement` and `GridSplitter` components.

### GridRowDefinition

Defines a row in a `Grid` as a child element of the `GridRowDefinitions` parameter.

#### Example

See the `Grid` example ([source](tests/BpfLayoutCoreTests/Pages/GridExample.razor)).

#### Parameters
* `string Height` (default: `"*"`): The height of this grid row, in WPF row size format.
* `double? MinHeight` (default: `null`): The minimum height of this row in pixels. Due to CSS limitations, `MinHeight` and `MaxHeight` may not work well when used together unless they are being used to restrict a `GridSplitter`.
* `double? MaxHeight` (default: `null`): The maximum height of this row in pixels. Due to CSS limitations, `MinHeight` and `MaxHeight` may not work well when used together unless they are being used to restrict a `GridSplitter`.
* `string? Name` (default: `null`): The name of this row. This is an extension to WPF. Grid elements may refer to row definitions by name instead of by index. Providing a name may assist with layout persistence if you are adding or removing a `GridSplitter` dynamically.
* `int? SortOrder` (default: `0`): The order of this row relative to other rows. This is an extension to WPF. Row indices on `GridElement` components are always relative to sorted order. Sorting is stable so that rows with unspecified or identical sort order will keep their position relative to each other. Using a sort index may be necessary if you are adding and removing rows dynamically in Blazor.
* 
### GridColumnDefinition

Defines a column in a `Grid` as a child element of the `GridColumnDefinitions` parameter.

#### Example

See the `Grid` example ([source](tests/BpfLayoutCoreTests/Pages/GridExample.razor)).

#### Parameters
* `string Width` (default: `"*"`): The width of this grid column, in WPF column size format.
* `double? MinWidth` (default: `null`): The minimum width of this column in pixels. Due to CSS limitations, `MinWidth` and `MaxWidth` may not work well when used together unless they are being used to restrict a `GridSplitter`.
* `double? MaxWidth` (default: `null`): The maximum width of this column in pixels. Due to CSS limitations, `MinWidth` and `MaxWidth` may not work well when used together unless they are being used to restrict a `GridSplitter`.
* `string? Name` (default: `null`): The name of this column. This is an extension to WPF. Grid elements may refer to column definitions by name instead of by index. Providing a name may assist with layout persistence if you are adding or removing a `GridSplitter` dynamically.
* `int? SortOrder` (default: `0`): The order of this column relative to other columns. This is an extension to WPF. Column indices on `GridElement` components are always relative to sorted order. Sorting is stable so that columns with unspecified or identical sort order will keep their position relative to each other. Using a sort index may be necessary if you are adding and removing columns dynamically in Blazor.

### GridElement

Container for a child element of a `Grid` used to model attached properties and FrameworkElement properties from WPF. This allows us to explicitly specify width, height, alignment, and margins for the child element relative to its containing layout element.

Note that `GridElement` itself does not represent a container that provides space for more child elements. It exists to override the layout properties of a single child component. If you would like to layout multiple elements under a `GridElement`, the child element of the `GridElement` should be a container, like a `div` element (or another BpfLayout element).

#### Example

See the `Grid` example ([source](tests/BpfLayoutCoreTests/Pages/GridExample.razor)).

#### Parameters

* `double? Width` (default: `null`): The width of the child element in pixels. If unspecified, the child element will size itself and align according to `HorizontalAlignment`, unless `HorizontalAlignment` is `HorizontalAlignment.Stretch`, in which case the child element will fill the width of its container.
* `double? Height` (default: `null`):  The height of the child element in pixels. If unspecified, the child element will size itself and align according to `VerticalAlignment`, unless `VerticalAlignment` is `VerticalAlignment.Stretch`, in which case the child element will fill the height of its container.
* `Thickness Margin` (default: no margin): The number of pixels with which to pad the child element. `Thickness` can be constructed with a uniform margin, a separate right/left and top/bottom margin, or a unique margin for each side.
* `HorizontalAlignment HorizontalAlignment` (default: `HorizontalAlignment.Stretch`): Defines how the child element should align itself horizontally relative to its container. `HorizontalAlignment.Stretch` will cause to to fill all horizontal space rather than align.
* `VerticalAlignment VerticalAlignment` (default: `VerticalAlignment.Stretch`): Defines how the child element should align itself vertically relative to its container. `VerticalAlignment.Stretch` will cause to to fill all vertical space rather than align.
* `string? Row` (default: `"0"`): A zero-based row index or row name indicating which row the child element should occupy. These correspond to the `GridRowDefinition` components supplied to the containing grid.
* `string? Column` (default: `"0"`): A zero-based column index or columnrow name indicating which column the child element should occupy. These correspond to the `GridColumnDefinition` components supplied to the containing grid.
* `int RowSpan` (default: `1`): The number of rows the child element should span, starting from the row indicated by `Row`. 
* `int ColumnSpan` (default: `1`): The number of columns the child element should span, starting from the column indicated by `Column`.

### GridSplitter

A sub-class of `GridElement` that occupies its own `Grid` row or column and allows the user to dynamically resize the rows or columns on either side of the row/column containing the `GridSplitter` component.

Note that while a `GridSplitter` is a `GridElement` in almost all respects: you must specify the row/column it occipies, must provide a row or column span if you want it to cover the whole grid, and may customize its appearance with child content.

#### Example ([source](tests/BpfLayoutCoreTests/Pages/GridSplitterExample.razor))

```
<Grid RootWidthCss="600px" RootHeightCss="600px" RowSnapOffset="50" ColumnDragInterval="10">
    @*
        All row/column definitions use pixel or * sizes. The splitter library being used cannot resize auto 
        sized rows or columns. However, you may use an auto size if it is not adjacent to a splitter.
    *@
    <GridRowDefinitions>
        <GridRowDefinition Height="100" />
        <GridRowDefinition Height="4" />
        <GridRowDefinition Height="*" />
    </GridRowDefinitions>
    <GridColumnDefinitions>
        <GridColumnDefinition Width="*" />
        <GridColumnDefinition Width="4" />
        <GridColumnDefinition Width="*" />
    </GridColumnDefinitions>
    <ChildContent>
        <GridElement Row="0" Column="0" HorizontalAlignment="@HorizontalAlignment.Center" VerticalAlignment="@VerticalAlignment.Center">
            <div>Top Left</div>
        </GridElement>
        <GridElement Row="2" Column="0" HorizontalAlignment="@HorizontalAlignment.Center" VerticalAlignment="@VerticalAlignment.Center">
            <div>Bottom Left</div>
        </GridElement>
        <GridElement Row="0" Column="2" HorizontalAlignment="@HorizontalAlignment.Center" VerticalAlignment="@VerticalAlignment.Center">
            <div>Top Right</div>
        </GridElement>
        <GridElement Row="2" Column="2" HorizontalAlignment="@HorizontalAlignment.Center" VerticalAlignment="@VerticalAlignment.Center">
            <div>Bottom Left</div>
        </GridElement>
        @* By setting a Row, this becomes a Row splitter; note that we must specify ColumnSpan to get it to stretch across the whole grid *@
        <GridSplitter Row="1" ColumnSpan="3" />
        @* By setting a Column, this becomes a Column splitter; note that we must specify RowSpan to get it to stretch across the whole grid *@
        <GridSplitter Column="1" RowSpan="3" />
    </ChildContent>
</Grid>
```

#### Parameters

The `GridSplitter` is a specialized `GridElement`, and has all the same parameters which all work the same way. That said, `GridSplitter` components are highly likely to use the default height, width, alignment, and margins, since the default will be to fill the entire row or column as defined. Users will want to specify `Row` or a row splitter or `Column` for a column splitter, and then provide a `ColumnSpan` for row splitters or `RowSpan` for column splitters if they desire the splitter to cover the whole grid, as usual.

Since `GridSplitter` is just a specialized `GridElement`, you can also override the default splitter style simply by providing your own child element.

Note that the `Grid` houses a number of parameters related to `GridSplitter` behavior, including parameters for drag interval and snap offset, as well as `SplitterResizedGrid`, an event that notifies listeners of new row and columns sizes when a splitter is dragged.

### StackPanel

Stacks elements next to each other horizontally or vertically, similar to flex box in CSS. Child elements can align themselves along the opposite axis to the stack panel's stack direction. Stack panels do not natively scroll if they overflow (see `ScrollViewer`). They represent a much simpler element than CSS flex box, and do not have options for wrapping or spacing, though elements may provide a margin to provide fixed spacing. `Grid` is the layout element of choice for precise and flexible spacing control.

#### Example ([source](tests/BpfLayoutCoreTests/Pages/StackPanelExample.razor))

```
<StackPanel Orientation="@Orientation.Vertical" RootWidthCss="600px" RootHeightCss="100%">
    <StackPanelElement Height="100" HorizontalAlignment="@HorizontalAlignment.Left" Margin="@(new Thickness(10, 0, 0, 50))">
        <StackPanel>
            <StackPanelElement VerticalAlignment="@VerticalAlignment.Top">
                <div>A B C</div>
            </StackPanelElement>
            <StackPanelElement VerticalAlignment="@VerticalAlignment.Center">
                <div>D E F</div>
            </StackPanelElement>
            <StackPanelElement VerticalAlignment="@VerticalAlignment.Bottom">
                <div>G H I</div>
            </StackPanelElement>
        </StackPanel>
    </StackPanelElement>
    <StackPanelElement Height="100" HorizontalAlignment="@HorizontalAlignment.Center" Margin="@(new Thickness(0, 50, 0, 50))">
        <StackPanel>
            <StackPanelElement VerticalAlignment="@VerticalAlignment.Bottom">
                <div>P Q R</div>
            </StackPanelElement>
            <StackPanelElement VerticalAlignment="@VerticalAlignment.Center">
                <div>M N O</div>
            </StackPanelElement>
            <StackPanelElement VerticalAlignment="@VerticalAlignment.Top">
                <div>J K L</div>
            </StackPanelElement>
        </StackPanel>
    </StackPanelElement>
    <StackPanelElement Height="100" HorizontalAlignment="@HorizontalAlignment.Right" Margin="@(new Thickness(0, 50, 10, 0))">
        <StackPanel>
            <StackPanelElement VerticalAlignment="@VerticalAlignment.Top">
                <div>S T U</div>
            </StackPanelElement>
            <StackPanelElement VerticalAlignment="@VerticalAlignment.Center">
                <div>V W X</div>
            </StackPanelElement>
            <StackPanelElement VerticalAlignment="@VerticalAlignment.Bottom">
                <div>Y Z !</div>
            </StackPanelElement>
        </StackPanel>
    </StackPanelElement>
</StackPanel>
```

#### Parameters

* `string RootWidthCss` (default: `"100%"`): The width of the stack panel element itself in CSS units. Once rooted, nested BpfLayout elements take their widths from `*Element` components, so this parameters is not necessary when the stack panel is nested directly under another BpfLayout element.
* `string RootHeightCss` (default: `"100%"`): The height of the stack panel element itself in CSS units. Once rooted, nested BpfLayout elements take their heights from `*Element` components, so this parameters is not necessary when the stack panel is nested directly under another BpfLayout element.
* `Orientation Orientation` (default: `Orientation.Horizontal`): Specifies if elements are layed out horizontally or vertically.

### StackPanelElement

Container for a child element of a `StackPanel` used to model attached properties and FrameworkElement properties from WPF. This allows us to explicitly specify width, height, alignment, and margins for the child element relative to its containing layout element.

Note that `StackPanelElement` itself does not represent a container that provides space for more child elements. It exists to override the layout properties of a single child component. If you would like to layout multiple elements under a `StackPanelElement`, the child element of the `StackPanelElement` should be a container, like a `div` element (or another BpfLayout element).

#### Example

See the `StackPanel` example ([source](tests/BpfLayoutCoreTests/Pages/StackPanelExample.razor)).

#### Parameters

* `double? Width` (default: `null`): The width of the child element in pixels. If unspecified, the child element will:
  * In a horizontal stack panel, size itself.
  * In a vertical stack panel, size itself and align according to `HorizontalAlignment`, unless `HorizontalAlignment` is `HorizontalAlignment.Stretch`, in which case the child element will fill the width of its container.
* `double? Height` (default: `null`):  The height of the child element in pixels. If unspecified, the child element will:
  * In a horizontal stack panel, size itself and align according to `VerticalAlignment`, unless `VerticalAlignment` is `VerticalAlignment.Stretch`, in which case the child element will fill the height of its container.
  * In a vertical stack panel, size itself.
* `Thickness Margin` (default: no margin): The number of pixels with which to pad the child element. `Thickness` can be constructed with a uniform margin, a separate right/left and top/bottom margin, or a unique margin for each side.
* `HorizontalAlignment HorizontalAlignment` (default: `HorizontalAlignment.Stretch`): In a vertical stack panel, defines how the child element should align itself horizontally relative to its container. `HorizontalAlignment.Stretch` will cause to to fill all horizontal space rather than align. In a horizontal stack panel, this parameter does nothing.
* `VerticalAlignment VerticalAlignment` (default: `VerticalAlignment.Stretch`): In a horizontal stack panel, defines how the child element should align itself vertically relative to its container. `VerticalAlignment.Stretch` will cause to to fill all vertical space rather than align. In a vertical stack panel, this parameter does nothing.

### ScrollViewer

Provides an region that will scroll instead of clip when its child elements overflow. Note that unlike other layout elements, the `ScrollViewer` is designed to have one single `ScrollViewerElement` under it. The `ScrollViewer` simply defines a scrollable region; it should be laid out and sized using the `StackPanel` and `Grid`.

#### Example ([source](tests/BpfLayoutCoreTests/Pages/ScrollViewerExample.razor))

```
<Grid RootWidthCss="600px" RootHeightCss="100%">
    <GridRowDefinitions>
        <GridRowDefinition Height="100" />
        <GridRowDefinition Height="*" />
    </GridRowDefinitions>
    <ChildContent>
        <GridElement Row="0" HorizontalAlignment="@HorizontalAlignment.Center">
            Example of a scrollable area
        </GridElement>
        <GridElement Row="1">
            <ScrollViewer>
                <ScrollViewerElement>
                    <StackPanel Orientation="@Orientation.Vertical">
                        <StackPanelElement Margin="@(new Thickness(50))">
                            <div>ABC</div>
                        </StackPanelElement>
                        <StackPanelElement Margin="@(new Thickness(50))">
                            <div>DEF</div>
                        </StackPanelElement>
                        <StackPanelElement Margin="@(new Thickness(50))">
                            <div>GHI</div>
                        </StackPanelElement>
                        <StackPanelElement Margin="@(new Thickness(50))">
                            <div>JKL</div>
                        </StackPanelElement>
                        <StackPanelElement Margin="@(new Thickness(50))">
                            <div>MNO</div>
                        </StackPanelElement>
                        <StackPanelElement Margin="@(new Thickness(50))">
                            <div>PQR</div>
                        </StackPanelElement>
                        <StackPanelElement Margin="@(new Thickness(50))">
                            <div>STU</div>
                        </StackPanelElement>
                        <StackPanelElement Margin="@(new Thickness(50))">
                            <div>VWX</div>
                        </StackPanelElement>
                        <StackPanelElement Margin="@(new Thickness(50))">
                            <div>YZ!</div>
                        </StackPanelElement>
                    </StackPanel>
                </ScrollViewerElement>
            </ScrollViewer>
        </GridElement>
    </ChildContent>
</Grid>
```

#### Parameters

* `string RootWidthCss` (default: `"100%"`): The width of the scroll viewer element itself in CSS units. Once rooted, nested BpfLayout elements take their widths from `*Element` components, so this parameters is not necessary when the scroll viewer is nested directly under another BpfLayout element.
* `string RootHeightCss` (default: `"100%"`): The height of the scroll viewer element itself in CSS units. Once rooted, nested BpfLayout elements take their heights from `*Element` components, so this parameters is not necessary when the scroll viewer is nested directly under another BpfLayout element.
* `ScrollBarVisibility HorizontalScrollBarVisibility` (default: `ScrollBarVisibility.Disabled`): Specifies how to scroll in the horizontal direction.
* `ScrollBarVisibility VerticalScrollBarVisibility` (default: `ScrollBarVisibility.Auto`): Specifies how to scroll in the vertical direction.

### ScrollViewerElement

Container for the single child element of a `ScrollViewer` used to model attached properties and FrameworkElement properties from WPF. This allows us to explicitly specify width, height, alignment, and margins for the child element relative to its containing layout element.

Note that `ScrollViewerElement` itself does not represent a container that provides space for more child elements. It exists to override the layout properties of a single child component. If you would like to layout multiple elements under a `ScrollViewerElement`, the child element of the `ScrollViewerElement` should be a container, like a `div` element (or another BpfLayout element).

#### Example

See the `ScrollViewer` example ([source](tests/BpfLayoutCoreTests/Pages/ScrollViewerExample.razor)).

#### Parameters

* `double? Width` (default: `null`): The width of the child element in pixels. If unspecified, the child element will size itself and align according to `HorizontalAlignment`, unless `HorizontalAlignment` is `HorizontalAlignment.Stretch` and horizontal scrolling is disabled, in which case the child element will fill the width of its container.
* `double? Height` (default: `null`):  The height of the child element in pixels. If unspecified, the child element will size itself and align according to `VerticalAlignment`, unless `VerticalAlignment` is `VerticalAlignment.Stretch` and vertical scrolling is disabled, in which case the child element will fill the height of its container.
* `Thickness Margin` (default: no margin): The number of pixels with which to pad the child element. `Thickness` can be constructed with a uniform margin, a separate right/left and top/bottom margin, or a unique margin for each side.
* `HorizontalAlignment HorizontalAlignment` (default: `HorizontalAlignment.Stretch`): If horizontal scrolling is disabled, defines how the child element should align itself horizontally relative to its container. `HorizontalAlignment.Stretch` will cause to to fill all horizontal space rather than align. If horizontal scrolling is enabled, this parameter does nothing.
* `VerticalAlignment VerticalAlignment` (default: `VerticalAlignment.Stretch`): If vertical scrolling is disabled, defines how the child element should align itself vertically relative to its container. `VerticalAlignment.Stretch` will cause to to fill all vertical space rather than align. If vertical scrolling is enabled, this parameter does nothing.

### Orientation

* `Orientation.Horizontal`: instructs a `StackPanel` to lay itself out as a row.
* `Orientation.Vertical`: instructs a `StackPanel` to lay itself out as a column.

### HorizontalAlignment

* `HorizontalAlignment.Left`: The child element should align to the left of its container.
* `HorizontalAlignment.Center`: The child element should align at the center of its container.
* `HorizontalAlignment.Right`: The child element should align to the right of its container.
* `HorizontalAlignment.Stretch`: The child element should stretch horizontally to fill its container.

### VerticalAlignment

* `VerticalAlignment.Top`: The child element should align to the top of its container.
* `VerticalAlignment.Center`: The child element should align at the center of its container.
* `VerticalAlignment.Bottom`: The child element should align to the bottom of its container.
* `VerticalAlignment.Stretch`: The child element should stretch vertically to fill its container.

### Thickness

Defines the border of a rectangle, with the ability to specify a different thickness for each side in pixels.

* `double Left`: Thickness of the left border of the rectangle, in pixels.
* `double Right`: Thickness of the right border of the rectangle, in pixels. 
* `double Top`: Thickness of the top border of the rectangle, in pixels.
* `double Bottom`: Thickness of the bottom border of the rectangle, in pixels.

### ScrollBarVisibility

* `ScrollBarVisibility.Disabled`: The `ScrollViewer` will preserve default clipping behavior and not scroll. Normal alignment and stretch rules apply.
* `ScrollBarVisibility.Auto`: The `ScrollViewer` will show a scroll bar only if content overflows.
* `ScrollBarVisibility.Hidden`: The `ScrollViewer` will preserve default clipping behavior and not scroll, but normal alignment and stretch rules are ignored, just as if scrolling was enabled.
* `ScrollBarVisibility.Visible`: The `ScrollViewer` will show a scroll bar at all times.

### SplitterResizedGridEventArgs

* `IReadOnlyCollection<RowSizeSpecification> Rows`: New WPF row size format values for each row in sort order after a `GridSplitter` has resized a `Grid` layout. 
* `IReadOnlyCollection<ColumnSizeSpecification> Columns`: New WPF column size format values for each column in sort order after a `GridSplitter` has resized a `Grid` layout.

#### RowSizeSpecification

* `string Height`: WPF row size format value for the specified row.
* `string? Name`: The name of the row, if any.

#### ColumnSizeSpecification

* `string Width`: WPF column size format value for the specified column.
* `string? Name`: The name of the column, if any.