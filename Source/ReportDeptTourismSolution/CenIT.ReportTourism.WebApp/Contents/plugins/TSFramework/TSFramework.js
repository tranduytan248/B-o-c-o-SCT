$.notifyDefaults({
    element: "body",
    position: null,
    type: "info",
    allow_dismiss: true,
    newest_on_top: true,
    showProgressbar: false,
    placement: {
        from: "top",
        align: "center"
    },
    offset: 20,
    spacing: 10,
    z_index: 9999,
    delay: 2000,
    timer: 1000,
    url_target: "_blank",
    mouse_over: null,
    animate: {
        enter: "animated fadeInDown",
        exit: "animated fadeOutUp"
    },
    onShow: null,
    onShown: null,
    onClose: null,
    onClosed: null,
    icon_type: "class",
    template: '<div data-notify="container" class="notify-width col-xs-11 col-sm-5 alert alert-{0}" role="alert">' +
        '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
        '<span data-notify="icon"></span> ' +
        '<span data-notify="title"><b>[{1}]</b></span> ' +
        '<span data-notify="message">{2}</span>' +
        '<div class="progress" data-notify="progressbar">' +
        '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
        "</div>" +
        '<a href="{3}" target="{4}" data-notify="url"></a>' +
        "</div>"
});
moment.locale("vi");

$.fn.datepicker.dates["vi"] = {
    days: ["Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy", "Chủ nhật"],
    daysShort: ["CN", "T2", "T3", "T4", "T5", "T6", "T7", "CN"],
    daysMin: ["CN", "T2", "T3", "T4", "T5", "T6", "T7", "CN"],
    months: [
        "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10",
        "Tháng 11", "Tháng 12"
    ],
    monthsShort: ["Th1", "Th2", "Th3", "Th4", "Th5", "Th6", "Th7", "Th8", "Th9", "Th10", "Th11", "Th12"],
    today: "Hôm nay"
};

$.fn.datepicker.defaults.language = "vi";
$.fn.datepicker.defaults.weekStart = 1;

$.extend(true,
    $.fn.dataTable.defaults,
    {
        language: {
            "lengthMenu": "<span >Hiển thị</span> <b>_MENU_</b> <span >dòng</span>",
            "zeroRecords": "<span >Không tìm thấy</span>",
            "emptyTable": "<span >Không có dữ liệu</span>",
            //"info": "<span >Đang hiển thị trang</span> _PAGE_ <span >trên</span> _PAGES_",
            "info":
                "<span >Hiển thị từ </span> <b>_START_</b> <span> đến </span> <b>_END_</b> <span >trên tổng số </span> <b>_TOTAL_</b> dữ liệu",
            "infoEmpty": "<span >Không có dữ liệu</span>",
            "infoFiltered": "(<span >Lọc từ</span> <b>_MAX_</b> <span >tổng dòng</span>)",
            "search": "<span >Tìm kiếm:</span>",
            "paginate": {
                "first": "<span >Trang đầu</span>",
                "last": "<span >Trang cuối</span>",
                "next": "<span >Tiếp</span>",
                "previous": "<span >Trước</span>"
            },
            "aria": {
                "sortAscending": ": <span >Sắp xếp tăng dần</span>",
                "sortDescending": ": <span >Sắp xếp giảm dần</span>"
            }
        }
    });

$.extend($.validator.messages,
    {
        required: "Hãy nhập.",
        remote: "Hãy sửa cho đúng.",
        email: "Hãy nhập email.",
        url: "Hãy nhập URL.",
        date: "Hãy nhập ngày.",
        dateISO: "Hãy nhập ngày (ISO).",
        number: "Hãy nhập số.",
        digits: "Hãy nhập chữ số.",
        creditcard: "Hãy nhập số thẻ tín dụng.",
        equalTo: "Hãy nhập thêm lần nữa.",
        extension: "Phần mở rộng không đúng.",
        maxlength: $.validator.format("Hãy nhập từ {0} kí tự trở xuống."),
        minlength: $.validator.format("Hãy nhập từ {0} kí tự trở lên."),
        rangelength: $.validator.format("Hãy nhập từ {0} đến {1} kí tự."),
        range: $.validator.format("Hãy nhập từ {0} đến {1}."),
        max: $.validator.format("Hãy nhập từ {0} trở xuống."),
        min: $.validator.format("Hãy nhập từ {0} trở lên.")
    });

toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

$.validator.addMethod("date",
    function (value, element) {
        var bits = value.match(/([0-9]+)/gi), str;
        if (bits === null) return true;
        if (bits.length === 2) {
            str = bits[0] + "/" + "01" + "/" + bits[1];
            return this.optional(element) || !/Invalid|NaN/.test(new Date(str));
        } else if (bits.length === 3) {
            if (!bits) {
                return this.optional(element) || false;
            }
            str = bits[1] + "/" + bits[0] + "/" + bits[2];
            return this.optional(element) || !/Invalid|NaN/.test(new Date(str));
        }
        return this.optional(element) || !/Invalid|NaN/.test(new Date(str));
    },
    "Nhập ngày theo định dạng [dd/mm/yyyy]");

$.fn.select2.defaults.set("amdBase", "select2/");
$.fn.select2.defaults.set("amdLanguageBase", "select2/i18n/");
//$.fn.select2.defaults.set("theme", "bootstrap");
$.fn.select2.defaults.set("width", "resolve");
$.fn.select2.defaults.set("language", "vi");

if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g,
            function (match, number) {
                return typeof args[number] != "undefined"
                    ? args[number]
                    : match;
            });
    };
}

$(function () {

    var htmlLoader =
        '<div style="display: none" id="loader" class="pageload-overlay" data-opening="M20,15 50,30 50,30 30,30 Z;M0,0 80,0 50,30 20,45 Z;M0,0 80,0 60,45 0,60 Z;M0,0 80,0 80,60 0,60 Z" data-closing="M0,0 80,0 60,45 0,60 Z;M0,0 80,0 50,30 20,45 Z;M20,15 50,30 50,30 30,30 Z;M30,30 50,30 50,30 30,30 Z">' +
            '<svg xmlns="http://www.w3.org/2000/svg" width="100%" height="100%" viewBox="0 0 80 60" preserveAspectRatio="none">' +
            '<path d="M30,30 50,30 50,30 30,30 Z"></path>' +
            "<desc>Created with Snap</desc>" +
            "<defs></defs>" +
            "</svg>" +
            "</div>";

    $("body").append('<div id="ModalContent"></div>');
    $("body").append(htmlLoader);
    $("body").append('<div id="ScrollTop"></div>');

    $(document, "table", "form").on("click",
        "[data-modal]",
        function () {
            var idModal = $(this).attr("data-modal-id");
            var idParentModal = $(this).attr("data-modal-id-parent");
            if ($("#modal_" + idModal).length > 0) {
                $("#modal_" + idModal).remove();
            }
            var modalClass = "";
            var modalDialog = "";

            var width = $(this).attr("data-width");
            if (typeof width == "undefined") width = $(this).attr("data_width");
            if (width == "full") {
                modalClass = "modal-content-full";
                modalDialog = "modal-dialog-full";
            }
            else if (width != undefined) {
                width = width.replace("px", "");
                if (parseInt(width) >= 1024) {
                    modalClass = modalDialog = "modal-xlg";
                } else if (parseInt(width) < 1024 && parseInt(width) >= 800) {
                    modalClass = modalDialog = "modal-lg";
                }
            }

            var templateModal = '<div class="modal modal-xs fade" parent-id="' +
                idParentModal +
                '" id="modal_' +
                idModal +
                '" role="dialog" data-backdrop="static" data-keyboard="false">' +
                '<div class="modal-dialog ' +
                modalDialog +
                '">' +
                "<!-- Modal content-->" +
                '<div class="modal-content ' +
                modalClass +
                '" id="modal-content">' +
                "</div>" +
                "</div>" +
                "</div>";
            $("#ModalContent").append(templateModal);
            $("#loader").addClass("show pageload-loading").css("display", "inherit");
            $("#modal_" + idModal + " #modal-content").load(this.href,
                function (data, textStatus, xhr) {
                    var resJSON = xhr.getResponseHeader("x-responded-json");
                    var status = 0;
                    if (resJSON != null)
                        status = JSON.parse(xhr.getResponseHeader("x-responded-json")).status;

                    if (status === 401) {
                        window.location.href = window.location.origin + "/Account/Login";
                    } else if (status === 404) {
                        window.location.href = window.location.origin + "/Error/NotFound";
                    } else if (status === 500) {
                        window.location.href = window.location.origin + "/Error/Error";
                    } else if (status === 405) {
                        window.location.href = window.location.origin + "/Error/AccessDenied";
                    } else {
                        if (_isJson(data)) {
                            var response = JSON.parse(data);
                            if (response.status != undefined) {
                                eval(response.message);
                                $("#loader").removeClass("show pageload-loading").css("display", "none");
                                $(this).remove();
                            }
                        } else {
                            $("#loader").removeClass("show pageload-loading").css("display", "none");
                            $("#modal_" + idModal).modal("show");
                        }
                    }
                });
            return false;
        });

    $(document, "table", "form").on("click",
        "[data-in-page]",
        function () {
            var formId = $(this).attr("data-form-id");
            if (formId != undefined) {
                $("#loader").addClass("show pageload-loading").css("display", "inherit");
                $("#" + formId + " #bodyForm").load(this.href,
                    function (data, textStatus, xhr) {
                        var resJSON = xhr.getResponseHeader("x-responded-json");
                        var status = 0;
                        if (resJSON != null)
                            status = JSON.parse(xhr.getResponseHeader("x-responded-json")).status;

                        if (status === 401) {
                            window.location.href = window.location.origin + "/Account/Login";
                        } else if (status === 404) {
                            window.location.href = window.location.origin + "/Error/NotFound";
                        } else if (status === 500) {
                            window.location.href = window.location.origin + "/Error/Error";
                        } else if (status === 405) {
                            window.location.href = window.location.origin + "/Error/AccessDenied";
                        } else {
                            if (_isJson(data)) {
                                var response = JSON.parse(data);
                                if (response.status != undefined) {
                                    eval(response.message);
                                    $("#loader").removeClass("show pageload-loading").css("display", "none");
                                    $(this).remove();
                                }
                            } else {
                                $("#loader").removeClass("show pageload-loading").css("display", "none");
                                $("#modal_" + idModal).modal("show");
                            }
                        }
                    });
                //function () {
                //    $("#loader").removeClass("show pageload-loading").css("display", "none");
                //});
            }
            return false;
        });

    $(document).on({
        'show.bs.modal': function () {
            var zIndex = 1040 + (10 * $(".modal:visible").length);
            $(this).css("z-index", zIndex);
            setTimeout(function () {
                $(".modal-backdrop").not(".modal-stack").css("z-index", zIndex - 1).addClass("modal-stack");
            },
                0);
        },
        'shown.bs.modal': function () {
            $(this).find("select").each(function () {
                var zIndex = 1040 + (10 * $(".modal:visible").length);
                setTimeout(function () {
                    preInitDatatable();
                    $(".modal-backdrop").not(".modal-stack").css("z-index", zIndex - 1).addClass("modal-stack");
                },
                    0);

                var dropdownParent = $(document.body);
                if ($(this).parents(".modal.in:first").length !== 0)
                    dropdownParent = $(this).parents(".modal.in:first");
                if (this.name.indexOf("_length") < 0) {
                    if (!$(this).hasClass("none-select2")) {
                        $(this).select2({
                            allowClear: true,
                            dropdownParent: dropdownParent,
                            placeholder: "Chọn 1 giá trị",
                            width: "element"
                        });
                    }
                } else {
                    if (!$(this).hasClass("none-select2")) {
                        $(this).select2({
                            allowClear: false,
                            dropdownParent: dropdownParent,
                            placeholder: "Chọn 1 giá trị",
                            width: "element"
                        });
                    }
                    $('select[name*="_length"] + span.select2').css("width", "65px");
                }
            });
        },
        'hidden.bs.modal': function () {
            if ($(".modal:visible").length > 0) {
                // restore the modal-open class to the body element, so that scrolling works
                // properly after de-stacking a modal.
                setTimeout(function () {
                    $(document.body).addClass("modal-open");
                },
                    0);
            }
            if ($(this).parent().is("#ModalContent")) {
                $(this).remove();
            }
        }
    },
        ".modal");

    $.AdminLTE.pushMenu.expandOnHover();
    _initDatePicker();
    _renderScrollTop();
    _initMenuView();
});

