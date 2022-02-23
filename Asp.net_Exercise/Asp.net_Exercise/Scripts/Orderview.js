function Detail(e) {
    let oid = $("#" + e.data.Id).data("oid");
    $.ajax({
        url: "/api/membersapi/GetOrderDetail?oid=" + oid,
        type: "get",
        success: json => {
            return json;
        }
    }).then(e => {
        $("#detail").css("display", "table");
        $("#detail tbody tr").remove();
        let T = 0;
        for (let i = 0; e.length > i; i++) {
            $("#detail").append(
                "<tr><td class='col-md-3' style='text-align:left'>" +
                "<img class='img-rounded prod_img' src='/UpdataFiles/" + e[i].img + "' />" +
                "<h5 class='prod_name'>" + e[i].name + "</h5><span class='feature'>" + e[i].feature + "</span></td>" +
                "<td>" + e[i].qty + "</td><td>NT$" + e[i].price + "</td><td>NT$" + e[i].total + "</td></tr>"
            )
            T += e[i].total;
        }
        $("#Order_Total").text("NT$" + T);
    })

}

function redirect() {
    $("#alert").text("尚未有訂單紀錄，即將回到首頁").show().delay(3000).fadeOut();
    setTimeout(() => {
        location.href = "/home/index";
    }, 3000);
}

$(function () {
    (!json.length) ? redirect() : null;
    for (let i = 0; json.length > i; i++) {
        let pay = json[i].pay == 1 ? "付款完成" : "付款失敗";
        $("#list tbody").append(
            "<tr><td>" + json[i].name + "</td>" +
            "<td>" + json[i].phone + "</td>" +
            "<td>" + json[i].email + "</td>" +
            "<td>" + json[i].store + "</td>" +
            "<td>" + json[i].tradeNo + "</td>" +
            "<td>" + json[i].time + "</td>" +
            "<td>" + pay + "</td>" +
            "<td><input type='button' class='btn btn-default' data-oid='" + json[i].oid + "' id='" + i + "' value='詳細' /><tr/>"
        );
        $("#" + i).on("click", { Id: i }, Detail);
    }
})