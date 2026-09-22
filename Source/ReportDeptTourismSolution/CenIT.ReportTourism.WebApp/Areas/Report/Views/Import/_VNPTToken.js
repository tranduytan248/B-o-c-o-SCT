debugger;

var signInProcess = false;
var checkCRLorOCSP = false;

var _ParentEle = "#FormUpload ";//"#SignTokenArea ";
var _SerialKey = "";
//var _ButtonCheckPlugin = "_CheckPlugin";
//var _ButtonSign = "_Sign";
//var _OutputSignedBase64 = "#_OutputSignedBase64";
//var _Certificate_Serial = "#_Certificate_Serial";
//var _Certificate_Base64 = "#_Certificate_Base64";
var _InputFileName = "#FileName";
var _InputFileExt = "#FileExt";
var _InputFileBase = "#FileDataBase64";
var _InputFilePath = "#inputFile_FileImport input[type='text']";
var _ArrAllowFileExts = ["xlsx", "xls", "xml", "pdf", "docx", "pptx", "txt"];
var _ButtonChooseFile = "#inputFile_FileImport button.btn-choose";


function createElement(idEle) {
    var $curEle = $(_ParentEle + idEle);
    if ($curEle == null || $curEle.length <= 0) {
        $(_ParentEle).append('<textarea id="{0}" class="hidden"></textarea>'.format(idEle.replace("#", "")));
    }
}

function initSignElements() {
    //createElement(_OutputSignedBase64);
    //createElement(_Certificate_Serial);
    //createElement(_Certificate_Base64);
    createElement(_InputFileName);
    createElement(_InputFileBase);
    createElement(_InputFilePath);
}

function chooseFileCallback(data) {    
    $(_ButtonChooseFile).prop("disabled", false);
    if (data === "") {
        $(_ParentEle + "#AdvancedSignArea").addClass("hidden");
        return;
    }

    if ($(_ParentEle + '#loading').is(':hidden')) {
        $(_ParentEle + '#loading').removeClass("hidden");
    }

    var jsonObj = JSON.parse(data);
    if (jsonObj != null && jsonObj.code != null) {
        toastr.error(jsonObj.error);
        return;
    }

    $(_InputFileBase).val(jsonObj.base64);

    var arrFilePaths = jsonObj.path.split('\\');
    var fullName = arrFilePaths[arrFilePaths.length - 1];
    $(_InputFilePath).val(fullName);

    var fileSplit = fullName.split(".");
    var fileName = fileSplit[0];
    var fileExtension = fileSplit[fileSplit.length - 1].toLowerCase();

    $(_InputFileName).val(fullName);
    $(_InputFileExt).val(fileExtension);

    if (fileExtension === "pdf") {
        $(_ParentEle + "#AdvancedSignArea").removeClass("hidden");
    }
    else {
        $(_ParentEle + "#AdvancedSignArea").addClass("hidden");
    }

    var enterpriseId = $("#DataImport #EnterpriseId").val();
    var typeSignature = $("#DataImport #TypeSignature").val();

    $.ajax({
        method: "POST",
        url: "/Report/Import/CheckDataFileSignToken",
        data: {
            EnterpriseId: enterpriseId,
            TypeSignature: typeSignature,
            FileName: fileName,
            FileExt: fileExtension,
            FileDataBase64: jsonObj.base64
        },
        async: false
    }).done(function (result) {
        $(_ParentEle + '#loading').addClass("hidden");

        if (!result.status) {
            if (result.message.length > 0) {
                eval(result.message);
                $("#inputFile_FileImport").find('button.btn.btn-reset').click();
            }
            if (!$("#TableDataImport").hasClass("hidden")) {
                $("#TableDataImport").addClass("hidden");
            }
        }
        else {
            initTableDataImportsFromFile(result.dataImport, result.typeReport);
            $("#TableDataImport").removeClass("hidden");
        }
    });
}

