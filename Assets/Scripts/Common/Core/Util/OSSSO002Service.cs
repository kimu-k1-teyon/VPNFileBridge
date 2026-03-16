// using seiko.framework.bases.service;
// using System.Threading.Tasks;
// using UnityEngine;
// using System.IO;
// using System.IO.Compression;
// using seiko.framework.bases.utils;
// using System;
// using seiko.framework.gyomu.structs;
// using seiko.framework.bases.log;
// using Cysharp.Threading.Tasks;
// using seiko.framework.bases.config;
// using seiko.framework.bases.exception;
// using System.Net;
// using seiko.utilities;

// /// <summary>
// /// 操作対象設定のService
// /// </summary>
// public class OSSSO002Service : BaseService
// {
//     // 設定情報 
//     private static readonly ApiConfig ApiConfig = ConfigProvider.AppConfigs.ApiConfig;
//     // 試行回数
//     private static readonly int RetryTimes = ApiConfig.ReteryTimes;

//     public async UniTask<string> MakeFolder()
//     {
//         string folderPath = "";

//         try
//         {
//             folderPath = Path.Combine(Application.persistentDataPath, "file/SourceDirectory");
//             // 操作実績同期フラグをTrueにする
//             CommonStaticStruct.syncJissekiFlag = true;
//             // フォルダが無ければ作成
//             if (!Directory.Exists(folderPath))
//             {
//                 await Task.Run(() =>
//                 {
//                     Directory.CreateDirectory(folderPath);
//                 });

//                 LogUtil.OutputInfo("MSD::MakeFolder.folder->" + folderPath);
//             }
//         }
//         catch (Exception e)
//         {
//             LogUtil.OutputError("フォルダ作成に失敗：" + e.ToString());
//         }

//         return folderPath;
//     }

//     /// <summary>
//     /// zipファイル作成。.txtファイルのみ、この時点で圧縮可能
//     /// </summary>
//     /// <param name="paths">[0]:実データ保存場所、[1]:zipファイル名</param>
//     /// <returns></returns>
//     public async UniTask<string> MakeZip(string[] pathes)
//     {
//         string zipFolderPath = Path.Combine(Application.persistentDataPath, pathes[0].ToString());
//         LogUtil.OutputInfo("MSD::MakeZip.zipFolderPath->" + zipFolderPath);

//         try
//         {
//             await Task.Run(() =>
//             {
//                 ZipFile.CreateFromDirectory(pathes[1], zipFolderPath, System.IO.Compression.CompressionLevel.Optimal, true);
//             });
//             await Task.Run(() =>
//             {
//                 ZipFile.OpenRead(zipFolderPath).Dispose();
//             });
//         }
//         catch (System.Exception)
//         {

//             throw;
//         }

//         return zipFolderPath;
//     }

//     /// <summary>
//     /// フルパス作成用の文字列結合
//     /// </summary>
//     /// <param name="path">対象パス</param>
//     /// <returns></returns>
//     public string AddDirDelimiter(string path)
//     {
//         char separator = Path.DirectorySeparatorChar;
//         if (!path.EndsWith(separator.ToString()))
//         {
//             path += separator;
//         }
//         return path;
//     }

//     /// <summary>
//     /// Logのフォルダパスを取得
//     /// </summary>
//     /// <returns></returns>
//     public async UniTask<string[]> GetLogPath()
//     {
//         string filePath = await FilePathUtil.SetOutputFilePathAsync("", ConfigProvider.AppConfigs.LogConfig.FilePath);
//         LogUtil.OutputInfo("GetLogPath.filePath::" + filePath);
//         string[] files = Directory.GetFiles(@filePath, "*.txt", System.IO.SearchOption.AllDirectories);

//         return files;
//     }

//     /// <summary>
//     /// DBのフォルダパスを取得
//     /// </summary>
//     /// <returns>ファイルパス「file\DB\sqlite.db」</returns>
//     public async UniTask<string> GetDBPath()
//     {
//         string resPath = await FilePathUtil.SetOutputFilePathAsync("", ConfigProvider.AppConfigs.DbConfig.FilePath);

//         LogUtil.OutputInfo("MSD::GetDBPath.resPath->" + resPath);

