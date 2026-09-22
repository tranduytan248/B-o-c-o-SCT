var _AccommodationActionURLs = {
    Accommodation_GetData: "/Accommodation/GetRoomTypes"
};
var _tableRoomType;
$(document).ready(function() {
    initTableRoomType();
});

function initTableRoomType() {
    _tableRoomType = $("#InfrastructureRooms").DataTable({
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
            "url": _AccommodationActionURLs.Accommodation_GetData,
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
                "data": "RoomTypeName",
                "defaultContent": ""
            },
            {
                "data": "TotalRooms",
                "defaultContent": ""
            },
            {
                "data": "AnnouncedPrice",
                "defaultContent": "",
                "className": "text-right",
                "render": function(data, type, row, meta) {
                    if (type === "display") {
                        var html = data.toLocaleString("he-IL", { style: "currency", currency: "VND" });
                        return html;
                    } else if (type === "sort") {
                        return data;
                    }
                    return data;
                }
            },
            {
                "data": "InfrastructureRoomId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {
                        //html += _renderButton(true,
                        //    "EditRoomType",
                        //    "fa fa-pencil btn btn-default btn-form",
                        //    "/Accommodation/EditRoomType/" + data,
                        //    "",
                        //    "Cập nhật");
                        //html += _renderButton(true,
                        //    "DeleteRoomType",
                        //    "fa fa-trash-o btn btn-danger btn-form",
                        //    "/Accommodation/DeleteRoomType/" + data,
                        //    "",
                        //    "Xoá");
                        html += '<a id="AccommodationEditRoomType' +
                            data +
                            '" data-modal="" data-modal-id="EditRoomType" class="fa fa-pencil btn btn-default btn-form" data-toggle="tooltip" title="" href="/Accommodation/EditRoomType/' +
                            data +
                            '" data-original-title="Cập nhật"></a>';
                        html += '<a id="AccommodationDeleteRoomType' +
                            data +
                            '" data-modal="" data-modal-id="DeleteRoomType" class="fa fa-trash-o btn btn-danger btn-form" data-toggle="tooltip" title="Xoá" href="/Accommodation/DeleteRoomType/' +
                            data +
                            '"></a>';
                    }
                    return html;
                }
            }
        ]
    });
}

function InfrastructureRooms_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tableRoomType.ajax.reload(null, false);
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