$(function () {
    $.ajax({
        url: "/api/homeapi/proddetalis?pid=" + pid,
        type: "get",
        success: e => {
            if (e[0].keep) {
                $('#keep').removeClass('keep');
                $('#keep').addClass('keepon');
            }
            $("#Img").append("<img src='/UpdataFiles/" + e[0].img.FileName + "' style=' width:100%;height:500px;' />");
            $("#Title").append("<p id='title'>" + e[0].prod.Name + "</p>");
            $("#currency").after("<span id='price'>" + e[0].prod.Price + "</span>");
            $("#Img2").before("<img src='/UpdataFiles/" + e[2].img.FileName + "' style='width:760px;height:910px;' />");
        },
        error: e => {
            $("#alert").text(e.responseJSON.Message).show().delay(3000).fadeOut();
        }
    });
});

function SelectColor(Color) {
    $("img").removeClass("select");
    $("#" + Color).addClass("select");
}

function SelectSize(Size) {
    $("a").removeClass("Select");
    $("#" + Size).addClass("Select");
}

function AddCart() {
    var color = $(".select").attr("id");
    var size = $(".Select").attr("id");
    //Javascript邏輯判斷機制與一般語言不同詳:https://ithelp.ithome.com.tw/articles/10191343
    (color && size) ? (member) ? AddAJAX(color, size)
        : $("#alert").text('請先登入').show().delay(3000).fadeOut()
        : $("#alert").text('請選擇顏色及尺寸').show().delay(3000).fadeOut();
}

function AddAJAX(color, size) {
    $.ajax({
        url: "/api/homeapi/AddCart?pid=" + pid + "&color=" + color + "&size=" + size,
        success: () => {
            $("#alert").text("成功新增").show().delay(1500).fadeOut();
            Cart();
        },
        error: e => {
            $("#alert").text(e.responseJSON.Message).show().delay(3000).fadeOut();
        }
    });
}

function Keep() {
    (member) ? ($("#keep").hasClass('keepon')) ? KeepAJAX(true) : KeepAJAX(false)
        : $("#alert").text('登入後才能使用收藏功能').show().delay(3000).fadeOut();
}

function KeepAJAX(boolean) {
    if (boolean) {
        $.ajax({
            url: "/api/membersapi/DelKeep?Pid=" + pid,
            success: () => {
                $('#keep').removeClass('keepon');
                $('#keep').addClass('keep');
                $("#alert").text('移除成功').show().delay(3000).fadeOut();
            }
        });
    }
    else {
        $.ajax({
            url: "/api/membersapi/Keep?Pid=" + pid,
            success: () => {
                $('#keep').removeClass('keep');
                $('#keep').addClass('keepon');
                $("#alert").text('新增成功').show().delay(3000).fadeOut();
            }
        });
    }
}

function Cart() {
    $.ajax({
        url: "/api/cartapi/Partial",
        success: e => {
            $(".dropdown-menu tbody tr").remove();
            $("#quantity").text("");
            for (var i = 0; e.length > i; i++) {
                $('#Tb').append(
                    "<tr><td><a href='/Home/proddetails?pid=" + e[i].pid + "'>" + e[i].name + "</a></td>" +
                    "<td>" + e[i].color + "</td>" +
                    "<td>" + e[i].size + "</td>" +
                    "<td>" + e[i].quantity + "</td></tr>"
                )
            }
            $('#quantity').append(e.length);
            if (!e.length) {
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
}