//         return resPath;
//     }

//     /// <summary>
//     /// zipファイルのパス取得
//     /// </summary>
//     /// <returns>zipファイル名</returns>
//     public async UniTask<string> GetZipPath()
//     {
//         await Task.Run(() =>
//         {
//             // string zipName = DateTime.Now.ToString("yyyyMMddHHmmss_") + "MR.zip";
//             // string resPath = FilePathUtil.SetFilePath(zipName);
//             // LogUtil.OutputInfo("MSD::GetZipPath.resPath->" + resPath);

//             // return resPath;
//         });
//         return null;
//     }

//     /// <summary>
//     /// ファイルのコピー：テキスト
//     /// </summary>
//     /// <param name="path"></param>
//     /// <param name="pathes"></param>
//     public async UniTask CopyFilesText(string[] logpath, string[] pathes)
//     {
//         await Task.Run(() =>
//         {
//             foreach (string f in logpath)
//             {
//                 int len = f.Length;
//                 File.Copy(f, AddDirDelimiter(pathes[1]) + f.Substring(len - 15), true);
//             }
//         });
//     }

//     /// <summary>
//     /// ファイルのコピー：DB
//     /// </summary>
//     /// <param name="path"></param>
//     /// <param name="pathes"></param>
//     public async UniTask CopyFilesDB(string path, string[] pathes)
//     {
//         await Task.Run(() =>
//         {
//             string fromFile = Path.Combine(Application.persistentDataPath, path, "sqlite.db");
//             string toFile = Path.Combine(Application.persistentDataPath, pathes[1], "sqlite.db");
//             // string to = AddDirDelimiter(pathes[1]) + "sqlite.db";
//             LogUtil.OutputInfo("fromFile: " + fromFile);
//             LogUtil.OutputInfo("toFile: " + toFile);
//             LogUtil.OutputInfo("MSD::CopyFilesDB.toCopy->" + toFile);

//             File.Copy(fromFile, toFile, true);
//         });
//     }

//     /// <summary>
//     /// multipart/form-dataへのデータ送信
//     /// </summary>
//     /// <param name="fromDataRequest">リクエストパラメータ</param>
//     /// <returns></returns>
//     public async UniTask<FromDataStruct> SendRequests(FromDataRequest fromDataRequest)
//     {
//         string response = await ExcuteRestApiReport(fromDataRequest, @"ReportDefect.do");
//         LogUtil.OutputInfo("レスポンスデータ::" + response);

//         FromDataStruct fromDataStruct = new FromDataStruct();
//         fromDataStruct.resParameter = int.Parse(response);
//         if (fromDataStruct.resParameter == 0)
//         {
//             string msg = "レスポンスデータが０です";
//             throw new Exception(msg);
//         }
//         return fromDataStruct;
//     }

//     /// <summary>
//     /// 不要ファイルの削除
//     /// </summary>
//     /// <param name="dir"></param>
//     /// <param name="zip"></param>
//     /// <returns></returns>
//     public async UniTask Deletedirzip(string dir, string zip)
//     {
//         await Task.Run(() =>
//         {
//             try
//             {
//                 DirectoryInfo di = new DirectoryInfo(dir);
//                 FileInfo[] files = di.GetFiles();
//                 foreach (FileInfo file in files)
//                 {
//                     file.Delete();
//                 }
//                 LogUtil.OutputInfo("MSD::Deletedirzip->ディレクトリ内のファイル削除に成功しました");
//             }
//             catch (Exception e)
//             {
//                 LogUtil.OutputInfo("ディレクトリ内のファイル削除に失敗/またはすでに削除済みです::" + e);
//             }

//             try
//             {
//                 File.Delete(zip);
//                 LogUtil.OutputInfo("MSD::Deletedirzip->zipファイルの削除に成功しました");
//             }
//             catch (Exception e)
//             {
//                 LogUtil.OutputInfo("zipファイル削除に失敗/またはすでに削除済みです::" + e);
//             }
//         });

//     }


