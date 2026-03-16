// using Cysharp.Threading.Tasks;
// using Mono.Data.Sqlite;
// using seiko.framework.bases.config;
// using seiko.framework.bases.db;
// using seiko.framework.bases.exception;
// using seiko.framework.bases.log;
// using seiko.framework.bases.service;
// using seiko.framework.bases.utils;
// using seiko.framework.gyomu.constant;
// using seiko.framework.gyomu.structs;
// using seiko.UI.Global;
// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Threading.Tasks;
// using UnityEngine;
// using Zenject;

// /// <summary>
// /// トップメニューService
// /// </summary>
// public class OSSTM001Service : BaseService
// {
//     [Inject]
//     CommonStruct commonStruct;
//     [Inject]
//     OSSTM001BlStruct osstm001BlStruct;
//     /// <summary>
//     /// マスタデータの取得
//     /// </summary>
//     /// <returns></returns>
//     public async UniTask GetMasterData()
//     {
//         // テーブルから操作箇所IDを取得
//         osstm001BlStruct.sosaKashoId = await Task.Run(() => CommonDbOperationUtil.SelectTarget());
//         Debug.Log("sosaKashoId: " + osstm001BlStruct.sosaKashoId);
//         // 操作箇所IDが変更されてない場合、処理を終了
//         if (commonStruct.sosaKashoId == osstm001BlStruct.sosaKashoId)
//             return;
//         GlobalUI.instance.Notice.ShowPopup(MessageKeyConstants.PROGRESS_COMMUNICATING);
//         // 操作箇所IDの設定
//         commonStruct.sosaKashoId = osstm001BlStruct.sosaKashoId;
//         //  マスタデータ取得（非同期）
//         //  タグマスタ取得（非同期）
//         await UniTask.WhenAll(GetMasterFromServer(), GetTagMasterFromServer());
//     }

//     /// <summary>
//     /// マスタデータ取得
//     /// </summary>
//     /// <returns></returns>
//     private async UniTask GetMasterFromServer()
//     {
//         try
//         {
//             // サーバからマスタデータを取得
//             MasterDataStruct masterDataStruct = await GetMasterDataFromServer(66495);
//             // MasterDataStruct masterDataStruct = await GetMasterDataFromServer(184);
//             if (masterDataStruct?.errorCode == 0)
//             {
//                 // マスタデータをローカルのDBに格納
//                 RegisterMaster(masterDataStruct);
//                 LogUtil.OutputInfo("マスタデータ登録完了");
//             }
//         }
//         catch (GyomuException e)
//         {
//             // 業務エラーの場合、特に何もせずにfinallyへ
//             LogUtil.OutputWarn("マスタデータの取得/更新失敗");
//             LogUtil.OutputWarn(e.ToString());
//         }
//         catch (Exception e)
//         {
//             LogUtil.OutputInfo("マスタデータ取得時エラー");
//             throw new SystemErrorException(e.ToString());
//         }
//         finally
//         {
//             // アプリ設定項目値の設定
//             SettingAppPreferences();
//         }
//     }

//     /// <summary>
//     /// タグマスタデータ取得
//     /// </summary>
//     /// <returns></returns>
//     private async UniTask GetTagMasterFromServer()
//     {
//         try
//         {
//             // サーバからタグマスタデータを取得
//             MasterDataStruct masterDataStruct = await GetMasterDataFromServer(64);
//             if (masterDataStruct?.errorCode == 0)
//             {
//                 // 現在日時の取得
//                 DateTime dt = DateTime.Now;
//                 string now = dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
//                 // マスタデータをローカルのDBに格納
//                 RegisterTag(masterDataStruct.tagList, now);
//                 LogUtil.OutputInfo("タグマスタデータ登録完了");
//             }
//         }
//         catch (GyomuException e)
//         {
//             LogUtil.OutputInfo("タグマスタ取得時業務エラー");
//             LogUtil.OutputInfo(e.ToString());
//         }
//         catch (Exception e)
//         {
//             LogUtil.OutputInfo("タグマスタ取得時エラー");
//             throw new SystemErrorException(e.ToString());
//         }
//     }

