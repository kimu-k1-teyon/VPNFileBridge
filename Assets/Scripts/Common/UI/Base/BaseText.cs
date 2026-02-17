/************************************************************************
 * All Right Reserved Copyright© 2025 SEIKO ELECTRIC CO.,LTD.
 * サンプルシステム：テキスト基底クラス
 * ======================================================================
 * Newly created by Taiga Shiraki on 2025/04/25.
 ************************************************************************/
using System;
using TMPro;

/// <summary>
/// 汎用テキスト制御クラス
/// </summary>
public class BaseText<T> : BaseUI<T> where T : Enum
{
    TextMeshProUGUI textMesh;

    /// <summary>
    /// テキスト表示を初期化
    /// </summary>
    protected void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// 表示中のテキストを取得
    /// </summary>
    /// <returns>現在のテキスト</returns>
    public string GetText()
    {
        return textMesh.text;
    }

    /// <summary>
    /// テキストを設定
    /// </summary>
    /// <param name="text">設定テキスト</param>
    public void SetText(string text)
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }

        textMesh.text = text;
    }

}
