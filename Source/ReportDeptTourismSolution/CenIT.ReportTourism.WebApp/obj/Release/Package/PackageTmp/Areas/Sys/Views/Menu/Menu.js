var _MenuActionURLs = {
    Menu_GetData: "/Sys/Menu/Get"
};
var _tableMenu;

$(document).ready(function() {
    initTableMenu();
});

function initTableMenu() {
    _tableMenu = $("#TableMenu").DataTable({
        "Responsive": true,
        "language": {
            "processing":
                "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "colReorder": true,
        "info": true,
        "autoWidth": false,
        "ajax":
        {
            "url": _MenuActionURLs.Menu_GetData,
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
                "data": "Link",
                "defaultContent": "",
                "orderable": false
            },
            {
                "data": "FunctionName",
                "defaultContent": "",
                "orderable": false
            },
            {
                "data": "MenuId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {

                        html += _renderButton(true,
                            "EditMenu",
                            "fa fa-pencil btn btn-default btn-form",
                            "/Sys/Menu/Edit/" + data,
                            "",
                            "Cập nhật");

                        html += _renderButton(true,
                            "DeleteMenu",
                            "fa fa-trash-o btn btn-danger btn-form",
                            "/Sys/Menu/Delete/" + data,
                            "",
                            "Xoá");
                    }

                    return html;
                }
            }
        ]
    });
}

function Menu_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tableMenu.ajax.reload(null, true);
                    response.status = undefined;
                }
            });
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}