//     /// <summary>
//     /// サーバからマスタデータを取得
//     /// </summary>
//     /// <returns>レスポンスデータ</returns>
//     private async UniTask<MasterDataStruct> GetMasterDataFromServer(int targets)
//     {
//         // リクエスト作成
//         MasterDataRequest masterDataRequest = new MasterDataRequest();
//         // リクエストボディの生成
//         masterDataRequest.targets = targets;
//         // Debug.Log(targets);
//         masterDataRequest.sosaKashoId = osstm001BlStruct.sosaKashoId;
//         // Debug.Log(osstm001BlStruct.sosaKashoId);
//         string json = JsonUtility.ToJson(masterDataRequest);
//         // Debug.Log("======================taskStart");
//         // 現地操作支援サーバへＪｓｏｎ(api/GetMasterData)を送信
//         string response = await ExcuteRestApiUtil.PostAsync(json, @"api/GetMasterData");
//         // Debug.Log("======================response");
//         // Debug.Log(response);
//         // 結果を格納
//         MasterDataStruct masterDataStruct = JsonUtility.FromJson<MasterDataStruct>(response);
//         return masterDataStruct;
//     }

//     /// <summary>
//     /// サーバから操作箇所名称を取得
//     /// </summary>
//     /// <returns>レスポンスデータ</returns>
//     public Dictionary<string, object> GetSosaKashoName(string shishaShishoId, string qhtKeitoId)
//     {
//         string sql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "selectSosaKashoName", SqlType.SELECT);
//         sql = sql.Replace("@param1", shishaShishoId).Replace("@param2", qhtKeitoId);
//         List<Dictionary<string, object>> preferences = DbAccessUtil.ExecuteReader(sql, null, DbConnectUtil.Connection);
//         if (preferences.Count > 0)
//         {
//             return preferences[0];
//         }

//         return new Dictionary<string, object>();
//     }

//     /// <summary>
//     /// マスタへの登録 
//     /// </summary>
//     /// <param name = "masterDataStruct">マスタデータ取得レスポンスデータ</param>
//     private void RegisterMaster(MasterDataStruct masterDataStruct)
//     {
//         // 現在日時の取得
//         DateTime dt = DateTime.Now;
//         string now = dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
//         try
//         {
//             // トランザクション開始
//             TransactionUtil.StartTransaction();
//             // DBへの登録
//             RegisterShisha(masterDataStruct.shishaList, masterDataStruct.nainenChikuList, now);
//             RegisterShisho(masterDataStruct.shishoList, masterDataStruct.nainenShimaList, now);
//             RegisterKeito(masterDataStruct.keitoList, now);
//             RegisterDenkisho(masterDataStruct.denkishoList, now);
//             RegisterShozoku(masterDataStruct.shozokuList, now);
//             RegisterShoin(masterDataStruct.shoinList, now);
//             RegisterEarthStatus(masterDataStruct.earthStatusList, now);
//             UpdatePreferences(masterDataStruct.preferences);
//             // コミット
//             TransactionUtil.Commit();
//         }
//         catch (GyomuException e)
//         {
//             throw e;
//         }
//         catch (Exception e)
//         {
//             // ロールバック
//             TransactionUtil.Rollback();
//             throw e;
//         }
//     }

