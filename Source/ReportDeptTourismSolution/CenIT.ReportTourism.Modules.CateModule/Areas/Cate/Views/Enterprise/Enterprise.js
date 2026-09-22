var _EnterpriseActionURLs = {
    Enterprise_GetData: "/Cate/Enterprise/Get"
};
var _tableEnterprise;
$(document).ready(function() {
    initTableEnterprise();
});

function initTableEnterprise() {
    _tableEnterprise = $("#DSEnterprise").DataTable({
        "Responsive": true,
        "language": {
            "processing":
                "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>"
        },
        "columnDefs": [
            { targets: [0, 1, 2, 8, 13], visible: true },
            { targets: "_all", visible: false }
        ],
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _EnterpriseActionURLs.Enterprise_GetData,
            "type": "POST",
            "dataType": "JSON",
            "data": {
                "TypeBusinessIds": function() {
                    return $("#Search select#ListTypeBusinessId").val() != null &&
                        $("#Search select#ListTypeBusinessId").val().length > 0
                        ? $("#Search select#ListTypeBusinessId").val()
                        : "";
                },
                "WardIds": function() {
                    return $("#Search select#ListWardId").val() != null &&
                        $("#Search select#ListWardId").val().length > 0
                        ? $("#Search select#ListWardId").val()
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
                "data": "OwnerEnterpriseName",
                "defaultContent": ""
            },
            {
                "data": "BusinessName",
                "defaultContent": ""
            },
            {
                "data": "TaxCode",
                "defaultContent": ""
            },
            {
                "data": "BusinessAddress",
                "defaultContent": ""
            },
            {
                "data": "StreetName",
                "defaultContent": ""
            },
            {
                "data": "WardName",
                "defaultContent": ""
            },
            {
                "data": "ProvinceName",
                "defaultContent": ""
            },
            {
                "data": "TypeBusinessName",
                "defaultContent": ""
            },
            {
                "data": "LegalRepresentationName",
                "defaultContent": ""
            },
            {
                "data": "LegalRepresentationPhone",
                "defaultContent": ""
            },
            {
                "data": "LegalRepresentationEmail",
                "defaultContent": ""
            },
            {
                "data": "Website",
                "defaultContent": ""
            },
            {
                "data": "EnterpriseId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {
                        if (row.IsActive) {
                            if (row.TypeBusiness == 1) {
                                html += _renderButton(true,
                                    "EditEnterpriseAccommodation",
                                    "fa fa-home btn btn-default btn-form bg-navy",
                                    "/Cate/TypeAccommodation/Info?enterpriseId=" + data,
                                    "",
                                    "Thông tin Lĩnh vực Lưu trú",
                                    1024);
                            } else if (row.TypeBusiness == 2) {
                                html += _renderButton(true,
                                    "EditEnterpriseTraveling",
                                    "fa fa-plane btn btn-default btn-form bg-teal",
                                    "/Cate/TypeTraveling/Info?enterpriseId=" + data,
                                    "",
                                    "Thông tin Lĩnh vực Lữ hành",
                                    1024);
                            } else if (row.TypeBusiness == 3) {
                                html += _renderButton(true,
                                    "EditEnterpriseTransportTourists",
                                    "fa fa-bus btn btn-default btn-form bg-purple",
                                    "/Cate/TypeTransportTourists/Info?enterpriseId=" + data,
                                    "",
                                    "Thông tin Dịch vụ vận chuyển hành khách",
                                    1024);
                            } else if (row.TypeBusiness == 4) {
                                html += _renderButton(true,
                                    "EditEnterpriseTouristAttraction",
                                    "fa fa-map-pin btn btn-default btn-form bg-orange",
                                    "/Cate/TypeTouristAttraction/Info?enterpriseId=" + data,
                                    "",
                                    "Thông tin Lĩnh vực/Điểm du lịch",
                                    1024);
                            } else if (row.TypeBusiness == 5) {
                                html += _renderButton(true,
                                    "EditEnterpriseServicesForTourist",
                                    "fa fa-wpbeginner btn btn-default btn-form bg-maroon",
                                    "/Cate/TypeServicesForTourist/Info?enterpriseId=" + data,
                                    "",
                                    "Thông tin Dịch vụ phục vụ khách du lịch",
                                    1024);
                            }
                            html += _renderButton(true,
                                "EditEnterprise",
                                "fa fa-pencil btn btn-default btn-form",
                                "/Cate/Enterprise/Edit/" + data,
                                "",
                                "Cập nhật",
                                1024);
                            html += _renderButton(true,
                                "ChangeStatusEnterprise",
                                "fa fa-lock btn bg-gray btn-form",
                                "/Cate/Enterprise/ChangeStatus/" + data,
                                "",
                                "Ngưng hoạt động Doanh nghiệp");
                        } else {
                            html += _renderButton(true,
                                "ChangeStatusEnterprise",
                                "fa fa-unlock btn bg-primary btn-form",
                                "/Cate/Enterprise/ChangeStatus/" + data,
                                "",
                                "Kích hoạt lại Doanh nghiệp");
                        }

                        html += _renderButton(true,
                            "DeleteEnterprise",
                            "fa fa-trash-o btn btn-danger btn-form",
                            "/Cate/Enterprise/Delete/" + data,
                            "",
                            "Xoá");
                    }
                    return html;
                }
            }
        ]
    });
}

function Enterprise_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    _tableEnterprise.ajax.reload(null, false);
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

function CateDoc_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    //_tableUnionResolution.ajax.reload(null, false);
                    var fileId = response.fileId;
                    $('li[id="' + fileId + '"]').remove();
                    response.status = undefined;
                }
            });
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}

function OnChangeProvince(cbbProvince, cbbWard) {
    if ($("#ProvinceName").length > 0) {
        $("#ProvinceName").val($(cbbProvince).children("option:selected").text());
    }
    var wardId = $(cbbWard).val();

    $(cbbWard).empty();
    $(cbbWard).trigger("chosen:updated");
    var selected = $(cbbProvince).val();
    var url = "/Cate/Enterprise/WardViaProvince?provinceId=" + selected;
    $.ajax({
        type: "GET",
        url: url,
        dataType: "JSON",
        success: function(response) {
            $(cbbWard).append('<option value=""></option>');
            $.each(response.Wards,
                function(index, item) {
                    var selected = "";
                    if (wardId.length > 0 && wardId != "0") {
                        if (item.WardId == wardId)
                            selected = "selected";
                    }
                    $(cbbWard).append('<option value="' +
                        item.WardId +
                        '" ' +
                        selected +
                        ">" +
                        item.WardName +
                        "</option>");
                });
        }
    });
}
