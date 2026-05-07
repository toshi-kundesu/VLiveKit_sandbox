# VLiveKit Design Guidelines

VLiveKit 全体で同じ手触りの Inspector / Editor UI を作るための、最小限のデザイン方針です。既存コンポーネントの挙動や serialized property の並びを壊さずに、パッケージ横断で「VLiveKit らしい見た目」と「迷わない操作導線」を足すことを目的にします。

## 目的

- パッケージごとに Inspector の見た目や情報設計がばらつくのを抑える。
- 既存の Inspector 挙動、Undo、Prefab override 表示、Multi Object Editing、SerializedProperty の編集体験を維持する。
- ユーザーが「このコンポーネントは何をするものか」「次に何をすればよいか」を Inspector 上で理解できるようにする。
- 将来的に共通 Editor ユーティリティへ切り出せるよう、まずは小さなルールから揃える。

## 基本方針

1. **既存 Inspector を主役にする**  
   デザインのためにフィールド構成を作り替えず、原則として `DrawDefaultInspector()` または既存の `SerializedProperty` 描画を残します。
2. **追加するのはヘッダーとフッターを中心にする**  
   既存挙動の前後に、概要・状態・リンク・補助アクションを足します。
3. **Editor 専用に閉じる**  
   共通 UI ヘルパーや CustomEditor は必ず `Editor` フォルダー、または Editor 専用 asmdef に置き、runtime assembly へ `UnityEditor` 参照を混ぜません。
4. **破壊的な自動変更を避ける**  
   Inspector を開いただけで scene / prefab / asset を dirty にしない設計にします。自動修復や生成はユーザー操作のボタンに限定します。
5. **パッケージの独立性を保つ**  
   個別 VLiveKit パッケージから installer パッケージ `com.toshi.vlivekit` へ依存させません。共通 UI が必要になった場合は、Editor 専用の軽量共通パッケージ化を検討します。

## 推奨 Inspector 構成

```text
[VLiveKit header]
  - パッケージ名 / 機能名
  - 1 行説明
  - 任意: 状態バッジ、警告、ドキュメントリンク

[既存 Inspector]
  - DrawDefaultInspector()
  - または既存 SerializedProperty ベースの UI
  - Undo / Prefab override / Multi Object Editing を維持

[VLiveKit footer]
  - Docs / Samples / Troubleshooting へのリンク
  - 任意: Validate / Setup / Reset など明示的な補助アクション
  - バージョン、依存関係、サポート情報
```

### ヘッダーに置くもの

- **コンポーネント名**: Unity の型名だけでは伝わりにくい場合、ユーザー向けの短い名称を表示します。
- **短い説明**: 1〜2 行で「何のためのコンポーネントか」を説明します。
- **状態表示**: 必須参照の不足、接続状態、生成済みデータの有無など、作業判断に必要な情報だけを表示します。
- **ドキュメント導線**: README、サンプル、トラブルシュートへのリンクを置きます。

### フッターに置くもの

- **補助アクション**: `Validate`, `Open Sample`, `Create Default Assets`, `Repair References` など、ユーザーが明示的に押すボタンにします。
- **安全な警告**: 実行時に問題になりそうな設定だけを短く表示します。自動修正は行いません。
- **関連リンク**: パッケージ README、リリースノート、サポート先などをまとめます。

## 実装パターン

既存の Inspector 挙動を残す場合は、CustomEditor で前後に共通 UI を挟む形を基本にします。

```csharp
using UnityEditor;

namespace Toshi.VLiveKit.Sample.Editor
{
    [CustomEditor(typeof(SampleComponent))]
    [CanEditMultipleObjects]
    public sealed class SampleComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            VLiveKitInspectorLayout.DrawHeader(
                title: "Sample Component",
                description: "VLiveKit sample component description.",
                docsUrl: "https://example.com/docs");

            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();

            VLiveKitInspectorLayout.DrawFooter(
                docsUrl: "https://example.com/docs",
                supportUrl: "https://example.com/support");
        }
    }
}
```

### 既存 CustomEditor がある場合

- 既存の `OnInspectorGUI()` の先頭にヘッダー、末尾にフッターを追加します。
- 既存の `SerializedProperty` 名や描画順は、移行理由がない限り変更しません。
- `EditorGUILayout.PropertyField` を使っている箇所では、Prefab override 表示が維持されるよう `serializedObject.Update()` / `ApplyModifiedProperties()` の流れを崩しません。
- `GUI.enabled` の変更、indent、label width、background color は必ず元に戻します。

## 見た目のトーン

- **シンプルで制作ツール寄り**: 派手な装飾よりも、ステージ制作・配信制作で迷わない情報設計を優先します。
- **色は意味がある時だけ使う**: 警告、成功、接続中、未設定などの状態表示に限定します。
- **Unity 標準 UI に馴染ませる**: IMGUI / UI Toolkit のどちらでも、Unity Editor のテーマに自然に見える余白・フォントサイズにします。
- **ブランド要素は控えめに**: ロゴや大きなバナーよりも、上部の小さな VLiveKit ラベルやパッケージ名で統一感を出します。

## 導入ステップ

1. まず 1 パッケージで、既存 Inspector を変えずにヘッダー / フッターだけ追加する。
2. 使い勝手を確認して、共通化したい表示要素を洗い出す。
3. `VLiveKitInspectorLayout` のような Editor 専用ヘルパーに切り出す。
4. 複数パッケージへ広げる前に、依存関係の置き場所を決める。
5. 各パッケージの README に Inspector のスクリーンショットと操作導線を追加する。

## 共通 UI ヘルパー化する時の注意

- installer パッケージ `com.toshi.vlivekit` へ個別パッケージを依存させない。
- runtime asmdef へ Editor UI ヘルパーを含めない。
- UI ヘルパーは IMGUI 版から始め、必要になった時だけ UI Toolkit 版を追加する。
- 各パッケージの npm 公開物に含める場合、Editor 専用コードと `.meta` を漏れなく含める。

## まず決めるとよいチェックリスト

- [ ] ヘッダーに出すパッケージ表示名のルール
- [ ] 1 行説明の文体
- [ ] Docs / Samples / Support リンクの置き場所
- [ ] 警告色・成功色・情報色の使い分け
- [ ] Validate / Setup など補助ボタンの命名
- [ ] 共通 Editor ユーティリティの配置先