$(document).ready(function () {
    _initElement();
    _initColumnDataTable();
    _initValidateForm();
    _initMenuView();
    //$("input.input-validation-error").focus(function () {
    //    $(this).removeClass("input-validation-error");
    //    $(this).parents('div.form-group').find("span.field-validation-error").html("");
    //});
});

$(document).ajaxStart(function () {
    if ($("#loader").css("display") === "none") {
        $("#loader").addClass("show pageload-loading").css("display", "inherit");
    }
});

$(document).ajaxComplete(function (event, xhr, settings) {
    _initElement();
    _initColumnDataTable();
    _initDatePicker();
    _initValidateForm();

    if ($("#loader").css("display") === "block") {
        $("#loader").removeClass("show pageload-loading").css("display", "none");
    }

    //$(".input-validation-error").focus(function () {
    //    $(this).removeClass("input-validation-error");
    //    $(this).parents('div.form-group').find("span.field-validation-error").html("");
    //});
});

$(window).on("load",
    function () {
        preInitDatatable();
        _initColumnDataTable();
    });

window.onerror = function (message, source, lineno, colno, error) {
    if ($("#loader").css("display") === "block") {
        $("#loader").removeClass("show pageload-loading").css("display", "none");
    }

    toastr.error(message);
    //toastr.error("Đã có lỗi xảy ra. Vui lòng kiểm tra lại.");
};

