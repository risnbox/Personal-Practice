$(function () {
    $("#btn").on("click", function () {
        let password = $("#psw").val();
        let check = $("#check").val();
        (password && check) ?
            (password == check) ?
                $.ajax({
                    url: "/api/membersapi/Resetpassword",
                    type: "post",
                    data: $("form").serialize(),
                    success: e => {
                        $("#alert").text("修改成功，請以新密碼登入").show().delay(2000).fadeOut();
                        setTimeout(() => {
                            location.href = "/home/index";
                        }, 2000);
                    },
                    error: e => { $("#alert").text(e.responseJSON.Message).show().delay(3000).fadeOut(); }
                })
                : $("#alert").text("確認密碼輸入錯誤").show().delay(3000).fadeOut()
        : $("#alert").text("密碼不可為空白").show().delay(3000).fadeOut();
    })
})