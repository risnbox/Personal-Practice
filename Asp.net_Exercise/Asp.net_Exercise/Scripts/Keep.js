function DelKeep(Pid) {
    var c = confirm("確定移除該收藏嗎?");
    if (c) {
        $.ajax({
            url: "/api/membersapi/DelKeep?Pid=" + Pid,
            success: () => {
                $("#Father li").remove();
            }
        })
    }
}

function redirect() {
    $("#alert").text("尚未有任何收藏，即將回到首頁").show().delay(3000).fadeOut();
    setTimeout(() => {
        location.href = "/home/index";
    }, 3000);
}

$(function () {
    (e.length) ? null : redirect();
    for (let i = 0; e.length > i; i++) {
        $('#Father').append(
            "<li class='prod' id='" + e[i].prod.Id + "'>" +
            "<img src='/Img/btn_del.png' class='delbtn' onclick='DelKeep(" + e[i].prod.Id + ")' id='" + e[i].prod.Id + "'/>" +
            "<a href='/home/ProdDetails?Pid=" + e[i].prod.Id + "'><img src='/UpdataFiles/" + e[i].img.FileName + "' class='img' /></a>" +
            "<p class='name'>" + e[i].prod.Name + "</p>" +
            "<p class='price'>NT$" + e[i].prod.Price + "</p>" +
            "</li>"
        )
    }
})