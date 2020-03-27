#!/bin/sh

dotnet publish --configuration Release
rsync -avz --delete src/AvaloniaUI.Homepage//bin/Release/netcoreapp3.1/publish/ grokys@95.216.209.17:/var/www/avaloniaui.net
