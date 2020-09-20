function DelKeep(Pid) {
    var c = confirm("確定移除該收藏嗎?");
    if (c) {
        $.ajax({
            url: "/members/DelKeep?Pid=" + Pid,
            success: function (data) {

                $("#Father li").remove();
            }
        })
    }
}
$(function () {
    var L = data.length;
    for (var i = 0; L > i; i++) {
        $('#Father').append(
            "<li class='prod' id='" + data[i].prod.Id + "'>" +
            "<img src='/Img/btn_del.png' class='delbtn' onclick='DelKeep(" + data[i].prod.Id + ")' id='" + data[i].prod.Id + "'/>" +
            "<a href='/home/ProdDetails?Pid=" + data[i].prod.Id + "'><img src='/UpdataFiles/" + data[i].img.FileName + "' class='img' /></a>" +
            "<p class='name'>" + data[i].prod.Name + "</p>" +
            "<p class='price'>NT$" + data[i].prod.Price + "</p>" +
            "</li>"
        )
    }

})