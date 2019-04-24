Title: Tutorial
Order: 0
---
In this tutorial we're going to be creating a simple TODO application in Avalonia using the Model-View-ViewModel (MVVM) pattern.

The finished application will look like this:

![The running application](images/wiring-up-views-run.png)

You can find the code for the completed application [here](https://github.com/grokys/todo-tutorial). 

## Model-View-ViewModel (MVVM)

The Model-View-ViewModel pattern (MVVM) is a common pattern used for writing GUI applications, and is the recommended pattern to use when writing Avalonia applications. We'll be assuming a [CRUD](https://en.wikipedia.org/wiki/Create,_read,_update_and_delete) application here, but most of
these concepts can be applied to all types of applications.

:::note
For this guide we're going to be using [ReactiveUI](https://reactiveui.net/) which is a MVVM framework based on [.NET Reactive Extensions](http://reactivex.io/). This guide will explain how to use MVVM and ReactiveUI with Avalonia but you can also see the [ReactiveUI documentation](https://reactiveui.net/docs/) for more detailed information.
:::

MVVM, as its name suggests has three parts:

- **Model**s hold the data related to the application domain, without concern for the display of the data. For example, in a CRUD application, the models would be the used to interface with the ORM.
- **View Model**s contain the logic of the application and describe the data that will be displayed, however they don't actually contain any code that creates controls. In this way the logic of the application can be easily unit tested without having to display anything on-screen. You can read more about view models in [the ReactiveUI documentation](https://reactiveui.net/docs/handbook/view-models/).
- **View**s are usually Avalonia `UserControls` which describe how to display a section of the UI. Each view typically has a related view model. Views don't contain business-related logic, instead they [bind](/docs/binding) to their view model to get data.

In addition, most applications will add _services_ to this mix, which usually implement the reading and writing of models and other application-specific logic.

:::note
The implementation of these concepts are usually tightly coupled to to the particular application being written and as such are not usually designed to be reusable outside the application. If you want to write re-usable controls, then a Lookless Control might be a a better choice.
:::

<a class="btn btn-primary" role="button" href="creating-the-project">
    Get started
</a>