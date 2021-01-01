function Count(Id) {//計算品項價錢

    var p = $("#P" + Id).text();//取得價錢
    var P = parseInt(p, 10);//序列化 text=>int
    var q = $("#Q" + Id).text();//取得數量
    var Q = parseInt(q, 10);//序列化 text=>int
    var data = P * Q;//得到總價
    $("#T" + Id).text(data);//將總價寫入Total欄
    Total();//呼叫計算購物車總價錢function

}

function Add(e) {//增加品項數量

    var Id = e.data.Id;//獲取click的品項ID
    var x = $("#Q" + Id).text();//透過ID獲得數量
    var N = $("#N" + Id).text();//獲得品名
    var F = $("#F" + Id).text();//獲得特徵
    var Q = parseInt(x, 10);//序列化 text=>int
    $.ajax({
        url: "/cart/QtyAdd?Name=" + N + "&Feature=" + F,//透過剛取得的參數呼叫後端進行動作
        type: "get",
        success: function (data) {
            if (data == "") {//錯誤訊息為空則繼續
                Q++;//數量++
                $('#Q' + Id).text(Q);//修改過後的數量寫回數量欄
                Count(Id);//呼叫計算品項價錢function
                $("#C" + Id).removeAttr("disabled");//由於數量大於1將 - 按鈕設為可用
            }
            else {
                alert(data);//彈出錯誤訊息
            }
        }
    })


}

function Cut(e) {//減少品項數量

    var Id = e.data.Id;//獲取click品項ID
    var x = $("#Q" + Id).text();//獲取數量
    var N = $("#N" + Id).text();//獲取品名
    var F = $("#F" + Id).text();//獲取特徵
    var Q = parseInt(x, 10);//格式化數量 text=>int
    $.ajax({
        url: "/cart/QtyCut?Name=" + N + "&Feature=" + F,
        type: "get",
        success: function (data) {
            if (data == "") {//錯誤訊息為空則繼續
                Q--;//
                $('#Q' + Id).text(Q);//將數量寫回數量欄
                Count(Id);//呼叫計算品項總價function
                if (Q <= 1) {
                    $("#C" + Id).attr("disabled", "");//數量<=1則將 - 按鈕設為不可用
                }
            }
            else {
                alert(data);//彈出錯誤訊息
            }
        }
    })


}

function Total() {//計算購物車總價錢

    var L = $("#CO_tbody tr").length;//得到共有幾項商品
    var T = 0;
    for (var i = 0; L > i; i++) {
        var t = $("#T" + i).text();//獲取品項總價
        t = parseInt(t, 10);//序列化 text=>int
        T += t;//計算購物車總價
    }
    $("#CO_Totalprice").text("NT$" + T);//寫入購物車總價

}

function Delete(e) {//刪除購物車商品

    var T = confirm("確定移除該商品嗎?");//彈出確認訊息
    if (T == true) {
        var Id = e.data.Id;
        var N = $("#N" + Id).text();
        var F = $("#F" + Id).text();
        $.ajax({
            url: "/cart/DeleteCart?Name=" + N + "&Feature=" + F,
            success: function (e) {
                if (e != "") {
                    alert(e);//如錯誤訊息不為空則印出
                }
                else {
                    $("#" + Id).remove();//移除品項
                    Total();//重新計算購物車總價
                }
            }
        });
    }

}

function NextStep1_2() {//下一步 step1=>stpe2
    $("#step1").css("display", "none");//將step1設為不可見
    $("#step2").css("display", "block");//將step2設為可見
    $("#Step1-text .CO_span1").removeClass("CO_span1Select");//頂部圖示移除class
    $("#Step1-text .CO_span2").removeClass("CO_span2Select");
    $("#Step2-text .CO_span1").addClass("CO_span1Select");//頂部圖示新增class
    $("#Step2-text .CO_span2").addClass("CO_span2Select");
}

function before2_1() {//上一步 step2=>step1
    $("#Name").text("");//將欄位清空
    $("#Phone").text("");
    $("#Email").text("");
    $("#step2").css("display", "none");//將step1設為不可見
    $("#step1").css("display", "block");//將step2設為可見
    $("#Step2-text .CO_span1").removeClass("CO_span1Select");//頂部圖示移除class
    $("#Step2-text .CO_span2").removeClass("CO_span2Select");
    $("#Step1-text .CO_span1").addClass("CO_span1Select");//頂部圖示新增class
    $("#Step1-text .CO_span2").addClass("CO_span2Select");
}

