﻿function Count(Id) {

    var p = $("#P" + Id).text();
    var P = parseInt(p, 10);
    var q = $("#Q" + Id).text();
    var Q = parseInt(q, 10);
    var data = P * Q;
    $("#T" + Id).text(data);
    Total();

}

function Add(e) {

    var Id = e.data.Id;
    var x = $("#Q" + Id).text();
    var N = $("#N" + Id).text();
    var F = $("#F" + Id).text();
    var Q = parseInt(x, 10);
    Q++;
    $.ajax({
        url: "/cart/QtyAdd?Name=" + N + "&Feature=" + F,
        type: "get",
        success: function (data) {
            if (data == "") {
                $('#Q' + Id).text(Q);
                Count(Id);
                $("#C" + Id).removeAttr("disabled");
            }
            else {
                alert(data);
            }
        }
    })


}

function Cut(e) {

    var Id = e.data.Id;
    var x = $("#Q" + Id).text();
    var N = $("#N" + Id).text();
    var F = $("#F" + Id).text();
    var Q = parseInt(x, 10);
    $.ajax({
        url: "/cart/QtyCut?Name=" + N + "&Feature=" + F,
        type: "get",
        success: function (data) {
            if (data == "") {
                Q--;
                $('#Q' + Id).text(Q);
                Count(Id);
                if (Q <= 1) {
                    $("#C" + Id).attr("disabled", "");
                }
            }
            else {
                alert(data);
            }
        }
    })


}

function Total() {

    var L = $("#CO_tbody tr:last").data("id");
    var T = 0;
    for (var i = 0; L >= i; i++) {
        var t = $("#T" + i).text()
        t = parseInt(t, 10);
        T += t;
    }
    $("#CO_Totalprice").text("NT$" + T);

}

function Delete(e) {

    var T = confirm("確定移除該商品嗎?");
    if (T == true) {
        var Id = e.data.Id;
        var N = $("#N" + Id).text();
        var F = $("#F" + Id).text();
        $.ajax({
            url: "/cart/DeleteCart?Name=" + N + "&Feature=" + F,
            success: function (e) {
                if (e != "") {
                    alert(e);
                }
                else {
                    $("#" + Id).remove();
                    window.location.reload();
                }
            }
        });
    }

}

function NextStep1_2() {

    $("#step1").css("display", "none");
    $("#step2").css("display", "block")
    $("#Step1-text .CO_span1").removeClass("CO_span1Select");
    $("#Step1-text .CO_span2").removeClass("CO_span2Select");
    $("#Step2-text .CO_span1").addClass("CO_span1Select");
    $("#Step2-text .CO_span2").addClass("CO_span2Select");

}
//其實應該直接用Submit處理表單
function Submitorder() {

    var c = confirm("確定提交訂單嗎?");
    var s = $("#store").val();
    if (c) {
        var Formdata = $('form').serialize();
        $.ajax({
            url: "/cart/submitorder",
            type: "post",
            data: Formdata + "&Sname=" + s,
            success: function (e) {
                if (e != "") {
                    e = JSON.parse(e);
                    var L = 0;
                    for (var x in e) {
                        L++;
                    }
                    for (var i = 0; L > i; i++) {
                        $("#" + e[i].key).text(e[i].message);
                    }
                }
                else {
                    alert("訂單提交成功,點擊確定回到首頁");
                    location.href = "/home/Index";
                }
            }
        })
    }
}

$(function () {

    $("div #step2").css("display", "none");
    var L = data.length;
    var T = 0;
    for (var i = 0; L > i; i++) {
        $("#CO_tbody").append(
            "<tr data-id='" + i + "' id='" + i + "'><td class'col-md-3'><a><img class='img-rounded'style='width:70px;height:70px;float:left;'src='/UpdataFiles/" + data[i].Img + "' /></a>" +
            "<h5 id='N" + i + "'>" + data[i].Name + "</h5><span class='CO_span4'id='F" + i + "'>" + data[i].Feature + "</span></td>" +
            "<td class='col-md-2' style='padding-left:0;text-align:left;'><button id='C" + i + "' class='btn btn-default CO-'>-</button><span id='Q" + i + "' class='CO_Qty'>" + data[i].Qty +
            "</span><button id='A" + i + "' class='btn btn-default CO+'>+</button></td><td class='col-md-1'>NT$<span id='P" + i + "'>" + data[i].Price + "</span></td>" +
            "<td class='col-md-1'>NT$<span id='T" + i + "'>" + data[i].Total + "</span></td><td class='col-md-1'><a id='D" + i + "' style='float:right;padding-right:10px;text-decoration:none;color:#bbb;cursor:pointer'>X</a></td>" +
            "</tr>"
        )
        var t = parseInt(data[i].Total, 10);
        T = T + t;
        $("#A" + i).on("click", { Id: i }, Add);
        $("#C" + i).on("click", { Id: i }, Cut);
        $("#D" + i).on("click", { Id: i }, Delete);
        var q = $("#Q" + i).text();
        var Q = parseInt(q);
        if (Q <= 1) {
            $("#C" + i).attr("disabled", "")
        }

    }
    $("#CO_Totalprice").text("NT$" + T);
    $("#Nextstep1").on("click", NextStep1_2);
    $("#Submitorder").on("click", Submitorder);

})