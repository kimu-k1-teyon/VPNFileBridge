// using Cysharp.Threading.Tasks;
// using seiko.framework.bases.config;
// using seiko.framework.bases.exception;
// using seiko.framework.bases.log;
// using System;
// using System.Collections.Generic;
// using System.Net;
// using System.Net.Http;
// using System.Text;
// using System.Threading.Tasks;
// using UnityEngine;

// namespace seiko.framework.bases.utils
// {
//     /// <summary>
//     /// 外部API連係クラス
//     /// </summary>
//     public static class ExcuteRestApiUtil
//     {
//         // HTTPクライアント
//         static HttpClient _client;

//         // 設定情報 
//         private static readonly ApiConfig ApiConfig = ConfigProvider.AppConfigs.ApiConfig;
//         // 試行回数
//         private static readonly int RetryTimes = ApiConfig.ReteryTimes;
//         // タイムアウト
//         private static readonly int Timeout = ApiConfig.TimeoutMS;
//         // ベースアドレス
//         private static readonly string BaseAddress = ApiConfig.BaseUrl;


//         private static void CloseCloent()
//         {
//             _client?.Dispose();
//             _client = null;
//         }


//         /// <summary>
//         /// サーバへのPOST通信（非同期）
//         /// </summary>
//         /// <param name="json">送信するJSON</param>
//         /// <param name="url">送信先URL</param>
//         /// <returns>通信結果</returns>
//         public static async UniTask<string> PostAsync(string json, string url)
//         {
//             _client = GetInstance();
//             await UniTask.SwitchToThreadPool();

//             try
//             {
//                 // カウント
//                 int count = 1;
//                 // 通信が失敗した場合、指定回数繰り返す
//                 while (count <= RetryTimes)
//                 {
//                     StringContent content = null;
//                     HttpResponseMessage httpResponse = null;

//                     try
//                     {
//                         // リクエストボディの設定
//                         content = new StringContent(json, Encoding.UTF8, "application/json");
//                         // 通信処理実行
//                         var postTask = _client.PostAsync(url, content);

//                         // ソフトタイムアウト
//                         var timeoutTask = Task.Delay(_client.Timeout);
//                         var completed = await Task.WhenAny(postTask, timeoutTask);

//                         if (completed != postTask)
//                         {
//                             _ = postTask.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
//                             CloseCloent();
//                             throw new GyomuException("POST通信タイムアウトエラー");
//                         }

//                         // 通信完了
//                         httpResponse = await postTask;

//                         // ステータスコードが200以外の場合、業務エラー
//                         if (httpResponse.StatusCode != HttpStatusCode.OK)
//                         {
//                             throw new GyomuException("通信エラー　コード：" + (int)httpResponse.StatusCode);
//                         }

//                         // 結果を返却
//                         return await httpResponse.Content.ReadAsStringAsync();
//                     }
//                     catch (TaskCanceledException)
//                     {
//                         count++;
//                     }
//                     catch (Exception e)
//                     {
//                         throw new GyomuException("POST通信エラー", e);
//                     }
//                     finally
//                     {
//                         httpResponse?.Dispose();
//                         content?.Dispose();

//                     }
//                 }

//                 // タイムアウトが発生したらクローズ
//                 CloseCloent();
//                 // 通信がうまくいかなかった場合は業務エラーを投げる
//                 throw new GyomuException("POST通信タイムアウトエラー");

//             }
//             finally
//             {
//                 // このメソッドが完了（return/throw）する直前に必ずメインへ戻す
//                 await UniTask.SwitchToMainThread();
//             }

//         }

//         /// <summary>
//         /// サーバへのGET通信（非同期）
//         /// </summary>
//         /// <param name="url">送信先URL</param>
//         /// <returns>通信結果</returns>
//         public static async UniTask<string> GetAsync(string url)
//         {
//             _client = GetInstance();

//             // カウント
//             int count = 1;
//             // 通信が失敗した場合、指定回数繰り返す
//             while (count <= RetryTimes)
//             {
//                 try
//                 {
//                     // 通信処理実行
//                     HttpResponseMessage httpResponse = await _client.GetAsync(url);

//                     // ステータスコードが200以外の場合、業務エラー
//                     if (httpResponse.StatusCode != HttpStatusCode.OK)
//                     {
//                         // Debug.Log("通信エラー　コード：" + (int)httpResponse.StatusCode);
//                         throw new GyomuException("通信エラー　コード：" + (int)httpResponse.StatusCode);
//                     }

//                     // 結果を返却
//                     return await httpResponse.Content.ReadAsStringAsync();
//                 }
//                 catch (TaskCanceledException)
//                 {
//                     // Debug.Log($"通信先: {url} 通信回数: {count} RetryTimes: {RetryTimes}");
//                     count++;
//                 }
//                 catch (Exception e)
//                 {
//                     throw new GyomuException("GET通信エラー", e);
//                 }
//             }

//             // タイムアウトが発生したらクローズ
//             CloseCloent();
//             // 通信がうまくいかなかった場合は業務エラーを投げる
//             throw new GyomuException("GET通信タイムアウトエラー");
//         }

//     }
// }
