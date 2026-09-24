var _DataImportActionURLs = {
    DataImport_GetData: "/Report/Import/Get"
};
var _tableDataImport;

$(document).ready(function() {
    initTableDataImport();
});

function initTableDataImport() {
    _tableDataImport = $("#DataImports").DataTable({
        "Responsive": true,
        "language": {
            "processing":
                "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>"
        },
        "order": [[0, "desc"]],
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _DataImportActionURLs.DataImport_GetData,
            "type": "POST",
            "dataType": "JSON",
            "data": {
                "EnterpriseIds": function() {
                    return $("#SearchDataReport select#ListEnterpriseId").val() != null &&
                        $("#SearchDataReport select#ListEnterpriseId").val().length > 0
                        ? $("#SearchDataReport select#ListEnterpriseId").val()
                        : "";
                },
                "FromMonth": function() { return $("#SearchDataReport #FromMonth").val(); },
                "ToMonth": function() { return $("#SearchDataReport #ToMonth").val(); },
                "TypeBusinessIds": function() {
                    return $("#SearchDataReport select#ListTypeBusinessId").val() != null &&
                        $("#SearchDataReport select#ListTypeBusinessId").val().length > 0
                        ? $("#SearchDataReport select#ListTypeBusinessId").val()
                        : "";
                },
            }
        },
        "columns": [
            {
                "data": "",
                "defaultContent": "1",
                "render": function(data, type, row, meta) {
                    return meta.row + 1;
                }
            },
            {
                "data": "EnterpriseName",
                "defaultContent": ""
            },
            {
                "data": "ForMonth",
                "defaultContent": "",
                "render": function(data, type, row, meta) {
                    if (data != null) {
                        return moment(data).format("MM/YYYY");
                    } else {
                        return "";
                    }
                }
            },
            {
                "data": "TypeReportName",
                "defaultContent": ""
            },
            {
                "data": "CreatedBy",
                "defaultContent": ""
            },
            {
                "data": "CreatedDate",
                "defaultContent": "",
                "render": function(data, type, row, meta) {
                    if (data != null) {
                        return moment(data).format("HH:mm:ss DD/MM/YYYY");
                    } else {
                        return "";
                    }
                }
            },
            {
                "data": "ReportId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {
                        if (row.CanDelete) {
                            html += _renderButton(true,
                                "EditReport",
                                "fa fa-pencil btn bg-navy btn-form",
                                "/Report/Import/Edit?enterpriseId=" +
                                row.EnterpriseId +
                                "&onMonth=" +
                                moment(row.ForMonth).format("YYYY-MM-DD"),
                                "",
                                "Chỉnh sửa báo cáo",
                                1024);
                        }

                        html += _renderButton(true,
                            "ViewDataImport",
                            "fa fa-eye btn btn-primary btn-form",
                            "/Report/Import/View?enterpriseId=" +
                            row.EnterpriseId +
                            "&onMonth=" +
                            moment(row.ForMonth).format("YYYY-MM-DD"),
                            "",
                            "Xem dữ liệu báo cáo",
                            "1024px");

                        //html += _renderButton(false,
                        //    "DownloadSignedDoc",
                        //    "fa fa-cloud-download btn bg-teal btn-form",
                        //    "/Report/Import/DownloadSignedDoc?enterpriseId=" +
                        //    row.EnterpriseId +
                        //    "&onMonth=" +
                        //    moment(row.ForMonth).format("YYYY-MM-DD"),
                        //    "",
                        //    "Tải báo cáo đã ký số");

                        if (row.CanDelete) {
                            html += _renderButton(true,
                                "DeleteDataImport",
                                "fa fa-trash-o btn btn-danger btn-form",
                                "/Report/Import/Delete?enterpriseId=" +
                                row.EnterpriseId +
                                "&onMonth=" +
                                moment(row.ForMonth).format("YYYY-MM-DD"),
                                "",
                                "Xoá",
                                800);
                        }
                    }
                    return html;
                }
            }
        ]
    });
    _tableDataImport.on("draw",
        function() {
            $("a[data-modal-id='DownloadSignedDoc']").each(function(idx, ele) {
                var href = $(ele).attr("href");
                $(ele).data("href", href);
                $(ele).attr("href", "javascript:void(0);");
                $(ele).on("click",
                    function(e) {
                        var url = $(this).data("href");
                        $.ajax({
                            type: "GET",
                            url: url,
                            success: function(response) {
                                if (typeof response != "undefined" && response.status) {
                                    window.location = response.downloadPath;
                                } else if (typeof response != "undefined" && !response.status) {
                                    eval(response.message);
                                }
                            }
                        });
                    });
            });
        });
}

function DataImport_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if (!response.status) {
            eval(response.message);
            return;
        }
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tableDataImport.ajax.reload(null, true);
                    $("#ActionArea").load("/Report/Import/ViewAction");
                    response.status = undefined;
                }
            });
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}

function OnChangeCombo(cbb, eleName) {
    $(eleName).val($(cbb).children("option:selected").text());
}
