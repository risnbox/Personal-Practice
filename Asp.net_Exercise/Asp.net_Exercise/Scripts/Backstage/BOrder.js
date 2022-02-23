$(function () {
    $("#submit").on('click', Table);
    $("#remove").on('click', () => {
        $("table tbody tr").remove();
        $(".form-control").val("");
    });
    let height = $("table").height();
    $("table tbody").css("height", height - 56);
    Table();
})  
function Table() {
    let id = Cookies.get('data');
    (id) ? $("input[name='mid']").val(id) : null;
    Cookies.remove("data");
    let data = $("#form1").serialize();
    $("table tbody tr").remove();
    $.ajax({
        url: "/api/borderapi/all",
        data: data,
        type: "get",
        success: e => {
        }
    }).then(e => {
        if (e.length == 0) { $("#alert").text("查無資料").show().delay(2000).fadeOut() }
        else {
            for (let i = 0; e.length >= i; i++) {
                let TradeNo = (e[i].TradeNo == "null") ? "付款失敗" : e[i].TradeNo;
                $("table tbody").append("<tr><td id='bottom-td'>" + e[i].MerchantTradeNo + "</td>" +
                    "<td id='bottom-td'>" + e[i].User_Id + "</td>" +
                    "<td id='bottom-td'>" + e[i].TradeDate.substr(0, 10) + "</td>" +
                    "<td id='bottom-td'>" + TradeNo + "</td>" +
                    "<td id='bottom-td'>" + e[i].Total + "</td>" +
                    "<td id='bottom-td'><input id='B" + i + "' type='button' class='btn btn-dark' value='詳細資料' /></tr>"
                )
                $("#B" + i).on('click', { Oid: e[i].MerchantTradeNo }, openwindow);
            }
        }
    })
}

function openwindow(e) {
    window.open("/BackStage/Order/Window?Oid=" + e.data.Oid, "test", config = "width=600,height=300,left=600,top=150");
}