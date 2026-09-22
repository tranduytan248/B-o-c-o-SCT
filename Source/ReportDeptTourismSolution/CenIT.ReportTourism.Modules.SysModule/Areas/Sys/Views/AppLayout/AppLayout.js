var _AppLayoutActionURLs = {
    AppLayout_GetData: "/Sys/AppLayout/Get"
};
var _tableAppLayout;

$(document).ready(function() {
    initTableAppLayout();
});

function initTableAppLayout() {
    _tableAppLayout = $("#DSAppLayout").DataTable({
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
            "url": _AppLayoutActionURLs.AppLayout_GetData,
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
                "data": "LayoutName",
                "defaultContent": ""
            },
            {
                "data": "LayoutView",
                "defaultContent": ""
            },
            {
                "data": "NumberContentPanel",
                "defaultContent": ""
            },
            {
                "data": "NumberCol",
                "defaultContent": ""
            },
            {
                "data": "Activated",
                "defaultContent": "",
                "className": "text-center",
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (data) {
                        html = '<i class="icon fa fa-check text-green"></i>';
                    }
                    return html;
                }
            },
            {
                "data": "LayoutId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {
                        html += _renderButton(true,
                            "EditAppLayout",
                            "fa fa-pencil btn btn-default btn-form",
                            "/Sys/AppLayout/Edit/" + data,
                            "",
                            "Cập nhật");
                        html += _renderButton(true,
                            "ChangeActiveAppLayout",
                            "fa fa-check btn btn-success btn-form",
                            "/Sys/AppLayout/ChangeActive/" + data,
                            "",
                            "Kích hoạt");

                        html += _renderButton(true,
                            "DeleteAppLayout",
                            "fa fa-trash-o btn btn-danger btn-form",
                            "/Sys/AppLayout/Delete/" + data,
                            "",
                            "Xoá");
                    }

                    return html;
                }
            }
        ]
    });
}

function AppLayout_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tableAppLayout.ajax.reload(null, true);
                    response.status = undefined;
                    location.reload();
                }
            });
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}