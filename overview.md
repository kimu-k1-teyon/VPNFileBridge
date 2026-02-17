# Overview

## 1. この文書の対象
- この `overview.md` は Unity Part（クライアント実装）の方針を定義する。
- サーバ実装と VPN 基盤は外部依存として扱い、本書では前提条件のみ記載する。

## 2. Unity Part の目的
- Unity の単一コードベースで Android / iOS / macOS / Windows 向けクライアントを提供する。
- VPN 経由でサーバへ接続し、指定パスへのアップロード / ダウンロードを安全に実行する。
- 既存運用中の VPN 仕様（L2TP over IPSec の ID / パスワード / 共有キー / 使用期限）へ移行可能な UI / 導線を整える。

## 3. スコープ
### 対象に含む
- Unity クライアント実装（接続、ログイン、パス指定、アップロード、ダウンロード）。
- 通信共通レイヤ実装（認証ヘッダ、タイムアウト、再試行、エラーハンドリング）。
- クライアント操作ログと障害時の復旧導線（再ログイン、再試行、ユーザー通知）。
- Android / iOS / macOS / Windows 向けビルド設定と再現可能なビルド手順。

### 対象外（現時点）
- サーバ API の実装・運用（起動、監視、バックアップ、鍵管理を含む）。
- VPN サーバの構築・アカウント発行・証明書配布。
- VPN 非経由アクセス、リアルタイム同期、共同編集、版管理。

## 4. 外部依存と前提条件
- VPN 接続先、認証情報、利用期限は外部システムで払い出される。
- ファイル転送 API（認証、許可パス制御、アップロード、ダウンロード）は提供済みを前提とする。
- 許可パスの最終判定はサーバ側のルールに従う。

## 5. 開発基準
- Unity バージョンは `6000.3.5f2` に固定する。
- 依存注入は VContainer の `[Inject]` フィールド注入を使用する（コンストラクタ注入は使わない）。
- アーキテクチャは MVP + State 駆動を採用する。

## 6. Architecture（Unity Part 方針）
### 6.1 MVP
- `<baseclass名>Installer` : `LifetimeScope`
- `<baseclass名>Model` : POCO
- `<baseclass名>View` : `MonoBehaviour`（必要 Component を Attach）
- `<baseclass名>Presenter`
- `<baseclass名>ServiceImpl` / `I<baseclass名>Service`
- View は参照保持と最小限の通知のみを担当する。
- 初期化は Presenter で実施し、アプリ起動時に一度だけ行う。
- Service の肥大化が見えた時点で責務単位に分割する。
- 実際の機能実行（開始 / 停止）は State で制御する。

### 6.2 Component（必要に応じて）
- `I<baseclass名><Component名>`
- `<baseclass名><Component名>`
- Component は View に Attach する `MonoBehaviour` とする。
- 利用側は Interface 経由で参照し、実体を差し替え可能にする。

### 6.3 推奨フォルダ（最低限）
- `Agent/`
- `Agent/Schedule/`
- `Agent/Skills/`
- `Agent/Tasks/`
- `Assets/Scripts/Common/Core/Database/`
- `Assets/Scripts/Common/Core/Error/`
- `Assets/Scripts/Common/Core/Log/`
- `Assets/Scripts/Common/CoreUtil/`
- `Assets/Scripts/Common/UI/`
- `Assets/Scripts/Common/UI/Base/`
- `Assets/Scripts/Constants/`
- `Assets/Scripts/Features/`

## 7. 成果物（Unity Part）
### 必須成果物
- Unity クライアント実装一式（接続、ログイン、アップロード、ダウンロード）。
- 4 プラットフォーム向けビルド設定と再現手順。
- 主要フローのテスト結果と動作確認手順。
- 既知制約・運用注意点をまとめたリリース向けドキュメント。

### 完了条件
- Unity `6000.3.5f2` で 4 プラットフォーム向けビルドが再現可能である。
- 4 プラットフォームで VPN 経由の接続、ログイン、転送が完了する。
- 許可外パス操作はサーバで拒否され、クライアント側でも適切に通知される。
- 通信断・トークン期限切れなど主要障害でクラッシュせず復旧導線を提供できる。
- 主要エラー時に原因追跡可能なクライアントログが残る。

## 8. Roadmap との対応
- 本 `overview.md` は Unity Part のため、主対象タスクは `roadmap.md` の `R2` 以降とする。
- `R1` 以前（サーバ / VPN 基盤）は前提条件として扱う。

## 9. 更新方針
- 目的・スコープ・完了条件に変更が出た場合は、この `overview.md` を先に更新する。
- 実装タスクや進捗は `roadmap.md` に切り分けて管理する。
- 詳細仕様は `Docs/Specifications/` 配下へ追加し、`overview.md` には要約のみを残す。

## 10. 仕様書作成
- `.agents/skills/specification-create/SKILL.md` を参照する。
