using System;
using UnityEngine;

namespace Assets.Scripts.Common.UI.Base
{
    public class BasePanel<T> : BaseUI<T> where T : Enum
    {
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

        private void SetActiveView(bool enabled)
        {
            gameObject.SetActive(enabled);
        }
    }

}
