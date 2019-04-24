[![Build Status](https://dev.azure.com/AvaloniaUI/avaloniaui.net/_apis/build/status/avaloniaui.net)](https://dev.azure.com/AvaloniaUI/avaloniaui.net/_build/latest?definitionId=1)

# Manual Build Instructions

1. Make sure you have [latest .NET Core SDK](https://dotnet.microsoft.com/) installed on your PC
2. Install the `Wyam.Tool` .NET Core Global Tool via `dotnet tool install -g Wyam.Tool`
3. Clone the repository and descend into repository root folder.
4. Use the command `wyam -i wwwroot` to build the website.

# View Build Artifacts

Use `wyam preview` to view what's built, or double-click `serve.cmd`.

Open `http://localhost:5080` in your browser!