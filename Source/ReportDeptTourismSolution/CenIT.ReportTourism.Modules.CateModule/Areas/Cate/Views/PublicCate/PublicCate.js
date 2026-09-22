var _PublicCateActionURLs = {
    PublicCate_GetData: "/Cate/PublicCate/Get"
};
var _tablePublicCate;
$(document).ready(function() {
    initTablePublicCate();
});

function initTablePublicCate() {
    _tablePublicCate = $("#DSPublicCate").DataTable({
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
            "url": _PublicCateActionURLs.PublicCate_GetData,
            "type": "POST",
            "dataType": "JSON",
            "data": {
                "cateTypeIds": function() {
                    return $("#Search select#ListCateTypeId").val() != null &&
                        $("#Search select#ListCateTypeId").val().length > 0
                        ? $("#Search select#ListCateTypeId").val()
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
                "data": "CateName",
                "defaultContent": ""
            },
            {
                "data": "CateTypeName",
                "defaultContent": ""
            },
            {
                "data": "CateId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {
                        if (row.HasChildsCate) {
                            html += _renderButton(true,
                                "ListChildCate",
                                "fa fa-list btn btn-success btn-form",
                                "/Cate/PublicCate/Childrens/" + data,
                                "",
                                "Danh mục con",
                                700);
                        }
                        html += _renderButton(true,
                            "EditPublicCate",
                            "fa fa-pencil btn btn-default btn-form",
                            "/Cate/PublicCate/Edit/" + data,
                            "",
                            "Cập nhật");
                        html += _renderButton(true,
                            "DeletePublicCate",
                            "fa fa-trash-o btn btn-danger btn-form",
                            "/Cate/PublicCate/Delete/" + data,
                            "",
                            "Xoá");
                    }
                    return html;
                }
            }
        ]
    });
}

function PublicCate_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tablePublicCate.ajax.reload(null, false);
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