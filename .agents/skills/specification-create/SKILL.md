---
name: specification-create
description: roadmap.md の機能項目を起点に、必要な Specification ドキュメントを最短で作成する。
---

# skills/specification-create.md

## 目的
roadmap.md の機能項目を起点に、必要な Specification ドキュメントを最短で作成する。

## 対象
- roadmap.md（必須の起点）
- Docs/Specifications/**

## 手順
1) roadmap.md で対象機能の「番号・名称・Description」を抽出
2) 既存 specification を確認し、重複/近似があれば流用方針を決める
3) Specification の設計方針を確定する（スコープ/入出力/前提/除外）
4) 出力先パスを決める  
   - Docs/Specifications/<機能名>/<機能名詳細>.md  
   - <機能名> と <機能名詳細> は kebab-case を推奨
5) 本文を作成（テンプレを使用）
6) 長文化しそうなら分割設計を行う  
   - Docs/Specifications/<機能名>/index.md（ナビゲーション）  
   - Docs/Specifications/<機能名>/<機能名詳細>.md（詳細）  
   - まずは 1 ファイルで開始し、必要時に分割する

## 作成前の確認（最小）
- roadmap.md の該当行はどれか（番号で指定）
- 既存 Specification との重複がないか
- 具体的に必要な成果物（例：実装/設計/ドキュメント/検証）

## 本文テンプレ（新規作成用）
```
# Docs/Specifications/<機能名詳細>.md

## 目的
（この Specification が達成すること）

## 対象
（扱う範囲/ファイル/成果物）

## 手順
1)
2)
3)

## ルール
- 命名規則：
- 既存構造維持：
- 変更範囲：

## 出力テンプレ
- 対象：
- 目的：
- 前提：
- 不明点（あれば）：
- 今回の出力：
```

## 分割ルール（必要時のみ）
- 1ファイルが概ね 120 行を超えたら分割を検討する
- index.md に全体の目次/トリガー/読み分けの説明を書く
- 詳細は機能別ファイルに切り出す（例：io.md / workflow.md / rules.md）

## 出力テンプレ（この Specification の応答）
- 対象機能（roadmap番号）：
- 既存Specificationの有無：
- 新規作成パス：
- 分割要否：
- 今回の出力：
