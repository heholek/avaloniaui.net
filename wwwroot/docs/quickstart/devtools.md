Title: Developer Tools
Order: 60
---

Avalonia has an inbuilt DevTools window which is enabled by calling the attached `AttachDevTools()`
method in a `Window` constructor. The default templates have this enabled when the program is
compiled in `DEBUG` mode:

```csharp
public class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
```

To open the DevTools, press F12, or pass a different `Gesture` to the `this.AttachDevTools()`
method.

:::note
There is a known issue when running under .NET core 2.1 that pressing F12 will cause the program to
quit. In this case, either switch to .NET core 2.0 or 3.0 or change the open gesture to something
different, such as `Ctrl+F12`.
:::
