var _myEnterpriseUrls = {
    enterpriseInfoUrl: "/MyEnterprise/Info/",
    enterpriseTypeBusinessInfoUrl: "/MyEnterprise/TypeBusinessInfo/"
};

function OnLoadEnterpriseInfo(cbb, eleInfo, eleTypeBusinessInfo) {
    $("#loader").addClass("show pageload-loading").css("display", "inherit");
    var enterpriseId = $(cbb).val();
    $(eleInfo).empty();
    $(eleInfo).load(_myEnterpriseUrls.enterpriseInfoUrl + enterpriseId);
    $(eleTypeBusinessInfo).empty();
    $(eleTypeBusinessInfo).load(_myEnterpriseUrls.enterpriseTypeBusinessInfoUrl + enterpriseId);
}

function CateDoc_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    var fileId = response.fileId;
                    $('li[id="' + fileId + '"]').remove();
                    response.status = undefined;
                }
            });
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}

function OnChangeDistrict(cbbDistrict, cbbWard) {
    if ($("#DistrictName").length > 0) {
        $("#DistrictName").val($(cbbDistrict).children("option:selected").text());
    }
    $(cbbWard).empty();
    var selected = $(cbbDistrict).val();
    var url = "/MyEnterprise/WardViaDistrict?districtId=" + selected;
    $.ajax({
        type: "GET",
        url: url,
        dataType: "JSON",
        success: function(response) {
            $(cbbWard).append('<option value=""></option>');
            $.each(response.Wards,
                function(index, item) {
                    $(cbbWard).append('<option value="' + item.WardId + '">' + item.WardName + "</option>");
                });
            $(".chosen-select").trigger("chosen:updated");
        }
    });
}

function OnChangeProvince(cbbProvince, cbbDistrict) {
    if ($("#ProvinceName").length > 0) {
        $("#ProvinceName").val($(cbbProvince).children("option:selected").text());
    }

    $(cbbDistrict).empty();
    $(".chosen-select").trigger("chosen:updated");
    var selected = $(cbbProvince).val();
    var url = "/Cate/Enterprise/DistrictsViaProvince?provinceId=" + selected;
    $.ajax({
        type: "GET",
        url: url,
        dataType: "JSON",
        success: function(response) {
            $(cbbDistrict).append('<option value=""></option>');
            $.each(response.Districts,
                function(index, item) {
                    $(cbbDistrict).append('<option value="' + item.DistrictId + '">' + item.DistrictName + "</option>");
                });
        }
    });
}

//function initDownloadDoc() {
//    var btnDownloadDoc = $("a[name='DownloadRefDoc']");
//    if (typeof btnDownloadDoc != "undefined") {
//        btnDownloadDoc.on('click',
//            function () {
//                var url = $(this).data('href');
//                $.ajax({
//                    type: 'GET',
//                    url: url,
//                    success: function (response) {
//                        if (typeof response != "undefined" && response != null && response.status) {
//                            window.location = response.downloadPath;
//                        } else if (typeof response != "undefined" && response != null && !response.status) {
//                            eval(response.message);
//                        }
//                    }
//                });
//            });
//    }
//}