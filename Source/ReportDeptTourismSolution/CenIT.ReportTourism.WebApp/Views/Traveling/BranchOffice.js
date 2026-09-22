
var _TravelingActionURLs = {
    Traveling_GetDataBranchOffices: "/Traveling/GetBranchOffices"
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
            "url": _TravelingActionURLs.Traveling_GetDataBranchOffices,
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
                        //html += _renderButton(true,
                        //    "EditBranchOffice",
                        //    "fa fa-pencil btn btn-default btn-form",
                        //    "/Traveling/EditBranchOffice/" + data,
                        //    "",
                        //    "Cập nhật");
                        //html += _renderButton(true,
                        //    "DeleteBranchOffice",
                        //    "fa fa-trash-o btn btn-danger btn-form",
                        //    "/Traveling/DeleteBranchOffice/" + data,
                        //    "",
                        //    "Xoá");
                        html +=
                            '<a id="TravelingEditBranchOffice' +
                            data +
                            '" data-modal="" data-modal-id="EditBranchOffice" class="fa fa-pencil btn btn-default btn-form" data-toggle="tooltip" title="" href="/Traveling/EditBranchOffice/' +
                            data +
                            '" data-original-title="Cập nhật"></a>';
                        html += '<a id="TravelingDeleteBranchOffice' +
                            data +
                            '" data-modal="" data-modal-id="DeleteBranchOffice" class="fa fa-trash-o btn btn-danger btn-form" data-toggle="tooltip" title="" href="/Traveling/DeleteBranchOffice/' +
                            data +
                            '" data-original-title="Xoá"></a>';
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