var _NationalActionURLs = {
    National_GetData: "/Cate/National/Get"
};
var _tableNatinal;
$(document).ready(function() {
    initTableNational();
});

function initTableNational() {
    _tableNatinal = $("#DSNational").DataTable({
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
            "url": _NationalActionURLs.National_GetData,
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
                "data": "NationalCode",
                "defaultContent": ""
            },
            {
                "data": "NationalName",
                "defaultContent": ""
            },
            {
                "data": "ContinentName",
                "defaultContent": ""
            },
            {
                "data": "NationalId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {

                        html += _renderButton(true,
                            "EditNational",
                            "fa fa-pencil btn btn-default btn-form",
                            "/Cate/National/Edit/" + data,
                            "",
                            "Cập nhật");

                        html += _renderButton(true,
                            "DeleteNational",
                            "fa fa-trash-o btn btn-danger btn-form",
                            "/Cate/National/Delete/" + data,
                            "",
                            "Xoá");
                    }

                    return html;
                }
            }
        ]
    });
}

function AppNational_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tableNatinal.ajax.reload(null, false);
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