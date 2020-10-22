function SelectColor(Color) {
    $("img").removeClass("select")
    $("#" + Color).addClass("select")
}
function SelectSize(Size) {
    $("a").removeClass("Select")
    $("#" + Size).addClass("Select")
}
function AddCart() {
    var color = $(".select").attr("id");
    var size = $(".Select").attr("id");
    //Javascript邏輯判斷機制與一般語言不同詳:https://ithelp.ithome.com.tw/articles/10191343
    if (color != null && size != null) {
        $.ajax({
            url: "/api/homeapi/AddCart?pid=" + pid + "&color=" + color + "&size=" + size,
            success: e => {
                alert('成功新增');
                window.location.reload();
            },
            error: e => {
                alert(e.responseJSON.Message);
            }
        })
    }
    else {
        alert('請選擇顏色及尺寸');
    }
}
function Keep() {
    if (member == null) {
        alert('登入後才能使用收藏功能');
    }
    else {
        if ($('#keep').hasClass('keepon')) {
            $.ajax({
                url: "/api/membersapi/DelKeep?Pid=" + pid,
                success: function (data) {
                    $('#keep').removeClass('keepon');
                    $('#keep').addClass('keep');
                }
            })
        }
        else {
            $.ajax({
                url: "/api/membersapi/Keep?Pid=" + pid,
                success: function (data) {
                    $('#keep').removeClass('keep')
                    $('#keep').addClass('keepon');
                }
            })
        }
    }
}
$(function () {
    $.ajax({
        url: "/api/homeapi/proddetalis?pid=" + pid,
        type: "get",
        success: e => {
            return e
        },
        error: e => {
            alert(e.responseJSON.Message);
        }
    }).then(e => {
        if (e[0].keep != null) {
            $('#keep').removeClass('keep');
            $('#keep').addClass('keepon');
        }
        $("#Img").append(
            "<img src='/UpdataFiles/" + e[0].img.FileName + "' style=' width:100%;height:500px;' />"
        )
        $("#Title").append(
            "<p id='title'>" + e[0].prod.Name + "</p>"
        )
        $("#currency").after(
            "<span id='price'>" + e[0].prod.Price + "</span>"
        )
        $("#Img2").before(
            "<img src='/UpdataFiles/" + e[2].img.FileName + "' style='width:760px;height:910px;' />"
        )
    })
})