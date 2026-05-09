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
- For non-trivial work, create a focused branch before editing, using the `codex/` prefix unless the user requests another branch name.
- Keep changes scoped to the requested package or sandbox area.
- Treat `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `dist/`, and generated Unity artifacts as uncommitted build/editor output unless the user specifically asks about them.
- Commit Unity `.meta` files together with their corresponding assets.
- Avoid broad package refactors unless the task explicitly requires cross-package changes.
- When editing submodules under `Packages/` or sandbox submodules under `Assets/`, remember that the submodule repository may need its own commit before the sandbox submodule pointer can be committed.

## VLiveKit Package/Release Rules

- `Packages/VLiveKit` is the installer/package-manager repository (`com.toshi.vlivekit`) and should stay installer-only.
- Put reusable test-scene helper components, such as scene description popups, in `VLiveKit_TestAssetsContainer` or the owning test package instead of `Packages/VLiveKit`.
- Kino-derived or Kino-inspired code that has been modified for `VLiveKit_LiveLensFilters` should be treated as VLiveKit LiveLensFilters code: keep menus/shader paths grouped under `toshi/LensFilters` while preserving third-party license notices.
- Keep `com.toshi.vlivekit` free of dependencies that require extra scoped registries; otherwise the installer can fail to update itself before it can add those registries.
- Do not add dependencies from `com.toshi.vlivekit` to individual VLiveKit packages. This package exists to install/check/update other packages, not to bundle them.
- Do not publish or package third-party binaries/tools as first-party VLiveKit packages unless the user explicitly approves the license plan. In particular, do not publish `VLiveKit_ThirdPartyUtilities` to npm by default.
- When bundling approved third-party texture/material assets in a public VLiveKit package, keep the source URL, copied license/terms, and included file scope in that package's `ThirdPartyNotices`.
- For third-party avatar/model assets used in public npm samples or test scenes, bundle only assets whose source terms explicitly allow redistribution, keep them under samples/test asset paths, and include separate third-party notices; do not mark VRM metadata as CC0 unless the source license is actually CC0.
- `VLiveKit_ThirdPartyUtilities` is intentionally private and may require GitHub permissions to clone; keep it as a sandbox/submodule dependency only and never npm-publish third-party assets from it.
- Keep Tripo Bridge / `Tripo3d_Unity_Bridge` only in the private third-party assets repository unless Tripo AI provides explicit redistribution terms; do not bundle it into public VLiveKit packages or npm releases.
- Keep the private third-party assets repository out of VLiveKit installer catalogs and install-all/update flows.
- Individual VLiveKit packages are separate submodules and npm packages. Updating one package requires changing that package's own `package.json`, committing/tagging/pushing that submodule, then updating the sandbox submodule pointer.
- The installer reads package metadata from `package-catalog.json`. Keep repository/documentation links there so Refresh can pick up catalog changes from the latest `com.toshi.vlivekit` release.
- Include `com.toshi.vlivekit` itself in the installer update list/catalog so the package manager can update itself through the same Refresh/Update flow.
- Local package folders and `Assets/` folders are detection targets only. The installer should not overwrite local/submodule/Assets installs automatically.
- `Refresh` should check the latest published npm registry state. It should not be assumed to track GitHub `main` or submodule HEADs directly.
- Use `npm.cmd` on Windows PowerShell for npm operations to avoid `npm.ps1` execution policy issues.
- Before publishing, run `npm.cmd pack --dry-run --cache D:\GitHub\VLiveKit_sandbox\.npm-cache` from the package root and confirm the tarball contains only intended files.
- For public UPM releases, prefer signed packages using Unity 6.3.5f2 or newer `-upmPack` before `npm.cmd publish` so Unity Package Manager does not show `Missing Signature`. The signing command requires `-cloudOrganization`, `-username`, and `-password`; use environment variables or a secure local secret flow, not pasted chat credentials.
- Verify signed `.tgz` files contain `.attestation.p7m` before publishing. Publish the signed tarball path with `npm.cmd publish <signed.tgz> --cache D:\GitHub\VLiveKit_sandbox\.npm-cache`.
- For UPM samples, publish the actual importable content from `Samples~/...` and list it in `package.json` `samples`; a visible `Sample/` folder may be kept only as a local development copy and should be excluded from npm packages when duplicated.
- After publishing, verify with `npm.cmd view <package> version dependencies --json --cache D:\GitHub\VLiveKit_sandbox\.npm-cache`.
- Release order for a package: bump `package.json`, update README install examples if needed, pack dry-run, publish to npm, commit, tag `vX.Y.Z`, push branch/tag, then commit/push the sandbox submodule pointer.
- When releasing a submodule change, push the matching `vX.Y.Z` tag from that submodule repository; do not rely on the sandbox submodule pointer alone as the release marker.
- When releasing `Packages/VLiveKit_VideoRack` with bundled FFmpeg, read `Assets/toshi.VLiveKit/VideoRack/ThirdPartyNotices/FFmpeg.md` first. Preserve the Windows executable payload (`Tools/FFmpeg/Windows/ffmpeg.exe` or all `ffmpeg.exe.part*.bytes` split files), `GPL-3.0.txt`, and upstream `README.txt`, and include clear Corresponding Source instructions in the release notes/download page.

## Unity/C# Conventions

- Inspector / Editor UI consistency guidance lives in `Docs/VLiveKitDesignGuidelines.md`; prefer preserving existing Inspector behavior and adding VLiveKit header/footer affordances before broader UI refactors.

- Prefer existing local namespaces, folder layout, asmdef boundaries, and editor/runtime separation.
- Put editor-only code under `Editor` folders and keep runtime assemblies free of `UnityEditor` references.
- Put custom Unity editor menu items under `toshi/...`; do not place VLiveKit windows or tools under `Tools/...`.
- Use Unity serialization-friendly patterns for MonoBehaviours and ScriptableObjects.
- Keep public serialized fields and inspector labels stable unless a migration is part of the task.
- Add comments only for non-obvious Unity lifecycle, rendering, shader, or timeline behavior.

## VLiveKit Editor UI Design Guidelines

- Treat Apple Human Interface Guidelines Foundations as the baseline for VLiveKit editor UI decisions: clarity, hierarchy, consistency, accessibility, direct feedback, and restrained use of color.
- Prefer native Unity editor controls, table/list structures, clear labels, separators, and spacing over decorative cards, banners, gradients, or custom visual styling.
- Avoid green-tinted, warning-tinted, or success-tinted backgrounds for general UI. Use neutral dark/light grays for surfaces and separators.
- Use color only when it carries a specific UI meaning. Keep the primary accent close to Apple system blue for primary actions and progress; do not use broad green accents to imply safety or success.
- Do not show exclamation/warning-style UI from VLiveKit code for ordinary states, success messages, fallback behavior, or recoverable Package Manager timing issues. Prefer inline status text, window notifications, neutral notices, and `Debug.Log` instead of `DisplayDialog`, `HelpBox(MessageType.Warning)`, `Debug.LogWarning`, or `Debug.LogError`.
- Use status labels and plain language to communicate package state (`Current`, `Missing`, `Local`, `Update`) rather than relying on color alone.
- Keep copy short, calm, and specific. Avoid alarming titles, excessive punctuation, or instructions that make normal installation feel risky. Backup guidance should be neutral and practical.
- Keep installer/package-manager surfaces dense enough for repeated editor use: avoid landing-page-like composition, oversized hero areas, nested cards, decorative imagery, and one-note palettes.
- Design for both Unity light and dark editor skins. Ensure contrast comes from text hierarchy and separators, not saturated background color.
- Bootstrap and first-run prompts should match the package manager tone: neutral, concise, and icon-free, with no scary warning dialogs generated by VLiveKit code.
- Share VLiveKit editor UI primitives through `VLiveKitEditorUI` for windows in `com.toshi.vlivekit`. Add new shared styles there instead of duplicating colors, spacing, panel, separator, header, or button rules in each window.

## Verification

- For repository state:

```powershell
git status --short --branch
git diff --submodule
```

- Before starting substantial work or publishing changes, check whether the current branch is behind/diverged from its upstream and whether a merge/rebase would conflict.

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
- When a durable project rule, release lesson, license constraint, recurring workflow, UI/design guideline, or implementation lesson becomes clear, proactively add a concise note to this file and mention it to the user. Avoid adding temporary conversation notes or noisy details.
- In Unity IMGUI editor windows, do not trust a standalone `stylesReady` flag after domain reloads; also check cached `GUIStyle` fields for null before skipping style initialization.
- When reading Unity Console entries via reflection, `LogEntry` may behave like a value type; after `GetEntryInternal` returns, read the populated entry back from the invocation argument array.
- Prefer UPM dependencies for common third-party libraries such as OscJack, uOSC, and uLipSync instead of vendoring their source inside VLiveKit packages; keep bundled copies only when the registry version does not match the required embedded version or the license/distribution plan needs explicit approval.
- In the VLiveKit installer, treat Unity Package Manager `PackageInfo.source` values `Local`/`Embedded` and manifest `file:` dependencies as local installs before registry update logic, so sandbox submodules are detected and never overwritten by installer updates.
- Keep Memo/Zenn editor entry points under `toshi/VLiveKit/Project/Zenn Window`; do not expose separate `Tools/Memo` commands for preview/start/create actions.
- For project-wide recommended settings, expose menu items as `Open` only; keep apply actions inside the window so users can review before changing project settings.
- When adding or changing a VLiveKit package feature, editor window, menu, sample, or installer-visible behavior, update that package's README with clear user-facing usage and package contents instead of adding explanatory UI to the installer.
- For Unity/HDRP API migrations, prefer version-gated helper methods around renamed light/entity APIs; suppress obsolete warnings only when Unity keeps an old callback shape in a version that marks the lookup API obsolete.
- For VLiveKit test/monitor editor windows such as NDI, LTC, and OSC tools, prefer a window-owned live mode that works without Play Mode; keep scene rig creation as an optional handoff path when a persistent setup is useful.
- Unity Package Manager does not support Git URL dependencies inside a package `package.json`; add Git packages to the project `Packages/manifest.json`, or publish/mirror them to a scoped registry before making another package depend on them.
- When analyzing or customizing HDRP Lit internals, confirm the active HDRP package version from `Packages/manifest.json` and `Library/PackageCache`; keep Lit shader properties, local keywords, and `LitAPI`/`BaseLitAPI` material validation logic in sync.
- For minimal HDRP shaders that include `VertMesh.hlsl` but own their fragment shader, define a small `Vert` wrapper that returns `PackVaryingsType(VertMesh(...))`; `VertMesh.hlsl` alone does not provide the `#pragma vertex Vert` entry point.
- For HDRP depth-buffer Sobel rim tests, sample `SampleCameraDepth` from view-space offsets around the object position and include a `DepthOnly` pass so the object can participate in the camera depth texture.
- For HDRP custom post-process test effects, add the component type to HDRP Global Settings Custom Post Process Orders at the matching injection point; tone-mapping replacements should run after post-process blurs and disable built-in Tonemapping to avoid double mapping.
- For LiveToon VRM 0.x shader conversion issues, compare converted Unity Material assets against the source `.vrm` `extensions.VRM.materialProperties` before changing converter logic; if values match but the model renders black, inspect the LiveToon shader lighting fallback rather than assuming property loss.
- LiveToon VRM conversion should preserve MToon culling values; if culling appears inverted, inspect whether the shader is using the legacy GBuffer/depth-normal path instead of the ForwardOnly toon pass before changing converter logic.
- For LiveToon VRM 0.x conversion, start from the LoadModel-style baseline: swap to the LiveToon shader, preserve existing MToon-compatible material properties such as `_CullMode`, `_SrcBlend`, `_DstBlend`, `_ZWrite`, and `_BlendMode`, then adjust only minimal derived state such as `renderQueue` and missing `_ShadeTexture`.
- MToon Transparent opacity should be carried through `_Color.a` multiplied by `_MainTex.a`; explicitly preserve `_Color` during LiveToon conversion before adding any extra transparent threshold behavior.
- After a LiveToon shader swap, restore `RenderType` tags and alpha keywords from `_BlendMode`; Unity can drop material override tags such as `RenderType: Transparent`, which breaks transparent eye materials even when `_BlendMode`, blend factors, and textures match the backup.
- For LiveToon VRM Transparent materials, keep `_BlendMode` as Transparent and preserve LiveToon's legacy transparent opacity formula from the ShadowCaster path before simplifying to `_Color.a * _MainTex.a`; the dither `clip` belongs to shadow/depth-style passes, not ordinary Forward alpha blending.
- LiveToon's Forward pass uses `_ZTeForLiOpa` for `ZTest`; keep Cutout and Transparent/TransparentWithZWrite materials on `LEqual` (`4`) while Opaque may use `Equal` (`3`), otherwise VRM transparency and alpha-tested facial/hair details can composite incorrectly.
- Guard LiveToon transparent atmospheric scattering behind `_ENABLE_FOG_ON_TRANSPARENT`; VRM conversion should enable the keyword for Transparent materials like the legacy LoadModel flow, while Opaque/Cutout materials keep it disabled.
- Do not add LiveToon cull indirection or forced debug cull properties until the shader-side culling path is proven to require it; first verify that Off/Front/Back values are preserved directly on `_CullMode`.
- The LiveToon shader converter should skip materials already using the LiveToon shader; restore from a backup before reconverting so original VRM 0.x cull, outline-cull, and keyword values are not replaced by stale converted values.
- When preserving a side-by-side Unity legacy snapshot, copy the historical files but regenerate Unity `.meta` GUIDs and rename shader paths, package names, asmdef names, and menu entries so the snapshot can coexist with the active implementation without collisions.
- LiveToon should not require a scene Directional Light just to stay visible: use scene Directional Lights for shadow-controlled key lighting, but provide a shader-side fallback key light when `_DirectionalLightCount` is zero and skip scene shadow sampling for that fallback path.
