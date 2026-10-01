(function ($) {
  "use strict";

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

  function drawChart(id, dataName, keys, colors, bar, integerCounts) {
    var element = document.getElementById(id);
    var data = chartData(dataName);
    if (!element || !window.Morris) return;
    var labels;
    try {
      labels = JSON.parse(element.getAttribute("data-labels") || "[]");
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
      resize: true,
      hideHover: "auto",
      parseTime: false,
      smooth: false,
      gridTextColor: "#777",
      gridLineColor: "#f4f4f4",
      yLabelFormat: function (value) {
        return Number(value).toLocaleString("vi-VN");
      },
    };
    if (integerCounts) {
      var maxCount = data.reduce(function (max, point) {
        return keys.reduce(function (value, key) {
          return Math.max(value, Number(point[key]) || 0);
        }, max);
      }, 0);
      options.ymin = 0;
      options.ymax = Math.max(4, Math.ceil(maxCount / 4) * 4);
      options.numLines = 5;
    }
    if (bar) new Morris.Bar(options);
    else new Morris.Line(options);
  }

  $(function () {
    if ($.fn.select2) {
      $(".management-dashboard .dashboard-filter-toolbar select").each(function () {
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
        .find("#filterMonth, #filterYear, #filterReportType, #metricSelect")
        .on("change", function () {
          form.submit();
        });
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
      true,
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
      ["Value"],
      ["#00a65a"],
      false,
      true,
    );
  });
})(jQuery);