function showNotify(title, icon, message, url, target, type) {
    $.notify({
        title: title,
        icon: icon,
        message: message,
        url: url === null || typeof url === "undefined" || url.length === 0 ? "#" : url,
        target: url === null || typeof target === "undefined" || target.length === 0 ? "_blank" : target
    },
        {
            type: type
        });
}

function showModalFromBody(body, modalId, parentModalId) {
    var idModal = modalId;
    var idParentModal = parentModalId;
    if ($("#modal_" + idModal).length > 0) {
        $("#modal_" + idModal).remove();
    }
    var width = "640px";
    var templateModal = '<div class="modal fade" parent-id="' +
        idParentModal +
        '" id="modal_' +
        idModal +
        '" role="dialog" data-backdrop="static" data-keyboard="false">' +
        '<div class="modal-dialog">' +
        "<!-- Modal content-->" +
        '<div class="modal-content" style="width: ' +
        width +
        ';" id="modal-content">' +
        "</div>" +
        "</div>" +
        "</div>";
    $("#ModalContent").append(templateModal);
    $("#loader").addClass("show pageload-loading").css("display", "inherit");
    $("#modal_" + idModal + " .modal-dialog").css("width", width);
    $("#modal_" + idModal + " #modal-content").css("width", width);
    $("#modal_" + idModal + " #modal-content").html(body);
    $("#loader").removeClass("show pageload-loading").css("display", "none");
    $("#modal_" + idModal).modal("show");
    return false;
}

