(function ($) {
    "use strict";
    var charts = [];

    function keep(chart) {
        charts.push(chart);
    }

    function chartData(name) {
        var node = document.querySelector('.md-chart-data[data-chart="' + name + '"]');
        if (!node) return [];

        try {
            return JSON.parse(node.textContent || node.innerText || "[]");
        } catch (error) {
            if (window.console && window.console.warn) {
                window.console.warn("Dashboard chart data could not be read:", name);
            }
            return [];
        }
    }

    function initSelects() {
        if (!$.fn.select2) return;
        $('.management-dashboard select.select2').each(function () {
            var $select = $(this);
            if (!$select.data('select2')) {
                $select.select2({ width: '100%', language: 'vi' });
            }
        });
    }

    function initTrend() {
        var element = document.getElementById('management-trend-chart');
        var data = chartData('trend');
        if (!element || !data.length || !window.Morris) return;

        keep(new Morris.Line({
            element: element,
            data: data,
            xkey: 'Month',
            xLabelFormat: function (value) { return moment(value).format('MM'); },
            ykeys: ['GtsXcnIndex'],
            labels: ['Doanh thu công nghiệp (Tỷ đồng)'],
            lineColors: ['#1686c9'],
            lineWidth: 2,
            pointSize: 3,
            hideHover: 'auto',
            resize: true,
            gridTextColor: '#7a8995',
            gridLineColor: '#e8edf0',
            yLabelFormat: function (value) { return value.toFixed(0); }
        }));
    }

    function initWarnings() {
        var severity = document.getElementById('warning-severity-chart');
        var severityData = chartData('severity');
        if (severity && severityData.length && window.Morris) {
            keep(new Morris.Donut({
                element: severity,
                data: severityData,
                colors: ['#dca13c', '#d17937', '#c84b4b'],
                resize: true,
                formatter: function (value) { return value.toLocaleString('vi-VN'); }
            }));
        }

        var trend = document.getElementById('warning-trend-chart');
        var trendData = chartData('warning-trend');
        if (trend && trendData.length && window.Morris) {
            keep(new Morris.Line({
                element: trend,
                data: trendData,
                xkey: 'Month',
                xLabelFormat: function (value) { return moment(value).format('MM'); },
                ykeys: ['Over10', 'Over20', 'Over30'],
                labels: ['Giảm trên 10%', 'Giảm trên 20%', 'Giảm trên 30%'],
                lineColors: ['#dca13c', '#d17937', '#c84b4b'],
                lineWidth: 2,
                pointSize: 3,
                hideHover: 'auto',
                resize: true,
                gridTextColor: '#7a8995',
                gridLineColor: '#e8edf0'
            }));
        }
    }

    function initCompletionTrend() {
        var element = document.getElementById('completion-trend-chart');
        var data = chartData('completion-trend');
        if (!element || !data.length || !window.Morris) return;

        keep(new Morris.Line({
            element: element,
            data: data,
            xkey: 'Month',
            xLabelFormat: function (value) { return moment(value).format('MM'); },
            ykeys: ['Completion'],
            labels: ['Hoàn thành'],
            lineColors: ['#1686c9'],
            lineWidth: 2,
            pointSize: 3,
            hideHover: 'auto',
            resize: true,
            ymin: 0,
            ymax: 100,
            yLabelFormat: function (value) { return value + '%'; },
            gridTextColor: '#7a8995',
            gridLineColor: '#e8edf0'
        }));
    }

    function watchContainerWidth() {
        var root = document.querySelector('.management-dashboard');
        if (!root || !window.ResizeObserver || !charts.length) return;
        var lastWidth = root.clientWidth;
        var frame;
        new ResizeObserver(function () {
            var width = root.clientWidth;
            if (Math.abs(width - lastWidth) < 1) return;
            lastWidth = width;
            if (frame) window.cancelAnimationFrame(frame);
            frame = window.requestAnimationFrame(function () {
                charts.forEach(function (chart) {
                    if (typeof chart.redraw === 'function') chart.redraw();
                });
            });
        }).observe(root);
    }

    $(function () {
        initSelects();
        initTrend();
        initWarnings();
        initCompletionTrend();
        watchContainerWidth();
    });
}(jQuery));