function NextStep2_3() {//下一步 step2=>step3
    var n = $("#name").val(), p = $("#phone").val(), s = $("#store").val(), E = $("#email").val();//獲取使用者輸入值
    var Formdata = $('#form1').serialize();//將表單序列化成(Key1/value1 & Key2value2)
    $.ajax({
        url: "/cart/VerifyOrder",//呼叫後端驗證資料
        type: "post",
        data: Formdata + "&Sname="+s,//可直接在FormBody後增加參數 &參數名=值
        success: function (e) {
            //如有錯誤訊息則印出
            if (e != "") {
                e = JSON.parse(e);//序列化成javascript物件
                var L = 0;
                for (var x in e) {
                    L++;
                }
                for (var i = 0; L > i; i++) {
                    $("#" + e[i].key).text(e[i].message);//將錯誤訊息印至輸入欄
                }
            }
        }
    }).then(e => {
        //如有錯誤訊息則不動作
        if (e == "") {
            $("#Name").text("");
            $("#Phone").text("");
            $("#Email").text("");
            $("#step2").css("display", "none");
            $("#step3").css("display", "block")
            $("#Step2-text .CO_span1").removeClass("CO_span1Select");
            $("#Step2-text .CO_span2").removeClass("CO_span2Select");
            $("#Step3-text .CO_span1").addClass("CO_span1Select");
            $("#Step3-text .CO_span2").addClass("CO_span2Select");
            $("#step3Table1 tbody").empty();
            $("#step3Table2").empty();
            $.ajax({
                url: "/api/CartAPI/CartList",
                type: "get",
                success: function (data) {
                    var L = data.length;
                    var T = 0;
                    for (var i = 0; L > i; i++) {
                        //印出最後確認清單
                        $("#step3Table1").append(
                            "<tr><td class = 'col-md-3'><img class='img-rounded'style='width:70px;height:70px;float:left;'src='/UpdataFiles/" + data[i].Img + "' />" +
                            "<h5 id='N" + i + "'>" + data[i].Name + "</h5><span class='CO_span4'id='F" + i + "'>" + data[i].Feature + "</span></td>" +
                            "<td class='col-md-2' style='padding-left:30px;text-align:left;'><span style='pading-left:30px' class='CO_Qty'> " + data[i].Qty +
                            "</span></td>" +
                            "<td class='col-md-1'>NT$<span>" + data[i].Total + "</span></td>" +
                            "</tr>"
                        )
                        var t = parseInt(data[i].Total, 10);
                        T = T + t;
                    }
                    $("#CO_Totalprice2").text("NT$" + T);
                }
            })
            $("#step3Table2").append(//寫入確認頁面下方使用者確認資料
                "<tr><td>" + n + "</td><td>" + p + "</td><td>" + s + "</td><td>" + E + "</td></tr>"
            );
        }
    })
    
}

function before3_2() {//上一步 step3=>step2
    $("#step3Table2 tbody").remove();//刪除最終確認品項避免重複寫入
    $("#step3").css("display", "none");
    $("#step2").css("display", "block")
    $("#Step3-text .CO_span1").removeClass("CO_span1Select");
    $("#Step3-text .CO_span2").removeClass("CO_span2Select");
    $("#Step2-text .CO_span1").addClass("CO_span1Select");
    $("#Step2-text .CO_span2").addClass("CO_span2Select");
}

function Submitorder() {//送出訂單至後端及綠界
    var s = $("#store").val();
    var form = $("#form1").serialize();//將表單內容序列化 { key1, value1 & key2, value2 }
    $.ajax({
        url: "/cart/createpaydata",
        data: form + "&sname=" + s,
        success: e => {
            return e;
        },
        error: e => {
            return e;
        }
    }).then(e => {
        let json = JSON.parse(e);
        var form = $("#form2");
        for (var x in json) {
            var input = "<input type='hidden'name='" + x + "' value='" + json[x] + "'/ >";//將參數動態新增至表單內供傳遞
            form.append(input);
        }
        alert("送出後會有說明測試參數，請按照提供的參數填寫，請勿輸入正確卡號！付款完成直接點即返回商店即可");
        form.submit();
        location.href = "/cart/Explanation";
    }).catch(e => {
        alert(e);
    })
}


$(function () {
    $.ajax({
        url: "/api/cartapi/cartlist",
        type: "get",
        success: function (data) {

            var L = data.length;
            var T = 0;
            for (var i = 0; L > i; i++) {
                //印出購物清單
                $("#CO_tbody").append(
                    "<tr data-id='" + i + "' id='" + i + "'><td class = 'col-md-3'><a><img class='img-rounded'style='width:70px;height:70px;float:left;'src='/UpdataFiles/" + data[i].Img + "' /></a>" +
                    "<h5 id='N" + i + "'>" + data[i].Name + "</h5><span class='CO_span4'id='F" + i + "'>" + data[i].Feature + "</span></td>" +
                    "<td class='col-md-2' style='padding-left:0;text-align:left;'><button id='C" + i + "' class='btn btn-default CO-'>-</button><span id='Q" + i + "' class='CO_Qty'>" + data[i].Qty +
                    "</span><button id='A" + i + "' class='btn btn-default CO+'>+</button></td><td class='col-md-1'>NT$<span id='P" + i + "'>" + data[i].Price + "</span></td>" +
                    "<td class='col-md-1'>NT$<span id='T" + i + "'>" + data[i].Total + "</span></td><td class='col-md-1'><a id='D" + i + "' style='float:right;padding-right:10px;text-decoration:none;color:#bbb;cursor:pointer'>X</a></td>" +
                    "</tr>"
                )
                var t = parseInt(data[i].Total, 10);
                T = T + t;
                $("#A" + i).on("click", { Id: i }, Add);//綁定事件
                $("#C" + i).on("click", { Id: i }, Cut);
                $("#D" + i).on("click", { Id: i }, Delete);
                var q = $("#Q" + i).text();//數量
                var Q = parseInt(q);
                if (Q <= 1) {//數量<=1時禁用 - 按鈕
                    $("#C" + i).attr("disabled", "")
                }
            }
            $("#CO_Totalprice").text("NT$" + T);
            $("#Nextstep1").on("click", NextStep1_2);
            $("#before1").on("click", before2_1);
            $("#Nextstep2").on('click', NextStep2_3);
            $("#before2").on("click", before3_2);
            $("#Submitorder").on("click", Submitorder);
        }
    })
})