//     /// <summary>
//     /// 支社マスタ情報テーブルへの登録
//     /// </summary>
//     /// <param name = "shishaList">支社マスタデータの配列</param>
//     /// <param name = "nainenChikuList">内燃力地区マスタデータの配列</param>
//     /// <param name = "now">現在日時</param>
//     private void RegisterShisha(ShishaList[] shishaList, NainenChikuList[] nainenChikuList, string now)
//     {
//         // 登録前削除
//         string deleteSql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "deleteShisha", SqlType.DELETE);
//         DbAccessUtil.ExecuteWriter(deleteSql, null, DbConnectUtil.Connection);
//         string sql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "insertShisha", SqlType.INSERT);
//         List<object> bindParameter;
//         // 支社マスタへの登録（QHT）
//         if (shishaList != null && shishaList.Length > 0)
//         {
//             foreach (ShishaList shisha in shishaList)
//             {
//                 bindParameter = new List<object>();
//                 bindParameter.Add(shisha.id);
//                 bindParameter.Add(shisha.name);
//                 bindParameter.Add(shisha.hyojiNo);
//                 bindParameter.Add(now);
//                 bindParameter.Add("1");
//                 DbAccessUtil.ExecuteWriter(sql, bindParameter, DbConnectUtil.Connection);
//                 bindParameter = null;
//             }
//         }

//         // 支社マスタへの登録（内燃力）
//         if (nainenChikuList != null && nainenChikuList.Length > 0)
//         {
//             foreach (NainenChikuList nainenChiku in nainenChikuList)
//             {
//                 bindParameter = new List<object>();
//                 bindParameter.Add(nainenChiku.id);
//                 bindParameter.Add(nainenChiku.name);
//                 bindParameter.Add(nainenChiku.hyojiNo);
//                 bindParameter.Add(now);
//                 bindParameter.Add("2");
//                 DbAccessUtil.ExecuteWriter(sql, bindParameter, DbConnectUtil.Connection);
//                 bindParameter = null;
//             }
//         }
//     }

//     /// <summary>
//     /// 支所マスタ情報テーブルへの登録
//     /// </summary>
//     /// <param name = "shishoList">支所マスタデータの配列</param>
//     /// <param name = "nainenShimaList">内燃力島マスタデータの配列</param>
//     /// <param name = "now">現在日時</param>
//     private void RegisterShisho(ShishoList[] shishoList, NainenShimaList[] nainenShimaList, string now)
//     {
//         // 登録前削除
//         string deleteSql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "deleteShisho", SqlType.DELETE);
//         DbAccessUtil.ExecuteWriter(deleteSql, null, DbConnectUtil.Connection);
//         string sql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "insertShisho", SqlType.INSERT);
//         List<object> bindParameter;
//         // 支所マスタへの登録（QHT）
//         if (shishoList != null && shishoList.Length > 0)
//         {
//             foreach (ShishoList shisho in shishoList)
//             {
//                 bindParameter = new List<object>();
//                 bindParameter.Add(shisho.id);
//                 bindParameter.Add(shisho.name);
//                 bindParameter.Add(shisho.shitenId);
//                 bindParameter.Add(shisho.sortFlag);
//                 bindParameter.Add(shisho.hyojiNo);
//                 bindParameter.Add(now);
//                 bindParameter.Add("1");
//                 DbAccessUtil.ExecuteWriter(sql, bindParameter, DbConnectUtil.Connection);
//                 bindParameter = null;
//             }
//         }

//         // 支所マスタへの登録（内燃力）
//         if (nainenShimaList != null && nainenShimaList.Length > 0)
//         {
//             foreach (NainenShimaList nainenShima in nainenShimaList)
//             {
//                 bindParameter = new List<object>();
//                 bindParameter.Add(nainenShima.id);
//                 bindParameter.Add(nainenShima.name);
//                 bindParameter.Add(nainenShima.shitenId);
//                 bindParameter.Add(nainenShima.sortFlag);
//                 bindParameter.Add(nainenShima.hyojiNo);
//                 bindParameter.Add(now);
//                 bindParameter.Add("2");
//                 DbAccessUtil.ExecuteWriter(sql, bindParameter, DbConnectUtil.Connection);
//                 bindParameter = null;
//             }
//         }
//     }

