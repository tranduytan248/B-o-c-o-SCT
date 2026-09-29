(function ($) {
    "use strict";

    function chartData(name) {
        var node = document.querySelector('.md-chart-data[data-chart="' + name + '"]');
        if (!node) return [];
        try { return JSON.parse(node.textContent || node.innerText || '[]'); }
        catch (error) { return []; }
    }

    function drawChart(id, dataName, keys, labels, colors, bar) {
        var element = document.getElementById(id);
        var data = chartData(dataName);
        if (!element || !window.Morris) return;
        if (!data.length || !data.some(function (point) {
            return keys.some(function (key) { return point[key] !== null && point[key] !== undefined; });
        })) {
            element.textContent = 'Chưa có dữ liệu cho biểu đồ.';
            element.className += ' leader-chart-empty';
            return;
        }
        var options = {
            element: element, data: data, xkey: 'Month', ykeys: keys, labels: labels,
            lineColors: colors, barColors: colors, resize: true, hideHover: 'auto',
            parseTime: false, gridTextColor: '#64748b', gridLineColor: '#e5eaf0',
            yLabelFormat: function (value) { return Number(value).toLocaleString('vi-VN'); }
        };
        if (bar) new Morris.Bar(options);
        else new Morris.Line(options);
    }

    $(function () {
        if ($.fn.select2) {
            $('.management-dashboard select.select2').each(function () {
                $(this).select2({ width: '100%', language: 'vi' });
            });
        }
        $('.management-dashboard .dashboard-filter-toolbar').each(function () {
            var form = this;
            $(form).find('#filterMonth, #filterYear, #filterReportType, #metricSelect').on('change', function () {
                form.submit();
            });
        });
        drawChart('receipt-overview-chart', 'receipt-overview',
            ['Type1', 'Type2', 'Type3'], ['Loại 1', 'Loại 2', 'Loại 3'],
            ['#1e40af', '#059669', '#d97706'], true);
        drawChart('analysis-trend-chart', 'analysis-trend', ['Value'], ['Giá trị'], ['#1e40af'], true);
        drawChart('warning-trend-chart', 'warning-trend', ['Value'], ['Doanh nghiệp giảm'], ['#dc2626'], true);
        drawChart('observation-trend-chart', 'observation-trend', ['Value'], ['Có dữ liệu nhập'], ['#059669'], false);
    });
})(jQuery);
