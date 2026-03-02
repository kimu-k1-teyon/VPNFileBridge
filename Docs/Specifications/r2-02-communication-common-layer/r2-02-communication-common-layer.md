# R2-02 通信共通レイヤ仕様

## 目的
- roadmap `R2-02 通信共通レイヤ実装` を実装可能な粒度まで具体化する。
- Unity クライアントの全 API 呼び出しを共通レイヤに集約し、認証ヘッダ付与、タイムアウト、リトライ挙動を統一する。
- 後続タスク (`R2-03` 以降) が直接 `UnityWebRequest` を扱わずに機能実装できる状態を作る。

## 対象
- 対象タスク: `R2-02`
- 対象成果物:
  - 通信共通レイヤの設計仕様
  - 実装時に必要な I/F、エラーコード、再試行条件
  - DoD 判定に必要な検証観点
- 対象外:
  - 画面 UI 実装 (`R2-03` 以降)
  - VPN 接続基盤の実装変更 (`R1-02`)
  - サーバ API 仕様変更 (`R1-*`)

## 前提
- `overview.md` の制約を適用する:
  - Unity `6000.3.5f2`
  - MVP + State 駆動
  - VContainer の `[Inject]` フィールド注入
- サーバ API は `R1-03` から `R1-06` で提供済み。
- 認可の最終判定はサーバ側で行う（クライアント側の判定は補助）。

## 手順
1. 通信処理の入口を 1 つに固定する (`IApiClient`)。
2. 認証ヘッダ、タイムアウト、リトライの責務を独立コンポーネントに分離する。
3. 全 Feature から共通 I/F を経由する利用ルールと禁止事項を定義する。
4. エラーコードと復帰可能/不可能条件を定義し、`R2-08` へ接続する。

## 設計方針
- 低レベル実装 (`UnityWebRequest`) は共通レイヤ内部に閉じ込める。
- Feature 層は「リクエスト DTO を渡して結果を受け取る」責務に限定する。
- 認証ヘッダの有無は API 単位で明示し、暗黙ルールを作らない。
- 再試行は「安全に再送できる条件」のみ自動化する。

## I/F 仕様
### 1) API 呼び出し入口
```csharp
public interface IApiClient
{
    Task<ApiResult<TResponse>> SendAsync<TResponse>(
        ApiRequest request,
        CancellationToken cancellationToken = default);
}
```

### 2) リクエスト/レスポンス契約
```csharp
public sealed class ApiRequest
{
    public string EndpointName { get; init; }          // 例: "auth/login"
    public HttpMethodKind Method { get; init; }        // GET/POST/PUT/DELETE
    public bool RequiresAuth { get; init; }            // Authorization 付与要否
    public string Path { get; init; }                  // 先頭 "/" 必須
    public IReadOnlyDictionary<string, string> Headers { get; init; }
    public string? JsonBody { get; init; }
    public int? TimeoutSecondsOverride { get; init; }  // null は既定値
    public RetryPolicyOverride? RetryOverride { get; init; }
}

public sealed class ApiResult<TResponse>
{
    public bool IsSuccess { get; init; }
    public int? HttpStatusCode { get; init; }
    public TResponse? Data { get; init; }
    public ApiError? Error { get; init; }
    public string TraceId { get; init; }               // 1 request 単位で採番
    public long ElapsedMilliseconds { get; init; }
}
```

### 3) 補助 I/F
```csharp
public interface IAuthTokenProvider
{
    bool TryGetAccessToken(out string token);
}

public interface IRetryPolicy
{
    bool ShouldRetry(RetryContext context);
    TimeSpan GetDelay(RetryContext context);
}
```

## 認証ヘッダ仕様
- `RequiresAuth = true` のとき、`Authorization: Bearer <token>` を付与する。
- トークン未取得時は送信せず `AUTH_TOKEN_MISSING` を返す。
- `RequiresAuth = false` のときは `Authorization` を付与しない。
- トークン文字列はログ出力禁止（マスク含む全文非表示）。