function updateModalBody(body, idModal) {
    $("#modal_" + idModal + " #modal-content").html(body);
    return false;
}

function showModalWithBody(body, modalId, modalwidth, parentModalId) {
    var idModal = modalId;
    var idParentModal = parentModalId;
    var width = modalwidth;

    if ($("#modal_" + idModal).length > 0) {
        $("#modal_" + idModal).remove();
    }
    var modalClass = "";
    var modalDialog = null;
    if (width != undefined) {
        width = isNaN(width) ? (width.indexOf("px") >= 0 ? width.replace("px", "") : width) : width;
        if (parseInt(width) >= 1024) {
            modalClass = "modal-xlg";
        } else if (parseInt(width) < 1024 && parseInt(width) >= 800) {
            modalClass = "modal-lg";
        }
    } else if (true) {
        modalClass = "modal-content-full";
        modalDialog = "modal-dialog-full";
    }
    var templateModal = '<div class="modal fade" parent-id="' +
        idParentModal +
        '" id="modal_' +
        idModal +
        '" role="dialog" data-backdrop="static" data-keyboard="false">' +
        '<div class="modal-dialog ' +
        (modalDialog != null ? modalDialog : modalClass) +
        '">' +
        "<!-- Modal content-->" +
        '<div class="modal-content ' +
        modalClass +
        '" id="modal-content">' +
        "</div>" +
        "</div>" +
        "</div>";
    $("#ModalContent").append(templateModal);
    $("#loader").addClass("show pageload-loading").css("display", "inherit");
    var modalContent = '<div class="modal-header bg-primary">' +
        '<button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
        '<span aria-hidden="true">&times;</span>' +
        "</button>" +
        '<h4 class="modal-title"><i class="fa fa-plus"></i>&nbsp Báo Cáo</h4>' +
        "</div>" +
        '<div class="modal-body form">' +
        '<div class="form-body" id="FormBody">' +
        "</div>" +
        "</div>" +
        "</div>" +
        '<div class="modal-footer">' +
        '<button type="button" class="btn btn-danger" data-dismiss="modal">Đóng</button>' +
        "</div>";
    $("#modal_" + idModal + " #modal-content").html(modalContent);
    $("#modal_" + idModal + " #modal-content #FormBody").html(body);

    $("#modal_" + idModal).modal("show");
    $("#loader").removeClass("show pageload-loading").css("display", "none");

    return false;
}

function showModalWithContent(modalHtml, idModal, modalwidth, parentModalId) {
    if ($("#modal_" + idModal).length > 0) {
        $("#modal_" + idModal).remove();
    }

    var modalClass = "";
    var width = modalwidth;
    var modalDialog = "";
    if (width != undefined && isNaN(width)) {
        width = width.replace("px", "");
        if (parseInt(width) >= 1024) {
            modalClass = modalDialog = "modal-xlg";
        } else if (parseInt(width) < 1024 && parseInt(width) >= 800) {
            modalClass = modalDialog = "modal-lg";
        }
    } else {
        modalClass = "modal-content-full";
        modalDialog = "modal-dialog-full";
    }

    var templateModal = '<div class="modal fade" parent-id="' +
        parentModalId +
        '" id="modal_' +
        idModal +
        '" role="dialog" data-backdrop="static" data-keyboard="false">' +
        '<div class="modal-dialog ' +
        modalDialog +
        '">' +
        "<!-- Modal content-->" +
        '<div class="modal-content ' +
        modalClass +
        '" id="modal-content">' +
        "</div>" +
        "</div>" +
        "</div>";
    $("#ModalContent").append(templateModal);
    $("#loader").addClass("show pageload-loading").css("display", "inherit");
    $("#modal_" + idModal + " #modal-content").html(modalHtml);
    //$("#modal_" + idModal + " #modal-content .modal-header").append();

    $("#modal_" + idModal).modal("show");
    $("#loader").removeClass("show pageload-loading").css("display", "none");

    return false;
}

