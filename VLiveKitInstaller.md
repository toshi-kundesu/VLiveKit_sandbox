# VLiveKit Installer

This is a tiny one-file installer for users who are not familiar with Git submodules or Unity Package Manager URLs.

## Drag and drop

1. Drop `Assets/VLiveKitInstaller.cs` into the target Unity project's `Assets` folder.
2. Wait for Unity to reload scripts.
3. The `VLiveKit Installer` popup checks the current project and shows missing packages.
4. Click `Install Missing` to add only the packages that are not already present.

The installer checks Unity Package Manager, `Packages/manifest.json`, local `Packages` folders or submodules, and matching `Assets/toshi.VLiveKit` folders.

You can run it again later from:

`Window > VLiveKit > Check Install Status`

`Window > VLiveKit > Install Missing Packages`

## Unitypackage

For release distribution, export `Assets/VLiveKitInstaller.cs` as a small `.unitypackage`.

Users can import it from:

`Assets > Import Package > Custom Package...`
