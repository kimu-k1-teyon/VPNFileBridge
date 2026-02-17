/************************************************************************
 * All Right Reserved Copyright© 2025 SEIKO ELECTRIC CO.,LTD.
 * サンプルシステム：UI共通基底クラス
 * ======================================================================
 * Newly created by Taiga Shiraki on 2025/04/25.
 ************************************************************************/
using System;
using UnityEngine;

/// <summary>
/// UI要素共通基底クラス
/// </summary>
public class BaseUI<T> : BaseUIAbstract where T : Enum
{
    [SerializeField] private T key;

    /// <summary>
    /// UI要素キー列挙値を取得
    /// </summary>
    /// <returns>キー列挙値</returns>
    public T KeyEnum => key;

    /// <summary>
    /// UI要素キーを取得
    /// </summary>
    /// <returns>キー列挙値</returns>
    public override Enum Key => key;
}