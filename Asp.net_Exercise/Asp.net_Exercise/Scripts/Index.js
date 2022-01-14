﻿$(function () {
    $("#Carousel").carousel({ interval: 3000 });
    if (err) {
        $("#alert").text(err).show().delay(1100).fadeOut();
    }
    //透過api取得資料
    $.ajax({
        url: "/api/homeapi/index",
        type: "get",
        success: e => {
            var L = e.length;
            for (let i = 1; L > i; i++) {
                $("#Father").append("<div id='Prod'><a id='P" + i + "' href='javascript:void(0);' onclick='GoView(" + e[i].prod.Id + ")'>" +
                    "<img id='Pimg' src='/UpdataFiles/" + e[i].img.FileName + "' / ></a><p id='Ptext'>" + e[i].prod.Name +
                    "</p><p id='Ptext'>NT$" + e[i].prod.Price + "</p></div>"
                )
            };
        },
        error: e => {
            $('#alert').text(e.responseJSON.Message).show().delay(1100).fadeOut();
        }
    });
});

function GoView(id) {
    window.open("/Home/ProdDetails?Pid=" + id, "_blank");
}