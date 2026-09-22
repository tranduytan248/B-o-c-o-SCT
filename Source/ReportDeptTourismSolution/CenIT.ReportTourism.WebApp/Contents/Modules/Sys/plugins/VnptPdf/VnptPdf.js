/*
 * 
*/
SignatureVisibleType = {
    ONLY_TEXT: 1,
    BOTH: 2,
    ONLY_IMAGE: 3
};

var _defaultImage =
    "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAACc1BMVEUAAABGRl1KUFNKUFNLUVVLUFRNUVVLUFRVVVVKUFNLUFRKUFNMT1RLUVNKT1VIUFhMT1NLUFNOTk5KUFRMUVVLUFRKT1VKT1NLUFRLUVRMT1NMUFVAYGBLUFRLUFRAQEBLUFRLUFRLT1RJSUlLTlJLUFRLS1pMUFRLUFNLUFRMUFRKUVVLUFRMU1NLUFRLUFRLUVRLUFRMUFRMUVNLUFRLT1RLUFRKUVRJUFRLUFNKUFRMT1RKUVdJUlJLUFRLUVRKUFVLT1RLUFRLUFVLUFRLUFRNUlJNTVlKVVVEVVVNTU1VVVWAgIBLUFVMT1VMT1RLUFNOTlhLUFRLUFRLUFVLTlVMT1NLUFRMT1VOTllMUFRMUVFNUVVLUFQzZmZLUFRJUVVQUFBVVVVLT1NMUFVLUFRLT1QAAABMUFRNTVNLUVVNTVVMUFVMUFVMT1NJSVtKT1RLT1NKUVVVVVVNTVVLUVFLUFRLT1NHVVVLUFRKTlNKUFVLUVRMUFRKUVRKUVRLUFRLUFRLUVRLUFRRUVFLUFVLUFNKUFNLUFNLT1RLUFRKUFVLT1RLT1RLT1RLUVROTlVMUFRJUlJKUFRKT1RLT1RLUFRKUVVLUFRMUFRJUFdHUlJKT1RLUFNLUFRKUFRLUFRJUlJLUFRLUFRLUFRLT1RMTFVNU1NLUFRMUVRKUFRLUVNLUFRLUFVITlVKUFVLUVROTlNKUFVMUFRLUFRNUlJLUFNLUVRKT1NMUVRLUFVKUVNLUFRLUVVPT1hKUFNJT1VLT1NMUVNLUFRMUFNLUFRKTlJKUlJJTlNLUFRLUFVMT1VLUFRLUFRLUFRLUFQAAADa8LodAAAAz3RSTlMAC1mNwfI/3QaQ/uKUXy0gh5kNhjn8WjeCrko2CNqjBP3C1wdB7BGA38hAda0l+3NYqXll4KfVTEnWwGEmHJ2OXeftaX34NRQYDwoMArtUkVwa7vYzTor6VxfpLzz1BfBCEAlErLCkAbMrex5sb00OZEdIAyEs84QS+TRg1NjRT6DPq7wTZtyo2Zr0svc9OogkvTh/Z7fleMZ2IxmXn5K5vzvo4ZV6GyjFW4li5s0nyuQuMIPrMpxVcZvTa4x+HVYqgWjbosk+HzFtuMeq4+pXMtrjAAAAAWJLR0QAiAUdSAAAAAlwSFlzAAAN1wAADdcBQiibeAAAAAd0SU1FB+IDHgsAAGxrpbQAAAPVSURBVFjD1Zf9X1NVHMc/MMA5UWKyQYVsyKCBOh+iqIQSyZgwpZwPWQhumOtJKMiMDEwlKkWkKLJEwLKyB1PStGft0Z4//1Ln7u5ud3O723W9Xunnh3u+33vP933PPfd+z/leQFZGpiErmykqO8uQmYEo5cxINVjRjBxVuHGmibNyZ8/JS1FzZufOommmMQy4gcw3Q5fM+eRcxSmw0AoUFt14kx6ElZabZat4HktgM9jJUl2DKOH8sqDhYHkFbiGdrNQFqChnVdBYwIVY5OLiJToBWMjFwXYpl+FWVkM3YBlvC7a3swYGGvQD7uCdwfYuLketoU4/oEoMW9LdvCfYXjVgBettaQGKV7IhLQDu5ar7ogCNbrd7tWjL3CE1CafZ3ZgI4FnD7LUqQIuUbZb78cA6JfW86+FYRbYkAKCxktwQAWyUYjY9iM0PhReAh9G6hdyYCIC2lvatqkcw+3w+v2htvpCkxC3zxSasCpDea7wGAR3bHomSwYba7RF3biAZ4NHYBfQxPK5265IBnnhyR5Q6ga6nIu7T18Ekpg3wd3RLcrQBdd1h5fQAz8jmziSAbaHZzoX/WdXk78JzvSHzeW3A7hf6JPXvAV7sC2tvBzz7ZHP/wLU+if8/4KXBapVeBta+EnFfTQGwNCqT2ovlRTKkek9ygLG1SqXNQE9BxB24LibxPwCYtzYU9qQBcBwQ030wKSEhYOcQnftdPBQvqGnYsTopIJdZbeJw+MrwkdcspGnv60kAWRwF3uCbV8QvEfvjmFNsvG9pA45wBfC2KB5j9M4BrhkBjpbQ5NAEHOY4cIwTrTYE1B0mOSX7+XR6tADH+a4H79HEE+9Xl34Qvl68iaHF0Ozlh1oATzmHR5TksYavF3ClYp7kR1oAfGxZ/wkpyotPRXHS3n7KFzybw3ql65SoDDW/RD+Oe0s+23K67ow0iN684MlmO2UDxmxOI4VcEKVVxudCSnFVyX2ycZbnbKkAYvWFi+e7xP3PWngBVwPAl3Z6T06Jf7NxxAPsYb9WdM94EVoHpTk5dwGZX4mh1PDrqB557NX6cfqGY0Cg1rp82iYV6BPAt2Lzi9Igt9sSxov/kwXo+k58QAOj3+MiXZfEy9wQ3eeQnf0//BhHNeILnuBPRvwsPf0O/hJAKScx3BB7v4vrGF/DuOQStys08Vfgslck2292dsYZ59Hfj/0RR+ebRSKd8gf+5F9SryKOZbQd5DwjUtciaUvZxaHLktP0N0/MFyc6dQAqJv+Riosi2bMK01V+ekQHQKh2iGeaQvYR0qEvWtJuZ7diTveNKua/ykKBgkauYAcAAAAldEVYdGRhdGU6Y3JlYXRlADIwMTgtMDMtMzBUMTE6MDA6MDArMDI6MDDZZlSaAAAAJXRFWHRkYXRlOm1vZGlmeQAyMDE4LTAzLTMwVDExOjAwOjAwKzAyOjAwqDvsJgAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAAASUVORK5CYII=";
