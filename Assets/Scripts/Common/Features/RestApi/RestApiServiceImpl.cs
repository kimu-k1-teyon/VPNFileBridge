using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Scripts.Common.Features.Config;
using Scripts.Common.Log;
using seiko.framework.bases.exception;
using UnityEngine;
using VContainer;

namespace Scripts.Common.Features.RestApi
{
    public class RestApiServiceImpl : IRestApiService
    {
        [Inject] RestApiModel _model;
        [Inject] IConfigEnviourment _configEnviourment;
        [Inject] ILogService _log;

        [Serializable]
        public class UploadRequest
        {
            public string requestedPath;
            public string fileBase64;
        }

        public async Task<string> PostAsync2()
        {
            try
            {
                _log.Write("StartTask");
                var req = new UploadRequest
                {
                    requestedPath = "documents/from-unity.json",
                    fileBase64 = "eyJoZWxsbyI6InVuaXR5In0="
                };
                var json = JsonUtility.ToJson(req);
                var url = @"api/UploadFile";

                StringContent content = null;
                HttpResponseMessage httpResponse = null;

                content = new StringContent(json, Encoding.UTF8, "application/json");
                var postTask = _model.Client.PostAsync(url, content);
                var timeoutTask = Task.Delay(_model.Client.Timeout);

                _log.Write("_model.Client.Timeout: " + _model.Client.Timeout.ToString());
                _log.Write("StartPost");
                var completed = await Task.WhenAny(postTask, timeoutTask);

                _log.Write("FinishPost");

                if (completed != postTask)
                {
                    _ = postTask.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
                    _log.Write("POST通信タイムアウトエラー");
                    return null;
                }

                httpResponse = await postTask;
                _log.Write("httpResponse.StatusCode: " + httpResponse.StatusCode);

                if (httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    _log.Write("通信エラー　コード：" + (int)httpResponse.StatusCode);
                    return null;
                }

                string response = await httpResponse.Content.ReadAsStringAsync();
                _log.Write("POST通信成功");
                return response;
            }
            catch (Exception e)
            {
                _log.Write("通信失敗: " + e.ToString());
                _log.Write("通信失敗: " + e.Message);
                return null;
            }
            finally
            {
                _log.Write("POST通信終了");
            }
        }

        public async Task<string> PostAsync()
        {
            try
            {
                _log.Write("StartTask");
                var req = new UploadRequest
                {
                    requestedPath = "documents/from-unity.json",
                    fileBase64 = "eyJoZWxsbyI6InVuaXR5In0="
                };

                var json = JsonUtility.ToJson(req);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var cts = new CancellationTokenSource(_model.Client.Timeout);

                _log.Write("_model.Client.Timeout: " + _model.Client.Timeout.ToString());
                _log.Write("StartPost");
                using var response = await _model.Client.PostAsync("/api/UploadFile", content, cts.Token);
                _log.Write("response.StatusCode: " + response.StatusCode.ToString());
                if (response.StatusCode != HttpStatusCode.OK) return null;

                return await response.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                _log.Write("通信失敗: " + e.Message);
                return null;
            }
            finally
            {
                // このメソッドが完了（return/throw）する直前に必ずメインへ戻す
                // await UniTask.SwitchToMainThread();
                Debug.Log("finally");
                _log.Write("POST通信終了");
            }
        }

        public async Task<string> PostAsync(string json, string url)
        {
            // await UniTask.SwitchToThreadPool();

            try
            {
                Debug.Log("通信先URL：" + url);
                await _log.WriteAsync("通信先URL：" + url);
                // カウント
                int count = 1;
                // 通信が失敗した場合、指定回数繰り返す

                StringContent content = null;
                HttpResponseMessage httpResponse = null;

                // リクエストボディの設定
                content = new StringContent(json, Encoding.UTF8, "application/json");
                // 通信処理実行
                var postTask = _model.Client.PostAsync(url, content);

                // ソフトタイムアウト
                var timeoutTask = Task.Delay(_model.Client.Timeout);
                var completed = await Task.WhenAny(postTask, timeoutTask);

                if (completed != postTask)
                {
                    _ = postTask.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
                    // CloseCloent();
                    Debug.Log("POST通信タイムアウトエラー");
                    await _log.WriteAsync("POST通信タイムアウトエラー");
                    return null;
                }

                // 通信完了
                httpResponse = await postTask;

                Debug.Log("httpResponse.StatusCode: " + httpResponse.StatusCode);

                // ステータスコードが200以外の場合、業務エラー
                if (httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    Debug.Log("通信エラー　コード：" + (int)httpResponse.StatusCode);
                    await _log.WriteAsync("通信エラー　コード：" + (int)httpResponse.StatusCode);
                    return null;
                }

                // 結果を返却
                string response = await httpResponse.Content.ReadAsStringAsync();
                await _log.WriteAsync("POST通信成功");
                return response;
                throw new GyomuException("POST通信タイムアウトエラー");

            }
            finally
            {
                // このメソッドが完了（return/throw）する直前に必ずメインへ戻す
                // await UniTask.SwitchToMainThread();
                Debug.Log("finally");
                await _log.WriteAsync("POST通信終了");
            }

        }
    }
}
