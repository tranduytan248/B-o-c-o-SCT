var _DataImportActionURLs = {
    DataImport_GetData: "/Report/Report/Get"
};
var _tableDataImport;
$(document).ready(function () {
    initTableDataImport();
});

function initTableDataImport() {
    _tableDataImport = $("#DataImports").DataTable({
        "Responsive": true,
        "language": {
            "processing":
                "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "paging": false,
        "searching":false,
        "ajax":
        {
            "url": _DataImportActionURLs.DataImport_GetData,
            "type": "POST",
            "dataType": "JSON",
            "data": {
                "OnMonth": function () { return $("#SearchDataReport #OnMonth").val(); }
            }
        },
        "columns": [
            {
                "data": "",
                "defaultContent": "1",
                "render": function (data, type, row, meta) {
                    return meta.row + 1;
                }
            },
            {
                "data": "Targets",
                "defaultContent": ""
            },
            {
                "data": "Unit",
                "defaultContent": ""
            },
            {
                "data": "PerformPreviousPeriod",
                "defaultContent": ""
            },
            {
                "data": "PerformInPeriod",
                "defaultContent": ""
            },
            {
                "data": "SamePeriodRateOfPerform",
                "defaultContent": ""
            },
            {
                "data": "AccumulatedBeginingOfYear",
                "defaultContent": ""
            },
            {
                "data": "SamePeriodRateOfAccumulated",
                "defaultContent": ""
            }
        ]
    });
}

//function DataImport_OnProcessSuccess(response, formId) {
//    if (response.status != undefined) {
//        $("#ModalContent #modal_" + formId).modal("hide");
//        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
//            function () {
//                if (response.status != undefined) {
//                    eval(response.message);
//                    _tableDataImport.ajax.reload(null, true);
//                    $("#ActionArea").load("/Report/Import/ViewAction");
//                    response.status = undefined;
//                }
//            });
//    } else {
//        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
//    }
//}

//function OnChangeCombo(cbb, eleName) {
//    $(eleName).val($(cbb).children("option:selected").text());
//}