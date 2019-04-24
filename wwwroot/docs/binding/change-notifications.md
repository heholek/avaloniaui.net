Title: Change Notifications
Order: 5
---

In order for Avalonia to know when a property on a view model has changed, the view model must
implement change notifications. The easiest way to do this is to use `ReactiveUI` and make your
view model class inherit from `ReactiveObject`.

You then add a setter for each property which calls `RaiseAndSetIfChanged`:

```csharp
using ReactiveUI;

public class MyViewModel : ReactiveObject
{
    private string caption;

    public string Caption
    {
        get => caption;
        set => this.RaiseAndSetIfChanged(ref caption, value);
    }
}
```

For more information see the [ReactiveUI documentation](https://reactiveui.net/docs/handbook/view-models/).

If you don't want a dependency on ReactiveUI, you can implement 
[`INotifyPropertyChanged` manually](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged)