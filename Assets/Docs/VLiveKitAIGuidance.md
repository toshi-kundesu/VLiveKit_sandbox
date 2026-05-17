# VLiveKit AI Guidance

This file is intentionally short and practical. It is for Unity AI Assistant, Codex, MCP clients, and other agents that need project-local context while editing this sandbox.

## Read Order

1. Read root `AGENTS.md` for release, licensing, submodule, and package rules.
2. Read `Assets/Project_Overview.md` for the current sandbox map.
3. Read the target package README and nearby code before changing a package.
4. Use the narrowest relevant Unity Console, editor, or package validation after changes.

## Safe Scene Work

- Inspect the active scene before edits. Capture scene name, path, dirty state, and root objects.
- Never save an already dirty scene unless the user explicitly asks.
- Prefer temporary objects named `VLiveKit_AI_*` for marker/probe objects.
- Put generated sandbox assets under `Assets/VLiveKitGenerated` unless a package or sample explicitly owns them.
- Remove temporary AI markers when they are no longer useful.
- After any editor script change, read Unity Console and fix compile errors first.

## UI Toolkit For AI-Friendly Tools

Use UI Toolkit for new sandbox tools that are meant to be inspected, modified, or driven by AI:

- Keep `.uxml`, `.uss`, and controller `.cs` files separate when practical.
- Give important controls stable `name` values such as `refresh-button`, `snapshot-field`, and `status-label`.
- Use USS classes for state such as `--hidden`, `--selected`, or `--dirty` instead of one-off inline styling.
- Keep layouts dense, neutral, and editor-like.

For existing VLiveKit package editor windows, do not convert IMGUI to UI Toolkit just for style. Preserve existing inspector/window behavior unless the task is specifically about UI migration.

## Code Preferences

- Keep comments in English.
- Keep editor-only code inside `Editor` folders.
- Keep runtime code free of `UnityEditor` references.
- Prefer data-driven settings and clear component names over hidden side effects.
- Update user-facing README content when adding a package feature, editor window, menu, sample, or installer-visible behavior.

## MCP Notes

- Unity MCP discovery files live under `%USERPROFILE%\.unity\mcp\connections`.
- Use the `project_path` recorded in the discovery file when launching the relay; it may point to the `Assets` folder rather than the project root.
- Useful low-risk tools are console read, active scene read, hierarchy read, and undoable GameObject creation.
- Treat `Unity_RunCommand` as powerful editor code execution. Keep commands small and inspectable.
