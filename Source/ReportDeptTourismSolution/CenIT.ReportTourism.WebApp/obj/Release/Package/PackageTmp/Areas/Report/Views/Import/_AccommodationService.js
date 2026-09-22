
var maximumNationality = 10;
var seletedNationalCode = "";
var seletedNationalName = "";
var arrSeletedNationals = [];

function ValidForm() {
    var isValid =
        ValidMinValue("#PerformPreviousPeriod", "11;12;13;14", 1, true) &
            ValidMinValue("#PerformInPeriod", "11;12;13;14", 1, true) &
            ValidMinValue("#CompareSamePeriodLastYear", "11;12;13;14", 1, true) &
            ValidTotalWithChild("#PerformPreviousPeriod", "5", "6;7") &
            ValidTotalWithChild("#PerformPreviousPeriod", "8", "9;10") &
            ValidTotalWithChild("#PerformPreviousPeriod", "11", "12;13;14") &
            ValidTotalWithChild("#PerformPreviousPeriod", "15", "16;17;18;19;20") &
            ValidTotalWithChild("#PerformInPeriod", "5", "6;7") &
            ValidTotalWithChild("#PerformInPeriod", "8", "9;10") &
            ValidTotalWithChild("#PerformInPeriod", "11", "12;13;14") &
            ValidTotalWithChild("#PerformInPeriod", "15", "16;17;18;19;20") &
            ValidTotalWithChild("#CompareSamePeriodLastYear", "5", "6;7") &
            ValidTotalWithChild("#CompareSamePeriodLastYear", "8", "9;10") &
            ValidTotalWithChild("#CompareSamePeriodLastYear", "11", "12;13;14") &
            ValidTotalWithChild("#CompareSamePeriodLastYear", "15", "16;17;18;19;20");
    return isValid;
}

Array.prototype.remove = function() {
    var what, a = arguments, L = a.length, ax;
    while (L && this.length) {
        what = a[--L];
        while ((ax = this.indexOf(what)) !== -1) {
            this.splice(ax, 1);
        }
    }
    return this;
};

function deleteRow(rowId) {
    var nationName = $("table#DataReport tr[id='" + rowId + "']").children("td:eq(0)").children("input[type='hidden']")
        .val();

    $.confirm({
        closeIcon: true,
        //theme: "bootstrap",
        boxWidth: "100%",
        closeIconClass: "fa fa-close",
        type: "red",
        icon: "fa fa-info-circle",
        title: "Xác nhận!",
        content: '<h4>Bạn muốn xoá dữ liệu <b class="text-danger">Quốc tịch [' + nationName + "]</b>?</h4>",
        autoClose: "cancel|5000",
        buttons: {
            confirm: {
                text: "[Y] Xác nhận",
                keys: ["Y"],
                action: function() {
                    $("table#DataReport tr[id='" + rowId + "']").remove();
                    arrSeletedNationals.remove(rowId);
                    maximumNationality = maximumNationality + 1;
                    ShowAddNations();
                }
            },
            cancel: {
                text: "[C] Huỷ",
                keys: ["C"],
                action: function() {

                }
            }
        }
    });
}

$("#ModalContent #modal_AddNational").on("shown.bs.modal",
    function() {
        $("select#NationalCode").select2({
            language: "vi",
            allowClear: true,
            placeholder: "Chọn 1 giá trị",
            maximumSelectionLength: maximumNationality
        });
        if (maximumNationality <= 0) {
            $("select#NationalCode").prop("disabled", true);
            toastr.error("Đã thêm tối đa 10 quốc tịch vào báo cáo.");
            $("#WarningNation").removeClass("hidden");
        }
    });