//     /// <summary>
//     /// 系統マスタ情報テーブルへの登録
//     /// </summary>
//     /// <param name = "keitoList">系統マスタデータの配列</param>
//     /// <param name = "now">現在日時</param>
//     private void RegisterKeito(KeitoList[] keitoList, string now)
//     {
//         // リストがない場合、終了
//         if (keitoList == null || keitoList.Length == 0)
//             return;
//         // 登録前削除
//         string deleteSql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "deleteKeito", SqlType.DELETE);
//         DbAccessUtil.ExecuteWriter(deleteSql, null, DbConnectUtil.Connection);
//         string sql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "insertKeito", SqlType.INSERT);
//         List<object> bindParameter;
//         // 系統給電制御所マスタへの登録
//         foreach (KeitoList keito in keitoList)
//         {
//             bindParameter = new List<object>();
//             bindParameter.Add(keito.id);
//             bindParameter.Add(keito.name);
//             bindParameter.Add(keito.sortFlag);
//             bindParameter.Add(keito.hyojiNo);
//             bindParameter.Add(now);
//             DbAccessUtil.ExecuteWriter(sql, bindParameter, DbConnectUtil.Connection);
//             bindParameter = null;
//         }
//     }

//     /// <summary>
//     /// 電気所マスタテーブルへの登録
//     /// </summary>
//     /// <param name = "denkishoList">電気所マスタデータの配列</param>
//     /// <param name = "now">現在日時</param>
//     private void RegisterDenkisho(DenkishoList[] denkishoList, string now)
//     {
//         // リストがない場合、終了
//         if (denkishoList == null || denkishoList.Length == 0)
//             return;
//         // 登録前削除
//         string deleteSql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "deleteDenkisho", SqlType.DELETE);
//         DbAccessUtil.ExecuteWriter(deleteSql, null, DbConnectUtil.Connection);
//         string sql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "insertDenkisho", SqlType.INSERT);
//         List<object> bindParameter;
//         // 電気所マスタへの登録
//         foreach (DenkishoList denkisho in denkishoList)
//         {
//             bindParameter = new List<object>();
//             bindParameter.Add(denkisho.id);
//             bindParameter.Add(denkisho.name);
//             bindParameter.Add(denkisho.shozokuId);
//             bindParameter.Add(denkisho.columnNo);
//             bindParameter.Add(denkisho.rowNo);
//             bindParameter.Add(denkisho.networkFlag);
//             bindParameter.Add(now);
//             DbAccessUtil.ExecuteWriter(sql, bindParameter, DbConnectUtil.Connection);
//             bindParameter = null;
//         }
//     }

//     /// <summary>
//     /// 所属マスタ情報テーブルへの登録
//     /// </summary>
//     /// <param name = "shozokuList">所属マスタデータの配列</param>
//     /// <param name = "now">現在日時</param>
//     private void RegisterShozoku(ShozokuList[] shozokuList, string now)
//     {
//         // リストがない場合、終了
//         if (shozokuList == null || shozokuList.Length == 0)
//             return;
//         // 登録前削除
//         string deleteSql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "deleteShozoku", SqlType.DELETE);
//         DbAccessUtil.ExecuteWriter(deleteSql, null, DbConnectUtil.Connection);
//         string sql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "insertShozoku", SqlType.INSERT);
//         List<object> bindParameter;
//         // 所属部署マスタへの登録
//         foreach (ShozokuList shozoku in shozokuList)
//         {
//             bindParameter = new List<object>();
//             bindParameter.Add(shozoku.id);
//             bindParameter.Add(shozoku.name);
//             bindParameter.Add(shozoku.kubun);
//             bindParameter.Add(shozoku.hyojiNo);
//             bindParameter.Add(now);
//             DbAccessUtil.ExecuteWriter(sql, bindParameter, DbConnectUtil.Connection);
//             bindParameter = null;
//         }
//     }

