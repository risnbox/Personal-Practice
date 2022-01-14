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
            setTimeout(() => {
                window.location.reload();
            }, 2000);
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
            }
        });
    }
    else {
        $.ajax({
            url: "/api/membersapi/Keep?Pid=" + pid,
            success: () => {
                $('#keep').removeClass('keep');
                $('#keep').addClass('keepon');
            }
        });
    }
}

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