//     /// <summary>
//     /// 調査データの送信
//     /// </summary>
//     /// <param name="fromDataRequest"></param>
//     /// <param name="path"></param>
//     /// <returns>レスポンス結果</returns>
//     public static async UniTask<string> ExcuteRestApiReport(FromDataRequest fromDataRequest, string path)
//     {
//         MemoryStream fs = null;
//         System.IO.Stream reqStream = null;
//         System.Net.HttpWebRequest req = null;
//         byte[] startData = null;
//         byte[] endData = null;
//         System.IO.StreamReader sr = null;
//         System.Text.Encoding enc = null;

//         try
//         {
//             //送信先のURL
//             string url = Path.Combine(ApiConfig.BaseUrl, path);
//             Debug.Log("url: " + url);
//             //文字コード
//             enc = System.Text.Encoding.GetEncoding("utf-8");
//             //区切り文字列
//             string boundary = "OSSBoundary";

//             //WebRequestの作成
//             req = (System.Net.HttpWebRequest)
//                 System.Net.WebRequest.Create(url);
//             if (ApiConfig.proxyAddress != null && ApiConfig.proxyAddress != "")
//             {
//                 req.Proxy = new WebProxy(ApiConfig.proxyAddress);
//             }

//             //メソッドにPOSTを指定
//             req.Method = "POST";
//             //ContentTypeを設定
//             req.ContentType = "multipart/form-data; boundary=" + boundary;

//             //POST送信するデータを作成
//             string postData = "";
//             postData = "--" + boundary + "\r\n" +
//                 "Content-Disposition: form-data; name=\"file\"; filename=\"" +
//                     fromDataRequest.filename + "\"\r\n" +
//                 "Content-Type: application/octet-stream\r\n" +
//                 "Content-Transfer-Encoding: binary\r\n\r\n";
//             LogUtil.OutputInfo("リクエストデータ(ヘッダのみ):" + postData);
//             //バイト型配列に変換
//             startData = enc.GetBytes(postData);
//             postData = "\r\n--" + boundary + "--\r\n";
//             endData = enc.GetBytes(postData);

//             fs = new MemoryStream(fromDataRequest.binary);

//             //POST送信するデータの長さを指定
//             req.ContentLength = startData.Length + endData.Length + fs.Length;

//             //データをPOST送信するためのStreamを取得
//             reqStream = req.GetRequestStream();
//             //送信するデータを書き込む
//             reqStream.Write(startData, 0, startData.Length);
//         }
//         catch (Exception e)
//         {
//             throw new SystemErrorException("POST通信設定時エラー", e);
//         }

//         LogUtil.OutputInfo("通信先URL：" + path);
//         // カウント
//         int count = 1;
//         // 通信が失敗した場合、指定回数繰り返す
//         while (count <= RetryTimes)
//         {
//             try
//             {
//                 //ファイルの内容を送信
//                 byte[] readData = new byte[0x1000];
//                 int readSize = 0;
//                 while (true)
//                 {
//                     readSize = fs.Read(readData, 0, readData.Length);
//                     if (readSize == 0)
//                         break;
//                     reqStream.Write(readData, 0, readSize);
//                 }

//                 reqStream.Write(endData, 0, endData.Length);

//                 //サーバーからの応答を受信するためのWebResponseを取得
//                 System.Net.HttpWebResponse res =
//                     (System.Net.HttpWebResponse)req.GetResponse();
//                 //応答データを受信するためのStreamを取得
//                 System.IO.Stream resStream = res.GetResponseStream();
//                 //受信して表示
//                 sr = new System.IO.StreamReader(resStream, enc);

//                 string response = await sr.ReadToEndAsync();

//                 Debug.Log(response);
//                 LogUtil.OutputInfo(response);

//                 return response;
//             }
//             catch (TaskCanceledException)
//             {
//                 LogUtil.OutputDebug("通信先：" + path + "　通信回数：" + count);
//                 count++;
//             }
//             catch (Exception e)
//             {
//                 throw new GyomuException("POST通信エラー", e);
//             }
//             finally
//             {
//                 //閉じる
//                 fs.Close();
//                 reqStream.Close();
//                 sr.Close();
//             }
//         }

//         // 通信がうまくいかなかった場合は業務エラーを投げる
//         throw new GyomuException("POST通信タイムアウトエラー");
//     }
// }