## タイムアウト仕様
- 既定タイムアウト: `30秒`
- `TimeoutSecondsOverride` 指定時はその値を優先（許容範囲 `5-300秒`）
- タイムアウト時は `NETWORK_TIMEOUT` を返し、再試行判定へ渡す。

## リトライ仕様
- 既定試行回数: `最大3回`（初回1回 + リトライ2回）
- 対象条件:
  - 通信断/名前解決失敗/TLS 失敗などのネットワーク例外
  - HTTP `408`, `429`, `500`, `502`, `503`, `504`
- 非対象条件:
  - HTTP `400`, `401`, `403`, `404`, `409`, `422`
  - ユーザーキャンセル
  - クライアントの入力不備（シリアライズ失敗など）
- 遅延戦略:
  - 指数バックオフ `500ms`, `1000ms`
  - `429/503` で `Retry-After` がある場合は `Retry-After` を優先（上限 `5秒`）
- 安全性ルール:
  - `POST` は既定で自動リトライしない
  - `RetryOverride` で明示許可された場合のみ `POST` リトライ可

## エラーコード仕様
| Code | 区分 | 意味 | 自動再試行 |
| --- | --- | --- | --- |
| `NETWORK_TIMEOUT` | Network | タイムアウト | Yes |
| `NETWORK_UNREACHABLE` | Network | 到達不能/名前解決失敗 | Yes |
| `TLS_HANDSHAKE_FAILED` | Network | TLS 失敗 | Yes |
| `AUTH_TOKEN_MISSING` | Auth | 認証必須 API で token 未取得 | No |
| `AUTH_UNAUTHORIZED` | Auth | HTTP 401 | No |
| `AUTH_FORBIDDEN` | Auth | HTTP 403 | No |
| `HTTP_CLIENT_ERROR` | HTTP | 4xx（上記以外） | No |
| `HTTP_SERVER_ERROR` | HTTP | 5xx | Yes |
| `RETRY_EXHAUSTED` | Retry | 再試行上限到達 | No |
| `REQUEST_CANCELED` | Client | ユーザー/画面遷移起因の中断 | No |
| `SERIALIZATION_FAILED` | Client | JSON 変換失敗 | No |
| `UNKNOWN_ERROR` | Client | 上記に分類不能 | No |

## 実装ルール
- 命名規則:
  - Interface: `I*`
  - 実装: `*Impl`
  - DTO: `*Request` / `*Response`
- 既存構造維持:
  - 共通通信層は `Assets/Scripts/Common/Core/` 配下に配置する
  - Feature 配下からは `IApiClient` のみ参照する
- 変更範囲:
  - `R2-02` では通信共通層の新規追加まで
  - 既存 Feature の置換は最小限のサンプル適用 + 後続タスクで全面移行
- 禁止事項:
  - Feature 層で `UnityWebRequest` を直接生成/送信しない
  - 認証トークンをログ・例外メッセージへ出力しない

## 受け入れ基準（R2-02 DoD 対応）
- 主要 API 呼び出しコードパスが `IApiClient.SendAsync` を経由している。
- 認証要否に応じた `Authorization` 付与が統一されている。
- タイムアウト・リトライの動作が仕様どおりに再現できる（正常系/異常系）。
- エラーコードが上表のいずれかに正規化され、呼び出し元で分岐可能。

## 検証観点
- 正常:
  - 認証不要 API の成功レスポンスを取得できる
  - 認証必須 API で token 付与して成功できる
- 異常:
  - token 未設定で `AUTH_TOKEN_MISSING`
  - 疑似タイムアウトで `NETWORK_TIMEOUT` と再試行発火
  - 5xx 応答で再試行後 `RETRY_EXHAUSTED` または成功
  - 4xx 応答で再試行しない

## 出力テンプレ
- 対象: `R2-02 通信共通レイヤ実装`
- 目的: 全 API 呼び出しの共通化（認証/タイムアウト/リトライ統一）
- 前提: `overview.md` の Unity 方針、`R1-*` API 提供済み
- 不明点（あれば）: サーバ API の正確なエンドポイント一覧と `Retry-After` 運用値
- 今回の出力: `R2-02` 実装開始に必要な通信共通レイヤ仕様（I/F・エラーコード・再試行条件を含む）
