var templatePanelHeader =
    '<div class="box-header"> <h3 class="box-title"></h3> <div class="box-tools pull-right"> <div class="btn-group"> <button type="button" class="btn btn-box-tool dropdown-toggle" data-toggle="dropdown"> &nbsp;<i class="fa fa-cogs"></i> </button> <ul class="dropdown-menu pull-right" role="menu"> <li> <a data-modal="true" data-modal-id="AddModule" href="/Modules/UpdatePanelModule?panelName={0}"><i class="fa fa-plus"></i>Thêm module</a> </li> </ul> </div> </div> </div>';

var templateBoxWidget = '<div id="{0}" moduleid="{2}" orderby="{1}" class="widget-item"> </div>';

var templateBoxHeader = '<div class="box-header with-border"> <h3 class="box-title"></h3></div>';

var templateBoxTool =
    '<div class="box-tools pull-right"> <button type="button" class="btn btn-box-tool" data-widget="collapse"> <i class="fa fa-minus"></i> </button> <div class="btn-group"> <button type="button" class="btn btn-box-tool dropdown-toggle" data-toggle="dropdown"> <i class="fa fa-gear"></i></button> <ul class="dropdown-menu pull-right" role="menu"> </ul> </div></div>';

var templateAddModule =
    '<li><a data-modal="true" data-modal-id="UpdatePanelModule" href="/Modules/UpdatePanelModule?panelName={0}"><i class="fa fa-pencil"></i>Thêm module</a></li>';

var templateRemoveModule =
    '<li><a data-modal="true" data-modal-id="DelModuleInPanel" href="/Modules/DelPanelModule?moduleId={0}&panelName={1}"><i class="text-red fa fa-trash"></i>Xoá module</a></li>';

$(function() {
    // widget boxes
    // widget box drag & drop example
    $(".widget-container-col").sortable({
        connectWith: ".widget-container-col",
        //items: '> .widget-box',
        //handle: '.widget-header',
        items: "> .widget-item",
        handle: ".box-header",
        cancel: ".fullscreen",
        opacity: 0.8,
        revert: true,
        forceHelperSize: true,
        placeholder: "widget-placeholder",
        forcePlaceholderSize: true,
        tolerance: "pointer",
        start: function(event, ui) {
            //when an element is moved, it's parent becomes empty with almost zero height.
            //we set a min-height for it to be large enough so that later we can easily drop elements back onto it
            //ui.item.parent().css({ 'min-height': ui.item.height() });
            //ui.sender.css({'min-height':ui.item.height() , 'background-color' : '#F5F5F5'})
        },
        update: function(event, ui) {
            ui.item.parent({ 'min-height': "" });
            if (ui.sender == null) {
                return;
            }
        }
    });

    $(".widget-container-col").sortable("disable");

    $(".widget-container-col").each(function(idx, item) {
        var url = $(item).attr("data-url");
        if (url.length > 0) {
            $(item).load(url,
                function(data, textStatus, xhr) {
                    renderModuleInPanel(this, data);
                });
        }
    });
});

function editDashboard(btn) {
    $(btn).addClass("hidden");
    $('button[id$="btnApply"]').removeClass("hidden");
    $('button[id$="btnCancel"]').removeClass("hidden");
    initBoxTool(true);
    $(".widget-container-col").sortable("enable");
}

function applyDashboard(btn) {
    $(btn).addClass("hidden");
    $('button[id$="btnCancel"]').addClass("hidden");
    $('button[id$="btnEdit"]').removeClass("hidden");
    initBoxTool(false);
    $(".widget-container-col").sortable("disable");
    saveModulePanel();
}

function cancelApplyDashboard(btn) {
    $(btn).addClass("hidden");
    $('button[id$="btnApply"]').addClass("hidden");
    $('button[id$="btnEdit"]').removeClass("hidden");
    initBoxTool(false);
    $(".widget-container-col").sortable("disable");
}

function initBoxTool(edit) {
    $(".widget-container-col").each(function(idx, item) {
        if (edit) {
            var dataBoxWidget = String.format(templatePanelHeader, $(item).attr("name"));
            $(item).parents(".widget-panel").prepend(dataBoxWidget);
            $(item).parents(".widget-panel").addClass("box-border");

            $(item).children(".widget-item").each(function(idx, widget) {
                if ($(widget).children(".box-widget").children(".box-header").length == 0) {
                    $(widget).children(".box-widget").prepend(templateBoxHeader);
                }
                if ($(widget).children(".box-widget").children(".box-header").children(".box-tools").length == 0) {
                    $(widget).children(".box-widget").children(".box-header").append(templateBoxTool);
                    var boxtool;
                    var menu;
                    if (widget.id.length > 0) {
                        boxtool = $(widget).children(".box-widget").children(".box-header").children(".box-tools");
                        menu = boxtool.find('ul[role="menu"]');
                        var htmlDelModule = String.format(templateRemoveModule,
                            $(widget).attr("moduleid"),
                            $(item).attr("name"));
                        $(menu).append(htmlDelModule);
                    } else {
                        boxtool = $(widget).children(".box-widget").children(".box-header").children(".box-tools");
                        menu = boxtool.find('ul[role="menu"]');
                        var htmlAddModule = String.format(templateAddModule, $(item).attr("name"));
                        $(menu).append(htmlAddModule);
                    }
                }
            });
        } else {
            $(item).parents(".widget-panel").removeClass("box-border");
            $(item).parents(".widget-panel").children(".box-header").remove();
            $(item).children(".widget-item").each(function(idx, widget) {
                $(widget).children(".box-widget").children(".box-header").children(".box-tools").remove();
            });
        }
    });
}

