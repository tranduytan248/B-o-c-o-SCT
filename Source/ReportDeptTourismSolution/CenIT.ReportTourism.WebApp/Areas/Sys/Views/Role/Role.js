
var _RoleActionURLs = {
    Role_GetData: "/Sys/Role/Get"
};
var _tableRole;

$(document).ready(function() {
    initTableRole();
});

function initTableRole() {
    _tableRole = $("#DSRole").DataTable({
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
            "url": _RoleActionURLs.Role_GetData,
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
                "data": "Name",
                "defaultContent": ""
            },
            {
                "data": "RoleId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {
                        html += _renderButton(true,
                            "PermissionRole",
                            "fa fa-list-ul btn btn-default btn-form",
                            "/Sys/Role/Permission/" + data,
                            "",
                            "Phân Quyền",
                            "860px");

                        html += _renderButton(true,
                            "EditRole",
                            "fa fa-pencil btn btn-default btn-form",
                            "/Sys/Role/Edit/" + data,
                            "",
                            "Cập nhật");

                        html += _renderButton(true,
                            "DeleteRole",
                            "fa fa-trash-o btn btn-danger btn-form",
                            "/Sys/Role/Delete/" + data,
                            "",
                            "Xoá");
                    }

                    return html;
                }
            }
        ]
    });
}

function Role_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tableRole.ajax.reload(null, true);
                    response.status = undefined;
                }
            });
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}

function OnEventActionChecked(chk) {
    var selectedActions = $('input[type="hidden"][id$="SelectedActions"]').val().split(",");

    if ($(chk)[0].checked) {
        selectedActions.push($(chk).val());
    } else {
        var index = selectedActions.indexOf(($(chk).val()));
        selectedActions.splice(index, 1);
    }
    $('input[type="hidden"][id$="SelectedActions"]').val(selectedActions);
}