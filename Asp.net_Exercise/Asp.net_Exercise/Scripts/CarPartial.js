$(function () {
    $.ajax({
        url: "/api/cartapi/Partial",
        success: e => {
            let L = e.length;
            for (var i = 0; L > i; i++) {
                $('#Tb').append(
                    "<tr><td><a href='/Home/proddetails?pid=" + e[i].pid + "'>" + e[i].name + "</a></td>" +
                    "<td>" + e[i].color + "</td>" +
                    "<td>" + e[i].size + "</td>" +
                    "<td>" + e[i].quantity + "</td></tr>"
                )
            }
            $('#quantity').append(L);
            if (!L) {
                $(".btn_cart").css('visibility', 'hidden');
                $('#Tb').append("<tr><td colspan='4'><h4>購物車內尚無商品</h4></td></tr>");
            }
            else {
                $(".btn_cart").css('visibility', 'visible');
            }
        },
        error: e => {
            alert(e.responseJSON.Message);
        }
    });
})