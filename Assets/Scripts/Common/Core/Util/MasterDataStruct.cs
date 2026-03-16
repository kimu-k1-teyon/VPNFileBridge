using System;

namespace seiko.framework.gyomu.structs
{
    /// <summary>
    /// マスタデータ取得レスポンスデータ
    /// </summary>
    [Serializable]
    public class MasterDataStruct
    {
        public int errorCode;
        public int targets;
        public int errorTargets;
        public string env;
        public ShishaList[] shishaList;
        public ShishoList[] shishoList;
        public KeitoList[] keitoList;
        public DenkishoList[] denkishoList;
        public ShozokuList[] shozokuList;
        public ShoinList[] shoinList;
        public TagList[] tagList;
        public EarthStatusList[] earthStatusList;
        public NainenChikuList[] nainenChikuList;
        public NainenShimaList[] nainenShimaList;
        public Preferences preferences;
    }

    /// <summary>
    /// 支社マスタデータ
    /// </summary>
    [Serializable]
    public class ShishaList
    {
        public int id;
        public string name;
        public int hyojiNo;
    }

    /// <summary>
    /// 支所マスタデータ
    /// </summary>
    [Serializable]
    public class ShishoList
    {
        public int id;
        public string name;
        public int shitenId;
        public string sortFlag;
        public int hyojiNo;
    }

    /// <summary>
    /// 系統マスタデータ
    /// </summary>
    [Serializable]
    public class KeitoList
    {
        public int id;
        public string name;
        public string sortFlag;
        public int hyojiNo;
    }

    /// <summary>
    /// 電気所マスタデータ
    /// </summary>
    [Serializable]
    public class DenkishoList
    {
        public int id;
        public string name;
        public int shozokuId;
        public int columnNo;
        public int rowNo;
        public string networkFlag;
    }

    /// <summary>
    /// 所属マスタデータ
    /// </summary>
    [Serializable]
    public class ShozokuList
    {
        public int id;
        public string name;
        public string kubun;
        public int hyojiNo;
    }

    /// <summary>
    /// 所員マスタデータ
    /// </summary>
    [Serializable]
    public class ShoinList
    {
        public int id;
        public string loginId;
        public string name;
        public int shozokuId;
        public int hyojiNo;
    }

    /// <summary>
    /// アース・許可証タグマスタデータ
    /// </summary>
    [Serializable]
    public class TagList
    {
        public string tagCode;
        public int hudaCode;
        public int kaiheikiId;
        public string kaiheikiName;
        public string pairTagCode;
    }

    /// <summary>
    /// アース付け状態データ
    /// </summary>
    [Serializable]
    public class EarthStatusList
    {
        public int kaiheikiId;
        public string kaiheikiTagCode;
        public int pairKaiheikiId;
        public string pairKaiheikiTagCode;
        public string sosaDenkishoName;
        public string sosaKaiheikiNo;
        public string sosaSosaNaiyo;
        public string sosaKakuninJiko;
        public string sosaTime;
    }

    /// <summary>
    /// 内燃力地区マスタデータ
    /// </summary>
    [Serializable]
    public class NainenChikuList
    {
        public int id;
        public string name;
        public int hyojiNo;
    }

    /// <summary>
    /// 内燃力島マスタデータ
    /// </summary>
    [Serializable]
    public class NainenShimaList
    {
        public int id;
        public string name;
        public int shitenId;
        public string sortFlag;
        public int hyojiNo;
    }

    /// <summary>
    /// アプリ設定項目値データ
    /// </summary>
    [Serializable]
    public class Preferences
    {
        public int prefsVersion;
        public int httpConnectTimeout;
        public int httpReadTimeout;
        public int testConnectionInterval;
        public int syncJissekiInterval;
    }
}
