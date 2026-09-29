var _myEnterpriseUrls = {
    enterpriseInfoUrl: "/MyEnterprise/Info/",
    enterpriseTypeBusinessInfoUrl: "/MyEnterprise/TypeBusinessInfo/",
    mainProductViaIndustriesUrl: "/MyEnterprise/MainProductViaIndustries"
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

// Nạp lại dropdown "Sản phẩm chính" theo các ngành công nghiệp đang chọn
function OnChangeIndustries(cbbIndustry, cbbProduct) {
    var industryIds = $.grep($(cbbIndustry).val() || [], function(id) { return id; }).join(",");

    // Đổi ngành liên tục: bỏ request cũ, chỉ lấy kết quả của lần chọn cuối
    var request = $(cbbProduct).data("request");
    if (request) request.abort();

    if (industryIds.length === 0) {
        FillMainProducts(cbbProduct, []);
        return;
    }

    request = $.ajax({
        type: "GET",
        url: _myEnterpriseUrls.mainProductViaIndustriesUrl,
        data: { industryIds: industryIds },
        dataType: "JSON",
        success: function(response) {
            FillMainProducts(cbbProduct, response.Products);
        },
        complete: function() {
            if ($(cbbProduct).data("request") === request) $(cbbProduct).removeData("request");
        }
    });
    $(cbbProduct).data("request", request);
}

// Giữ lại sản phẩm đang chọn nếu vẫn thuộc danh sách mới, ngược lại bỏ chọn
function FillMainProducts(cbbProduct, products) {
    var productId = $(cbbProduct).val();

    $(cbbProduct).empty().append('<option value=""></option>');
    $.each(products || [],
        function(index, item) {
            $(cbbProduct).append($("<option></option>").val(item.Value).text(item.Text));
        });

    var hasProduct = !!productId && $(cbbProduct).find("option").filter(function() {
        return this.value === productId;
    }).length > 0;
    $(cbbProduct).val(hasProduct ? productId : "").trigger("change");
}
