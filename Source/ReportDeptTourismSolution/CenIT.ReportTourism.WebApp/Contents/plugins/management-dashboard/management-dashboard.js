(function ($) {
  "use strict";
  var charts = {};

  function chartData(name) {
    var node = document.querySelector(
      '.md-chart-data[data-chart="' + name + '"]',
    );
    if (!node) return [];
    try {
      return JSON.parse(node.textContent || node.innerText || "[]");
    } catch (error) {
      return [];
    }
  }

  function drawChart(id, dataName, keys, colors, bar, integerCounts, rateMode) {
    var element = document.getElementById(id);
    var data = chartData(dataName);
    if (element) {
      var previous=$(element).data("dashboard-chart");
      if (previous && previous.raphael) previous.raphael.remove();
      $(element).off();
      $(element).empty(); $(element).removeClass("dashboard-chart-empty");
    }
    if (!element || !window.Morris) return;
    var labels;
    try {
      labels = JSON.parse(element.getAttribute("data-labels") || "[]");
      if (id === "observation-trend-chart") labels = rateMode ? ["Tỷ lệ DN có dữ liệu"] : ["DN có dữ liệu", "Có tệp (mọi loại)"];
    } catch (error) {
      labels = keys;
    }
    if (
      !data.length ||
      !data.some(function (point) {
        return keys.some(function (key) {
          return point[key] !== null && point[key] !== undefined;
        });
      })
    ) {
      element.textContent = element.getAttribute("data-empty") || "";
      element.className += " dashboard-chart-empty";
      return;
    }
    var options = {
      element: element,
      data: data,
      xkey: "Month",
      ykeys: keys,
      labels: labels,
      lineColors: colors,
      barColors: colors,
      resize: false,
      hideHover: "auto",
      parseTime: false,
      smooth: false,
      gridTextColor: "#777",
      gridLineColor: "#f4f4f4",
      yLabelFormat: function (value) {
        if (value === null || value === undefined) return "—";
        var number = Number(value);
        if (number !== 0 && Math.abs(number) < 0.01) return number < 0 ? ">−0,01" : "<0,01";
        return number.toLocaleString("vi-VN", { maximumFractionDigits: rateMode ? 1 : 2 }) + (rateMode ? "%" : "");
      },
    };
    options.hoverCallback = function (index, chartOptions, content) {
      var point = data[index];
      if (point.Coverage !== undefined) content += '<div class="morris-hover-point">' + (dataName === 'warning-trend' ? 'DN so sánh hợp lệ / được phân: ' : 'Có dữ liệu / được phân: ') + point.Coverage + ' / ' + point.Expected + '</div>';
      return content;
    };
    if (rateMode) { options.ymin = 0; options.ymax = 100; }
    if (integerCounts && !rateMode) {
      var maxCount = data.reduce(function (max, point) {
        return keys.reduce(function (value, key) {
          return Math.max(value, Number(point[key]) || 0);
        }, max);
      }, 0);
      options.ymin = 0;
      options.ymax = Math.max(4, Math.ceil(maxCount / 4) * 4);
      options.numLines = 5;
    }
    var chart = bar ? new Morris.Bar(options) : new Morris.Line(options);
    $(element).data("dashboard-chart",chart);
    charts[id] = chart;
  }

  $(function () {
    var resizeTimer;
    $(window).off("resize.dashboardCharts").on("resize.dashboardCharts",function () {
      window.clearTimeout(resizeTimer);
      resizeTimer=window.setTimeout(function () {
        Object.keys(charts).forEach(function (id) { if ($("#"+id).is(":visible")) charts[id].resizeHandler(); });
      },100);
    });
    if ($.fn.select2) {
      $(".management-dashboard .dashboard-filter-toolbar select").each(function () {
        if (this.id === "enterpriseId") {
          var select = $(this);
          select.select2({ width:"100%",language:"vi",allowClear:false,
            ajax: { url:select.attr("data-options-url"),dataType:"json",delay:250,
              data:function (params) { return { q:params.term || "",page:params.page || 1,reportType:select.attr("data-report-type") || null }; },
              processResults:function (data,params) { if (!params.page || params.page === 1) data.results.unshift({ id:"all",text:"Tất cả doanh nghiệp" }); return data; }
            }
          });
          return;
        }
        $(this).select2({
          width: "100%",
          language: "vi",
          allowClear: false,
          minimumResultsForSearch: $(this).closest(".dashboard-filter-primary").length ? Infinity : 0,
        });
      });
    }
    $(".management-dashboard .dashboard-filter-toolbar").each(function () {
      var form = this;
      $(form)
        .find("#filterMonth, #filterYear, #filterReportType, #metricSelect, #status")
        .on("change", function () {
          form.submit();
        });
    });
    $(".dashboard-detail-link").on("click", function (event) {
      event.preventDefault();
      var modal = $("#dashboard-enterprise-detail");
      modal.find(".modal-body").text("Đang tải…"); modal.modal("show");
      $.get(this.href).done(function (html) { modal.find(".modal-body").html(html); })
        .fail(function () { modal.find(".modal-body").text("Không tải được dữ liệu. Vui lòng thử lại."); });
    });
    $(".dashboard-chart-toggle").on("click", function () {
      var id=$(this).attr("data-chart-id"), rate=$(this).attr("data-mode") === "rate";
      if (id === "receipt-overview-chart") drawChart(id,"receipt-overview",rate ? ["Type1Rate","Type2Rate","Type3Rate"] : ["Type1","Type2","Type3"],["#3c8dbc","#00a65a","#f39c12"],true,true,rate);
      else drawChart(id,"observation-trend",rate ? ["Rate"] : ["Value","Files"],["#00a65a","#3c8dbc"],false,true,rate);
      $(this).addClass("active").siblings(".dashboard-chart-toggle").removeClass("active");
    });
    drawChart(
      "receipt-overview-chart",
      "receipt-overview",
      ["Type1", "Type2", "Type3"],
      ["#3c8dbc", "#00a65a", "#f39c12"],
      true,
      true,
    );
    drawChart(
      "analysis-trend-chart",
      "analysis-trend",
      ["Value"],
      ["#3c8dbc"],
      false,
    );
    drawChart(
      "warning-trend-chart",
      "warning-trend",
      ["Value"],
      ["#dd4b39"],
      true,
      true,
    );
    drawChart(
      "observation-trend-chart",
      "observation-trend",
      ["Value", "Files"],
      ["#00a65a", "#3c8dbc"],
      false,
      true,
    );
  });
})(jQuery);
