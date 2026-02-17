/************************************************************************
 * All Right Reserved Copyright© 2025 SEIKO ELECTRIC CO.,LTD.
 * サンプルシステム：ボタン基底クラス
 * ======================================================================
 * Newly created by Taiga Shiraki on 2025/04/25.
 ************************************************************************/
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Common.UI.Base
{
    /// <summary>
    /// 汎用ボタン制御クラス
    /// </summary>
    public class BaseButton<T> : BaseUI<T>, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler where T : Enum
    {
        public bool EnableHighlight = true;
        public virtual bool IsSetBtnKey => true;

        private Action onClick;
        private TextMeshProUGUI textMesh;
        private Image buttonImage;
        private Color originalColor;

        private bool isPressed;


        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Awake()
        {
            IsEnabled = true;

            if (IsSetBtnKey)
            {
                gameObject.name = Key.ToString();
            }
            buttonImage = GetComponent<Image>();
            if (buttonImage != null)
            {
                originalColor = buttonImage.color;
            }
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        /// <summary>
        /// ボタンの有効状態を取得
        /// </summary>
        public bool IsEnabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                SetActiveView(enabled);
            }
        }

        /// <summary>
        /// ボタンのクリックリスナーを設定
        /// </summary>
        /// <param name="action">クリック処理</param>
        public void SetListener(Action action)
        {
            if (onClick != null)
            {
                RemoveAllListeners();
            }

            onClick = action;
        }

        /// <summary>
        /// すべてのリスナーを削除
        /// </summary>
        public void RemoveAllListeners()
        {
            onClick = null;
        }

        /// <summary>
        /// ボタンがクリックされたときの処理を実行
        /// </summary>
        public void OnClick()
        {
            onClick?.Invoke();
        }

        /// <summary>
        /// ポインタがボタン上で押されたときの処理
        /// </summary>
        /// <param name="eventData">ポインタイベントデータ</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            if (EnableHighlight && buttonImage != null)
            {
                buttonImage.color = originalColor * 0.9f;
            }
        }

        /// <summary>
        /// ポインタがボタン上で離されたときの処理
        /// </summary>
        /// <param name="eventData">ポインタイベントデータ</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (EnableHighlight && buttonImage != null)
            {
                buttonImage.color = originalColor;
            }

            if (!isPressed)
            {
                return;
            }

            if (onClick == null)
            {
                Debug.Log("noEvent");
            }

            onClick?.Invoke();
            isPressed = false;
        }

        /// <summary>
        /// ポインタがボタン外に出たときの処理
        /// </summary>
        /// <param name="eventData">ポインタイベントデータ</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isPressed) return;
            isPressed = false;
            if (EnableHighlight && buttonImage != null)
            {
                buttonImage.color = originalColor;
            }
        }

        /// <summary>
        /// ボタンテキストを設定
        /// </summary>
        /// <param name="text">表示するテキスト</param>
        public void SetText(string text)
        {
            if (textMesh == null && transform.childCount > 0 &&
                transform.GetChild(0).TryGetComponent<TextMeshProUGUI>(out var found))
            {
                textMesh = found;
            }

            if (textMesh != null)
            {
                textMesh.text = text;
            }
        }

        /// <summary>
        /// ボタンテキストを取得
        /// </summary>
        /// <returns>現在のテキスト</returns>
        public string GetText()
        {
            if (textMesh == null && transform.childCount > 0 &&
                transform.GetChild(0).TryGetComponent<TextMeshProUGUI>(out var found))
            {
                textMesh = found;
            }

            string result = null;
            if (textMesh != null)
            {
                result = textMesh.text;
            }

            return result;
        }

        /// <summary>
        /// ボタンの表示状態を更新
        /// </summary>
        /// <param name="isActive">表示有無のフラグ</param>
        private void SetActiveView(bool isActive)
        {
            foreach (Transform childTransform in GetComponentsInChildren<Transform>())
            {

                if (childTransform.GetComponent<TMP_Text>() == null)
                {
                    continue;
                }

                if (!childTransform.TryGetComponent<CanvasGroup>(out var canvasGroup))
                {
                    canvasGroup = childTransform.gameObject.AddComponent<CanvasGroup>();
                }

                canvasGroup.alpha = isActive ? 1.0f : 0.2f;
                canvasGroup.interactable = isActive;
                canvasGroup.blocksRaycasts = isActive;
            }

        }

        public void SetColor(Color color)
        {
            color.a = originalColor.a;
            buttonImage.color = color;
        }
    }
}