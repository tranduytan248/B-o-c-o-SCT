var _MessageActionURLs = {
    Message_GetData: "/Sys/Message/Get"
};
var _tableMessage;

$(document).ready(function() {
    initTableMessage();
});

function initTableMessage() {
    _tableMessage = $("#TableMessage").DataTable({
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
            "url": _MessageActionURLs.Message_GetData,
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
                "data": "LangCode",
                "defaultContent": ""
            },
            {
                "data": "LabelKey",
                "defaultContent": ""
            },
            {
                "data": "Message",
                "defaultContent": ""
            },
            {
                "data": "LangCode",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var key = data + row.LabelKey;
                    var html = "";
                    if (type === "display") {
                        html += _renderButton(true,
                            "EditMessage",
                            "fa fa-pencil btn btn-default btn-form",
                            "/Sys/Message/Edit/" + key,
                            "",
                            "Cập nhật");

                        html += _renderButton(true,
                            "DeleteMessage",
                            "fa fa-trash-o btn btn-danger btn-form",
                            "/Sys/Message/Delete/" + key,
                            "",
                            "Xoá");
                    }

                    return html;
                }
            }
        ]
    });
}

function Message_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tableMessage.ajax.reload(null, true);
                    response.status = undefined;
                }
            });
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}