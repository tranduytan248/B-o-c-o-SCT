
var _TypeTravelingActionURLs = {
    TypeTraveling_GetDataBranchOffices: "/Cate/TypeTraveling/GetBranchOffices"
};
var _tableBranchOffice;
$(document).ready(function() {
    initTableBranchOffice();
});

function initTableBranchOffice() {
    _tableBranchOffice = $("#DataBranchOffices").DataTable({
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
            "url": _TypeTravelingActionURLs.TypeTraveling_GetDataBranchOffices,
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
                "data": "BranchName",
                "defaultContent": ""
            },
            {
                "data": "BranchAddress",
                "defaultContent": ""
            },
            {
                "data": "BranchOfficeId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {
                        html += _renderButton(true,
                            "EditBranchOffice",
                            "fa fa-pencil btn btn-default btn-form",
                            "/Cate/TypeTraveling/EditBranchOffice/" + data,
                            "",
                            "Cập nhật");
                        html += _renderButton(true,
                            "DeleteBranchOffice",
                            "fa fa-trash-o btn btn-danger btn-form",
                            "/Cate/TypeTraveling/DeleteBranchOffice/" + data,
                            "",
                            "Xoá");
                    }
                    return html;
                }
            }
        ]
    });
}

function BranchOffices_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tableBranchOffice.ajax.reload(null, false);
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