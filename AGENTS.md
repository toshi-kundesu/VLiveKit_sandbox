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
- Keep downloaded Unity HDRP sample scene assets in `VLiveKit_TestAssetsContainer` rather than project-level `Assets/TestAssets`; the test-assets package should own sample dependencies such as HDRP, VFX Graph, Cinemachine, Timeline, and Input System.
- Kino-derived or Kino-inspired code that has been modified for `VLiveKit_LiveLensFilters` should be treated as VLiveKit LiveLensFilters code: keep menus/shader paths grouped under `toshi/LensFilters` while preserving third-party license notices.
- Keep `com.toshi.vlivekit` free of dependencies that require extra scoped registries; otherwise the installer can fail to update itself before it can add those registries.
- Do not add dependencies from `com.toshi.vlivekit` to individual VLiveKit packages. This package exists to install/check/update other packages, not to bundle them.
- Do not publish or package third-party binaries/tools as first-party VLiveKit packages unless the user explicitly approves the license plan. In particular, do not publish `VLiveKit_ThirdPartyUtilities` to npm by default.
- Keep `VLiveKit_ThirdPartyUtilities` npm-disabled for accident prevention: `private: true`, empty `files`, a failing `prepack`, and an all-payload `.npmignore`; it is private/local only and should not produce npm tarballs.
- When bundling approved third-party texture/material assets in a public VLiveKit package, keep the source URL, copied license/terms, and included file scope in that package's `ThirdPartyNotices`.
- For third-party avatar/model assets used in public npm samples or test scenes, bundle only assets whose source terms explicitly allow redistribution, keep them under samples/test asset paths, and include separate third-party notices; do not mark VRM metadata as CC0 unless the source license is actually CC0.
- `VLiveKit_ThirdPartyUtilities` is intentionally private and may require GitHub permissions to clone; keep it as a sandbox/submodule dependency only and never npm-publish third-party assets from it.
- Keep AvaPo! (BOOTH item 7249970) as a private local/sandbox-only third-party utility; its terms prohibit sharing, redistribution, resale, copying, reverse engineering, and modification of the tool, so do not publish it in npm/UPM releases or installer catalogs without explicit permission.
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
- For Zenn/Memo writeups about shader rendering lessons, separate general Unity/VRM concepts such as depth, culling, transparency, and material conversion from package-specific implementation notes so the article remains reusable.
- For Unity/HDRP API migrations, prefer version-gated helper methods around renamed light/entity APIs; suppress obsolete warnings only when Unity keeps an old callback shape in a version that marks the lookup API obsolete.
- For VLiveKit test/monitor editor windows such as NDI, LTC, and OSC tools, prefer a window-owned live mode that works without Play Mode; keep scene rig creation as an optional handoff path when a persistent setup is useful.
- Unity Package Manager does not support Git URL dependencies inside a package `package.json`; add Git packages to the project `Packages/manifest.json`, or publish/mirror them to a scoped registry before making another package depend on them.
- For LiveLensFilters, keep Kino as the external `jp.keijiro.kino.post-processing` package dependency and do not rebrand or bundle it as Toshi code; Cinema should stay a project manifest/installer Git dependency unless it is published to a registry.
- When analyzing or customizing HDRP Lit internals, confirm the active HDRP package version from `Packages/manifest.json` and `Library/PackageCache`; keep Lit shader properties, local keywords, and `LitAPI`/`BaseLitAPI` material validation logic in sync.
- For minimal HDRP shaders that include `VertMesh.hlsl` but own their fragment shader, define a small `Vert` wrapper that returns `PackVaryingsType(VertMesh(...))`; `VertMesh.hlsl` alone does not provide the `#pragma vertex Vert` entry point.
- For standalone HDRP shaders that call `SpaceTransforms.hlsl` helpers, include HDRP `ShaderVariables.hlsl` before `SpaceTransforms.hlsl` so `UNITY_MATRIX_M`/`UNITY_MATRIX_I_M` are defined.
- For HDRP depth-buffer Sobel rim tests, sample `SampleCameraDepth` from view-space offsets around the object position and include a `DepthOnly` pass so the object can participate in the camera depth texture.
- For HDRP custom post-process test effects, add the component type to HDRP Global Settings Custom Post Process Orders at the matching injection point; tone-mapping replacements should run after post-process blurs and disable built-in Tonemapping to avoid double mapping.
- For LiveLensFilters CreativeFx HDRP post-processes, sample the camera color input through normalized UV helpers instead of `LOAD_TEXTURE2D_X` with `_ScreenSize`-derived positions; RTHandle scaling can otherwise read black on Unity 6000/HDRP 17.
- For large LiveLensFilters post-process sample grids such as shaped bokeh, avoid forcing nested loops to unroll on D3D11; keep loops explicit or reduce the sample count so the shared shader stays supported.
- For LiveLensFilters LayerBloom custom passes, keep the depth-bound layer source RT physical-size-compatible with CameraDepthStencil, but size the final composite from the current HD camera viewport and sample scaled HDRP camera RTHandles with `rtHandleScale`; otherwise GameView resolution changes can leave the image stuck in the lower-left of a larger allocation.
- For LiveToon VRM 0.x shader conversion issues, compare converted Unity Material assets against the source `.vrm` `extensions.VRM.materialProperties` before changing converter logic; if values match but the model renders black, inspect the LiveToon shader lighting fallback rather than assuming property loss.
- LiveToon VRM conversion should preserve MToon culling values; if culling appears inverted, reconvert to create a fresh material generation before changing converter logic.
- For LiveToon VRM 0.x conversion, start from the LoadModel-style baseline: swap to the LiveToon shader, preserve existing MToon-compatible material properties such as `_CullMode` and `_BlendMode`, then derive `_SrcBlend`, `_DstBlend`, `_ZWrite`, `_AlphaToMask`, `renderQueue`, tags, and keywords from `_BlendMode` instead of keeping stale state floats.
- MToon Transparent opacity should be carried through `_Color.a` multiplied by `_MainTex.a`; explicitly preserve `_Color` during LiveToon conversion before adding any extra transparent threshold behavior.
- After a LiveToon shader swap, restore `RenderType` tags and alpha keywords from `_BlendMode`; Unity can drop material override tags such as `RenderType: Transparent`, which breaks transparent eye materials even when `_BlendMode`, blend factors, and textures match the backup.
- For LiveToon VRM Transparent materials, keep `_BlendMode` as Transparent and preserve LiveToon's legacy transparent opacity formula from the ShadowCaster path before simplifying to `_Color.a * _MainTex.a`; the dither `clip` belongs to shadow/depth-style passes, not ordinary Forward alpha blending.
- Keep LiveToon on the `ForwardOnly` toon path and do not re-enable the legacy HDRP `GBuffer` pass; mixing the two can make depth and toon color ownership hard to reason about.
- When the LiveToon `GBuffer` pass is disabled, keep a `DepthOnly` pass for Opaque, Cutout, and TransparentWithZWrite materials, while discarding normal Transparent materials; otherwise HDRP sky/fog and later geometry can behave as if the toon object has no depth.
- For LiveToon HDRP transparent ordering issues, compare against HDRP Lit/Unity Toon Shader transparent depth pre/post passes before changing cull values or material queues; pure `Queue=Transparent` + `ZWrite Off` can sort incorrectly by renderer distance.
- Keep LiveToon's Forward pass on `ZTest LEqual`, and make converted materials write `_ZTeForLiOpa = LEqual` only for compatibility; hard-coding `ZTest Less` can make sky/transparent composition visibly wrong even when it appears to reduce some far-camera ordering artifacts.
- Do not blindly preserve MToon source `renderQueue` values during LiveToon conversion; high transparent queues such as `3500` can make eye highlights and brows draw through other HDRP geometry, while low Cutout queues such as `2000` can bypass the intended alpha-test bucket.
- Keep LiveToon TransparentWithZWrite materials on render queue `2501` with `_ZWrite` enabled; using the normal Transparent queue `3000` can make HDRP Lit ordering look camera-distance dependent.
- Keep LiveToon's outline pass depth-tested with `ZTest LEqual` and the material's `_ZWrite` state; investigate ordering artifacts from material queues/depth ownership before hard-coding outline depth behavior.
- Keep LiveToon's outline pass on a lightweight outline-only fragment; routing it through the full forward lighting path can combine HDRP light-loop variants with custom shadow samplers and exceed D3D11 sampler limits, producing magenta variants.
- In HDRP 6000.3, `SHADOW_LOW` compatibility fallback defines punctual and directional shadow filters but not area shadow filters; if a LiveToon pass includes HDRP lighting without area shadow variants, add an explicit `AREA_SHADOW_MEDIUM` fallback before `Lighting.hlsl`.
- Do not enable LiveToon TransparentDepthPrepass/Postpass for ordinary MToon Transparent materials by default; alpha mipmaps can make cutout areas write broad depth at distance. Reserve those passes for TransparentWithZWrite or explicit debug cases.
- Avoid clip-space z nudges such as `positionCS.z += 0.001 * positionCS.w` in LiveToon forward, depth, and outline passes; they can desynchronize color/depth ownership and expose magenta or wrong-order surfaces near the camera near clip plane.
- Guard LiveToon transparent atmospheric scattering behind `_ENABLE_FOG_ON_TRANSPARENT`; VRM conversion should enable the keyword for Transparent materials like the legacy LoadModel flow, while Opaque/Cutout materials keep it disabled.
- Do not add LiveToon cull indirection or forced debug cull properties until the shader-side culling path is proven to require it; first verify that Off/Front/Back values are preserved directly on `_CullMode`.
- The LiveToon shader converter should be non-destructive and generational: create fresh LiveToon material copies on every conversion run, assign those copies to renderer slots, keep source material assets unchanged, and use legacy backup restore only for old in-place conversions.
- LiveToon conversion should preserve MToon outline settings by default and rebuild outline width/color keywords from `_OutlineWidthMode` and `_OutlineColorMode`; only disable outlines through an explicit debug option.
- Keep per-scene LiveToon conversion settings in a runtime MonoBehaviour with Editor-only inspector actions; do not put UnityEditor/AssetDatabase calls in the runtime component.
- When preserving a side-by-side Unity legacy snapshot, copy the historical files but regenerate Unity `.meta` GUIDs and rename shader paths, package names, asmdef names, and menu entries so the snapshot can coexist with the active implementation without collisions.
- LiveToon should not require a scene Directional Light just to stay visible: use scene Directional Lights for shadow-controlled key lighting, but when `_DirectionalLightCount` is zero, build the shader-side fallback key from attenuated punctual lights first, then use the camera-facing fallback only if no punctual light reaches the pixel.
- Keep LiveToon punctual lights as secondary accents: avoid ad hoc large multipliers or double exposure application on point/spot light paths, and expose a small material-level intensity control instead.
- Keep LiveToon custom stage rim and MToon rim separate: preserve the original MToon `_RimColor`/`_RimTexture`/`_RimLightingMix`/`_RimFresnelPower`/`_RimLift` path without an extra MToon-only multiplier, drive `_CustomRimIntensity` from bounded punctual diffuse light rather than the full punctual shaded color, and allow a modest no-directional-light final-color base lift so local fixtures visibly brighten the character.
- For LiveToon environment lighting, keep `_IndirectLightIntensity` as Ambient Probe diffuse, expose it in `LiveToonInspector_MToonBase` alongside Reflection Probe/Sky controls, and sample HDRP environment data from the LightLoop in the `ForwardOnly` toon path rather than re-enabling GBuffer.
- For LiveToon shader material UI, wrap the normal `MToon.MToonInspector` with `LiveToonInspector_MToonBase` and append LiveToon-only options below it instead of forking or replacing the MToon inspector behavior.
- For LiveToon character-level look controls such as spherical face normals, face-space Directional Light limiting, and perspective correction, prefer a root MonoBehaviour that writes MaterialPropertyBlock values per renderer/material slot; keep source material assets unchanged and fade perspective correction from the character ground/root so contact does not drift.
- For LiveToon face-space Directional Light limiting, prefer soft/sticky yaw shaping over hard nearest-step quantization so face shadows linger at flattering angles and ease through transition angles without popping.
- Keep LiveToon perspective correction applied consistently across forward, outline, depth-only, transparent depth, and shadow caster vertex paths; correcting only the color pass desynchronizes HDRP depth/transparent composition and can make body parts appear cut out.
- Keep LiveToon face renderer/material roles manual by default; hair role name auto-detection is useful, but face role mistakes affect spherical normals and face-space light limiting too strongly.
- For normal LiveToon character setup, expose the root entry point as `LiveToonSetup`; it should detect a Humanoid Animator under the target root and wire the character look controller plus front-hair custom shadow light automatically.
- LiveToonSetup character-wide look controls such as shadow-boundary saturation should use per-renderer/material-slot MaterialPropertyBlock values so source material assets remain unchanged, and should restore material-local defaults when the override is disabled.
- For LiveToon front-hair custom shadows, keep the depth producer and shader sampler on the same explicit light-space VP matrix, sync that virtual light from the scene Directional Light, and prefer crisp point-filtered custom depth with small bias/strong strength over broad HDRP shadow-map reuse.
- Keep LiveToon hair specular gated by the `_isHair` material role, surface facing, and Directional Light shadow attenuation; expose the tuning properties in `LiveToonInspector_MToonBase` so hair highlights are not hidden behind the base MToon inspector.
- Keep LiveToon hair jitter texture wired by default: converted LiveToon materials should receive the package `Shader/jitter.png` on `_JitterTex` when empty, and the shader fallback should be neutral gray rather than white to avoid a constant specular offset.
- Keep LiveToon front-hair shadow caster lists limited to front-hair/hair renderers; face/body renderers can overwrite the custom depth map and hide bang shadows. When bounds are unspecified, fit from the caster set instead of the whole character.
- For LiveToon shadow-boundary saturation boosts, avoid applying the boost to very dark or transparent-edge pixels, and keep boosted colors from becoming lower-luminance than the original shade; otherwise outlines and hair shadow details can turn into black artifacts.
- When exposing LiveToon shadow-boundary saturation through setup components, wire the control into both the shader's overlay saturation and boundary blend weight; writing only the `_Sat` MaterialPropertyBlock is not enough if the shader does not consume it.
- For isolated LiveToon front-hair shadow debugging, prefer `LiveToonBoxShadowLight`: an explicit box projection with manually assigned caster and receiver renderers, applying receiver-only MaterialPropertyBlock values before returning to the automatic front-hair shadow setup.
- For broad LiveToon custom-shadow debugging, `LiveToonBoxShadowLight` can collect caster and receiver renderers separately from a target root; avoid enabling both with silhouette-only projection unless intentionally stress-testing, because the whole character silhouette will darken every receiver.
- For full-body LiveToon self shadow, use `LiveToonBoxShadowLight` full-body self shadow mode: collect the target root as both caster and receiver, force depth comparison, auto-fit the projection box to renderer bounds, and use a 4096 point-filtered depth texture from one custom light direction.
- LiveToon box/custom shadow projections may need texture-coordinate correction on D3D render targets; expose Flip U/Flip V and silhouette inversion on debugging lights instead of baking a single projection orientation assumption.
- `LiveToonSetup` should create or reuse a head-child `VLiveBoxLight` with `LiveToonBoxShadowLight` front-hair-to-face defaults applied once, leave caster/receiver renderer lists manual, assign the setup `Source Directional Light`, and sync only that light's rotation so the box position follows the head while the projection direction follows the specified scene light.
- For `ExecuteAlways` LiveToon character helpers, keep Play Mode responsive with per-frame updates, but throttle Edit Mode work and render custom depth maps only after relevant settings, transform, or light-direction changes.
- For LiveLensFilters on Unity 6000.3/HDRP 17, do not keep Unity Visual Compositor 0.30.7-preview as a default dependency; it targets an older HDRP/Core stack, can compile `RenderShadowOneLightHDRP` against removed LightLoop fields such as `splineVisibility`, and also pulls legacy `streaming-image-sequence` import noise. Keep legacy Visual Compositor nodes optional behind an explicit define/manual dependency.
- For custom Kino-era LiveLensFilters effects such as `GenshinBloom`, `GenshinColorGrading`, and `diffusion`, restore only the VLiveKit-owned runtime/shader files under CreativeFx with `toshi/LensFilters` shader/menu paths and preserve their old MonoScript/shader GUIDs for scene/profile compatibility; do not re-add the full vendored Kino package.
- Do not copy Heartfelt-derived raindrop shader source into public VLiveKit releases by default. `yumayanagisawa/Unity-Raindrops` has an MIT repository license file, but its README/shader header still identify the Heartfelt source as `CC BY-NC-SA 3.0`; if an explicit temporary reference implementation is requested, keep attribution in shader comments and `THIRD_PARTY_NOTICES`, and review/remake it before npm/UPM publication.
- For LiveLensFilters `RainOnLens`, avoid inherited Heartfelt-style full-frame lightning or brightness fades; keep highlight changes local to droplets so downstream bloom/exposure does not pulse in static scenes.
- For LiveLensFilters `LayerBloom`, keep the layer-source color buffer depth-compatible when binding camera depth, but keep the final composite buffer at the current camera viewport size; fullscreen blit passes should pass normalized triangle UVs plus RTHandle scale/bias explicitly rather than sampling from `CustomPassCommon`'s position-only `Varyings`.
- For LiveLensFilters `LayerBloom`, clamp both the re-rendered layer source brightness and final bloom contribution; HDRP/LiveToon highlights and exposure globals can make a static character produce changing raw HDR values, which otherwise turns layer bloom into runaway white clipping.
- For LiveLensFilters `LayerBloom`, keep source brightness normalization available and enabled for character bloom; threshold-based bloom can flicker when auto exposure or background brightness makes the re-rendered source cross the threshold frame to frame.
- For LiveLensFilters `LayerBloom` color behavior, keep source-color bloom, tint-only bloom, and source-color-tinted bloom as explicit modes so character bloom can preserve the target layer's rendered colors without losing the older tint-multiply behavior.
- For LiveLensFilters `LayerBloom`, keep final composite behavior selectable rather than hard-coded additive; additive preserves the old HDR glow, while screen/soft-add/lighten/overlay are useful when character bloom should stay gentle over bright backgrounds.
- For LiveLensFilters `LayerBloom` material targeting, keep the default layer renderer-list path intact and use manual submesh drawing only for `Material` / `LayerAndMaterial` target modes; support exact material matches plus optional same-shader/base-name matches for runtime material instances or clones from VRM/LiveToon conversion.
- For LiveLensFilters mask-based CustomPass effects such as offset rim lights, render the character mask with an override material that has a mesh vertex pass, then run a separate fullscreen composite pass; a fullscreen-triangle vertex shader cannot be shared with the mesh mask pass.
- For LiveLensFilters `MaskOffsetRimLight`, prefer an original-material mask source when character shaders apply vertex-space look corrections such as LiveToon perspective correction, and keep inside-rim placement available so the rim can stay clipped to the character silhouette instead of leaking outside it.
- For LiveLensFilters `MaskOffsetRimLight`, directional-light rim direction should project the selected Directional Light or `RenderSettings.sun` into screen space, with an invert toggle because Unity directional lights point along the light-ray direction while artists often want the visible rim on the light-source side.
- For LiveLensFilters screen-space wiggle effects, keep the camera-image UV transform post-process separate from the noise driver, and use a slight zoom above 1 when offsetting the image so clamped screen edges stay outside the visible frame.
- For Unity player-build compatibility, keep Editor folders isolated with Editor-only asmdefs and use runtime-safe base types such as `RuntimeAnimatorController` instead of `UnityEditor.Animations.AnimatorController` in runtime assemblies.
- For third-party utilities moved into a UPM package, arbitrary source folders such as `script/` may not compile even when sibling `Editor/` scripts do; add a scoped asmdef or move code under a proper package compile folder before relying on Editor references.
- For AvaPo! attachable helper MonoBehaviours such as `AvatarPoseEditorAgent` and `CaptureBackgroundFollower`, keep the owning asmdef runtime-compatible; guard editor-only methods with `#if UNITY_EDITOR`, because Unity rejects `AddComponent` for components compiled into an Editor-only assembly.
- For AvaPo! folder viewers, scan both `Assets` and the mounted UPM package path such as `Packages/com.toshi.vlivekit.thirdpartyutilities/AvaPo!`; default bundled poses disappear from the UI if the viewer assumes an `Assets`-only install.
- For AvaPo! editor previews using `PreviewRenderUtility`, clean up shared previews and thumbnail preview utilities from `OnDisable` and `AssemblyReloadEvents.beforeAssemblyReload`; do preview cleanup in a `finally` path so domain reloads do not leak preview scenes.
- AvaPo! preview clones should instantiate the current scene avatar, not the corresponding prefab asset, so LiveToon/HDRP material overrides are preserved; hide preview clones recursively and disable non-render skeleton behaviours instead of destroying components, because RequireComponent dependencies can make removal fail.
- AvaPo! Pose Viewer thumbnails should not convert or replace preview clone materials now that preview-scene rendering handles HDRP/LiveToon; keep clone materials referencing the current scene material state to avoid accidental MToon/Unlit/pink preview regressions.
- For AvaPo! pose preview regressions, preserve the original-material/magenta missing-shader baseline before adding shader fallbacks; do not let blank gray captures overwrite thumbnail caches.
- For AvaPo! HDRP previews, set preview cameras to render all layers and keep HDRP `HDAdditionalCameraData.clearColorMode/backgroundColorHDR` in sync with the IMGUI background mode; otherwise Character-layer avatars can disappear and HDRP may show sky even when the UI says Dark.
- For AvaPo! Humanoid pose thumbnails, prefer directly applying `Animator` muscle curves from `AnimationUtility.GetCurveBindings` to `HumanPose.muscles`, plus `RootT`/`RootQ` to `bodyPosition`/`bodyRotation`, because preview-scene `AnimationMode.SampleAnimationClip` can leave clones in the current scene pose.
- For AvaPo! preview clones, resolve the Humanoid `Animator` from children before adding a new root Animator; container objects are common, and adding an empty root Animator prevents pose clips from reaching the real avatar rig.
- HhotateA AvatarPoseLibrary is MIT-approved for `VLiveKit_TestAssetsContainer`; keep its runtime `AvatarPoseLibrary` MonoBehaviour compile-safe without VRC SDK, and gate VRC/NDMF/Modular Avatar editor behavior through package-version defines so non-VRC sandboxes still compile without missing-script warnings.
- Keep the HhotateA AvatarPoseLibrary custom inspector available in non-VRC sandboxes; do not put the whole editor asmdef behind VRC/NDMF/Modular Avatar define constraints, only the VRChat build/export plugins.
- For PerformerAct `AutoEyeDirt` camera look, avoid per-eye direct `Quaternion.LookRotation` toward a close camera; use a shared gaze direction from the eye center and rotate from the captured default eye pose so near-camera convergence and model-specific eye-bone axes do not twist the eyes.
- For PerformerAct `AutoAnimationSetup`, include `BlendShapeFollower` in the automatic character setup so facial BlendShape jitter is wired with the same root workflow as blink, eye dart, breathing, lip sync, and controller facial controls.
- `AutoAnimationSetup` should expose a `Resetup Auto Animation` context menu that re-detects core character references and rebuilds generated breathing, motion-jitter, and facial-jitter defaults so older scene setups can be repaired in place.
- Keep PerformerAct `VRMWind` under the runtime assembly and wire it from `AutoAnimationSetup`; preserve WindForVRM attribution and Apache-2.0 notice under `ThirdPartyNotices` whenever redistributing or moving the runtime script.
- Keep `Assets/Settings/HDRPDefaultResources/DefaultSettingsVolumeProfile.asset` as a small project default profile; do not import sample, VisualCompositor, Kino, or LiveLensFilters test overrides into it, because missing VolumeComponent scripts can make empty scenes throw `SerializedObjectNotCreatableException` from HDRP Volume/Sky editors.
- If Unity 6000/HDRP floods `SerializedObjectNotCreatableException` from `VolumeComponentEditor` after domain reload or MCP setup, first clear selected/locked VolumeProfile inspectors and disable Unity Search `indexOnEditorStartup`; restored Assistant windows can also spam server refresh errors independently of MCP bridge health.
- For AI-assisted Unity/MCP work, keep the compact project map in `Assets/Project_Overview.md` and task-facing guidance in `Assets/Docs/VLiveKitAIGuidance.md`; update them when package roles, scene entry points, or AI-safe scene-edit workflows change.
