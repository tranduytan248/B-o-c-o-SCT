$(document).ready(function() {
    $("a[name='DownloadRefDoc']").on("click",
        function() {
            var url = $(this).data("href");
            $.ajax({
                type: "GET",
                url: url,
                success: function(response) {
                    if (typeof response != "undefined" && response != null && response.status) {
                        window.location = response.downloadPath;
                    } else if (typeof response != "undefined" && response != null && !response.status) {
                        eval(response.message);
                    }
                }
            });
        });
});
$(document).ajaxComplete(function(event, xhr, settings) {
    var btnDownloadDoc = $("a[name='DownloadRefDoc']");
    if (typeof btnDownloadDoc != "undefined") {
        btnDownloadDoc.on("click",
            function() {
                var url = $(this).data("href");
                $.ajax({
                    type: "GET",
                    url: url,
                    success: function(response) {
                        if (typeof response != "undefined" && response != null && response.status) {
                            window.location = response.downloadPath;
                        } else if (typeof response != "undefined" && response != null && !response.status) {
                            eval(response.message);
                        }
                    }
                });
            });
    }
});