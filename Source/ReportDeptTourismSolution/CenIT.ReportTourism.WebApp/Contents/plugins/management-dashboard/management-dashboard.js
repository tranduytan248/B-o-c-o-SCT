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

    function initDashboardFilters() {
        $('.dashboard-filter-toolbar').each(function () {
            var form = this;
            var monthSelect = form.querySelector('#filterMonth');
            var yearSelect = form.querySelector('#filterYear');
            var metric = form.querySelector('#hiddenFilterMetric') || form.querySelector('input[name="metric"]');
            var metricSelect = form.querySelector('#metricSelect');
            var period = form.querySelector('.dashboard-period-input');
            var year = form.querySelector('input[name="year"]');
            var month = form.querySelector('input[name="month"]');

            function syncLegacyPeriod() {
                if (!period || !/^\d{4}-\d{2}$/.test(period.value)) return;
                if (year) year.value = period.value.slice(0, 4);
                if (month) month.value = String(parseInt(period.value.slice(5), 10));
            }

            // Auto submit when period changes
            if (monthSelect) {
                $(monthSelect).on('change', function () {
                    form.submit();
                });
            }
            if (yearSelect) {
                $(yearSelect).on('change', function () {
                    form.submit();
                });
            }

            if (period) {
                $(period).on('change', function () {
                    syncLegacyPeriod();
                    form.submit();
                });
            }

            $(form).on('submit', function () {
                syncLegacyPeriod();
                if (metric && metricSelect) {
                    metric.value = metricSelect.value;
                }
            });

            // Re-render select2 width when collapse panel opens
            $('#advancedFiltersPanel').on('shown.bs.collapse', function () {
                $(this).find('select.select2').each(function () {
                    $(this).select2({ width: '100%', language: 'vi' });
                });
            });
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
            xLabelFormat: function (value) { return typeof moment !== 'undefined' ? moment(value).format('MM') : value; },
            ykeys: ['GtsXcnIndex'],
            labels: ['Doanh thu công nghiệp (Tỷ đồng)'],
            lineColors: ['#1686c9'],
            lineWidth: 2,
            pointSize: 4,
            hideHover: 'auto',
            resize: true,
            gridTextColor: '#7a8995',
            gridLineColor: '#e8edf0',
            yLabelFormat: function (value) { return value.toLocaleString('vi-VN'); }
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
                xLabelFormat: function (value) { return typeof moment !== 'undefined' ? moment(value).format('MM') : value; },
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
            xLabelFormat: function (value) { return typeof moment !== 'undefined' ? moment(value).format('MM') : value; },
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

    function initObservationTrend() {
        var element = document.getElementById('observation-trend-chart');
        var data = chartData('observation-trend');
        if (!element || !data.length || !window.Morris) return;

        keep(new Morris.Line({
            element: element,
            data: data,
            xkey: 'Month',
            xLabelFormat: function (value) { return typeof moment !== 'undefined' ? moment(value).format('MM') : value; },
            ykeys: ['Files', 'Indicators'],
            labels: ['Có tệp trong kỳ', 'Có giá trị chỉ tiêu'],
            lineColors: ['#1686c9', '#059669'],
            lineWidth: 2,
            pointSize: 4,
            hideHover: 'auto',
            resize: true,
            ymin: 0,
            gridTextColor: '#7a8995',
            gridLineColor: '#e8edf0',
            yLabelFormat: function (value) { return value.toFixed(0); }
        }));
    }

    function initProgressWorklist() {
        var search = document.getElementById('progressSearch');
        var state = document.getElementById('progressState');
        if (!search || !state) return;
        var rows = document.querySelectorAll('[data-progress-enterprise]');
        var count = document.querySelector('.progress-worklist-count');
        var empty = document.querySelector('.progress-worklist-empty');

        function update() {
            var query = search.value.trim().toLocaleLowerCase('vi');
            var shown = 0;
            Array.prototype.forEach.call(rows, function (row) {
                var file = row.getAttribute('data-has-file') === 'true';
                var value = row.getAttribute('data-has-value') === 'true';
                var text = (row.cells[0] ? row.cells[0].textContent : '').toLocaleLowerCase('vi');
                var matches = text.indexOf(query) !== -1;
                var status = state.value;
                matches = matches && (
                    status === 'all' ||
                    (status === 'file' && file) ||
                    (status === 'no-file' && !file) ||
                    (status === 'value' && value) ||
                    (status === 'no-value' && !value)
                );
                row.hidden = !matches;
                if (matches) shown++;
            });
            if (count) count.textContent = shown + ' doanh nghiệp';
            if (empty) empty.hidden = (shown !== 0);
        }

        search.addEventListener('input', update);
        state.addEventListener('change', update);
    }

    function watchContainerWidth() {
        var root = document.querySelector('.management-dashboard');
        if (!root) return;

        $(window).on('resize', function () {
            charts.forEach(function (chart) {
                if (typeof chart.redraw === 'function') chart.redraw();
            });
        });

        if (window.ResizeObserver && charts.length) {
            var lastWidth = root.clientWidth;
            new ResizeObserver(function () {
                var width = root.clientWidth;
                if (Math.abs(width - lastWidth) < 1) return;
                lastWidth = width;
                charts.forEach(function (chart) {
                    if (typeof chart.redraw === 'function') chart.redraw();
                });
            }).observe(root);
        }
    }

    $(function () {
        initSelects();
        initDashboardFilters();
        initTrend();
        initWarnings();
        initCompletionTrend();
        initObservationTrend();
        initProgressWorklist();
        watchContainerWidth();
    });
}(jQuery));
