---
name: quality-gate
description: 返信の直前に自己検収する。Intent-Capture依頼なら A/B + おすすめ+理由 + 次アクション が揃っているかをチェックし、欠けていれば修正してから返す。
---

## Checklist
- Intent-Capture 扱いの依頼か？（新機能/設計/仕様/要件/タスク分解）
  - Yes → A/B + Recommendation + Next action があるか
  - Yes → Questions がある場合、各質問に（おすすめ: ...）が付いているか
  - Yes → コード/実装に踏み込んでいないか
- Preflight 行があるか
- State 行があるか（`State: Intent-Capture` または `State: Normal Answer`）
- state 判定が依頼内容と一致しているか
  - 一般質問/通常Q&Aなら `State: Normal Answer`
  - 新機能/設計/仕様/要件/タスク分解なら `State: Intent-Capture`
- ユーザーが次にやることが1つに絞れているか
