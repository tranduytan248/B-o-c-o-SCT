using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Report
{
    public class ReportDataImportModel
    {
        public long RowIndex { get; set; }

        public long ReportId { get; set; }

        [CustomDisplayName("DataImport_Label_Enterprise")]
        [CustomRequired]
        public int? EnterpriseId { get; set; }

        public string EnterpriseName { get; set; }


        [CustomDisplayName("DataImport_Label_ForMonth")]
        [CustomRequired]
        public DateTime? ForMonth { get; set; } = DateTime.Now;

        [CustomDisplayName("DataImport_Label_TypeReport")]
        public int TypeReport { get; set; }

        [CustomDisplayName("DataImport_Label_TypeReport")]
        public string TypeReportName { get; set; }

        [CustomDisplayName("DataImport_Label_FileImport")]
        [CustomRequired]
        public HttpPostedFileBase FileImport { get; set; }

        public DataTable DataImport { get; set; }

        [CustomDisplayName("DataImport_Label_Targets")]
        public string Targets { get; set; }

        [CustomDisplayName("DataImport_Label_Unit")]
        public string Unit { get; set; }

        [CustomDisplayName("DataImport_Label_Code")]
        public string Code { get; set; }

        [CustomDisplayName("DataImport_Label_PerformPreviousPeriod")]
        public double? PerformPreviousPeriod { get; set; }

        [CustomDisplayName("DataImport_Label_PerformInPeriod")]
        public double? PerformInPeriod { get; set; }

        [CustomDisplayName("DataImport_Label_AccumulatedBeginingOfYear")]
        public double? AccumulatedBeginingOfYear { get; set; }

        [CustomDisplayName("DataImport_Label_ComparedSamePeriodLastYear")]
        public double? ComparedSamePeriodLastYear { get; set; }

        public int DayDeadlineSendReport { get; set; } = 0;
        public int DayDeadlineSendReportLate { get; set; } = 0;

        public bool IsWrong { get; set; }

        public bool CanDelete { get; set; } = false;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? TotalRow { get; set; } = 0;

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }

        public List<ListItem> ListEnterprises { get; set; } = new List<ListItem>();

        public string AccessToken { get; set; }

        public string ReportFile { get; set; }

        public bool EnableSignDigitalDoc { get; set; } = false;

        public string VNPTTokenSerialKey { get; set; }

        /// <summary>
        ///     Loại ký số:
        ///     2 -> ký số token
        ///     1 -> ký số trực tiếp
        ///     0 -> tải file đã ký số lên
        /// </summary>
        public int? TypeSignature { get; set; } = 0;

        /// <summary>
        ///     Properties for Plugin CA Token
        /// </summary>
        public string FileName { get; set; }

        public string FileExt { get; set; }
        public string FileDataBase64 { get; set; }
    }
}