function Count(Id) {//計算品項價錢

    let p = $("#P" + Id).text();//取得價錢
    let P = parseInt(p, 10);//序列化 text=>int
    let q = $("#Q" + Id).text();//取得數量
    let Q = parseInt(q, 10);//序列化 text=>int
    let data = P * Q;//得到總價
    $("#T" + Id).text(data);//將總價寫入Total欄
    Total();//呼叫計算購物車總價錢function
}

function Add(e) {//增加品項數量

    let Id = e.data.Id;//獲取click的品項ID
    let x = $("#Q" + Id).text();//透過ID獲得數量
    let N = $("#N" + Id).text();//獲得品名
    let F = $("#F" + Id).text();//獲得特徵
    let Q = parseInt(x, 10);//序列化 text=>int
    $.ajax({
        url: "/api/cartapi/QtyAdd?Name=" + N + "&Feature=" + F,//透過剛取得的參數呼叫後端進行動作
        type: "get",
        success: e => {
            Q++;//數量++
            $('#Q' + Id).text(Q);//修改過後的數量寫回數量欄
            Count(Id);//呼叫計算品項價錢function
            $("#C" + Id).removeAttr("disabled");//由於數量大於1將 - 按鈕設為可用
        },
        error: e => {
            alert(e.responseJSON.Message);
        }
    });
}

function Cut(e) {//減少品項數量

    let Id = e.data.Id;//獲取click品項ID
    let x = $("#Q" + Id).text();//獲取數量
    let N = $("#N" + Id).text();//獲取品名
    let F = $("#F" + Id).text();//獲取特徵
    let Q = parseInt(x, 10);//格式化數量 text=>int
    $.ajax({
        url: "/api/cartapi/QtyCut?Name=" + N + "&Feature=" + F,
        type: "get",
        success: e => {
            Q--;
            $('#Q' + Id).text(Q);//將數量寫回數量欄
            Count(Id);//呼叫計算品項總價function
            (Q <= 1) ? $("#C" + Id).attr("disabled", "") : null;//數量<=1則將 - 按鈕設為不可用
        }, error: e => {
            alert(e.responseJSON.Message);//彈出錯誤訊息
        }
    });
}

function Total() {//計算購物車總價錢
    let L = $("#CO_tbody tr").length;//得到共有幾項商品
    let T = 0;
    for (let i = 0; L > i; i++) {
        let t = $("#T" + i).text();//獲取品項總價
        t = parseInt(t, 10);//序列化 text=>int
        T += t;//計算購物車總價
    }
    $("#CO_Totalprice").text("NT$" + T);//寫入購物車總價

}

function Delete(e) {//刪除購物車商品
    let T = confirm("確定移除該商品嗎?");//彈出確認訊息
    if (T) {
        let Id = e.data.Id;
        let N = $("#N" + Id).text();
        let F = $("#F" + Id).text();
        $.ajax({
            url: "/api/cartapi/DeleteCart?Name=" + N + "&Feature=" + F,
            success: e => {
                $('#alert').text("移除成功").show().delay(3000).fadeOut();
                $("#" + Id).remove();//移除品項
                Total();//重新計算購物車總價
                if (!$("#CO_tbody tr").length) {
                    $("#alert").text("購物車無商品，即將回到首頁").show().delay(3000).fadeOut();
                    setTimeout(() => {
                        location.href = "/home/index";
                    }, 3000);
                }
            }, error: e => {
                alert(e.responseJSON.Message);
            }
        });

    }
}

