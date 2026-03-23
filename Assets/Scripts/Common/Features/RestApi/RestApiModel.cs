using System;
using System.IO;
using System.Net.Http;
using Scripts.Common.Features.Config;
using UnityEngine;

namespace Scripts.Common.Features.RestApi
{
    public class RestApiModel
    {
        public HttpClient Client { get; set; }
        public APIConfig APIConfig { get; set; }
        public int UploadTimeoutMS { get; set; } = 5 * 60 * 1000;

        public string BaseUrl { get; set; }
        public string LogFileName { get; set; }

        public string HealthEndpoint => string.Concat(BaseUrl, "/api/Health");
        public string UploadEndpoint => string.Concat(BaseUrl, "/api/Upload");
        public string DownloadEndpoint => string.Concat(BaseUrl, "/api/Download");
        public string GetMasterDataEndpoint => string.Concat(BaseUrl, "/api/GetMasterData");
        public string LogDirectoryPath => Path.Combine(Application.persistentDataPath, "M3Logs");
        public string LogFilePath => Path.Combine(LogDirectoryPath, LogFileName);
    }

    [Serializable]
    public class GetMasterDataResult
    {
        public bool isSuccess;
        public long statusCode;
        public int areaId;
        public MasterCategoryRecord[] ms_categories;
        public MasterAreaRecord[] ms_areas;
        public MasterInspectorRecord[] ms_inspectors;
        public MasterDetectionTypeRecord[] ms_detection_types;
        public MasterJudgeConditionRecord[] ms_judge_conditions;
        public MasterInspectionTargetRecord[] ms_inspection_targets;
        public MasterInspectionItemRecord[] ms_inspection_items;
        public MasterInspectionTargetAttachmentRecord[] ms_inspection_target_attachments;
        public string message;
    }

    [Serializable]
    public class MasterCategoryRecord
    {
        public int category_id;
        public string category_name;
        public int display_no;
    }

    [Serializable]
    public class MasterAreaRecord
    {
        public int area_id;
        public int category_id;
        public string area_name;
        public int display_no;
    }

    [Serializable]
    public class MasterInspectorRecord
    {
        public int inspector_id;
        public int area_id;
        public string inspector_name;
        public int display_no;
    }

    [Serializable]
    public class MasterDetectionTypeRecord
    {
        public int detection_type_id;
        public string detection_type_name;
        public int display_no;
    }

    [Serializable]
    public class MasterJudgeConditionRecord
    {
        public int judge_condition_id;
        public string judge_condition_key;
        public int input_kind;
        public int criteria_kind;
        public int min_criteria;
        public int max_criteria;
        public int min_inputs;
        public int max_inputs;
        public int requires_options_flg;
        public string description;
    }

    [Serializable]
    public class MasterInspectionTargetRecord
    {
        public int inspection_target_id;
        public int area_id;
        public int detection_type_id;
        public string inspection_target_name;
        public int display_no;
    }

    [Serializable]
    public class MasterInspectionItemRecord
    {
        public int inspection_item_id;
        public int inspection_target_id;
        public int judge_condition_id;
        public int inspection_item_key;
        public string valid_from;
        public string valid_to;
        public string item_name;
        public string unit;
        public string options_json;
        public string criteria_json;
        public int display_no;
    }

    [Serializable]
    public class MasterInspectionTargetAttachmentRecord
    {
        public int target_attachment_id;
        public int inspection_target_id;
        public string file_name;
        public string file_kind;
    }

    [Serializable]
    public class GetMasterDataRequestDto
    {
        public int areaId;
    }

    [Serializable]
    public class GetMasterDataResponseDto
    {
        public int areaId;
        public MasterCategoryRecord[] ms_categories;
        public MasterAreaRecord[] ms_areas;
        public MasterInspectorRecord[] ms_inspectors;
        public MasterDetectionTypeRecord[] ms_detection_types;
        public MasterJudgeConditionRecord[] ms_judge_conditions;
        public MasterInspectionTargetRecord[] ms_inspection_targets;
        public MasterInspectionItemRecord[] ms_inspection_items;
        public MasterInspectionTargetAttachmentRecord[] ms_inspection_target_attachments;
    }

    [Serializable]
    public class DownloadResult
    {
        public bool isSuccess;
        public long statusCode;
        public string fileName;
        public byte[] fileBytes;
        public string message;
    }

    [Serializable]
    public class DownloadRequestDto
    {
        public string uploadId;
    }
}