function _isJson(str) {
    try {
        return (JSON.parse(str) && !!str);
    } catch (e) {
        return false;
    }
}

function _initColumnDataTable() {
    if (typeof ($.fn.dataTable) == "undefined")
        return;
    var dataTable = $.fn.dataTable.tables({ visible: true, api: true });
    if (dataTable.length > 0) {
        var colvis = new $.fn.dataTable.ColVis(dataTable,
            {
                buttonText: "Hiển thị Cột",
                activate: "mouseclick",
                exclude: [0],
                className: "left",
                columnText: function (dt, idx, title) {
                    return (idx + 1) + ": " + title;
                }
            });

        var tableId = dataTable[0].id;
        if ($("#" + tableId + "_length").children().length < 2) {
            $(colvis.button()).css("float", "left");
            $("#" + tableId + "_length").append(colvis.button());
        }
    }
}

function _initDatePicker() {
    if ($(".datepicker").length <= 0) return;
    if (!$(".datepicker").hasClass("datepicker-dropdown")) {
        $(".datepicker").datepicker("remove");
        $(".datepicker").inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });
        $(".datepicker").datepicker({
            autoclose: true,
            format: "dd/mm/yyyy",
            todayhighlight: true,
            orientation: "auto",
            todaybtn: true,
            todayhighlight: true,
            minDate: null,
            maxDate: null,
            weekStart: 1
        });
    }

    $(".input-group > .input-group-addon").on("click",
        function () {
            var datePickerElement = $(this).next();
            if ($(datePickerElement).hasClass("datepicker")) {
                $(datePickerElement).datepicker("show");
            } else {
                $(this).next().trigger("click");
            }
        });
}

function _initElement() {
    /**
     * Register submit form event
     */
    $.each($("button[type='submit']"),
        function (idx, btnSubmit) {
            if ($(btnSubmit).parents("form").length == 0) {
                $(btnSubmit).unbind("click");
                $(btnSubmit).on("click",
                    function () {
                        //if ($("[type='submit']").parents('div.modal').find('form').length > 0) {
                        //    $("[type='submit']").parents('div.modal').find('form').submit();
                        //}
                        if ($(this).parents("div.modal").find("form").length > 0) {
                            $(this).parents("div.modal").find("form").submit();
                        }
                    });
            }
        });
    //if ($("button[type='submit']").parents('form').length == 0) {
    //    $("button[type='submit']").unbind("click");
    //    $("button[type='submit']").on("click",
    //        function() {
    //            //if ($("[type='submit']").parents('div.modal').find('form').length > 0) {
    //            //    $("[type='submit']").parents('div.modal').find('form').submit();
    //            //}
    //            if ($(this).parents("div.modal").find("form").length > 0) {
    //                $(this).parents("div.modal").find("form").submit();
    //            }
    //        });
    //}
    /*
     * Rigister select element
     */
    $.each($("select"),
        function (idx, ele) {
            if (!$(ele).data("select2") && !$(ele).hasClass("none-select2")) {
                $(ele).select2({
                    allowClear: true,
                    placeholder: "Chọn 1 giá trị",
                    width: "element",
                    dropdownParent: $(this).parent()
                    //dropdownParent: $(".modal-body")
                });
            }
        });

    $("div.modal").on("scroll",
        function (event) {
            $(this).find("select").each(function () {
                if (!$(this).hasClass("none-select2")) {
                    $(this).select2({
                        allowClear: true,
                        placeholder: "Chọn 1 giá trị",
                        width: "element",
                        dropdownParent: $(".modal-body")
                    });
                }
            });
        });

    //$("select").select2({
    //    allowClear: true,
    //    placeholder: "Chọn 1 giá trị",
    //    width: "element"
    //});

    $.grep($('select[name*="_length"]'),
        function (val) {
            $(val).select2("destroy").select2({
                allowClear: false,
            });
        });
}