function addNational() {
    var nationalCode = $("select#NationalCode").val();
    $.each(nationalCode,
        function(idx, code) {
            if (maximumNationality == 0) {
                toastr.error("Đã thêm tối đa 10 quốc tịch vào báo cáo.");
                $("#WarningNation").removeClass("hidden");
                return;
            }
            var nationalName = $('select#NationalCode option[value="' + code + '"]').text();
            seletedNationalCode = code;
            seletedNationalName = nationalName;

            if (arrSeletedNationals.includes(seletedNationalCode)) {
                toastr.error(String.format("Quốc tịch [{0}] đã tồn tại trong báo cáo.", seletedNationalName));
            } else {
                arrSeletedNationals.push(seletedNationalCode);
                /*
                 * 0: Lãnh đạo quản lý - Title
                 * 1: Người - Unit
                 * 2: 16 - Index
                 * 3: 15 - Code
                 */
                var trTemplate =
                    '<tr id="{3}"><td class="national"><input id="Target_{2}"name="Target_{2}"type="hidden"value="{0}">{0}<div class="tools"><button onclick="deleteRow(\'{3}\')" class="no-border" type="button"><i class="fa fa-trash-o"></i></button></div></td><td><input id="Unit_{2}"name="Unit_{2}"type="hidden"value="{1}">{1}</td><td><input id="Code_{2}"name="Code_{2}"type="hidden"value="{3}">{3}</td><td><input class="form-control text-right"id="PerformPreviousPeriod_{2}"name="PerformPreviousPeriod_{2}"placeholder="0" type="number" min="0"></td><td><input class="form-control text-right"id="PerformInPeriod_{2}"name="PerformInPeriod_{2}"placeholder="0" type="number" min="0"></td><td><input class="form-control text-right"id="CompareSamePeriodLastYear_{2}"name="CompareSamePeriodLastYear_{2}"placeholder="0"type="number"min="0"></td></tr>';
                var newRow = String.format(trTemplate,
                    seletedNationalName,
                    "Người",
                    (22 + (10 - maximumNationality)),
                    seletedNationalCode);
                $("table#DataReport tbody").append(newRow);
                maximumNationality = maximumNationality - 1;
                toastr.success(String.format("Thêm Quốc tịch [{0}] vào báo cáo thành công.", seletedNationalName));
            }
        });
    $("#ModalContent #modal_AddNational").modal("hide");
    if (maximumNationality <= 0) {
        $("td > a#AddNational").remove();
    }
}

$(document).ready(function() {
    InitNationTool();
});

function ShowAddNations() {
    var btnAddNations =
        '<a id="AddNational" class="btn btn-sm btn-primary pull-right" data-modal="true" data-modal-id="AddNational" href="/Report/Import/AddNational" style="margin: unset;"><i class="fa fa-plus"></i>&nbsp;Thêm</a>';
    var tdColspan = $("input#Target_21").parent("td");
    if (maximumNationality != 0) {
        var tr = $(tdColspan).parent("tr");
        var colspan = $(tdColspan).attr("colspan");
        if (colspan == 6) {
            $(tr).append("<td>" + btnAddNations + "</td>");
            $(tdColspan).attr("colspan", 5);
        }
    }
}

function InitNationTool() {
    var tdColspan = $("input#Target_21").parent("td");
    var nationalTools =
        '<div class="tools"><button onclick="deleteRow(\'{0}\')" class="no-border" type="button"><i class="fa fa-trash-o"></i></button></div>';

    var allNextTr = $(tdColspan).parent("tr").nextAll();
    $.each(allNextTr,
        function(idx, tr) {
            var tdCode = $(tr).children("td:eq(2)");
            if (typeof tdCode != "undefined") {
                var inputCode = $(tdCode).children('input[type = "hidden"]');
                if (typeof inputCode != "undefined") {
                    var nationCode = $(inputCode).val();
                    if (typeof nationCode != "undefined" && nationCode.length > 0) {
                        $(tr).attr("id", nationCode);
                    }
                    var firstCol = $(tr).children("td:eq(0)");
                    if (typeof firstCol != "undefined") {
                        firstCol.addClass("national");
                        firstCol.append(String.format(nationalTools, nationCode));
                        maximumNationality = maximumNationality - 1;
                    }
                }
            }
        });
    ShowAddNations();
}