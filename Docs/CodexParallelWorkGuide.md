# Codex Parallel Work Guide

VLiveKit sandbox を複数の Codex スレッドで同時に参照・調査・修正するときの指示書です。

目的は、同じ workspace を見ている複数スレッドが互いの変更を壊さず、最後に coordinator thread が統合しやすい形で成果を受け取ることです。

## 基本方針

- 1スレッド = 1責務に限定する。
- 編集するスレッドは、原則として 1 package / 1 directory tree だけを所有する。
- 共有ファイルは coordinator thread だけが触る。
- 調査スレッドは読み取り専用にする。
- サブモジュール配下の変更は、そのサブモジュールの commit が必要になる前提で扱う。
- 他スレッドやユーザーの未コミット変更を revert / reset / checkout しない。

## 2台PCで長時間フル稼働させる方針

同一 share folder を2台のPCから同時に見る場合、もっとも危ないのは「複数スレッドが同じ git worktree に同時書き込みすること」です。

長時間稼働させたいときは、以下の比率にします。

- 調査専用 Worker: 多めに立てる。ファイル編集禁止。長時間回してよい。
- 実装 Worker: 少なめに立てる。1 package だけ編集可。
- Coordinator: 1つだけ。共有ファイル、manifest、ProjectSettings、submodule pointer を統合する。
- Unity Editor 起動: 原則1台だけ。もう1台は静的解析・ドキュメント・package metadata 調査に使う。

推奨:

- PC1: Coordinator + 実装 Worker 1つ + Unity確認役
- PC2: 調査 Worker 複数 + ドキュメント/Memo Worker

避ける:

- 2台で同時に Unity Editor を同じ project path に対して起動する。
- 2台で同時に `git add`, `git commit`, `git switch`, `git pull`, `npm pack`, Unity import を走らせる。
- 同じ submodule を2スレッドで編集する。

長時間 Worker は、短い修正で止まらないように「修正できるものは修正、触れないものは coordinator 向け実装指示に変換、最後に優先順位付き backlog を作る」と指示します。

## ご飯前に投げる推奨セット

2台PCでしばらく放置するなら、この構成が安全です。

### PC1

- Coordinator: 全体の作戦・統合待ち。
- Worker B: Camera Unit 実装。編集可は `Packages/VLiveKit_camera/Assets/toshi.VLiveKit/VLiveCameraUnit/**` のみ。
- Unity Verify Worker: 原則待機。Unityを開く必要が出た時だけ coordinator の指示で1回だけ起動。

### PC2

- Worker C: ArtNetLink 実装。編集可は `Packages/VLiveKit_ArtNetLink/Assets/toshi.VLiveKit/ArtNetLink/**` のみ。
- Worker E: LiveLensFilters 調査。編集禁止。
- Worker F: PerformerAct 調査。編集禁止。
- Worker H: Memo / Discord log 企画整理。編集可は `Packages/Memo/**` のみ、Unity側は触らない。

### さらに余裕がある場合

以下は調査専用で追加します。

- Package Metadata Auditor: 全 package の `package.json`, samples, dependencies, repositoryUrl, license を読む。編集禁止。
- Editor UI Auditor: `DisplayDialog`, `HelpBox`, `Debug.LogWarning`, menu path を横断検索する。編集禁止。
- Release Risk Auditor: npm pack に入りそうな third-party / generated / duplicated files を調査する。編集禁止。

## 長時間 Worker 共通プロンプト

長く走らせる Worker には、通常の共通指示に加えて以下を貼ってください。

```md
長時間作業モードで進めてください。

方針:
- すぐ終わる小修正だけで止まらず、担当範囲を深く読み続ける。
- 修正できるものは、指定された編集可範囲だけで安全に修正する。
- 編集禁止範囲や共有ファイルに関わるものは、実装せず coordinator 向け指示に変換する。
- 途中で1つの問題が片付いたら、同じ担当範囲内で次の問題を探す。
- ただし、同じファイルを他スレッドが触っていそうなら実装せず報告する。
- 大規模リファクタ、package version bump、manifest変更、ProjectSettings変更、publish/tag/push はしない。

優先順位:
1. コンパイルを壊す可能性が高いもの
2. Runtime / Editor 分離
3. package 公開時に混ざると危険なもの
4. README / package metadata の不整合
5. UI guideline とのズレ
6. 後で別Codexへ投げやすい実装指示化

最後に必ず:
- 変更したファイル一覧
- 読んだ主要ファイル一覧
- 修正済み項目
- 未修正だが重要な項目
- coordinator に渡す次の指示文
- 検証結果
をまとめてください。
```

