$(function () {
    $.ajax({
        url: "/api/Analyticsapi/test",                           //須加上day維度否則無法計算瀏覽器數量
        data: { start: "7daysago,31daysago", end: "today,today", dimension: "ga:day,ga:browser,ga:region", metric: "ga:newusers,ga:users" },
        method : "get",
        success: e => {
            var data = {
                Browser: {
                    Chrome: 0, Edge: 0, Firefox: 0,
                },
                City: {
                    Keelung: 0, NewTaipei: 0, Taipei: 0, Taoyuan: 0, HsinchuCounty: 0,
                    Hsinchu: 0, Taichung: 0, Kaohsiung: 0
                }
                 
            }
            for (let i = 0; e[0].reports[0].data.rows.length > i; i++) {
                let b = e[0].reports[0].data.rows[i].dimensions[1];
                let c = e[0].reports[0].data.rows[i].dimensions[2];
                b == "Chrome" ? data.Browser.Chrome++ : b == "Edge" ? data.Browser.Edge++ : b == "Firefox" ? data.Browser.Firefox++ : null;
                c == "Keelung City" ? data.City.Keelung++ : c == "Taichung City" ? data.City.Taichung++ : c == "New Taipei City" ?
                data.City.NewTaipei++ : c == "Taipei City" ? data.City.Taipei++ : c == "Taoyuan City" ? data.City.Taoyuan++ : c == "Hsinchu County" ?
                data.City.HsinchuCounty : c == "Hsinchu City" ? data.City.Hsinchu++ : c == "Kaohsiung City" ? data.City.Kaohsiung++ : null;
            }
            FlotPie(data);
            FlotBars(data);
            $("#U7d").text(e[0].reports[0].data.totals[0].values[0]);
            $("#U31d").text(e[0].reports[0].data.totals[1].values[0]);
        }
    })
})

function FlotPie(data) {
    let Flotdata0 = [
        { label: "Google", data: data.Browser.Chrome, color: "#17a2b8" },
        { label: "Edge", data: data.Browser.Edge, color: "#dc3545" },
        { label: "Firefox", data: data.Browser.Firefox, color: "#7D0096" }
    ];
    $.plot(("#ShowFlot-0"), Flotdata0, {
        series: {
            pie: {//圖表設定
                show: true,
                radius: 600,//圓角
                label: {//圖表內部標籤設定
                    show: true,
                    align: "center",
                    radius: 0.7,
                    formatter: function (label, series) {
                        return '<div class="FlotLabel">' + label + '<br/>' + Math.round(series.percent) + '%</div>';
                    },
                    threshold: 0.1
                }
            }
        },
        legend: {//說明標籤設定 此處選擇另行新增因此false
            show: false,
        },
        grid: {//觸發提示框事件設定
            hoverable: true,
            borderWidth: 2,
            backgroundColor: { colors: ["#ffffff", "#EDF5FF"] }
        }
    });
    //右側標籤
    for (let i = 0; Flotdata0.length > i; i++) {
        $("#Flotlabelfather-0").append('<div><p class="Flot-color" style="background-color:' + Flotdata0[i].color + '"></p><div class="Flot-text">' + Flotdata0[i].label +': '+ Flotdata0[i].data + '</div></div>');
    }
    //提示框事件
    let previousPoint = null, previousLabel = null;//用來判別事件前後的標籤文字
    //flot的function
    $.fn.UseTooltip = function () {
        //綁定plothover事件
        $(this).on("plothover", function (event, pos, item) {//雖用不到event,pos但不填會抓不到item
            if (item) {
                //若觸發元素不同&離開hover狀態 則重製提示框
                if ((previousLabel != item.series.label) || (previousPoint != item.dataIndex)) {
                    previousPoint = item.dataIndex;
                    previousLabel = item.series.label;
                    $("#tooltip").remove();
                    var color = item.series.color;
                    showTooltip(pos.pageX,
                        pos.pageY,
                        color,
                        "<strong>" + item.series.label + "</strong>: <strong>" + item.datapoint[1][0][1] + "</strong> 位");
                }
            } else {
                $("#tooltip").remove();
                previousPoint = null;
            }
        });
    };
    $("#ShowFlot-0").UseTooltip();
}

function FlotBars(data) {
    let ldata = [[0, data.City.Keelung], [1, data.City.NewTaipei], [2, data.City.Taipei], [3, data.City.Taichung], [4, data.City.Kaohsiung], [5, data.City.Taoyuan], [6, data.City.Hsinchu], [7, data.City.HsinchuCounty]];
    let Flotdata1 = [{ label: "用戶總數", data: ldata, color: "#5482FF" }];
    let bottomtag = [[0, "基隆"], [1, "新北"], [2, "台北"], [3, "台中"], [4, "高雄"], [5, "桃園"], [6, "新竹市"], [7, "新竹縣"]];

    $.plot($("#flot-placeholder"), Flotdata1, {
        series: {
            bars: {//圖表設定
                show: true,
                align: "center",
                barWidth: 0.5
            }
        },
        xaxis: {//x軸標籤設定 需要額外插件flot.axislabels
            axisLabel: "地區名字",
            axisLabelUseCanvas: true,
            axisLabelFontSizePixels: 12,
            axisLabelFontFamily: 'Verdana, Arial',
            axisLabelPadding: 10,
            ticks: bottomtag
        },
        legend: {//說明標籤屬性設定
            noColumns: 0,
            labelBoxBorderColor: "#000000",
            position: "nw"
        },
        grid: {//觸發提示框事件設定
            hoverable: true,
            borderWidth: 2,
            backgroundColor: { colors: ["#ffffff", "#EDF5FF"] }
        }
    });

    //提示框事件
    let previousPoint = null, previousLabel = null;//用來判別事件前後的標籤文字
    //flot的function
    $.fn.UseTooltip = function () {
        //綁定plothover事件
        $(this).on("plothover", function (event, pos, item) {//雖用不到event但不填會抓不到item
            if (item) {
                //若觸發元素不同&離開hover狀態 則重製提示框
                if ((previousLabel != item.series.label) || (previousPoint != item.dataIndex)) {
                    previousPoint = item.dataIndex;
                    previousLabel = item.series.label;
                    $("#tooltip").remove();
                    var x = item.datapoint[0];//(觸發的)當前停留的X標籤值
                    var y = item.datapoint[1];//(觸發的)當前停留的Y標籤值
                    var color = item.series.color;
                    showTooltip(pos.pageX,
                        pos.pageY,
                        color,
                        "<strong>" + item.series.label + "</strong><br>" + item.series.xaxis.ticks[x].label + " : <strong>" + y + "</strong> 位");
                }
            } else {
                $("#tooltip").remove();
                previousPoint = null;
            }
        });
    };
    $("#flot-placeholder").UseTooltip();
}

//設定觸發Flothover(Flot內件事件)時如何顯示提示框
function showTooltip(x, y, color, contents) {
    $('<div id="tooltip">' + contents + '</div>').css({
        position: 'absolute',
        display: 'none',
        top: y - 40,
        left: x - 120,
        border: '2px solid ' + color,
        padding: '3px',
        //屬性包含 - 因此用字串寫key
        'font-size': '9px',
        'border-radius': '5px',
        'background-color': '#fff',
        'font-family': 'Verdana, Arial, Helvetica, Tahoma, sans-serif',
        opacity: 0.9
    }).appendTo("body").fadeIn(200);
}