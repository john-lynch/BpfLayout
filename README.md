# BpfLayout
A Blazor component library attempting to replicate WPF layout panels like Grid, StackPanel, and ScrollViewer in Blazor.

## Why?

As a programmer moving familiar with how layout works in WPF, moving to Blazor with its HTML/CSS-based layout was incredibly frustrating. Elements were constantly resizing themselves when I didn't expect them to, things would scroll as I expected, etc. I wanted something that could give me the control I had with WPF in Blazor.

## Goals
* Replicate as closely as possible the layout elements in WPF so that their use in Blazor would be intuitive.
* Work out-of-the-box with arbitrary child elements, especially in other component libraries like MudBlazor.
* Make it obvious why a layout is behaving the way that it is. Minimize the need to go hunting around in browser debugging tools.

## Challenges

There are a couple of key challenges, beyond simply mapping WPF layout behavior to HTML/CSS, when trying to replicate WPF functionality in HTML/CSS:
1. Blazor components are very weakly opinionated. They can contain effectively arbitrary HTML/CSS, and do not provide anything like the base functionality that `FrameworkElement` provides in WPF. As a result, getting a bunch of disparate components to play by similar layout rules is challenging.
2. Blazor has no notion of attached properties like WPF does. You cannot natively attach, for example, `Grid` properties to an arbitrary component or piece of HTML/CSS in Blazor.

## Solutions

To solve the above problems, BpfLayout adopts the following convention:

Each layout panel has two parts: the panel itself, that provides the properties that the equivalent panel in WPF provides, and a wrapper "element" component used to host an arbitrary Blazor component or HTML/CSS. This wrapper component provides the parameters that `FrameworkElement` provides in WPF and attempts to impart them (using some internal CSS) to the hosted Blazor component or HTML/CSS.

## Examples

For example, here is how a `Grid` works in BpfLayout:

```
<Root WidthCss="800px" HeightCss="600px">
  <Grid>
    <GridRowDefinitions>
      <GridRowDefinition Height="100" />
      <GridRowDefinition Height="Auto" />
      <GridRowDefinition Height="*" />
    </GridRowDefinitions>
    <GridColumnDefinitions>
      <GridColumnDefinition Width ="200" />
      <GridColumnDefinition Width ="*" />
      <GridColumnDefinition Width ="2*" />
      <GridColumnDefinition Width ="Auto" />
    </GridColumnDefinitions>
  </Grid>
  <ChildContent>
    <GridElement Row="0" Column="0" HorizontalAlignment="HorizontalAlignment.Stretch" VerticalAlignment="VerticalAlignment.Stretch">
      <div class="some-class">
        Hello, World!
      </div>
    </GridElement>
    <GridElement Row="0" Column="1" Width="300">
      <StackPanel Orientation="Orientation.Horizontal">
        <StackPanelElement Width="150">
          <div class="some-class3">
            More Hello, World!
          </div>
        </StackPanelElement>
        <StackPanelElement Height="10" VerticalAlignment="VerticalAlignment.Bottom">
          <div class="some-class4">
            More Hello, World!
          </div>
        </StackPanelElement>
      </StackPanel>
    </GridElement>
    <GridElement Row="1" Column="2" Width="100" Height="50" HorizontalAlignment="HorizontalAlignment.Center">
      <div class="some-class2">
        Hello, World! Again!
      </div>
    </GridElement>
  </ChildContent>
</Root>
```
Here we a `Grid` with three rows and four columns, demonstrating the various sizing options for rows and columns. How does this differ from a WPF `Grid`?
* Note the presence of a `Root` element. This lets the user specify a CSS-based width and height for an element, then turns layout over to the BPF layout system. A Blazor page could contain many roots, at the same level, or nested underneath each other, perhaps inside an external Blazor component. Each `Root` establishes a new consistent context for subsequent BpfLayout components.
* The `Grid` itself does not have the standard `FrameworkElement` properties for dimension or alignment. All `FrameworkElement` properties are present on the `*Element` classes, like `StackPanelElement` or `GridElement`. The `Root` is effectively sizing our base `Grid` here. If we were to nest a `Grid` inside a `Grid` or a `StackPanel`, the `FrameworkElement` properties for the nested `Grid` would come from the `GridElement` or `StackPanel` element that hosts it. This keeps layout consistent between internal and external components in the BpfLayout system.
* The row and column definitions aren't direct properties of the `Grid`. In WPF, we would see `Grid.RowDefinitions`, using XAML syntax to specify a direct value for the `Grid` property `RowDefinitions`. We don't have that option in Blazor, so instead, the `RenderFragment` parameters `GridRowDefinitions` and `GridColumnDefinitions` are used to supply this information.
* Child elements of the `Grid` are not placed directly under the `Grid` itself. Instead, they are placed under the `ChildContent` `RenderFragment` property. Again, this is simply how Blazor works.
* Elements are not placed directly under `ChildContent`. Instead, all child elements are wrapped in an element component. For a `Grid`, this is `GridElement`. These element components supply the equivalent `FrameworkElement` properties from WPF and attempt to transform their child elements to honor these properties. Subsequent BpfLayout components can be nested inside these element components. In this example, the `div` tags take all their sizing and alignment information from the element component in which they are nested.
* The `Row` and `Column` properites aren't attached properties as in WPF, but are rather properties of the `GridElement` class. 
