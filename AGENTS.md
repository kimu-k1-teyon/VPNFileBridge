# Project AGENTS.md

## Mandatory Read Order（作業開始前に必ず読む）
1) ./overview.md  （無ければ ./Docs/overview.md を探す）
2) ./roadmap.md   （無ければ ./Docs/roadmap.md を探す）
3) ./.agents/skills/intent-capture/SKILL.md（※フル読みは不要。必要時にスキルを使う）

## Preflight requirement（必須）
- 返信の冒頭に必ず1行だけ出す：
  Preflight: overview=OK|NG, roadmap=OK|NG, intent=OK|NG
- overview または roadmap が NG の場合：
  - 推測で進めない
  - その旨を報告して停止し、ユーザーにファイルの場所/内容の提示を依頼する

## State declaration（必須）
- Preflight 行の次に、適用した state を必ず1行で明記する：
  State: Intent-Capture | Normal Answer
- state 判定ルール：
  - 新機能/設計相談/仕様整理/要件定義/タスク分解/アーキ判断 → Intent-Capture
  - それ以外（一般質問、事実確認、既存仕様の確認、翻訳、要約、軽微なQ&A）→ Normal Answer
- 迷う場合は Intent-Capture に寄せず、いったん Normal Answer を選び、必要なら確認質問を1つだけ行う

## How to work（読む/参照のルール）
- ユーザー依頼に回答する前に、必ず overview.md と roadmap.md を開いて要点を把握してから返す
- 仕様・前提・制約が overview/roadmap にある場合はそれを優先する（ユーザーの明示指示が最優先）

## Routing（スキル使用の起動条件）
- 新機能/設計相談/仕様整理/タスク分解/アーキ判断が含まれる依頼は、まず intent-capture を使う（実装やコード提示に入らない）
- 一般質問や通常のQ&Aは intent-capture を使わず、Normal Answer で回答する

## Quality gate（任意だが推奨）
- 返信を出す直前に quality-gate を適用し、漏れ（A/B・おすすめ・Next action 等）を検査してから返す

## Skills
- usage-create: 機能を問わず、Docs/Guides に再利用可能な Usage ドキュメントを作成・更新する。 (file: /Users/masaokitarou/work/TestServer/.agents/skills/usage-create/SKILL.md)