## Read-only 深掘り Worker テンプレ

このテンプレは何本立てても比較的安全です。

```md
今回は read-only の長時間調査です。ファイル編集は禁止です。

対象:
- <directory or theme>

やること:
- 対象範囲をできるだけ深く読む。
- 問題候補を優先度順に整理する。
- 各問題について、対象ファイル、理由、修正方針、リスク、検証方法を書く。
- そのまま別Codexに貼れる実装指示を作る。
- 共有ファイル変更が必要な場合は、直接編集せず coordinator 向け提案にする。

時間の使い方:
- 最初の発見だけで終わらず、同じ範囲で関連ファイル・asmdef・package.json・README まで追う。
- 可能なら `rg` で類似問題を横断検索する。
- 最後は「今すぐ実装すべき」「後でよい」「保留」に分ける。
```

## 実装 Worker 長時間テンプレ

```md
このスレッドは実装 Worker です。指定範囲だけ編集できます。

編集可:
- <one package directory>

編集禁止:
- 共有ファイル
- 他 package
- ProjectSettings
- Packages/manifest.json
- Packages/packages-lock.json
- package-catalog.json

長時間作業:
- まず担当範囲を調査する。
- 競合しにくく、担当範囲内で閉じる修正から順に実装する。
- 1つ修正して終わらず、同じ担当範囲で関連する問題を続けて探す。
- 共有ファイルが必要な変更は「coordinator への提案」にする。
- 最後に、次の Worker がそのまま続きから作業できる backlog を作る。

完了条件:
- 自分の編集範囲外を変更していない。
- `git status --short --branch` で自分の変更を説明できる。
- 実行可能な範囲で検証済み。
- 未検証なら理由と推奨検証を書いている。
```

## 最初に貼る共通指示

各 Codex スレッドの最初に、以下を貼ってから個別タスクを渡してください。

```md
このスレッドは VLiveKit_sandbox を参照して作業します。

重要:
- 既存の未コミット変更を戻さない。
- 指定された作業範囲以外のファイルを編集しない。
- `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `dist/` は触らない。
- Unity 生成物や ProjectSettings を勝手に整形・更新しない。
- 共有ファイルは編集禁止。必要な変更は「提案」として報告する。
- サブモジュール配下を編集する場合、その package の中だけで完結させる。
- 作業前後に `git status --short --branch` を確認し、自分が触ったファイルを明記する。
- 競合しそうな変更を見つけたら、実装せず報告に切り替える。

あなたの担当範囲:
- 編集可: <ここに担当ディレクトリを書く>
- 編集禁止: それ以外すべて

