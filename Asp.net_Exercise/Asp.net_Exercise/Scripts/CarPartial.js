$(function () {

    var L = Cdata.length;
    for (var i = 0; L > i; i++) {
        $('#Tb').append(
            "<tr><td><a href='/Home/proddetails?pid=" + Cdata[i].pid + "'>" + Cdata[i].name + "</a></td>" +
            "<td>" + Cdata[i].color + "</td>" +
            "<td>" + Cdata[i].size + "</td>" +
            "<td>" + Cdata[i].quantity + "</td></tr>"
        )
    }
    $('#quantity').append(L);
    if (L == 0) {
        $(".btn_cart").css('visibility', 'hidden');
        $('#Tb').append(
            "<tr><td colspan='4'><h4>購物車內尚無商品</h4></td></tr>"
        )
    }
    else {
        $(".btn_cart").css('visibility', 'visible');
    }
})