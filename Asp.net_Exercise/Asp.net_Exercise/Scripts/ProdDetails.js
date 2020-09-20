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
            url: "/Home/AddCart?pid=" + data[0].prod.Id + "&color=" + color + "&size=" + size,
            success: function (data) {
                if (data == 1) {
                    alert('請先登入');
                }
                if (data == 2) {
                    alert('帳號尚未驗證');
                    location.href = '/members/EmailValidationView';
                }
                if (data == 0) {
                    alert('成功新增');
                    window.location.reload();
                }
            },
            error: function () {
                alert('伺服器錯誤');
            }
        })
    }
    else {
        alert('請選擇顏色及尺寸');
    }
}
function Keep() {
    if (member == null) {
        alert('登入後才能使用收藏功能')
    }
    else {
        if ($('#keep').hasClass('keepon')) {
            $.ajax({
                url: "/members/DelKeep?Pid=" + data[0].prod.Id,
                success: function (data) {
                    $('#keep').removeClass('keepon');
                    $('#keep').addClass('keep');
                }
            })
        }
        else {
            $.ajax({
                url: "/members/Keep?Pid=" + data[0].prod.Id,
                success: function (data) {
                    $('#keep').removeClass('keep')
                    $('#keep').addClass('keepon');
                }
            })
        }
    }
}
$(function () {
    if (data[0].keep != null) {
        $('#keep').removeClass('keep')
        $('#keep').addClass('keepon');
    }
    $("#Img").append(
        "<img src='/UpdataFiles/" + data[0].img.FileName + "' style=' width:100%;height:500px;' />"
    )
    $("#Title").append(
        "<p id='title'>" + data[0].prod.Name + "</p>"
    )
    $("#currency").after(
        "<span id='price'>" + data[0].prod.Price + "</span>"
    )
    $("#Img2").before(
        "<img src='/UpdataFiles/" + data[2].img.FileName + "' style='width:760px;height:910px;' />"
    )
})