var _imagePrefix = "data:image/png;base64,";
var _pageWidth = 0;
var _pageHeight = 0;
var _pdf = null;
var _numOfPages = 0;
var _signBox;
var _pdfPage;
var _signImage;
var _signText;
var _posX, _posY, _signWidth, _signHeigth;

var _dataChanged = false;

var _completeBtnCallBack = null;

var VnptPdf = (function(plugin) {
    var _options = new PdfOptions();

    /*
    */
    plugin.initPlugin = function() {
        $("body").append(pdfWorkingArea);
        $("#pdf-complete").click(function() {
            $(".pdf-working-area").hide();
            _completeBtnCallBack();
        });
        $("#pdf-cancel").click(function() {
            $(".pdf-working-area").hide();
        });
    };

    /*
    */
    plugin.initData = function(file, callBack) {
        _options.imageSrc = _defaultImage;
        _completeBtnCallBack = callBack;

        //Step 2: Read the file using file reader
        var fileReader = new FileReader();

        fileReader.onload = function() {

            //Step 4:turn array buffer into typed array
            var typedarray = new Uint8Array(this.result);

            //Step 5:PDFJS should be able to read this
            PDFJS.getDocument(typedarray).then(function(pdf) {
                _pdf = pdf;
                _numOfPage = pdf.numPages;
                $("#pdf-total-pages").text(" of " + pdf.numPages);

                pdf.getPage(1).then(function(page) {
                    _options.page = 1;
                });
            });

            // Listener when click change page
            changePageListener();
            // Complete button press listener
            advancedSign();
            // Change image listener
            changeImageListener();

            //
            _dataChanged = true;
        };
        //Step 3:Read the file as ArrayBuffer
        fileReader.readAsArrayBuffer(file);
    };

    /*
        
    */
    plugin.start = function() {
        if (!_dataChanged) {
            $(".pdf-working-area").show();
        } else {
            _dataChanged = false;
            // Reload view page
            _pdf.getPage(_options.page).then(function(page) {
                handlePage(page);
            });
        }
    };

    /*
        Trả về PDF signature options
    */
    plugin.getPdfOptions = function() {
        _options.rectangle =
            "" +
            _options.x +
            "," +
            _options.y +
            "," +
            (_options.x + _options.width) +
            "," +
            (_options.y + _options.height);
        return _options;
    };

    /*
        Xử lý khi click Ký dữ liệu
    */
    function advancedSign() {
    }

    /*
        Xử lý hiển thị pdf page + signature box
    */
    function handlePage(page) {
        //We need to pass it a scale for "getViewport" to work
        var scale = 1;

        var xview = page.getViewport(1);
        _pageWidth = Math.floor(xview.width);
        _pageHeight = Math.floor(xview.height);
        console.log("width=" + _pageWidth + ". height=" + _pageHeight);

        //Grab the viewport with original scale
        // Pdf using 72 units per inch, while your screen dpi difference ex 96dpi ->>
        var viewport = page.getViewport(1 / 72 * getDpi());

        var canvas = document.getElementById("dropHere");
        var context = canvas.getContext("2d");
        canvas.height = viewport.height;
        canvas.width = viewport.width;


        // Render PDF page into canvas context.
        var renderContext = {
            canvasContext: context,
            viewport: viewport
        };
        page.render(renderContext);

        initSignatureVisible();
        signaturePositionChangeListener();
    }

    function signaturePositionChangeListener() {
        $(".sign-pos").bind("cut copy paste drag drop",
            function(e) {
                e.preventDefault();
            });

    }

    /*
        Get screen dpi
    */
    function getDpi() {
        return document.getElementById("dpi").offsetWidth;
    }

    /*
    */
    function initSignatureVisible() {
        _signBox = $("#dragThis");
        _pdfPage = $("#dropHere");
        _signImage = $("#signature-img");
        _signText = $("#signature-text");

        _pdfPage.show();
        $(".pdf-working-area").show();

        var boundX = _pdfPage[0].offsetLeft;
        var boundY = _pdfPage[0].offsetTop;
        var xPos = boundX + 21;

        //21 = 15 * getDpi() / 72;

        var h = Math.floor(_signBox[0].offsetHeight / getDpi() * 72);
        var yPos = boundY + 21;
        _signBox.css({ 'top': yPos, 'left': xPos, 'position': "absolute" });
        $("#signature-img").attr("src", "data:image/png;base64," + _options.imageSrc);

        _options.x = 15;
        _options.y = _pageHeight - 16 - 60;
        var w = Math.floor(_signBox[0].offsetWidth / getDpi() * 72);
        var h = Math.floor(_signBox[0].offsetHeight / getDpi() * 72);
        _options.width = w;
        _options.height = h;

        $("#signbox-xpos").val(_options.x);
        $("#signbox-ypos").val(_options.y);

        setupSignatureBox();

        changeVisibleType();
    }

    /*
        Initial signature box with dragable, resiable
    */
    function setupSignatureBox() {
        _signBox
            .draggable({
                containment: $("#dropHere"),
                drag: function() {
                    var boundX = _pdfPage[0].offsetLeft;
                    var boundY = _pdfPage[0].offsetTop;
                    var top = _signBox[0].offsetTop;
                    var left = _signBox[0].offsetLeft;
                    var offset = $(this).offset();
                    var xPos = Math.floor((left - boundX) / getDpi() * 72);
                    _options.x = xPos;

                    var h = Math.floor(_signBox[0].offsetHeight / getDpi() * 72);
                    var yPos = _pageHeight - Math.floor((top - boundY) / getDpi() * 72) - h;
                    _options.y = yPos;
                    $("#signbox-xpos").val(xPos);
                    $("#signbox-ypos").val(yPos);
                },
                stop: function() {
                    var boundX = _pdfPage.offset().left;
                    var boundY = _pdfPage.offset().top;
                    var finalOffset = $(this).offset();
                    var finalxPos = finalOffset.left;
                    var finalyPos = finalOffset.top;
                }
            })
            .resizable({
                resize: function(event, ui) {
                    _changeSignBoxSize($(this));
                },
                stop: function(event, ui) {
                    _changeSignBoxSize($(this));
                }
            });

        $("#dropHere").droppable({
            accept: "#dragThis",
            over: function() {
                $("#dragThis").draggable("option", "containment", $(this));
            }
        });
    }

    /*
        Initial signature box with dragable, resiable
    */
    function _changeSignBoxSize(box) {
        var w = Math.floor(box[0].offsetWidth / getDpi() * 72);
        var h = Math.floor(box[0].offsetHeight / getDpi() * 72);

        //Change _options value
        _options.width = w;
        _options.height = h;

        // Set value on input field
        $("#signbox-width").val(w);
        $("#signbox-height").val(h);
    }

    /**
     * Change signature visible type 
     */
    function changeVisibleType() {
        var signTypeObj = $("input[type=radio][name=sign-visible-type]");
        signTypeObj.change(function() {
            var type = $(this).val();
            switch (type) {
            case "1":
                // Signature visible text only
                signatureVisibleTextOnly();
                break;
            case "2":
                // Signature visible both text and image
                signatureVisibleBoth();
                break;
            default:
                // Signature visible image only
                signatureVisibleImageOnly();
                break;
            }
        });
    }

    /*
        Initial signature box with dragable, resiable
    */
    function signatureVisibleTextOnly() {
        _options.visibleType = SignatureVisibleType.ONLY_TEXT;
        _signImage.hide();
        _signText.removeAttr("style");
        _signText.css({ 'width': "100%" });
    }

    /*
        Initial signature box with dragable, resiable
    */
    function signatureVisibleBoth() {
        _options.visibleType = SignatureVisibleType.BOTH;
        _signImage.removeAttr("style");
        _signText.removeAttr("style");
        _signText.css({ 'width': "50%" });
    }

    /*
        Initial signature box with dragable, resiable
    */
    function signatureVisibleImageOnly() {
        _options.visibleType = SignatureVisibleType.ONLY_IMAGE;
        _signText.hide();
        _signImage.removeAttr("style");
        _signImage.css({ 'margin': "0px auto", 'float': "unset" });
        _signImage.show();
    }

    /*
        Change signature page
    */
    function changePageListener() {
        $("#pdf-sign-page-btn").click(function() {
            var p = Number($("#pdf-sign-page").val());
            if (isNaN(p) || p < 1 || p > VnptPdf.NumOfPage) {
                $("#pdf-sign-page").val(VnptPdf.BottomLeftX);
                return;
            }
            _options.page = p;

            // Reload view page
            _pdf.getPage(_options.page).then(function(page) {
                handlePage(page);
            });
        });
        $("#pdf-sign-font-btn").click(function() {
            var z = Number($("#pdf-sign-font").val());
            if (isNaN(z)) {
                alert("Cỡ chữ không hợp lệ");
                return;
            }
            _options.fontSize = z;
            var fz = "" + z * getDpi() / 72 + "px";
            $("#signature-text span").css({ 'font-size': fz });
        });
    }

    /*
        Change signature image
    */
    function changeImageListener() {
        $(document).on(
            "change",
            "#pdf-sign-image-file :file",
            function(event) {
                if (!$(this).get(0).files) {
                    return;
                }

                var input = $(this),
                    numFiles = input.get(0).files
                        ? input
                        .get(0).files.length
                        : 1,
                    label = input.val().replace(
                        /\\/g,
                        "/").replace(/.*\//, "");
                $("#pdf-sign-img-name").val(label);
                var f = input.get(0).files[0];

                var reader = new FileReader();
                reader.addEventListener("load",
                    function() {
                        _options.imageSrc = this.result.replace(_imagePrefix, "");
                        $("#signature-img").attr("src", _imagePrefix + _options.imageSrc);
                    });
                if (f) {
                    reader.readAsDataURL(f);
                }
            });
    }

    /*
    */
    function PdfOptions() {
        this.x = 0;
        this.y = 0;
        this.width = 0;
        this.height = 0;
        this.page = 1;
        this.fontSize = 10;
        this.imageSrc = "";
        this.rectangle = "";
        this.visibleType = SignatureVisibleType.BOTH;
    }

    return plugin;

}(VnptPdf || {}));

var pdfWorkingArea = "";
pdfWorkingArea += "";
pdfWorkingArea += "<div id='dpi' style='height: 1in; left: -100%; position: absolute; top: -100%; width: 1in;'><\/div>";
pdfWorkingArea += "<div class=\"pdf-working-area\">";
pdfWorkingArea += "    <div class=\"pdf-action-menu\">";
pdfWorkingArea += "        <div class=\"pdf-action-menu-content\">";
pdfWorkingArea += "            <fieldset>";
pdfWorkingArea += "                <legend>Vị trí chữ ký<\/legend>";
pdfWorkingArea += "                <div class=\"pdf-size-row\">";
pdfWorkingArea +=
    "                    <span class=\"pdf-size-lbl\">X Pos = <\/span><input class=\"sign-pos\" id=\"signbox-xpos\" type=\"text\" style=\"width: 50px;\" value=\"15\" \/>";
pdfWorkingArea +=
    "                    <span class=\"pdf-size-lbl\">Y Pos = <\/span><input class=\"sign-pos\" id=\"signbox-ypos\" type=\"text\" style=\"width: 50px;\" value=\"765\" \/>";
pdfWorkingArea += "                <\/div>";
pdfWorkingArea += "                <div class=\"pdf-size-row\">";
pdfWorkingArea +=
    "                    <span class=\"pdf-size-lbl\">Width = <\/span><input class=\"sign-pos\" id=\"signbox-width\" type=\"text\" style=\"width: 50px;\" value=\"200\" \/>";
pdfWorkingArea +=
    "                    <span class=\"pdf-size-lbl\">Height = <\/span><input class=\"sign-pos\" id=\"signbox-height\" type=\"text\" style=\"width: 50px;\" value=\"60\" \/>";
pdfWorkingArea += "                <\/div>";
pdfWorkingArea += "                <div class=\"pdf-size-row\">";
pdfWorkingArea += "                    <span>Trang chứa chữ ký:<\/span><br \/>";
pdfWorkingArea +=
    "                    <span class=\"pdf-size-lbl\"><\/span><input type=\"text\" id=\"pdf-sign-page\" style=\"width: 50px;\" value=\"1\" \/><span class=\"pdf-size-lbl pdf-all-page-lbl\" id=\"pdf-total-pages\"><\/span><button class=\"pdf-btn\" id=\"pdf-sign-page-btn\" style=\"width: 50px;\">Set<\/button>";
pdfWorkingArea += "                <\/div>";
pdfWorkingArea += "            <\/fieldset>";
pdfWorkingArea += "            <fieldset>";
pdfWorkingArea += "                <legend>Kiểu hiển thị chữ ký<\/legend>";
pdfWorkingArea += "                <div>";
pdfWorkingArea +=
    "                    <input type=\"radio\" name=\"sign-visible-type\" value=\"1\"> Chỉ hiển thị text<br \/>";
pdfWorkingArea +=
    "                    <input type=\"radio\" name=\"sign-visible-type\" value=\"2\" checked> Hiển thị text và hình ảnh<br \/>";
pdfWorkingArea +=
    "                    <input type=\"radio\" name=\"sign-visible-type\" value=\"3\"> Chỉ hiển thị hình ảnh<br \/>";
pdfWorkingArea += "                <\/div>";
pdfWorkingArea += "                <div class=\"pdf-size-row\">";
pdfWorkingArea +=
    "                    <span class=\"pdf-size-lbl\">Cỡ chữ: <\/span><input type=\"text\" id=\"pdf-sign-font\" style=\"width: 50px;\" value=\"8\" \/><span class=\"pdf-size-lbl\" ><\/span><button class=\"pdf-btn\" id=\"pdf-sign-font-btn\" style=\"width: 50px;\">Set<\/button>";
pdfWorkingArea += "                <\/div>";
pdfWorkingArea += "            <\/fieldset>";
pdfWorkingArea += "            <fieldset>";
pdfWorkingArea += "                <legend>Tùy chọn hình ảnh<\/legend>";
pdfWorkingArea +=
    "                <div class=\"pdf-input-group\"><input id=\"pdf-sign-img-name\" type=\"text\" class=\"pdf-file-name\"";
pdfWorkingArea += "                                   readonly=\"readonly\">";
pdfWorkingArea += "                    <span class=\"pdf-input-group-btn\">";
pdfWorkingArea += "                        <span class=\"pdf-btn pdf-btn-file\" id=\"pdf-sign-image-file\">";
pdfWorkingArea += "                            ... <input id=\"select-file\" name=\"SetupFile\" type=\"file\"";
pdfWorkingArea += "                                       accept=\"image\/x-png,image\/gif,image\/jpeg\" required>";
pdfWorkingArea += "                        <\/span>";
pdfWorkingArea += "                    <\/span> ";
pdfWorkingArea += "                <\/div>";
pdfWorkingArea += "            <\/fieldset>";
pdfWorkingArea += "            <div style=\"text-align: center; padding-top: 15px;\">";
pdfWorkingArea += "                <button class=\"pdf-btn\" id=\"pdf-complete\">Ký dữ liệu<\/button>";
pdfWorkingArea += "            <\/div>";
pdfWorkingArea += "            <div style=\"text-align: center; padding-top: 15px;\">";
pdfWorkingArea += "                <button class=\"pdf-btn pdf-btn-error\" id=\"pdf-cancel\">Hủy<\/button>";
pdfWorkingArea += "            <\/div>";
pdfWorkingArea += "        <\/div>";
pdfWorkingArea += "    <\/div>";
pdfWorkingArea += "    <div class=\"pdf-page\">";
pdfWorkingArea += "        <canvas id=\"dropHere\" class=\"pdf-viewport\"><\/canvas>";
pdfWorkingArea += "        <div id=\"dragThis\">";
pdfWorkingArea += "            <img id=\"signature-img\" src=\"\" \/>";
pdfWorkingArea += "            <div id=\"signature-text\">";
pdfWorkingArea += "                <span>Ký bởi: Tên chủ chứng thư<\/span>";
pdfWorkingArea += "                <br \/>";
pdfWorkingArea += "                <span>Thời gian ký: dd\/MM\/yyyy<\/span>";
pdfWorkingArea += "            <\/div>";
pdfWorkingArea += "        <\/div>";
pdfWorkingArea += "    <\/div>";
pdfWorkingArea += "<\/div>";