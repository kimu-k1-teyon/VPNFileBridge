/************************************************************************
 * All Right Reserved Copyright© 2025 SEIKO ELECTRIC CO.,LTD.
 * サンプルシステム：入力フィールド基底クラス
 * ======================================================================
 * Newly created by Taiga Shiraki on 2025/04/25.
 ************************************************************************/
using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 汎用入力フィールド制御クラス
/// </summary>
public class BaseInputField<T> : BaseUI<T> where T : Enum
{
    protected TMP_InputField _inputField;

    /// <summary>
    /// 入力フィールドを初期化
    /// </summary>
    void Awake()
    {
        _inputField = GetComponents<TMP_InputField>()?[0];
    }

    /// <summary>
    /// 入力フィールドのテキストを設定
    /// </summary>
    /// <param name="text">設定するテキスト</param>
    public void SetInputText(string text)
    {
        if (_inputField == null)
        {
            _inputField = GetComponents<TMP_InputField>()?[0];
        }
        _inputField.text = text;
    }

    /// <summary>
    /// 入力フィールドの値を取得
    /// </summary>
    /// <returns>入力フィールドの現在値</returns>
    public string GetInputValue()
    {
        return _inputField.text;
    }

    /// <summary>
    /// 入力フィールドの操作可能状態を設定
    /// </summary>
    /// <param name="isTnteractable">操作可能にするかどうか</param>
    public void SetInteractable(bool isTnteractable)
    {
        _inputField.interactable = isTnteractable;
    }

}