function NextStep1_2() {//下一步 step1=>stpe2
    if (!$("#CO_tbody tr").length) { 
        $('#alert').text('購物車內無商品').show().delay(3000).fadeOut();
        setTimeout(() => {
            location.href = "/home/index";
        }, 2500);
    }
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
    let n = $("#name").val(), p = $("#phone").val(), s = $("#store").val(), E = $("#email").val();//獲取使用者輸入值
    let Formdata = $('#form1').serialize();//將表單序列化成(Key1/value1 & Key2value2)
    $.ajax({
        url: "/cart/VerifyOrder",//呼叫後端驗證資料
        type: "post",
        data: Formdata + "&Sname=" + s,//可直接在FormBody後增加參數 &參數名=值
        success: e => {
            if (e) {
                E = JSON.parse(e);//序列化成javascript物件
                for (let i = 0; E.length > i; i++) {
                    $("#" + E[i].k).text(E[i].m);//將錯誤訊息印至輸入欄
                }
            }
        }
    }).then(e => {
        if (!e) {
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
            $("#step3Table2 tbody").empty();
            $.ajax({
                url: "/api/CartAPI/CartList",
                type: "get",
                success: e => {
                    let T = 0;
                    for (let i = 0; e.length > i; i++) {
                        //印出最後確認清單
                        $("#step3Table1").append(
                            "<tr><td class = 'col-md-3'><img class='img-rounded'style='width:70px;height:70px;float:left;'src='/UpdataFiles/" + e[i].Img + "' />" +
                            "<h5 id='N" + i + "'>" + e[i].Name + "</h5><span class='CO_span4'id='F" + i + "'>" + e[i].Feature + "</span></td>" +
                            "<td class='col-md-2' style='padding-left:30px;text-align:left;'><span style='pading-left:30px' class='CO_Qty'> " + e[i].Qty +
                            "</span></td>" +
                            "<td class='col-md-1'>NT$<span>" + e[i].Total + "</span></td>" +
                            "</tr>"
                        )
                        let t = parseInt(e[i].Total, 10);
                        T = T + t;
                    }
                    $("#CO_Totalprice2").text("NT$" + T);
                },error: e => {
                    alert(e.responseJSON.Message);
                }
            });
            $("#step3Table2 tbody").append(//寫入確認頁面下方使用者確認資料
                "<tr><td style='vertical-align: middle;'>" + n + "</td><td style='vertical-align: middle;'>" + p + "</td><td style='vertical-align: middle;'>" + s + "</td><td style='vertical-align: middle;'>" + E + "</td></tr>"
            );
        }
    });
}

function before3_2() {//上一步 step3=>step2
    $("#step3Table2 tbody").empty();//刪除最終確認避免重複寫入
    $("#step3").css("display", "none");
    $("#step2").css("display", "block")
    $("#Step3-text .CO_span1").removeClass("CO_span1Select");
    $("#Step3-text .CO_span2").removeClass("CO_span2Select");
    $("#Step2-text .CO_span1").addClass("CO_span1Select");
    $("#Step2-text .CO_span2").addClass("CO_span2Select");
}

function Submitorder() {//送出訂單至後端及綠界
    let s = $("#store").val();
    let form = $("#form1").serialize();//將表單內容序列化 { key1, value1 & key2, value2 }
    $.ajax({
        url: "/cart/createpaydata",
        data: form + "&sname=" + s,
        success: e => {
            let json = JSON.parse(e);
            let form = $("#form2");
            for (let x in json) {
                let input = "<input type='hidden'name='" + x + "' value='" + json[x] + "'/ >";//將參數動態新增至表單內供傳遞
                form.append(input);
            }
            alert("送出後會有說明測試參數，請按照提供的參數填寫，請勿輸入正確卡號！付款完成直接點即返回商店即可");
            form.submit();
            location.href = "/cart/Explanation";
        },
        error: e => {
            alert(e.responseJSON.Message);
        }
    });
}


$(function () {
    $.ajax({
        url: "/api/cartapi/cartlist",
        type: "get",
        success: e => {
            let T = 0;
            for (let i = 0; e.length > i; i++) {
                //印出購物清單
                $("#CO_tbody").append(
                    "<tr data-id='" + i + "' id='" + i + "'><td class = 'col-md-3'><a><img class='img-rounded'style='width:70px;height:70px;float:left;'src='/UpdataFiles/" + e[i].Img + "' /></a>" +
                    "<h5 id='N" + i + "'>" + e[i].Name + "</h5><span class='CO_span4'id='F" + i + "'>" + e[i].Feature + "</span></td>" +
                    "<td class='col-md-2' style='padding-left:0;text-align:left;'><button id='C" + i + "' class='btn btn-default CO-'>-</button><span id='Q" + i + "' class='CO_Qty'>" + e[i].Qty +
                    "</span><button id='A" + i + "' class='btn btn-default CO+'>+</button></td><td class='col-md-1'>NT$<span id='P" + i + "'>" + e[i].Price + "</span></td>" +
                    "<td class='col-md-1'>NT$<span id='T" + i + "'>" + e[i].Total + "</span></td><td class='col-md-1'><a id='D" + i + "' style='float:right;padding-right:10px;text-decoration:none;color:#bbb;cursor:pointer'>X</a></td>" +
                    "</tr>"
                )
                let t = parseInt(e[i].Total, 10);
                T = T + t;
                $("#A" + i).on("click", { Id: i }, Add);//綁定事件
                $("#C" + i).on("click", { Id: i }, Cut);
                $("#D" + i).on("click", { Id: i }, Delete);
                let q = $("#Q" + i).text();//數量
                let Q = parseInt(q);
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
        }, error: e => {
            alert(e.responseJSON.Message);
        }
    });
})