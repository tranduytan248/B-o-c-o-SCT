var _ModuleActionURLs = {
    Module_GetData: "/Sys/Modules/Get"
};
var _tableModule;

$(document).ready(function() {
    initTableModule();
});

function initTableModule() {
    _tableModule = $("#DSModule").DataTable({
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
            "url": _ModuleActionURLs.Module_GetData,
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
                "data": "ModuleName",
                "defaultContent": ""
            },
            {
                "data": "Description",
                "defaultContent": ""
            },
            {
                "data": "ModuleId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {
                        html += _renderButton(true,
                            "PermissionModule",
                            "fa fa-th-list btn btn-default btn-form",
                            "/Sys/Modules/Permission/" + data,
                            "",
                            "Phân quyền truy cập");

                        html += _renderButton(true,
                            "EditModule",
                            "fa fa-pencil btn btn-default btn-form",
                            "/Sys/Modules/Edit/" + data,
                            "",
                            "Cập nhật");

                        html += _renderButton(true,
                            "DeleteModule",
                            "fa fa-trash-o btn btn-danger btn-form",
                            "/Sys/Modules/Delete/" + data,
                            "",
                            "Xoá");
                    }

                    return html;
                }
            }
        ]
    });
}

function Module_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tableModule.ajax.reload(null, true);
                    response.status = undefined;
                }
            });
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}