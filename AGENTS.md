# AGENTS.md

This repository is a Unity sandbox project for developing and validating VLiveKit packages.

## Project Context

- Unity version: `2022.3.9f1`
- Render pipeline: HDRP `14.0.8`
- Main branch: `main`
- Package dependencies are declared in `Packages/manifest.json`.
- VLiveKit packages live under `Packages/` as git submodules and are referenced by Unity Package Manager with local `file:` dependencies.
- `Packages/Memo` is for articles and production notes, not a Unity runtime package.

## Working Rules

- Preserve user changes. Do not revert local edits or submodule changes unless explicitly asked.
- Keep changes scoped to the requested package or sandbox area.
- Treat `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `dist/`, and generated Unity artifacts as uncommitted build/editor output unless the user specifically asks about them.
- Commit Unity `.meta` files together with their corresponding assets.
- Avoid broad package refactors unless the task explicitly requires cross-package changes.
- When editing submodules under `Packages/`, remember that the submodule repository may need its own commit before the sandbox submodule pointer can be committed.

## VLiveKit Package/Release Rules

- `Packages/VLiveKit` is the installer/package-manager repository (`com.toshi.vlivekit`) and should stay installer-only.
- Keep `com.toshi.vlivekit` free of dependencies that require extra scoped registries; otherwise the installer can fail to update itself before it can add those registries.
- Do not add dependencies from `com.toshi.vlivekit` to individual VLiveKit packages. This package exists to install/check/update other packages, not to bundle them.
- Do not publish or package third-party binaries/tools as first-party VLiveKit packages unless the user explicitly approves the license plan. In particular, do not publish `VLiveKit_ThirdPartyUtilities` to npm by default.
- Individual VLiveKit packages are separate submodules and npm packages. Updating one package requires changing that package's own `package.json`, committing/tagging/pushing that submodule, then updating the sandbox submodule pointer.
- The installer reads package metadata from `package-catalog.json`. Keep repository/documentation links there so Refresh can pick up catalog changes from the latest `com.toshi.vlivekit` release.
- Include `com.toshi.vlivekit` itself in the installer update list/catalog so the package manager can update itself through the same Refresh/Update flow.
- Local package folders and `Assets/` folders are detection targets only. The installer should not overwrite local/submodule/Assets installs automatically.
- `Refresh` should check the latest published npm registry state. It should not be assumed to track GitHub `main` or submodule HEADs directly.
- Use `npm.cmd` on Windows PowerShell for npm operations to avoid `npm.ps1` execution policy issues.
- Before publishing, run `npm.cmd pack --dry-run --cache D:\GitHub\VLiveKit_sandbox\.npm-cache` from the package root and confirm the tarball contains only intended files.
- After publishing, verify with `npm.cmd view <package> version dependencies --json --cache D:\GitHub\VLiveKit_sandbox\.npm-cache`.
- Release order for a package: bump `package.json`, update README install examples if needed, pack dry-run, publish to npm, commit, tag `vX.Y.Z`, push branch/tag, then commit/push the sandbox submodule pointer.
- When releasing a submodule change, push the matching `vX.Y.Z` tag from that submodule repository; do not rely on the sandbox submodule pointer alone as the release marker.
- When releasing `Packages/VLiveKit_VideoRack` with bundled FFmpeg, read `Assets/toshi.VLiveKit/VideoRack/ThirdPartyNotices/FFmpeg.md` first. Preserve `Tools/FFmpeg/Windows/ffmpeg.exe`, `GPL-3.0.txt`, and upstream `README.txt`, and include clear Corresponding Source instructions in the release notes/download page.

## Unity/C# Conventions

- Prefer existing local namespaces, folder layout, asmdef boundaries, and editor/runtime separation.
- Put editor-only code under `Editor` folders and keep runtime assemblies free of `UnityEditor` references.
- Put custom Unity editor menu items under `toshi/...`; do not place VLiveKit windows or tools under `Tools/...`.
- Use Unity serialization-friendly patterns for MonoBehaviours and ScriptableObjects.
- Keep public serialized fields and inspector labels stable unless a migration is part of the task.
- Add comments only for non-obvious Unity lifecycle, rendering, shader, or timeline behavior.

## Verification

- For repository state:

```powershell
git status --short --branch
git diff --submodule
```

- For submodules:

```powershell
git submodule status
git submodule update --init --recursive
```

- For Unity validation, prefer the narrowest relevant Editor or package test. If Unity batchmode is already running for this workspace, avoid launching a second Unity instance.

## Codex Notes

- Use this file as the project memory for Codex CLI/Codex Desktop.
- Prefer concise Japanese explanations when the user writes in Japanese.
- If a task depends on current external docs, verify them from official sources before implementing.
- When a durable project rule, release lesson, license constraint, or recurring workflow becomes clear, proactively add a concise note to this file and mention it to the user. Avoid adding temporary conversation notes or noisy details.
- Keep VLiveKitPackageManager UI restrained: two-color styling, dark/light base plus cyan accent, and no decorative multicolor signal lines.
- In Unity IMGUI editor windows, do not trust a standalone `stylesReady` flag after domain reloads; also check cached `GUIStyle` fields for null before skipping style initialization.
- When reading Unity Console entries via reflection, `LogEntry` may behave like a value type; after `GetEntryInternal` returns, read the populated entry back from the invocation argument array.