//     /// <summary>
//     /// 所員マスタテーブルへの登録
//     /// </summary>
//     /// <param name = "shoinList">所員マスタデータの配列</param>
//     /// <param name = "now">現在日時</param>
//     private void RegisterShoin(ShoinList[] shoinList, string now)
//     {
//         // リストがない場合、終了
//         if (shoinList == null || shoinList.Length == 0)
//             return;
//         // 登録前削除
//         string deleteSql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "deleteShoin", SqlType.DELETE);
//         DbAccessUtil.ExecuteWriter(deleteSql, null, DbConnectUtil.Connection);
//         string sql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "insertShoin", SqlType.INSERT);
//         List<object> bindParameter;
//         // 所員マスタへの登録
//         foreach (ShoinList shoin in shoinList)
//         {
//             bindParameter = new List<object>();
//             bindParameter.Add(shoin.id);
//             bindParameter.Add(shoin.loginId);
//             bindParameter.Add(shoin.name);
//             bindParameter.Add(shoin.shozokuId);
//             bindParameter.Add(shoin.hyojiNo);
//             bindParameter.Add(now);
//             DbAccessUtil.ExecuteWriter(sql, bindParameter, DbConnectUtil.Connection);
//             bindParameter = null;
//         }
//     }

//     /// <summary>
//     /// アース付け状態情報テーブルへの登録
//     /// </summary>
//     /// <param name = "earthStatusList">アース付け状態データの配列</param>
//     /// <param name = "now">現在日時</param>
//     private void RegisterEarthStatus(EarthStatusList[] earthStatusList, string now)
//     {
//         // リストがない場合、終了
//         if (earthStatusList == null || earthStatusList.Length == 0)
//             return;
//         // 登録前削除
//         string deleteSql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "deleteEarthStatus", SqlType.DELETE);
//         DbAccessUtil.ExecuteWriter(deleteSql, null, DbConnectUtil.Connection);
//         string sql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "insertEarthStatus", SqlType.INSERT);
//         List<object> bindParameter;
//         // アース付け状態テーブルへの登録
//         foreach (EarthStatusList earthStatus in earthStatusList)
//         {
//             bindParameter = new List<object>();
//             bindParameter.Add(earthStatus.kaiheikiId);
//             bindParameter.Add(earthStatus.kaiheikiTagCode);
//             bindParameter.Add(earthStatus.pairKaiheikiTagCode);
//             bindParameter.Add(earthStatus.sosaDenkishoName);
//             bindParameter.Add(earthStatus.sosaKaiheikiNo);
//             bindParameter.Add(earthStatus.sosaSosaNaiyo);
//             bindParameter.Add(earthStatus.sosaKakuninJiko);
//             bindParameter.Add(earthStatus.sosaTime);
//             bindParameter.Add(now);
//             DbAccessUtil.ExecuteWriter(sql, bindParameter, DbConnectUtil.Connection);
//             bindParameter = null;
//         }
//     }

//     /// <summary>
//     /// アプリ設定項目値マスタテーブルの更新
//     /// </summary>
//     /// <param name = "preferences">アプリ設定項目値データ</param>
//     private void UpdatePreferences(Preferences preferences)
//     {
//         // 設定情報がない場合、終了
//         if (preferences == null || preferences.prefsVersion == 0)
//             return;
//         string sql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "updatePreferences", SqlType.UPDATE);
//         // アプリ設定項目値マスタテーブルの更新
//         List<object> bindParameter = new List<object>();
//         bindParameter.Add(preferences.prefsVersion);
//         bindParameter.Add(preferences.httpConnectTimeout);
//         bindParameter.Add(preferences.httpReadTimeout);
//         bindParameter.Add(preferences.testConnectionInterval);
//         bindParameter.Add(preferences.syncJissekiInterval);
//         DbAccessUtil.ExecuteWriter(sql, bindParameter, DbConnectUtil.Connection);
//     }

