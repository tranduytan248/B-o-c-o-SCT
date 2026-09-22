var _NationalActionURLs = {
    National_GetData: "/Cate/Wards/Get"
};
var _tableNatinal;
$(document).ready(function() {
    initTableNational();
    $("#ListProvinceId").change(function() {
        try {
            var value = $(this).val();
            $("#ListDistrictId").empty();
            if (value != null && value != undefined)
                value = value.join("|");
            else
                value = "|";
            $.ajax({
                url: "/Cate/Wards/WardViaListDistrict",
                dataType: "JSON",
                contentType: "application/json; charset=utf-8",
                data: { provinceId: value }, // you could throw any javascript object you like here
                success: function(data) {
                    $("#ListDistrictId").append('<option value=""></option>');
                    $.each(data.Wards,
                        function(index, item) {
                            console.log(item);
                            $("#ListDistrictId").append('<option value="' +
                                item.DistrictId +
                                '">' +
                                item.DistrictName +
                                "</option>");
                        });
                    // process the results
                }
            });
        } catch (e) {
            console.log(e.message);
        }

    });
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
                },
                "DistrictIds": function() {
                    return $("#Search select#ListDistrictId").val() != null &&
                        $("#Search select#ListDistrictId").val().length > 0
                        ? $("#Search select#ListDistrictId").val()
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
                "data": "DistrictName",
                "defaultContent": ""
            },
            {
                "data": "WardCode",
                "defaultContent": ""
            },
            {
                "data": "WardName",
                "defaultContent": ""
            },
            {
                "data": "WardId",
                "style": "width:100px;",
                "orderable": false,
                "render": function(data, type, row, meta) {
                    var html = "";
                    if (type === "display") {

                        html += _renderButton(true,
                            "Edit",
                            "fa fa-pencil btn btn-default btn-form",
                            "/Cate/Wards/Edit/" + data,
                            "",
                            "Cập nhật");

                        html += _renderButton(true,
                            "Delete",
                            "fa fa-trash-o btn btn-danger btn-form",
                            "/Cate/Wards/Delete/" + data,
                            "",
                            "Xoá");
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