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

  function drawChart(id, dataName, keys, colors, bar) {
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
      element.className += " text-muted text-center";
      element.style.lineHeight = element.offsetHeight + "px";
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
      gridTextColor: "#777",
      gridLineColor: "#f4f4f4",
      yLabelFormat: function (value) {
        return Number(value).toLocaleString("vi-VN");
      },
    };
    if (bar) new Morris.Bar(options);
    else new Morris.Line(options);
  }

  $(function () {
    if ($.fn.select2) {
      $(".management-dashboard select.select2").each(function () {
        $(this).select2({ width: "100%", language: "vi" });
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
    );
    drawChart(
      "observation-trend-chart",
      "observation-trend",
      ["Value"],
      ["#00a65a"],
      false,
    );
  });
})(jQuery);
