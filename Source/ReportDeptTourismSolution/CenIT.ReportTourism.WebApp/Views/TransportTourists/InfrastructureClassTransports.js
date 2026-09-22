var _TransportTouristsActionURLs = {
    TransportTourists_GetData: "/TransportTourists/GetClassTransports"
};

var _tableClassTransport;
$(document).ready(function() {
    initTableClassTransport();
});

function initTableClassTransport() {
    _tableClassTransport = $("#InfrastructureClassTransports").DataTable({
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
            "url": _TransportTouristsActionURLs.TransportTourists_GetData,
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
                "data": "TypeTransportName",
                "defaultContent": ""
            },
            {
                "data": "ClassTransportName",
                "defaultContent": ""
            },
            {
                "data": "TotalTransport",
                "defaultContent": ""
            },
            {
                "data": "InfrastructureClassTransportId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {
                        html += _renderButton(true,
                            "EditClassTransport",
                            "fa fa-pencil btn btn-default btn-form",
                            "/TransportTourists/EditClassTransport/" + data,
                            "",
                            "Cập nhật");
                        html += _renderButton(true,
                            "DeleteClassTransport",
                            "fa fa-trash-o btn btn-danger btn-form",
                            "/TransportTourists/DeleteClassTransport/" + data,
                            "",
                            "Xoá");
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
                    _tableClassTransport.ajax.reload(null, false);
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