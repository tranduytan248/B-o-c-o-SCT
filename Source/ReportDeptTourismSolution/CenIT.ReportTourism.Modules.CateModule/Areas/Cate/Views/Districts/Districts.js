var _NationalActionURLs = {
    National_GetData: "/Cate/Districts/Get"
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
            "dataType": "JSON",
            "data": {
                "ProvincesIds": function() {
                    return $("#Search select#ListProvinceId").val() != null &&
                        $("#Search select#ListProvinceId").val().length > 0
                        ? $("#Search select#ListProvinceId").val()
                        : "";
                }
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
                "data": "ProvinceName",
                "defaultContent": ""
            },
            {
                "data": "DistrictCode",
                "defaultContent": ""
            },
            {
                "data": "DistrictName",
                "defaultContent": ""
            },
            {
                "data": "DistrictId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {
                        html =
                            '<a data-modal="" data-modal-id="Edit" href="/Cate/Districts/Edit/' +
                            data +
                            '" class="fa fa-pencil btn btn-default btn-form" title="Cập nhật" data-toggle="tooltip"></a>' +
                            '<a data-modal="" data-width="1024" data-modal-id="ListWards" href="/Cate/Wards/WardByDistrict/' +
                            data +
                            '" class="fa fa-list btn btn-default btn-form" title="Danh sách Phường/Xã" data-toggle="tooltip"></a>';

                        html +=
                            '<a data-modal="" data-modal-id="Delete" href="/Cate/Districts/Delete/' +
                            data +
                            '" class="fa fa-trash-o btn btn-danger btn-form" title="Xóa" data-toggle="tooltip"></a>';

                    }

                    return html;
                }
            }
        ]
    });
}

function App_OnProcessSuccess(response, formId) {
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