//     /// <summary>
//     /// アプリ設定項目値の設定
//     /// </summary>
//     private void SettingAppPreferences()
//     {
//         // DBからアプリ設定項目値取得
//         string sql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "selectPreferences", SqlType.SELECT);
//         List<Dictionary<string, object>> preferences = DbAccessUtil.ExecuteReader(sql, null, DbConnectUtil.Connection);
//         // アプリ設定項目値の設定
//         CommonStaticStruct.testConnectionInterval = Convert.ToInt32(preferences[0]["test_connection_interval"]);
//         CommonStaticStruct.syncJissekiInterval = Convert.ToInt32(preferences[0]["sync_jisseki_interval"]);
//     }

//     /// <summary>
//     /// タグマスタ情報テーブルへの登録
//     /// </summary>
//     /// <param name = "tagList">アース・許可証タグマスタデータの配列</param>
//     /// <param name = "now">現在日時</param>
//     private void RegisterTag(TagList[] tagList, string now)
//     {
//         // リストがない場合、終了
//         if (tagList == null || tagList.Length == 0)
//             return;
//         try
//         {
//             // トランザクション開始
//             TransactionUtil.StartTransaction();
//             // 登録前削除
//             string deleteSql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "deleteTag", SqlType.DELETE);
//             DbAccessUtil.ExecuteWriter(deleteSql, null, DbConnectUtil.Connection);
//             string sql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "insertTag", SqlType.INSERT);
//             List<object> bindParameter;
//             // タグマスタへの登録
//             foreach (TagList tag in tagList)
//             {
//                 bindParameter = new List<object>();
//                 bindParameter.Add(tag.tagCode);
//                 bindParameter.Add(tag.hudaCode);
//                 bindParameter.Add(tag.kaiheikiId);
//                 bindParameter.Add(tag.kaiheikiName);
//                 bindParameter.Add(tag.pairTagCode);
//                 bindParameter.Add(now);
//                 DbAccessUtil.ExecuteWriter(sql, bindParameter, DbConnectUtil.Connection);
//                 bindParameter = null;
//             }

//             // コミット
//             TransactionUtil.Commit();
//         }
//         catch (GyomuException e)
//         {
//             throw e;
//         }
//         catch (Exception e)
//         {
//             // ロールバック
//             TransactionUtil.Rollback();
//             throw e;
//         }
//     }

//     /// <summary>
//     /// 開閉器添付ファイルDlテーブルの更新
//     /// </summary>
//     public void UpdateKaiheikiFileListDl()
//     {
//         try
//         {
//             // トランザクション開始
//             TransactionUtil.StartTransaction();
//             string sql = DbAccessUtil.GetSql("OSSTM001Sql.xml", "updateKaiheikiFileListDl", SqlType.UPDATE);
//             DbAccessUtil.ExecuteWriter(sql, null, DbConnectUtil.Connection);
//             // Debug.Log(sql);
//             // コミット
//             TransactionUtil.Commit();
//             LogUtil.OutputInfo("開閉器ファイルクリア");
//         }
//         catch (GyomuException e)
//         {
//             throw e;
//         }
//         catch (Exception e)
//         {
//             // ロールバック
//             TransactionUtil.Rollback();
//             throw e;
//         }
//     }

//     /// <summary>
//     /// データベースのVACUUM
//     /// </summary>
//     public void VacuumDataBase()
//     {
//         try
//         {
//             string folderPath = "";
//             // DBパスを取得
//             folderPath = @ConfigProvider.AppConfigs.DbConfig.FilePath;
//             string filePath = Path.Combine(folderPath, @ConfigProvider.AppConfigs.DbConfig.FileName);
//             // sqlite.dbを取得
//             FileInfo file = new FileInfo(filePath);
//             // DBの接続
//             using (var connection = new SqliteConnection("Data Source=" + filePath))
//             {
//                 connection.Open();
//                 // テーブルの作成
//                 var command = connection.CreateCommand();
//                 command.CommandText = "vacuum;";
//                 command.ExecuteNonQuery();
//             }

//             LogUtil.OutputInfo("VACUUMを実行しました。");
//         }
//         catch (Exception e)
//         {
//             LogUtil.OutputError(e.ToString());
//             Debug.Log(e.ToString());
//         }
//     }
// }