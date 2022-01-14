$(function () {
    $.ajax({
        url: "/api/Analyticsapi/test",                                      //須加上day維度否則無法計算瀏覽器數量
        data: { start: "7daysago,31daysago", end: "today,today", dimension: "ga:day,ga:browser", metric: "ga:newusers" },
        method : "get",
        success: e => {
            console.log(e);
            var data = {Chrome:0, Edge:0, Firefox:0 }
            for (let i = 0; e[0].reports[0].data.rows.length > i; i++) {
                e[0].reports[0].data.rows[i].dimensions[1] == "Chrome" ? data.Chrome++ : null;
                e[0].reports[0].data.rows[i].dimensions[1] == "Edge" ? data.Edge++ : null;
                e[0].reports[0].data.rows[i].dimensions[1] == "Firefox" ? data.Firefox++ : null;
            }
            Flot(data);
            $("#U7d").text(e[0].reports[0].data.totals[0].values[0]);
            $("#U31d").text(e[0].reports[0].data.totals[1].values[0]);
        }
    })
})

function Flot(data) {
    let Flotdata = [
        { label: "Google", data: data.Chrome, color: "#17a2b8" },
        { label: "Edge", data: data.Edge, color: "#17a2b8" },
        { label: "Firefox", data: data.Firefox, color: "#7D0096" }
    ];
    $.plot(("#ShowFlot"), Flotdata, {
        series: {
            pie: {
                show: true,
                radius: 300,
                label: {
                    show: true,
                    radius: 0.7,
                    formatter: function (label, series) {
                        return '<div class="FlotLabel">' + label + '<br/>' + Math.round(series.percent) + '%</div>';
                    },
                    threshold: 0.1
                }
            }
        },
        legend: {
            show: false,
        }
    });
    for (let i = 0; Flotdata.length > i; i++) {
        $("#Flotlabelfather").append('<div><p class="Flot-color" style="background-color:' + Flotdata[i].color + '"></p><div class="Flot-text">' + Flotdata[i].label +': '+ Flotdata[i].data + '</div></div>');
    }
}