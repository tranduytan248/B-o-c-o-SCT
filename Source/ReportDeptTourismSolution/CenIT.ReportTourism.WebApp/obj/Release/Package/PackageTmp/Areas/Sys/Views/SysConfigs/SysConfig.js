var _AppConfigActionURLs = {
    AppConfig_GetData: "/Sys/SysConfigs/Get"
};
var _tableAppConfig;

$(document).ready(function() {
    initTableAppConfig();
});

function initTableAppConfig() {
    _tableAppConfig = $("#DSConfig").DataTable({
        "Responsive": true,
        "language": {
            "processing":
                "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _AppConfigActionURLs.AppConfig_GetData,
            "type": "POST",
            "dataType": "JSON"
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
                "data": "ConfigKey",
                "defaultContent": ""
            },
            {
                "data": "ConfigValue",
                "defaultContent": ""
            },
            {
                "data": "ConfigDesc",
                "defaultContent": ""
            },
            {
                "data": "ConfigId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {

                        html += _renderButton(true,
                            "EditConfig",
                            "fa fa-pencil btn btn-default btn-form",
                            "/Sys/SysConfigs/Edit/" + data,
                            "",
                            "Cập nhật");

                        html += _renderButton(true,
                            "DeleteConfig",
                            "fa fa-trash-o btn btn-danger btn-form",
                            "/Sys/SysConfigs/Delete/" + data,
                            "",
                            "Xoá");
                    }

                    return html;
                }
            }
        ]
    });
}

function AppConfig_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tableAppConfig.ajax.reload(null, false);
                    response.status = undefined;
                }
            });
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}