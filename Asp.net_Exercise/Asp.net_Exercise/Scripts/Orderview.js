function Detail(e) {
    var Id = e.data.Id;
    var oid = $("#" + Id).data("oid");
    $.ajax({
        url: "/api/membersapi/GetOrderDetail?oid=" + oid,
        type: "get",
        success: function (json) {
            console.log(json);
            $("#detail tbody tr").remove();
            var T = 0;
            for (var i = 0; json.length > i; i++) {
                $("#detail").append(
                    "<tr><td class='col-md-3' style='text-align:left'>" +
                    "<img class='img-rounded prod_img' src='/UpdataFiles/" + json[i].img + "' />" +
                    "<h5 class='prod_name'>" + json[i].name + "</h5><span class='feature'>" + json[i].feature + "</span></td>" +
                    "<td>" + json[i].qty + "</td><td>NT$" + json[i].price + "</td><td>NT$" + json[i].total + "</td></tr>"
                )
                T += json[i].total;
            }
            $("#Order_Total").text("NT$" + T);
        }
    })

}

$(function () {

    for (var i = 0; L > i; i++) {
        $("#list tbody").append(
            "<tr><td>" + json[i].name + "</td>" +
            "<td>" + json[i].phone + "</td>" +
            "<td>" + json[i].email + "</td>" +
            "<td>" + json[i].store + "</td>" +
            "<td>" + json[i].tradeNo + "</td>" +
            "<td>" + json[i].time + "</td>" +
            "<td><input type='button' class='btn btn-default' data-oid='" + json[i].oid + "' id='" + i + "' value='詳細' /><tr/>"
        );
        $("#" + i).on("click", { Id: i }, Detail);
    }
})