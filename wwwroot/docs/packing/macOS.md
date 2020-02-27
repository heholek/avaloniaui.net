Title: macOS Packing
Order: 0
---

macOS applications are typically distributed in a `.app` [application bundle](https://en.wikipedia.org/wiki/Bundle_(macOS)#macOS_application_bundles). To make .NET Core and Avalonia projects work in a `.app` bundle, some extra legwork has to be done after your application has gone through the publishing process.

With Avalonia, you'll have a `.app` folder structure that looks like this:

```
MyProgram.app
|
----Contents\
    |
    ------_CodeSignature\ (stores code signing information)
    |     |
    |     ------CodeResources
    |
    ------MacOS\ (all your DLL files, etc. -- the output of `dotnet publish`)
    |     |
    |     ---MyProgram.dll
    |     |
    |     ---Avalonia.dll
    |
    ------Resources\
    |     |
    |     -----MyProgramIcon.icns (icon file)
    |
    ------Info.plist [stores information on your bundle identifier, version, etc.)
```

For more information on `Info.plist`, see [Apple's documentation here]((https://developer.apple.com/library/archive/documentation/General/Reference/InfoPlistKeyReference/Introduction/Introduction.html)).

# Making the application bundle

There are a few options available for creating the `.app` file/folder structure. You can do this on any operating system, since a `.app` file is just a set of folders laid out in a specific format and the tooling isn't specific to one operating system.

If at any point the tooling gives you an error that your assets file doesn't have a target for `osx-64`, add the following [runtime identifiers](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) to the top `<PropertyGroup>` in your `.csproj`: 

```xml
<RuntimeIdentifiers>osx-x64</RuntimeIdentifiers>
```

Add other runtime identifiers as necessary. Each one should be separated by a semicolon (;).

## dotnet-bundle

[dotnet-bundle](https://github.com/egramtel/dotnet-bundle) is a [NuGet package](https://www.nuget.org/packages/Dotnet.Bundle/) that publishes your project and then creates the `.app` file for you.

You'll first have to add the project as a `PackageReference` in your project. Add it to your project via NuGet package manager or by adding the following line to your `.csproj` file (run `dotnet restore` after you do this!):

```xml
<PackageReference Include="Dotnet.Bundle" Version="*" />
```

After that, you can create your `.app` by executing the following on the command line:

```
dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64
```

You can specify other parameters for the `dotnet msbuild` command. For instance, if you want to publish in release mode:

```
dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64 -property:Configuration=Release
```

or if you want to specify a different app name:

```
dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64 -p:CFBundleDisplayName=MyBestThingEver
```

By default, this will put the `.app` file in the same place as the `publish` output: `[project directory]/bin/{Configuration}/netcoreapp3.1/osx-x64/publish/MyBestThingEver.app`.

For more information on the parameters you can send, see the [dotnet-bundle documentation](https://github.com/egramtel/dotnet-bundle).

## Manual

First, publish your application ([dotnet publish documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish)):

```
dotnet publish -r osx-x64 --configuration Release
```

Create your `Info.plist` file, adding or modifying keys as necessary:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleIconFile</key>
    <string>myicon-logo.icns</string>
    <key>CFBundleIdentifier</key>
    <string>com.identifier</string>
    <key>CFBundleName</key>
    <string>DotPurple</string>
    <key>CFBundleVersion</key>
    <string>1.0.0</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.12</string>
    <key>CFBundleExecutable</key>
    <string>MyApp.Avalonia</string>
    <key>CFBundleInfoDictionaryVersion</key>
    <string>6.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0</string>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
```

You can then create your `.app` folder structure as outlined at the top of this page. If you want a script to do it for you, you can use something like this (macOS/Unix):

```bash
#!/bin/bash
APP_NAME="/path/to/your/output/MyApp.app"
PUBLISH_OUTPUT_DIRECTORY="/path/to/your/publish/output/netcoreapp3.1/osx-64/publish/."
INFO_PLIST="/path/to/your/Info.plist"
ICON_FILE="/path/to/your/myapp-logo.icns"

if [ -d "$APP_NAME" ]
then
    rm -rf "$APP_NAME"
fi

mkdir "$APP_NAME"

mkdir "$APP_NAME/Contents"
mkdir "$APP_NAME/Contents/MacOS"
mkdir "$APP_NAME/Contents/Resources"

cp "$INFO_PLIST" "$APP_NAME/Contents/Info.plist"
cp "$ICON_FILE" "$APP_NAME/Contents/Resources/$ICON_FILE"
cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_NAME/Contents/MacOS"
```

# Signing Your App

Once you have your `.app` file created, you'll probably want to sign your app so that it can be notarized and distributed to your users without Gatekeeper giving you a hassle. Notarization is required for apps distributed outside the app store starting in macOS 10.15 (Catalina), and you'll have to enable [hardened runtime](https://developer.apple.com/documentation/security/hardened_runtime?language=objc) and run `codesign` on your `.app` in order to notarize it successfully.

You'll need a Mac computer for this step, unfortunately, as we have to run the `codesign` command line tool that comes with Xcode.

## Running codesign and enabling hardened runtime

Enabling hardened runtime is done in the same step as code signing. You have to codesign everything in the `.app` bundle under the `Contents/MacOS` folder, which is easiest to do with a script since there are a lot of files. In order to sign your files, you need an Apple developer account. In order to notarize your app, you'll need to do the following steps with a [Developer ID certificate](https://developer.apple.com/developer-id/), which requires a paid Apple developer subscription.

You'll also need to have the Xcode command line tools installed. You can get those by installing Xcode and running it or by running `xcode-select --install` on the command line and following the prompts to install the tools

First, enable Hardened Runtime with [exceptions](https://developer.apple.com/documentation/security/hardened_runtime?language=objc) by creating an `MyAppEntitlements.entitlements` file:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>com.apple.security.cs.allow-jit</key>
    <true/>
    <key>com.apple.security.automation.apple-events</key>
    <true/>
</dict>
</plist>
```

Then, run this script to do all the code signing for you:

```bash
#!/bin/bash
APP_NAME="/path/to/your/output/MyApp.app"
ENTITLEMENTS="/path/to/your/MyAppEntitlements.entitlements"
SIGNING_IDENTIFY="Developer ID: MyCompanyName" # matches Keychain Access certificate name

find "$APP_NAME/Contents/MacOS/"|while read fname; do
    if [[ -f $fname ]]; then
        echo "[INFO] Signing $fname"
        codesign --force --timestamp --options=runtime --entitlements "$ENTITLEMENTS" --sign "$SIGNING_IDENTITY" $fname
    fi
done

echo "[INFO] Signing app file"

codesign --force --timestamp --options=runtime --entitlements "$ENTITLEMENTS" --sign "$SIGNING_IDENTITY" "$APP_NAME"
```

The `--options=runtime` part of the `codesign` line is what enables the hardened runtime with your app. Because [.NET Core may not be fully compatible with hardened runtime](https://github.com/dotnet/runtime/issues/10562#issuecomment-503013071), we add some exceptions to use JIT-compiled code and allow for Apple Events to be sent. The JIT-compiled code exception is required to run Avalonia apps under hardened runtime. We add the second exception for Apple Events to fix an error that shows up in your console (and any potential issues at the notarization stage).

Once your app is code signed, you can verify that it signed properly by making sure that the following command outputs no errors:

```
codesign --verify --verbose /path/to/MyApp.app
```

## Notarizing your software

Notarization allows your app to be distributed outside the macOS App Store. You can read more about it [here](https://developer.apple.com/documentation/xcode/notarizing_macos_software_before_distribution). If you run into any issues during the process, Apple has a helpful document of potential fixes [here](https://developer.apple.com/documentation/xcode/notarizing_macos_software_before_distribution/resolving_common_notarization_issues?language=objc).

For more information on customizing your notarization workflow and more flags you may need to send when running `xcrun altool`, [check out Apple's documentation](https://developer.apple.com/documentation/xcode/notarizing_macos_software_before_distribution/customizing_the_notarization_workflow?language=objc).

The following steps were modified from [this StackOverflow post](https://stackoverflow.com/a/53121755/3938401):

1. Make sure your `.app` is code signed properly
2. Run `xcrun altool --notarize-app -f MyApp.app --primary-bundle-id com.identifier -u username -p password`. You can use a password in your keychain by passing `-p "@keychain:AC_PASSWORD", where AC_PASSWORD is the key. The account has to be registered as an Apple Developer.
3. If the upload is successful, you'll get a UUID back for your request token like this: `28fad4c5-68b3-4dbf-a0d4-fbde8e6a078f`
4. You can check notarization status using that token like this: `xcrun altool --notarization-info 28fad4c5-68b3-4dbf-a0d4-fbde8e6a078f -u username -p password`. This could take some time -- eventually it will succeed or fail.
5. If it succeeds, you have to staple the notarization to the app: `xcrun stapler staple MyApp.app`. You can validate this by running `xcrun stapler validate MyApp.app`.

Once notarization is complete, you should be able to distribute your application!

Note that if you distribute your app in a `.dmg`, you will want to modify the steps slightly:

1. Notarize your `.app` as normal
2. Add your notarized and stapled (`xcrun stapler`) to the DMG.
3. Notarize your `.dmg` file
4. Staple the notarization to the `.dmg` file: `xcrun stapler staple MyApp.dmg`.