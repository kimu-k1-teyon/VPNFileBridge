/************************************************************************
 * All Right Reserved Copyright© 2025 SEIKO ELECTRIC CO.,LTD.
 * サンプルシステム：UI共通抽象クラス
 * ======================================================================
 * Newly created by Taiga Shiraki on 2025/04/25.
 ************************************************************************/
using System;
using UnityEngine;

/// <summary>
/// UI要素共通抽象基底クラス
/// </summary>
public abstract class BaseUIAbstract : MonoBehaviour
{
    /// <summary>
    /// UI要素キーを取得
    /// </summary>
    /// <returns>キー列挙値</returns>
    public abstract Enum Key { get; }
}