function renderModuleInPanel(item, data) {
    var isEdit = $('button[id$="btnEdit"]').hasClass("hidden");
    $(item).empty();
    if (data.length > 0) {
        var dataModules = $.parseJSON(data);
        if (dataModules.length > 0) {
            $(dataModules).each(function(idx, module) {
                var dataBoxWidget =
                    String.format(templateBoxWidget, module.ModuleName, module.OrderBy, module.ModuleId);
                $(item).append(dataBoxWidget);
                $(item).children("#" + module.ModuleName)
                    .html(module.ModuleHtml);

                if (isEdit) {
                    if ($(item).parents(".widget-panel").children(".box-header").length == 0) {
                        var dataBoxWidget = String.format(templatePanelHeader, $(item).attr("name"));
                        $(item).parents(".widget-panel").prepend(dataBoxWidget);
                        $(item).parents(".widget-panel").addClass("box-border");
                    }

                    $(item).children(".widget-item").each(function(idx, widget) {
                        if ($(widget).children(".box-widget").children(".box-header").length == 0) {
                            $(widget).children(".box-widget").prepend(templateBoxHeader);
                        }
                        if ($(widget).children(".box-widget").children(".box-header").children(".box-tools").length ==
                            0) {
                            $(widget).children(".box-widget").children(".box-header").append(templateBoxTool);
                            var boxtool;
                            var menu;
                            if (widget.id.length > 0) {
                                boxtool = $(widget).children(".box-widget").children(".box-header")
                                    .children(".box-tools");
                                menu = boxtool.find('ul[role="menu"]');
                                var htmlDelModule = String.format(templateRemoveModule,
                                    $(widget).attr("moduleid"),
                                    $(item).attr("name"));
                                $(menu).append(htmlDelModule);
                            } else {
                                boxtool = $(widget).children(".box-widget").children(".box-header")
                                    .children(".box-tools");
                                menu = boxtool.find('ul[role="menu"]');
                                var htmlAddModule = String.format(templateAddModule, $(item).attr("name"));
                                $(menu).append(htmlAddModule);
                            }
                        }
                    });
                }
            });
        }
    }
}

function PanelModule_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        $("#ModalContent #modal_" + formId).modal("hide");
        $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
            function() {
                if (response.status != undefined) {
                    eval(response.message);
                    response.status = undefined;
                    var panel = $("div[name=" + response.panelName + "]");
                    if (typeof panel != "undefined") {
                        $(panel).load($(panel).attr("data-url"),
                            function(data, textStatus, xhr) {
                                renderModuleInPanel(this, data);
                            });
                    }
                }
            });
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}

var mappingPanelModule = [];

function saveModulePanel() {
    var dataPanelModules = [];
    initPanelModule();

    if (typeof mappingPanelModule === "undefined") {
        return;
    }

    $.each(mappingPanelModule,
        function(idx, item) {
            $.each(item.Modules,
                function(idx, module) {
                    dataPanelModules.push({
                        ContentPanelId: item.ContentPanelID,
                        ModuleId: module.ModuleId,
                        OrderBy: module.ModuleId
                    });
                });
        });

    var urlSavePanelModule = "/Modules/SaveModulesContent";
    $.ajax({
        type: "POST",
        url: urlSavePanelModule,
        dataType: "JSON",
        data: {
            panelModules: dataPanelModules
        },
        success: function(response) {
            eval(response.message);
        }
    });
}

function initPanelModule() {
    $(".widget-container-col").each(function(idx, panel) {
        var contentPanelID = panel.id;
        if ($(panel).children(".widget-item").length > 0) {
            $(panel).children(".widget-item").each(function(idx, module) {
                if (mappingPanelModule.length > 0) {
                    var contentPanel = $.grep(mappingPanelModule,
                        function(panel) {
                            return panel.ContentPanelID === contentPanelID;
                        });
                    if (contentPanel.length === 0) {
                        mappingPanelModule.push(
                            {
                                ContentPanelID: contentPanelID,
                                Modules: [
                                    {
                                        ModuleId: $(module).attr("moduleid"),
                                        ModuleName: module.id,
                                        OrderBy: $(module).attr("orderby")
                                    }
                                ]
                            });

                    } else {
                        contentPanel[0].Modules.push({
                            ModuleId: $(module).attr("moduleid"),
                            ModuleName: module.id,
                            OrderBy: $(module).attr("orderby")
                        });
                    }
                } else {
                    mappingPanelModule.push({
                        ContentPanelID: contentPanelID,
                        Modules: [
                            {
                                ModuleId: $(module).attr("moduleid"),
                                ModuleName: module.id,
                                OrderBy: $(module).attr("orderby")
                            }
                        ]
                    });
                }
            });
        }
    });
}