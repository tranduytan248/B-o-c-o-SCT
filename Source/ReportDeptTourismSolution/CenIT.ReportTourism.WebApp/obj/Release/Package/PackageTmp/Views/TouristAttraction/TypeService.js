
var _TouristAttractionActionURLs = {
    TouristAttraction_GetDataTypeServices: "/TouristAttraction/GetTypeServices"
};
var _tableTypeService;
$(document).ready(function() {
    initTableTypeService();
});

function initTableTypeService() {
    _tableTypeService = $("#DataTypeServices").DataTable({
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
            "url": _TouristAttractionActionURLs.TouristAttraction_GetDataTypeServices,
            "type": "POST",
            "dataType": "JSON",
            "data": {
                "enterpriseId": function() { return enterpriseId; }
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
                "data": "ServiceName",
                "defaultContent": ""
            },
            {
                "data": "TouristAttractionTypeServiceId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {
                        html += _renderButton(true,
                            "EditTypeService",
                            "fa fa-pencil btn btn-default btn-form",
                            "/TouristAttraction/EditTypeService/" + data,
                            "",
                            "Cập nhật");
                        html += _renderButton(true,
                            "DeleteTypeService",
                            "fa fa-trash-o btn btn-danger btn-form",
                            "/TouristAttraction/DeleteTypeService/" + data,
                            "",
                            "Xoá");
                    }
                    return html;
                }
            }
        ]
    });
}

function TypeServices_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tableTypeService.ajax.reload(null, false);
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