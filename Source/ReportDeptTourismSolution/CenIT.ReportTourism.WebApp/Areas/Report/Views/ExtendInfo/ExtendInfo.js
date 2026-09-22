var _ExtendInfoActionURLs = {
    ExtendInfo_GetData: "/Report/ExtendInfo/Get"
};
var _tableExtendInfo;
$(document).ready(function() {
    initTableExtendInfo();
});

function initTableExtendInfo() {
    _tableExtendInfo = $("#DSExtendInfo").DataTable({
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
            "url": _ExtendInfoActionURLs.ExtendInfo_GetData,
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
                "data": "ForMonth",
                "defaultContent": "",
                "render": function(data, type, row, meta) {
                    if (type == "display") {
                        if (data != null) {
                            return moment(data).format("MM/YYYY");
                        } else {
                            return "";
                        }
                    }
                }
            },
            {
                "data": "TotalGuestViaShip",
                "defaultContent": "",
                "render": function(data, type, row, meta) {
                    if (type == "display") {
                        if (data != null) {
                            return data.toLocaleString() + " lượt";
                        } else {
                            return "";
                        }
                    }
                }
            },
            {
                "data": "Id",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {
                        html += _renderButton(true,
                            "EditExtendInfo",
                            "fa fa-pencil btn btn-default btn-form",
                            "/Report/ExtendInfo/Edit/" + data,
                            "",
                            "Cập nhật");
                    }

                    return html;
                }
            }
        ]
    });
}

function ExtendInfo_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tableExtendInfo.ajax.reload(null, false);
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