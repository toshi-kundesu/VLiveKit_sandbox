# VLiveKit Sandbox

VLiveKit packages をまとめて開発・検証するための Unity sandbox project です。

各機能 package は `Packages/` 配下の git submodule として配置し、Unity Package Manager では `file:` 参照で読み込んでいます。個別 package の詳細は、それぞれの repository root と package root の `README.md` を確認してください。

## Environment

- Unity: `2022.3.9f1`
- Render pipeline: HDRP `14.0.8`
- Main branch: `main`

## Packages

| Package | Submodule | Purpose |
| --- | --- | --- |
| `com.toshi.vlivekit.artnetlink` | `Packages/VLiveKit_ArtNetLink` | Art-Net / DMX 受信と照明制御 |
| `com.toshi.vlivekit.cameraunit` | `Packages/VLiveKit_camera` | ライブ制作向けカメラ制御 |
| `com.toshi.vlivekit.ledvision` | `Packages/VLiveKit_LEDVision` | LED スクリーン表現と LTCGI 連携 |
| `com.toshi.vlivekit.lensfilters` | `Packages/VLiveKit_LiveLensFilters` | post-process / lens filter 表現 |
| `com.toshi.vlivekit.livetoon` | `Packages/VLiveKit_LiveToon` | toon shader / character look |
| `com.toshi.vlivekit.performeract` | `Packages/VLiveKit_PerformerAct` | performer / character control |
| `com.toshi.vlivekit.stagebuilder` | `Packages/VLiveKit_StageBuilder` | stage layout / builder assets |
| `com.toshi.vlivekit.stageeffect` | `Packages/VLiveKit_StageEffect` | stage effect package container |
| `com.toshi.vlivekit.testassetscontainer` | `Packages/VLiveKit_TestAssetsContainer` | test assets / debug utilities |
| `com.toshi.vlivekit.thirdpartyutilities` | `Packages/VLiveKit_ThirdPartyUtilities` | shared third-party utilities |
| `com.toshi.vlivekit.videorack` | `Packages/VLiveKit_VideoRack` | video preparation tools |
| `com.toshi.memo` | `Packages/Memo` | Zenn articles / production notes |

## Clone

Submodule を含めて clone します。

```powershell
git clone --recurse-submodules https://github.com/toshi-kundesu/VLiveKit_sandbox.git
```

既に clone 済みの場合は submodule を初期化・更新します。

```powershell
git submodule update --init --recursive
```

## Development

1. Unity Hub からこの repository root を開きます。
2. Unity version は `2022.3.9f1` を使います。
3. Package の追加・削除は `Packages/manifest.json` と `Packages/packages-lock.json` を確認します。
4. Submodule 側を更新したら、sandbox 側でも submodule pointer を commit します。

よく使う確認コマンド:

```powershell
git status --short --branch
git submodule status
git diff --submodule
```

## Notes

- `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `node_modules/` は commit しません。
- Unity が生成した `.meta` は、asset と対応しているものだけ commit します。
- `Packages/Memo` は記事管理用で、Unity runtime package ではありません。
- Third-party asset は、それぞれの license / README を確認して扱います。