// sign data
function signCallback(dataJson) {
    $(_ParentEle + '#loading').addClass("hidden");
    var jsObj = JSON.parse(JSON.parse(dataJson)[0]);
    if (jsObj.code != 0) {
        toastr.error(jsObj.code);
        return;
    }

    $(_InputFileBase).val(jsObj.data);
    $('#modal_ImportData button[type="submit"]').click();
}

function sign() {
    $(_ParentEle + '#loading').removeClass("hidden");
    $(_ParentEle + '#message').text('Đang ký dữ liệu...');

    if ($(_InputFileBase).val() === "" || $(_InputFileBase).val().length == 0) {
        $(_ParentEle + '#loading').addClass("hidden");
        toastr.error("Không có dữ liệu ký.");
        return;
    }
    if ($(_InputFileName).val() == null || $(_InputFileName).val().length == 0) {
        $(_ParentEle + '#loading').addClass("hidden");
        toastr.error("Chưa chọn tệp cần ký.");
        return;
    }

    var fileSplit = $(_InputFileName).val().split(".");
    var fileExtension = fileSplit[fileSplit.length - 1].toLowerCase();

    if (!_ArrAllowFileExts.includes(fileExtension)) {
        $(_ParentEle + '#loading').addClass("hidden");
        toastr.error("Định dạng dữ liệu chưa được hỗ trợ.");
        return;
    }
    var sigOptions = null;
    if (fileExtension === "pdf" && $("input[type='checkbox'][id='AdvancedSign']").is(":checked")) {
        sigOptions = new PdfSigner();
        sigOptions.AdvancedCustom = true;
    }
    SignAdvanced($(_InputFileBase).val(), fileExtension, sigOptions);
}

function SignAdvanced(data, type, sigOption) {
    var dataJS = {};

    var arrData = [];
    // 1			
    dataJS.data = data;
    dataJS.type = type;
    dataJS.sigOptions = JSON.stringify(sigOption);

    var jsData = "";
    jsData += JSON.stringify(dataJS);
    //
    arrData.push(jsData);
    var serial = "";

    vnpt_plugin.signArrDataAdvanced(arrData, serial, true, signCallback);
}

function setLicenseCallback(data) {
    var jsObj = JSON.parse(data);
    if (jsObj.code < 0) {
        toastr.error("License không tồn tại. Vui lòng kiểm tra lại.");
        $(_ButtonChooseFile).prop("disabled", true);
        $(_ParentEle + '#message').text('License không tồn tại. Vui lòng kiểm tra lại.');

        return;
    }

    $(_ButtonChooseFile).prop("disabled", true);
    $(_ParentEle + '#loading').addClass("hidden");
    $(_ParentEle + '#message').text('');

    vnpt_plugin.chooseFile(chooseFileCallback);
}

function setLicense(functionCallback) {
    $(_ParentEle + '#message').text('Đang kiểm tra License...');
    vnpt_plugin.setLicenseKey(_SerialKey, functionCallback);
}

function checkPluginCallback(data) {
    if (data === "1") {
        //initSignElements();
        setLicense(setLicenseCallback);
    }
    else {
        //$(_ParentEle + '#loading').addClass("hidden");
        toastr.error("VNPT-CA Plugin chưa được cài đặt hoặc chưa được bật");
        $(_ParentEle + '#message').html('VNPT-CA Plugin chưa được cài đặt hoặc chưa được bật. <a href="/Contents/Modules/Report/plugins/vnpt-ca/VNPT_Products_Plugin_ky_so_1.0.2.2.exe">Tải về VNPT-CA Plugin</a>');
    }
}

function checkPlugin(functionCallback) {
    _SerialKey = $("input[type='hidden'][id='VNPTTokenSerialKey']").val();
    $(_ParentEle + '#loading').removeClass("hidden");
    $(_ParentEle + '#message').text('Đang kiểm tra cài đặt Vnpt-CA Plugin...');
    vnpt_plugin.checkPlugin(functionCallback);
}

