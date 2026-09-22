function SysLog_OnProcessSuccess(response, formID) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formID).modal("hide");
        $("#ModalContent #modal_" + formID).on("hidden.bs.modal",
            function () {
                if (response.status != undefined) {
                    eval(response.message);
                    if (response.fileName != undefined) {
                        $("li#" + response.fileName).remove();
                    } else {
                        window.location.reload();
                    }
                    response.status = undefined;
                }
            });
    } else {
        $("#ModalContent #modal_" + formID + " #bodyForm").html(response);
    }
}

function OnSearchFile() {
    var fromMonth = $("#SearchFile #FromMonth").val();
    var toMonth = $("#SearchFile #ToMonth").val();
    $("#tab_ErrLog .box-body").load("/SysLog/ListErrFile?fromMonth=" + moment(fromMonth, "MM/YYYY").format("YYYY-MM-DD") + "&toMonth=" + moment(toMonth, "MM/YYYY").format("YYYY-MM-DD"));
}