$(function () {
    $("#submit").on("click", () => {
        let o = $("#old").val();
        let n = $("#new").val();
        let c = $("#check").val();
        n == c ? $("form").submit() : $("#alert").text("新密碼輸入不一致").show().delay(2000).fadeOut();
    })
})