成果物:
- 変更したファイル一覧
- 何を直したか
- 残した TODO
- 実行した検証
- coordinator が次に統合するときの注意
```

## Coordinator Thread の役割

Coordinator thread だけが、全体統合と共有ファイル変更を担当します。

Coordinator が触ってよい共有ファイル:

- `AGENTS.md`
- `Packages/manifest.json`
- `Packages/packages-lock.json`
- `ProjectSettings/**`
- `Docs/**`
- sandbox ルートの `README.md`
- `Packages/VLiveKit/package-catalog.json`

Coordinator の仕事:

- タスクを package 単位に分割する。
- 各スレッドの編集可ディレクトリを明示する。
- 各スレッドの報告を読み、重複・競合・順序依存を整理する。
- 最後に sandbox 側の submodule pointer や manifest を更新する。
- release / npm publish / tag / push は coordinator が明示的に実施する。

## Worker Thread の分割例

### Worker A: Installer

編集可:

- `Packages/VLiveKit/**`

編集禁止:

- `Packages/manifest.json`
- `Packages/packages-lock.json`
- 他 package

依頼例:

```md
Packages/VLiveKit だけを対象に、Package Manager UI と package-catalog 周辺を調査・修正してください。
ただし `package-catalog.json` を編集する必要がある場合は、実装せず coordinator 向け提案として報告してください。
```

### Worker B: Camera Unit

編集可:

- `Packages/VLiveKit_camera/Assets/toshi.VLiveKit/VLiveCameraUnit/**`
- `Packages/VLiveKit_camera/README.md`

依頼例:

```md
VLiveCameraUnit の Runtime に混ざっている UnityEditor 参照を調査し、Editor 専用処理を Editor フォルダへ移すか `#if UNITY_EDITOR` で安全に分離してください。
他 package と sandbox manifest は触らないでください。
```

### Worker C: ArtNetLink

編集可:

- `Packages/VLiveKit_ArtNetLink/Assets/toshi.VLiveKit/ArtNetLink/**`
- `Packages/VLiveKit_ArtNetLink/README.md`

依頼例:

```md
ArtNetLink の Runtime / Editor 分離を見直してください。
特に LightingCSVReader, ObjectPositionCSVExporter, ArtNetRecorder の UnityEditor 依存を確認し、Runtime assembly が Editor API を必要としない形にしてください。
```

### Worker D: VideoRack

編集可:

- `Packages/VLiveKit_VideoRack/Assets/toshi.VLiveKit/VideoRack/**`
- `Packages/VLiveKit_VideoRack/README.md`

依頼例:

```md
VideoRack の Editor windows と FFmpeg 同梱まわりを確認してください。
FFmpeg license / ThirdPartyNotices / package include 対象に不整合があれば、修正または coordinator 向け報告にまとめてください。
```

### Worker E: LiveLensFilters

編集可:

- `Packages/VLiveKit_LiveLensFilters/Assets/toshi.VLiveKit/LiveLensFilters/**`
- `Packages/VLiveKit_LiveLensFilters/README.md`

依頼例:

```md
LiveLensFilters の package.json / nested package / HDRP version / samples を確認してください。
package 公開時に不要な nested package.json や Samples の重複が入らないかを重点的に見てください。
```

### Worker F: PerformerAct

編集可:

- `Packages/VLiveKit_PerformerAct/Assets/toshi.VLiveKit/PerformerAct/**`
- `Packages/VLiveKit_PerformerAct/README.md`

依頼例:

```md
PerformerAct の Runtime にある UnityEditor 参照と third-party 内包状態を確認してください。
EVMC4U / UniVRM / uOSC / osc-jack の license と package 公開上のリスクは、勝手に削除せず coordinator 向けに整理してください。
```

### Worker G: LEDVision

編集可:

- `Packages/VLiveKit_LEDVision/Assets/toshi.VLiveKit/LEDVision/**`
- `Packages/VLiveKit_LEDVision/README.md`

依頼例:

```md
LEDVision の LTCGI 同梱部分、MenuItem path、HelpBox / DisplayDialog / warning tone を確認してください。
third-party 由来の大規模改変は避け、VLiveKit 側で包める改善と release risk を分けて報告してください。
```

### Worker H: Memo

編集可:

- `Packages/Memo/**`

編集禁止:

- Unity package / ProjectSettings / other packages

依頼例:

```md
Packages/Memo の Discord log と articles を読み、今後 Codex に依頼しやすい記事案・ツール案を整理してください。
Unity runtime package としての変更はしないでください。
```

## 共有ファイルを触りたくなった場合

Worker は共有ファイルを編集せず、以下の形式で報告してください。

```md
共有ファイル変更提案:
- 対象: Packages/manifest.json
- 理由: HDRP version が package.json と不一致
- 推奨変更: com.unity.render-pipelines.high-definition を 14.0.8 に揃える / または AGENTS.md を更新する
- 影響範囲: LiveLensFilters, StageEffect, sandbox ProjectSettings
- coordinator が確認すべきこと: Unity 2022.3.9f1 を維持するか、Unity 6 / HDRP 17 系へ上げるか
```

## 報告フォーマット

各 Worker の最終報告は、この形に揃えると統合しやすいです。

```md
担当:
- <package / directory>

変更したファイル:
- <path>
- <path>

変更内容:
- <短く具体的に>

検証:
- <実行したコマンド / Unity確認 / 未実行理由>

共有ファイル変更提案:
- <必要なければ「なし」>

残タスク:
- <なければ「なし」>

統合時の注意:
- <submodule commit が必要、package version bump が必要、manifest 更新が必要、など>
```

## 同時実行時の禁止事項

- 複数スレッドで同じ package を編集しない。
- Worker が `git reset`, `git checkout --`, `git clean`, 大量削除を実行しない。
- Worker が `git pull`, merge, rebase を勝手にしない。
- Worker が root manifest / ProjectSettings / package-catalog を直接編集しない。
- Worker が package version bump / npm publish / tag / push をしない。
- Worker が third-party asset を削除・移動・再配布前提で整理しない。
- Worker が Unity を複数同時起動しない。

## 推奨する進め方

1. Coordinator thread を1つ立てる。
2. Coordinator がこの文書を読んで、Worker ごとの担当範囲を決める。
3. Worker thread は read-only 調査から始める。
4. Coordinator が「この Worker は編集可」と判断したものだけ、package 単位で修正させる。
5. Worker は自分の担当 package 以外を触らず報告する。
6. Coordinator が submodule commit、sandbox pointer、manifest、catalog、AGENTS.md を最後に統合する。

## 調査だけさせるときのテンプレ

```md
今回は調査だけしてください。ファイル編集は禁止です。

対象:
- <directory>

見てほしいこと:
- <観点>

出力:
- 修正候補を優先度順に並べる
- それぞれに対象ファイル、理由、推奨修正、リスク、次に投げる実装指示を書く
```

## 実装させるときのテンプレ

```md
以下の範囲だけ編集して実装してください。

編集可:
- <directory>

編集禁止:
- 共有ファイル
- 他 package
- ProjectSettings

実装内容:
- <具体的な変更>

完了条件:
- Runtime assembly が Editor API を参照しない
- 既存 serialized field 名を変えない
- Unity .meta が必要な asset 移動なら一緒に更新する
- 変更ファイルと検証結果を報告する
```

## ご飯前ランチャー用プロンプト集

以下をそれぞれ別スレッドに投げると、長時間並列で動かしやすいです。

### 1. Coordinator

```md
Docs/CodexParallelWorkGuide.md を読んで、これから複数Codexスレッドで並列作業する前提の coordinator をしてください。

あなたは原則として実装せず、各Workerの成果を統合する役です。

やること:
- 現在の `git status --short --branch` を確認する。
- 既存未コミット変更をユーザー変更として保護する。
- 各Workerに渡す担当範囲を確定する。
- Worker報告を受け取ったら、競合しない統合順を作る。
- 共有ファイル変更が必要なものだけ、最後にまとめて提案する。

触ってよい:
- まだ触らない。まずは調査と統合計画のみ。

出力:
- Workerごとの担当範囲
- 同時に走らせてよいWorker
- 後回しにすべきWorker
- 統合時の注意
```

### 2. Camera Unit 実装 Worker

```md
Docs/CodexParallelWorkGuide.md を読んで、VLiveCameraUnit の実装Workerをしてください。

編集可:
- Packages/VLiveKit_camera/Assets/toshi.VLiveKit/VLiveCameraUnit/**
- Packages/VLiveKit_camera/README.md

編集禁止:
- それ以外すべて
- Packages/manifest.json
- Packages/packages-lock.json
- ProjectSettings/**

長時間作業:
- Runtime 配下の UnityEditor 参照を優先して調査・修正してください。
- Editor 専用処理は Editor フォルダへ移すか、型参照ごと `#if UNITY_EDITOR` に閉じてください。
- asmdef の Editor / Runtime 分離も確認してください。ただし共有ファイルは触らない。
- 1件直して終わらず、担当範囲内で関連問題を続けて探してください。

最後に:
- 変更ファイル
- 修正内容
- 検証
- 残タスク
- coordinator 向けの次指示
をまとめてください。
```

### 3. ArtNetLink 実装 Worker

```md
Docs/CodexParallelWorkGuide.md を読んで、ArtNetLink の実装Workerをしてください。

編集可:
- Packages/VLiveKit_ArtNetLink/Assets/toshi.VLiveKit/ArtNetLink/**
- Packages/VLiveKit_ArtNetLink/README.md

編集禁止:
- それ以外すべて
- Packages/manifest.json
- Packages/packages-lock.json
- ProjectSettings/**

長時間作業:
- Runtime / Editor 分離を重点的に見てください。
- LightingCSVReader, ObjectPositionCSVExporter, ArtNetRecorder の UnityEditor 依存を優先してください。
- Runtime assembly が Editor API を必要としない状態を目指してください。
- CSV import / Prefab配置 / AssetDatabase.Refresh など、Editor tool に分けるべきものは分離案または実装にしてください。

最後に:
- 変更ファイル
- 修正内容
- 検証
- 残タスク
- coordinator 向けの次指示
をまとめてください。
```

### 4. LiveLensFilters Read-only Auditor

```md
Docs/CodexParallelWorkGuide.md を読んで、LiveLensFilters の read-only 長時間調査をしてください。

編集禁止です。ファイルは一切変更しないでください。

対象:
- Packages/VLiveKit_LiveLensFilters/**

調査:
- package.json と nested package.json
- HDRP version の不整合
- samples / Samples~ / package内容
- asmdef
- README
- npm pack 時に混ざると危険なもの

出力:
- 優先度付きの修正候補
- 対象ファイル
- 理由
- 推奨修正
- そのまま別Codexに投げられる実装指示
```

### 5. PerformerAct Read-only Auditor

```md
Docs/CodexParallelWorkGuide.md を読んで、PerformerAct の read-only 長時間調査をしてください。

編集禁止です。ファイルは一切変更しないでください。

対象:
- Packages/VLiveKit_PerformerAct/**

調査:
- Runtime 内 UnityEditor 参照
- EVMC4U / UniVRM / uOSC / osc-jack の内包状態
- license / redistribution risk
- package.json / README / samples
- asmdef 分離

出力:
- 今すぐ直すべきもの
- 公開前に確認すべき license / third-party risk
- 別Codexへ投げる実装指示
- coordinator が判断すべきこと
```

### 6. Package Metadata Auditor

```md
Docs/CodexParallelWorkGuide.md を読んで、全VLiveKit package metadata の read-only 長時間調査をしてください。

編集禁止です。ファイルは一切変更しないでください。

対象:
- Packages/VLiveKit*/**/package.json
- Packages/VLiveKit/package-catalog.json
- 各 README.md

