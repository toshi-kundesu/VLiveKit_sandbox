# VLiveKit Sandbox Project Overview

This document is a compact map for AI assistants and MCP sessions working in the VLiveKit sandbox project.

## Project

- Unity: 6000.3.14f1
- Render pipeline: HDRP 17.3.0
- Purpose: Develop, validate, and stage VLiveKit Unity packages before package releases.
- Package manager: local `file:` dependencies in `Packages/manifest.json`.

## Repository Shape

- `Packages/VLiveKit`: installer and package-manager UI for `com.toshi.vlivekit`.
- `Packages/VLiveKit_*`: individual VLiveKit package submodules.
- `Packages/Memo`: writing notes and articles, not a Unity runtime package.
- `Assets/VLiveKitGenerated`: sandbox-generated assets and temporary editor helpers.
- `Docs`: project-level design, Codex, and AI workflow notes.

## Package Roles

| Path | Role |
| --- | --- |
| `Packages/VLiveKit` | Installer/catalog/update surfaces. Keep installer-only. |
| `Packages/VLiveKit_LiveToon` | HDRP toon shader, VRM conversion, character look/setup tools. |
| `Packages/VLiveKit_LiveLensFilters` | HDRP custom passes and post-process lens effects. |
| `Packages/VLiveKit_TestAssetsContainer` | Reusable test-scene helpers and sample dependencies. |
| `Packages/VLiveKit_VideoRack` | Video/FFmpeg-related package work. |
| `Packages/VLiveKit_camera` | Camera-related VLiveKit package work. |
| `Packages/VLiveKit_ThirdPartyUtilities` | Private/local third-party utilities only. Do not npm-publish. |

## Scene Editing Through AI/MCP

Use MCP scene tools conservatively:

1. Read the active scene name, path, dirty state, and root hierarchy before changing anything.
2. If the scene is already dirty, avoid creating/loading/saving scenes unless the user asks.
3. Put AI-created temporary scene objects under clear names such as `VLiveKit_AI_*` or `MCP_Codex_*`.
4. Prefer undoable editor operations and leave scenes unsaved by default.
5. Read Unity Console after changes and fix compile errors before continuing.

The sandbox helper window `toshi/VLiveKit/AI/Scene Snapshot` provides a UI Toolkit view that copies an AI-readable scene summary and creates/removes temporary marker objects without saving the scene.

## UI And Editor Guidance

- For existing VLiveKit package editor windows, preserve current IMGUI behavior and shared `VLiveKitEditorUI` primitives unless a UI Toolkit migration is requested.
- For new AI-facing sandbox tools, UI Toolkit is preferred because named UXML elements and USS classes give agents a stable structure to inspect and modify.
- Keep runtime assemblies free of `UnityEditor` references. Put editor-only code under `Editor` folders.
- Keep UI copy calm and tool-like. Avoid decorative editor surfaces.

## Common Gotchas

- Submodules under `Packages/` may need their own commits before the sandbox can commit pointer updates.
- `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `dist/`, and generated Unity artifacts are editor output unless the task says otherwise.
- The installer must not overwrite local, submodule, or `Assets/` installs automatically.
- Unity Package Manager registry state, not GitHub `main`, is the source of truth for package update checks.
