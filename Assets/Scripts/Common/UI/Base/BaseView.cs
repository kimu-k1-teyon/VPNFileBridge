/************************************************************************
 * All Right Reserved Copyright© 2025 SEIKO ELECTRIC CO.,LTD.
 * サンプルシステム：画面表示制御基底クラス
 * ======================================================================
 * Newly created by Taiga Shiraki on 2025/04/25.
 ************************************************************************/
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common.UI.Base
{
    /// <summary>
    /// 汎用画面表示制御クラス
    /// </summary>
    public abstract class BaseView : MonoBehaviour
    {
        private List<BaseUIAbstract> _uiElements;

        /// <summary>
        /// 初期化処理
        /// </summary>
        protected virtual void Awake()
        {
            _uiElements = new List<BaseUIAbstract>(FindInActiveObjectsInScene<BaseUIAbstract>(true));
        }

        /// <summary>
        /// 指定されたキーに対応するUI要素を取得
        /// </summary>
        /// <typeparam name="TUI">UI要素型</typeparam>
        /// <typeparam name="TEnum">キー列挙型</typeparam>
        /// <param name="key">取得対象のキー</param>
        /// <returns>取得されたUI要素</returns>
        protected TUI GetUI<TUI, TEnum>(TEnum key)
            where TUI : BaseUI<TEnum>
            where TEnum : Enum
        {
            foreach (var ui in _uiElements)
            {
                if (ui is TUI typed && ui.Key.Equals(key))
                    return typed;
            }
            return null;
        }

        protected List<BaseUI<TEnum>> GetUIs<TUI, TEnum>() where TUI : BaseUI<TEnum> where TEnum : Enum
        {
            var results = new List<BaseUI<TEnum>>();
            foreach (var ui in _uiElements)
            {
                if (ui is TUI typed)
                {
                    results.Add(typed);
                }
            }
            return results;
        }

        /// <summary>
        /// DontDestroyOnLoadシーンを安全に取得するためのプローブ方式
        /// </summary>
        private static Scene GetDontDestroyOnLoadScene()
        {
            // まずは名前検索（存在すればそのまま使う）
            var ddolByName = SceneManager.GetSceneByName("DontDestroyOnLoad");
            if (ddolByName.IsValid()) return ddolByName;

            // 名前で取れない環境向けの確実な方法：一時GOをDDOLに移してそのsceneを得る
            var probe = new GameObject("_ddol_probe_");
            UnityEngine.Object.DontDestroyOnLoad(probe);
            var ddol = probe.scene;
            UnityEngine.Object.DestroyImmediate(probe); // 片付け
            return ddol;
        }

        /// <summary>
        /// シーン内要素リスト取得処理
        /// </summary>
        /// <typeparam name="T">検索対象の型</typeparam>
        /// <param name="isIncludeInactive">非アクティブオブジェクト追加判定</param>
        /// <returns>指定型要素リスト</returns>
        protected static List<T> FindInActiveObjectsInScene<T>(bool isIncludeInactive)
        {
            var results = new List<T>();
            var scene = SceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();

            foreach (var root in roots)
            {
                results.AddRange(root.GetComponentsInChildren<T>(isIncludeInactive));
            }


            // 2) DontDestroyOnLoad シーンを追加で走査
            var ddolScene = GetDontDestroyOnLoadScene();
            if (ddolScene.IsValid() && ddolScene.isLoaded)
            {
                foreach (var root in ddolScene.GetRootGameObjects())
                    results.AddRange(root.GetComponentsInChildren<T>(isIncludeInactive));
            }

            return results;
        }
    }
}