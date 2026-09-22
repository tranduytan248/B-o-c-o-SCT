var _NationalActionURLs = {
    National_GetData: "/Cate/Provinces/Get"
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
                "data": "ProvinceCode",
                "defaultContent": ""
            },
            {
                "data": "ProvinceName",
                "defaultContent": ""
            },
            {
                "data": "ProvinceId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    console.log(row);
                    var _html = "";
                    if (type === "display") {

                        _html =
                            '<a data-modal="" data-modal-id="Edit" href="/Cate/Provinces/Edit/' +
                            data +
                            '" class="fa fa-pencil btn btn-default btn-form" title="Cập nhật" data-toggle="tooltip"></a>' +
                            '<a data-modal="" data-width="1024" data-modal-id="ListDistricts" href="/Cate/Districts/DistrictByProvince/' +
                            data +
                            '" class="fa fa-list btn btn-default btn-form" title="Danh sách Quận/Huyện" data-toggle="tooltip"></a>';

                        _html +=
                            '<a data-modal="" data-modal-id="Delete" href="/Cate/Provinces/Delete/' +
                            data +
                            '" class="fa fa-trash-o btn btn-danger btn-form" title="Xóa" data-toggle="tooltip"></a>';

                    }


                    return _html;
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

function Street_OnDeleteSuccess(response, formId) {
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
    } else if ($(response).hasClass("modal-header")) {
        $("#modal_DeleteStreet" + " #modal-content").html(response);
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}