function _renderButton(isModal, modalId, eleClass, urlAction, icon, title, modalWidth) {
    var attrWidth = modalWidth !== undefined ? String.format('data-width="{0}"', modalWidth) : "";

    var tmpUrlAction = urlAction;
    var idxParram = tmpUrlAction.indexOf("?");
    if (idxParram > -1) {
        tmpUrlAction = tmpUrlAction.substring(0, idxParram);
    }

    var pActions = tmpUrlAction != null ? tmpUrlAction.split("/") : [];
    pActions = pActions.filter(function (v) { return v !== "" });
    var id = pActions.join("");

    var dataParrams = {
        controllerName: pActions.length > 2 ? pActions[1] : pActions[0],
        actionName: pActions.length > 2 ? pActions[2] : pActions[1],
        areaName: pActions.length > 2 ? pActions[0] : null
    };

    $.ajax({
        type: "GET",
        async: false,
        url: "/App/ActionIsAllow",
        dataType: "JSON",
        data: dataParrams,
        success: function (response) {
            if (!response.status) {
                $("a#" + id).remove();
            }
        }
    });

    var template =
        '<a {7} id={6} {0} data-modal-id="{1}" class="{2}" data-toggle="tooltip" title="{5}" href="{3}">{4}</a>';
    var sModal = isModal ? 'data-modal=""' : "";
    return String.format(template, sModal, modalId, eleClass, urlAction, icon, title, id, attrWidth);
}

function _renderScrollTop() {
    var slideToTop = $("#ScrollTop");
    slideToTop.html('<i class="fa fa-chevron-up"></i>');
    slideToTop.css({
        position: "fixed",
        bottom: "20px",
        right: "25px",
        width: "40px",
        height: "40px",
        color: "#eee",
        'font-size': "",
        'line-height': "40px",
        'text-align': "center",
        'background-color': "#222d32",
        'cursor': "pointer",
        'border-radius': "5px",
        'z-index': "99999",
        'opacity': ".7",
        'display': "none"
    });
    slideToTop.on("mouseenter",
        function () {
            $(this).css("opacity", "1");
        });
    slideToTop.on("mouseout",
        function () {
            $(this).css("opacity", ".7");
        });
    $(".wrapper").append(slideToTop);
    $(window).scroll(function () {
        if ($(window).scrollTop() >= 150) {
            if (!$(slideToTop).is(":visible")) {
                $(slideToTop).fadeIn(500);
            }
        } else {
            $(slideToTop).fadeOut(500);
        }
    });
    $(slideToTop).click(function () {
        $("html, body").animate({
            scrollTop: 0
        },
            500);
    });
    $(".sidebar-menu li:not(.treeview) a").click(function () {
        var $this = $(this);
        var target = $this.attr("href");
        if (typeof target === "string") {
            $("html, body").animate({
                scrollTop: ($(target).offset().top) + "px"
            },
                500);
        }
    });
}

