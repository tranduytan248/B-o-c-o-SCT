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
    //$(eleTypeBusinessInfo).load(_myEnterpriseUrls.enterpriseTypeBusinessInfoUrl + enterpriseId);
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

function OnChangeProvince(cbbProvince, cbbWard) {
    if ($("#ProvinceName").length > 0) {
        $("#ProvinceName").val($(cbbProvince).children("option:selected").text());
    }

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
                    $(cbbWard).append('<option value="' + item.WardId + '">' + item.WardName + "</option>");
                });
        }
    });
}
