Title: Grid
---
The `Grid` control is a *Panel* and useful to organize other controls in columns and rows.
It is capable of joining neighboring rows and columns using column span or row span.
[`ColumnDefinition`] [`RowDefinition`] can be used to define the relative and absolute diviation
of the resulting grid.

Use the `Grid.Column` and `Grid.Row` attached properties to assign child elements into their respective
cell positions.

An example of a Grid with 3 equal Rows and 3 Columns with (1 fixed width), (2 grabbin the rest relativly) would be:

```xml
<Grid ColumnDefinitions="100,1.5*,4*" RowDefinitions="Auto,Auto,Auto"  Margin="4">
    <TextBlock Text="Col0Row0:" Grid.Row="0" Grid.Column="0"/>
    <TextBlock Text="Col0Row1:" Grid.Row="0" Grid.Column="0"/>
    <TextBlock Text="Col0Row2:" Grid.Row="0" Grid.Column="0"/>
    <CheckBox Content="Col2Row0" Grid.Row="0" Grid.Column="2"/>
    <Button Content="SpansCol1-2Row1-2" Grid.Row="1" Grid.Column="1" Grid.RowSpan="2" Grid.ColumnSpan="2"/>
</Grid>
```

`Remark`: Note that the Column 1 will get 1.5 parts plus Column 2 will get 4 parts of the remainder of the space that Colum 0 left.

This is the simplified way to defining columns and rows. The above grid is equal to the following 
more verbose definition:

```xml
<Grid Margin="4">
	<Grid.ColumnDefinitions>
	  <ColumnDefinition Width="100"/>
	  <ColumnDefinition Width="1.5*"/>
	  <ColumnDefinition Width="4*"/>
	</Grid.ColumnDefinitions>
	<Grid.RowDefinitions>
	  <RowDefinition Height="Auto"/>
	  <RowDefinition Height="Auto"/>
	  <RowDefinition Height="Auto"/>
	</Grid.RowDefinitions>
	<TextBlock Text="Col0Row0:" Grid.Row="0" Grid.Column="0"/>
	<TextBlock Text="Col0Row1:" Grid.Row="0" Grid.Column="0"/>
	<TextBlock Text="Col0Row2:" Grid.Row="0" Grid.Column="0"/>
	<CheckBox Content="Col2Row0" Grid.Row="0" Grid.Column="2"/>
	<Button Content="SpansCol1-2Row1-2" Grid.Row="1" Grid.Column="1" Grid.RowSpan="2" Grid.ColumnSpan="2"/>
</Grid>
```


## Common Properties

|Property|Description|
|--------|-----------|
|`ColumnDefinitions`|A collection of `ColumnDefinition`s describing the width and max or min width of a column|
|`RowDefinitions`|A collection of `RowDefinition`s describing the height and max or min height of a row|
|`Grid.Column`|Attached property to assign a control into a zero based column|
|`Grid.Row`|Attached property to assign a control into a zero based row|
|`Grid.ColumnSpan`|Attached property to define the number of columns a control will span|
|`Grid.RowSpan`|Attached property to define the number of rows a control will span|

## Remark

The WPF known Grid column alignments over several grids is not available, yet.

## Pseudoclasses

None