$(function () {
    setInterval("CheckPay()", 2000);//每兩秒進行一次函式 參考資料:https://www.itread01.com/content/1544697736.html
})
function CheckPay() {
    $.ajax({
        url: "/api/cartapi/CheckPay",
        type: "get",
        success: e => {
            if (e != "") {
                $('#alert').text('付款成功!即將跳轉至訂單頁面').show().delay(1500).fadeOut();//通知頁面運用 參考網址 :https://blog.csdn.net/qq_32217519/article/details/73499562?utm_medium=distribute.pc_relevant.none-task-blog-BlogCommendFromMachineLearnPai2-3.channel_param&depth_1-utm_source=distribute.pc_relevant.none-task-blog-BlogCommendFromMachineLearnPai2-3.channel_param
                setTimeout(function () { location.href = "/members/orderview"; }, 1700);
            }
        }
    })
}