調査:
- package name / displayName / version
- dependencies の Unity/HDRP/Cinemachine version
- repositoryUrl / documentationUrl
- samples
- license
- nested package.json
- installer catalog と package 実体のズレ

出力:
- packageごとの不整合一覧
- 共有ファイル変更が必要な項目
- packageごとに分離して投げられる修正指示
```

### 7. Editor UI Auditor

```md
Docs/CodexParallelWorkGuide.md を読んで、VLiveKit editor UI の read-only 長時間調査をしてください。

編集禁止です。ファイルは一切変更しないでください。

対象:
- Packages/VLiveKit*/Assets/toshi.VLiveKit/**/*.cs
- Docs/VLiveKitDesignGuidelines.md

調査:
- DisplayDialog
- HelpBox Warning / Error
- Debug.LogWarning / Debug.LogError
- MenuItem が Tools/ に出ている箇所
- VLiveKitEditorUI を共有すべき箇所
- 通常状態なのに警告調になっている箇所

出力:
- package別の改善候補
- すぐ直してよい軽微なもの
- third-party由来なので触らない方がよいもの
- 別Codexへ投げる実装指示
```

### 8. Memo / Product Ideas Worker

```md
Docs/CodexParallelWorkGuide.md を読んで、Memo と Discord log から今後の実装・記事・ツール案を長時間整理してください。

編集可:
- Packages/Memo/**

編集禁止:
- Unity package
- ProjectSettings
- Packages/manifest.json
- 他 package

対象:
- Packages/Memo/scraps/vlivehouse-discord-log-2024-2026.md
- Packages/Memo/articles/**

やること:
- 既存記事と重複しない新規記事案を整理する。
- VLiveKit の package / tool として育てる案を整理する。
- 「今すぐCodexに投げる実装指示」形式に変換する。
- 可能なら Packages/Memo/private/ に企画メモとして追加する。

出力:
- 記事案
- ツール案
- package別の実装候補
- 優先順位
```

## 放置前チェック

ご飯前に投げる直前、Coordinator か人間がこれだけ確認します。

- どの Worker が編集可で、どの Worker が read-only か決めた。
- 同じ package を編集する Worker が2つ以上いない。
- Unity Editor を起動するPCは1台だけにした。
- 共有ファイルを触る Worker がいない。
- publish / tag / push / git reset / git clean を禁止した。
- 戻ってきたら、まず Coordinator に全Worker報告を貼る。
