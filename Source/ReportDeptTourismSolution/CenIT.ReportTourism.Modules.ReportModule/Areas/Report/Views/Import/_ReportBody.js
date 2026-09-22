
function TooltipErrInput(ele, msg) {
    $(ele).parent().addClass("has-error");
    $(ele).attr("title", msg);
    $(ele).data("bs.tooltip", false).tooltip();
}

function ClearTooltipErrInput(ele) {
    $(ele).parent().removeClass("has-error");
    $(ele).attr("title", "");
    //$(ele).tooltip('destroy');
    //$(ele).data('bs.tooltip', false).tooltip('dispose');
}

function ValidMinNumber(eles) {
    $(eles).each(function(idx, ele) {
        if ($(ele).val() < 0) {
            TooltipErrInput(ele, "Giá trị phải lớn hơn hoặc bằng 0");
        }
    });
}

if (!String.prototype.format) {
    String.prototype.format = function() {
        var args = arguments;
        return this.replace(/{(\d+)}/g,
            function(match, number) {
                return typeof args[number] != "undefined"
                    ? args[number]
                    : match;
            });
    };
}

function GetTargetValue(idxEle) {
    //var idxEle = $(eleVal).id.split('_')[1];
    var targetVal = $("input[type='hidden'][id='Target_" + idxEle + "'").val();
    return targetVal;
}

function ValidTotalWithChild(eleName, total, lstChilds) {

    var totalEle = String.format("{0}_{1}", eleName, total);
    var targetTotal = GetTargetValue(total);

    var totalVal = $(totalEle).val();
    var totalChild = 0;
    var targetChilds = "";
    var arrTargetChild = [];

    ClearTooltipErrInput(totalEle);

    $.each(lstChilds.split(";"),
        function(idx, child) {
            var childEle = String.format("{0}_{1}", eleName, child);

            var targetValChild = GetTargetValue(child);
            arrTargetChild.push(targetValChild);
            var childVal = $(childEle).val();
            totalChild += childVal.length > 0 ? parseFloat(childVal) : 0;
        });
    targetChilds = arrTargetChild.join(", ");
    if (totalChild > 0 && (totalVal.length == 0 || totalVal <= 0 || totalVal != totalChild)) {
        TooltipErrInput(totalEle,
            String.format("[{0}] phải bằng tổng [{1}] (Giá trị đúng: [{2}])", targetTotal, targetChilds, totalChild));
        return false;
    }
    return true;
}

function ValidMinValue(eleName, lstEleIds, minValue, allowEmpty) {
    var isValid = true;

    $.each(lstEleIds.split(";"),
        function(idx, eleId) {
            var eleValid = String.format("{0}_{1}", eleName, eleId);
            var eleTitle = GetTargetValue(eleId);

            ClearTooltipErrInput(eleValid);

            var currentVal = $(eleValid).val();
            var eleIsValid = true;
            if (!allowEmpty && (typeof currentVal == "undefined" || currentVal.length <= 0 || currentVal == 0)) {
                eleIsValid = false;
            } else if (parseFloat(currentVal) > 0 && parseFloat(currentVal) < minValue) {
                eleIsValid = false;
            }
            if (!eleIsValid) {
                TooltipErrInput(eleValid,
                    String.format("Giá trị [{0}] phải lớn hơn hoặc bằng {1}", eleTitle, minValue));
                isValid = isValid & eleIsValid;
            }
        });

    return isValid;
}