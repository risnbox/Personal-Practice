$(function () {
    $("#Carousel").carousel({ interval: 3000 });
    //透過api取得資料後非同步新增至頁面
    $.ajax({
        url: "/api/homeapi/index",
        type: "get",
        success: e => {
            return e
        },
        error: e => {
            alert(e.responseJSON.Message);
        }
    }).then(e => {
        var L = e.length;
        for (var i = 1; L > i; i++) {
            $("#Father").append("<div id='Prod'><a id='P" + i + "' href='javascript:void(0);' onclick='GotoView(" + e[i].prod.Id + ")'>" +
                "<img id='Pimg' src='/UpdataFiles/" + e[i].img.FileName + "' / ></a><p id='Ptext'>" + e[i].prod.Name +
                "</p><p id='Ptext'>NT$" + e[i].prod.Price + "</p></div>"
            )
        };
    })
});
function GotoView(id) {
    window.open("/Home/ProdDetails?Pid=" + id, "_blank");
}