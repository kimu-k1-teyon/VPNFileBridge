/************************************************************************
 * All Right Reserved Copyright© 2025 SEIKO ELECTRIC CO.,LTD.
 * サンプルシステム：ドロップダウン基底クラス
 * ======================================================================
 * Newly created by Taiga Shiraki on 2025/04/25.
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.Events;
using static TMPro.TMP_Dropdown;

namespace Assets.Scripts.Common.UI.Base
{
    /// <summary>
    /// 汎用ドロップダウン制御クラス
    /// </summary>
    public class BaseDropdown<T> : BaseUI<T> where T : Enum
    {
        protected TMP_Dropdown dropdown;
        private Dictionary<string, string> dropdownList;


        /// <summary>
        /// 初期化処理
        /// </summary>
        protected void Awake()
        {
            dropdown = GetComponents<TMP_Dropdown>()?[0];
        }

        /// <summary>
        /// ドロップダウンにリストを設定して初期選択を設定
        /// </summary>
        /// <param name="list">設定する項目リスト</param>
        /// <param name="firstKey">初期選択するキー</param>
        public void UpdateDropdown(Dictionary<string, string> list, string firstKey)
        {
            dropdownList = list;
            dropdown.ClearOptions();
            CreateDropdown(firstKey);
        }

        /// <summary>
        /// ドロップダウン項目を内部的に生成
        /// </summary>
        /// <param name="firstKey">初期選択するキー</param>
        private void CreateDropdown(string firstKey)
        {
            if (dropdownList.Count <= 0)
            {
                return;
            }

            var optionDataList = new List<OptionData>();
            foreach (var item in dropdownList.Values)
            {
                OptionData newData = new()
                {
                    text = item
                };

                optionDataList.Add(newData);
            }

            dropdown.AddOptions(optionDataList);

            var keys = dropdownList.Keys.ToList();
            var index = keys.IndexOf(firstKey);

            dropdown.value = index;

        }

        /// <summary>
        /// 値変更時のリスナーを設定
        /// </summary>
        /// <param name="action">実行する変更処理</param>
        public void SetOnValueChangedListener(Action action)
        {
            dropdown.onValueChanged.AddListener(delegate { action(); });
        }

        public void SetOnValueChangedListener(UnityAction<int> call)
        {
            dropdown.onValueChanged.AddListener(call);
        }

        /// <summary>
        /// 選択されている値を取得
        /// </summary>
        /// <returns>選択された値の文字列</returns>
        public string GetSelectedValue()
        {
            return dropdown.options[dropdown.value].text;
        }

        /// <summary>
        /// 選択されているキーを取得
        /// </summary>
        /// <returns>選択されたキーの文字列</returns>
        public string GetSelectedKey()
        {
            var value = dropdown.options[dropdown.value].text;
            return dropdownList.FirstOrDefault(o => o.Value == value).Key.ToString();
        }

        /// <summary>
        /// 配列からドロップダウンに項目を追加
        /// </summary>
        /// <param name="names">追加する項目名の配列</param>
        public void AddOptions(string[] names)
        {
            var options = new List<OptionData>();

            for (int i = 0; i < names.Length; i++)
            {
                OptionData newData = new OptionData();
                newData.text = names[i];
                options.Add(newData);
            }
            dropdown.AddOptions(options);
        }

    }
}