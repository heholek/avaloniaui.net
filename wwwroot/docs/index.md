Title: Avalonia
Order: 50
---

Welcome to Avalonia! Avalonia is a cross platform XAML Framework for .NET Framework, .NET Core and Mono.

Avalonia is shipped as .NET standard 2.0 [NuGet packages](docs/quickstart/packages) and a [set of templates](/docs/quickstart/create-new-project) for Visual Studio and .NET core. We also have a [visual designer](/docs/quickstart/vs-designer) for Visual Studio.

This page is designed as a whirlwind tour of Avalonia. There are plenty of links throughout to more detailed documentation on each area, or you can navigate using the sections on the left.

:::note
If you're familiar with WPF or UWP then you should feel right at home with Avalonia. Although Avalonia is not API compatible with either of these frameworks (and so controls can't be used without porting), there's a lot of similarity. If you're a WPF user we have [a page which describes the main differences between WPF and Avalonia](/docs/quickstart/from-wpf).
:::


## Programming with Avalonia

Avalonia lets you develop an appliction using the [XAML markup language](docs/quickstart/into-to-xaml) and C# (or another .NET language) for code. You generally use XAML markup to implement the appearance of an application while using code to implement its behavior.

### Markup

XAML is an XML-based markup language that implements an application's appearance declaratively. You typically use it to create windows and user controls, and to fill them with controls, shapes, and graphics.

The following example uses XAML to implement the appearance of a window that contains a single button:

:::filename
XAML
:::
```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="AvaloniaApplication1.MainWindow"
        Title="Window with Button"
        Width="250" Height="100">
  
  <!-- Add button to window -->
  <Button Name="button">Click Me!</Button>

</Window>
```

Specifically, this XAML defines a window and a button by using the Window and Button elements, respectively. Each element is configured with attributes, such as the Window element's Title attribute to specify the window's title-bar text. At run time, Avalonia converts the elements and attributes that are defined in markup to instances of Avalonia classes. For example, the Window element is converted to an instance of the Window class whose Title property is the value of the Title attribute.

The following images show the user interface that is defined by the XAML in the previous example running on Windows:

Since XAML is XML-based, the UI that you compose with it is assembled in a hierarchy of nested elements known as an [element tree](docs/advanced/trees). The element tree provides a logical and intuitive way to create and manage UIs.

![Application window](/docs/quickstart/images/click-me.png)

### Code-behind

The main behavior of an application is to implement the functionality that responds to user interactions, including handling events (for example, clicking a menu, tool bar, or button) and calling business logic and data access logic in response. In Avalonia, this behavior can be implemented in code that is associated with markup. This type of code is known as [code-behind](/docs/quickstart/codebehind). The following example shows the updated markup from the previous example and the code-behind:

:::filename
XAML
:::
```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="AvaloniaApplication1.MainWindow"
        Title="Window with Button"
        Width="250" Height="100">
  
  <!-- Add button to window -->
  <Button Name="button" Click="button_Click">Click Me!</Button>

</Window>
```

:::filename
C#
:::
```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication1
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void button_Click(object sender, RoutedEventArgs e)
        {
            // Change button text when button is clicked.
            var button = (Button)sender;
            button.Content = "Hello, Avalonia!";
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
```

In this example, the code-behind implements a class that derives from the [`Window`](/docs/quickstart/window) class. The `x:Class` attribute is used to associate the markup with the code-behind class. `InitializeComponent` is called from the code-behind class's constructor to merge the UI that is defined in markup with the code-behind class. The combination of `x:Class` and `InitializeComponent` ensure that your implementation is correctly initialized whenever it is created. The code-behind class also implements an event handler for the button's `Click` event. When the button is clicked, the event handler changes the text of the button by setting a property on the `Button` control.

### The Model-View-ViewModel Pattern

As well as writing code in code-behind, Avalonia supports using the [Model-View-ViewModel](docs/quickstart/mvvm) pattern (or MVVM). MVVM is a common way to structure UI applications that separates view logic from application logic in a way that allows your applications to become unit-testable.

MVVM relies upon Avalonia's [binding](/docs/binding) capabilities to separate your application into a View layer which displays standard Avalonia windows and controls, and a ViewModel layer which defines the functionality of the application independently of Avalonia itself. The following example shows the code from the previous example implemented using the MVVM pattern:

:::filename
XAML
:::
```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="AvaloniaApplication1.MainWindow"
        Title="Window with Button"
        Width="250" Height="100">

  <!-- Add button to window -->
  <Button Content="{Binding ButtonText}" Command="{Binding ButtonClicked}"/>

</Window>
```

:::filename
C#
:::
```csharp
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication1
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        string buttonText = "Click Me!";

        public string ButtonText
        {
            get => buttonText;
            set 
            {
                buttonText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonText)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void ButtonClicked() => ButtonText = "Hello, Avalonia!";
    }
}
```

In this example, the code-behind assigns the `Window`'s [`DataContext`](/docs/binding/datacontext) property to an instance of `MainWindowViewModel`. The XAML then uses an Avalonia [`{Binding}`](/docs/binding/bindings) to bind the `Button`'s `Content` property to the `ButtonText` property on the `MainWindowViewModel`. It also binds the `Button`'s [`Command`](/docs/binding/binding-to-commands) property to the `ButtonClicked` method on the `MainWindowViewModel`.

When the `Button` is clicked it invokes its `Command`, causing the bound `MainWindowViewModel.ButtonClicked` method to be called. This method then sets the `ButtonText` property which raises the `INotifyPropertyChanged.PropertyChanged` event, causing the `Button` to re-read its bound value and update the UI.

## Controls

## Input and Commands

## Layout

## Data Binding

## Graphics

## Animation

## Text and Typography

## Data Templates