function _initValidateForm() {
    if ($("form").length <= 0) return;
    $("form").validate({
        errorClass: "help-block animation-slideDown",
        errorElement: "div",
        errorPlacement: function (error, e) {
            e.parents(".form-group > div").append(error);
        },
        highlight: function (e) {
            $(e).closest(".form-group").removeClass("has-success has-error").addClass("has-error");
            $(e).closest(".help-block").remove();
        },
        success: function (e) {
            e.closest(".form-group").removeClass("has-success has-error");
            e.closest(".help-block").remove();
        }
    });

    var dataValidate = window.mvcClientValidationMetadata == undefined
        ? undefined
        : window.mvcClientValidationMetadata[0];
    if (typeof dataValidate != "undefined") {
        var dataFields = dataValidate.Fields;
        dataFields.forEach(function (item, idx) {
            var validateRules = {};
            var validateMessages = {};

            item.ValidationRules.forEach(function (itemRule) {
                switch (itemRule.ValidationType) {
                    case "required":
                        {
                            validateRules[itemRule.ValidationType] = true;
                            validateMessages[itemRule.ValidationType] = itemRule.ErrorMessage;
                        }
                        break;
                    case "url":
                        {
                            validateRules[itemRule.ValidationType] = true;
                            validateMessages[itemRule.ValidationType] = itemRule.ErrorMessage;
                        }
                        break;
                    case "date":
                        {
                            validateRules[itemRule.ValidationType] = true;
                            validateMessages[itemRule.ValidationType] = itemRule.ErrorMessage;
                        }
                        break;
                    case "equalto":
                        {
                            validateRules["equalTo"] = itemRule.ValidationParameters.other.replace("*.", "#");
                            validateMessages["equalTo"] = itemRule.ErrorMessage;
                        }
                        break;
                    case "length":
                        {
                            var paramsLength = itemRule.ValidationParameters;
                            if (typeof paramsLength.min !== "undefined") {
                                validateRules["minlength"] = paramsLength.min;
                                validateMessages["minlength"] = itemRule.ErrorMessage;
                            }
                            if (typeof paramsLength.max !== "undefined") {
                                validateRules["maxlength"] = paramsLength.max;
                                validateMessages["maxlength"] = itemRule.ErrorMessage;
                            }
                        }
                        break;
                    case "range":
                        {
                            var paramsRange = itemRule.ValidationParameters;
                            validateRules[itemRule.ValidationType] = [paramsRange.min, paramsRange.max];
                            validateMessages[itemRule.ValidationType] = itemRule.ErrorMessage;
                        }
                        break;
                    default:
                        {
                            validateRules[itemRule.ValidationType] = true;
                            validateMessages[itemRule.ValidationType] = itemRule.ErrorMessage;
                        }
                        break;
                }
            });

            $("form").validate().settings.rules[item.FieldName] = validateRules;
            $("form").validate().settings.messages[item.FieldName] = validateMessages;
        });
    }
}

function _initMenuView() {
    var url = window.location.href;

    $(".treeview a").each(function () {
        if (this.href.toString().replace("#", "").toLowerCase() === (url.toLowerCase().toString())) {
            var name = $(this).attr("name");
            if (name != undefined) {
                var ids = name.split(",");
                var i;
                for (i = 0; i < ids.length; i++) {
                    $("#" + ids[i]).addClass("active");
                }
            }
        }
    });
}

function _showPassword(inpt) {
    if ("password" == $(inpt).attr("type")) {
        $(inpt).prop("type", "text");
    } else {
        $(inpt).prop("type", "password");
    }
}

function onDownloadDoc(btnDownloadDoc) {
    //var btnDownloadDoc = $("a[name='DownloadRefDoc']");
    if (typeof btnDownloadDoc != "undefined") {
        var url = $(btnDownloadDoc).data("href");
        $.ajax({
            type: "GET",
            url: url,
            success: function (response) {
                if (typeof response != "undefined" && response != null && response.status) {
                    window.location = response.downloadPath;
                } else if (typeof response != "undefined" && response != null && !response.status) {
                    eval(response.message);
                }
            }
        });
    }
}

function preInitDatatable() {
    var dataTables = $.fn.dataTable.tables();
    $.each(dataTables,
        function (idx, dataTable) {
            var tableId = dataTable.id;

            if ($("#" + tableId + "_wrapper").parent().parent("div.box-body").length == 0) {
                $($($("#" + tableId + "_wrapper").parent()[0]).parent()[0]).append(
                    $('<div class="box box-success box-body"></div>')
                    .append($($("#" + tableId + "_wrapper")[0]).parent()[0]));
            }
        });
}

function OnChangeCombo(cbb, eleName) {
    $(eleName).val($(cbb).children("option:selected").text());
}