
function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}
$(document).ready(function () {
    Dateselect();
});
function Dateselect() {
    $("#SelectMonth").datepicker({
        format: "mm/yyyy",
        viewMode: "months",
        minViewMode: "months",
        autoclose: true
    }).on('changeDate', function (e) {
        var selectedMonth = e.date;
        var urlStatisticForMonth = "/Report/Dashboard/GetStatictis" + "?forMonth=" + formatDate(selectedMonth).format("YYYY-MM-DD");
        var dtmmyyyy = formatDate(selectedMonth).format("MM/YYYY");
        document.getElementById('forMonth').innerHTML = dtmmyyyy.split('-')[1] + '/' + dtmmyyyy.split('-')[0];
        $("div#statisticOnMonth").load(urlStatisticForMonth, function (data, textStatus, xhr) {

            if (xhr.status === 401) {
                window.location.href = "/Account/Login";
            } else if (xhr.status === 404) {
                window.location.href = "/Error/NotFound";
            } else if (xhr.status === 500) {
                window.location.href = "/Error/Error";
            } else if (xhr.status === 405) {
                window.location.href = "/Error/AccessDenied";
            }

            $("#loader").removeClass("show pageload-loading").css("display", "none");
            Dateselect();
        });
    });
}
