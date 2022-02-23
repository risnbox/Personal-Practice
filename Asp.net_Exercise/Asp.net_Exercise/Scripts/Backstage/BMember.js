$(function () {
    var height = $("table").height();
    $("table tbody").css("height", height - 56);
    $("#remove").on("click", remove);
    $("#submit").on("click", Search);
    Table();
})

function Table(e) {
    $("table tbody tr").remove();
    let Tvalue = e != null ? e : data;
    for (let i = 0; Tvalue.length > i; i++) {
        let Google, FB, Gender, Phone, order;
        (Tvalue[i].Gender) ? Gender = Tvalue[i].Gender : Gender = '未填';
        (Tvalue[i].Phone) ? Phone = Tvalue[i].Phone : Phone = '未填';
        (Tvalue[i].Order) ? order = "value='共" + Tvalue[i].Order + "筆'" : order = "value='無訂單' disabled";
        (Tvalue[i].Google) ? Google = "是" : Google = "否";
        (Tvalue[i].FB) ? FB = "是" : FB = "否";
        $("table tbody").append(
            "<tr><td>" + Tvalue[i].Id + "</td>" +
            "<td id='td-lg'>" + Tvalue[i].Name + "</td>" +
            "<td id='td-lg'>" + Tvalue[i].Email + "</td>" +
            "<td id='td-lg'>" + Phone + "</td>" +
            "<td>" + Gender + "</td>" +
            "<td>" + Google + "</td>" +
            "<td>" + FB + "</td>" +
            "<td><select id='S" + i + "' data-befor='" + Tvalue[i].Status + "'><option value=0>未啟用</option><option value=1>已啟用</option><option value=2>停權</option></select ></td > " +
            "<td><input id='O" + i + "' type='button' class='btn btn-dark'" + order + " /></td></tr>"
        )
        Tvalue[i].Status == 0 ? $("#S" + i).val(0) : Tvalue[i].Status == 1 ? $("#S" + i).val(1) : $("#S" + i).val(2);
        $("#O" + i).on('click', { Id: Tvalue[i].Id }, GotoOrder);
        $("#S" + i).on('change', { Id: Tvalue[i].Id }, Selected);
    }
}

function remove() {
    $("#FB").get(0).selectedIndex = 0;
    $("#google").get(0).selectedIndex = 0;
    $("#gender").get(0).selectedIndex = 0;
    $("#status").get(0).selectedIndex = 0;
    Table();
}

function Search() {
    var fb = $("#FB").get(0).selectedIndex;
    var google = $("#google").get(0).selectedIndex;
    var gender = $("#gender").val();
    var status = $("#status").get(0).selectedIndex;
    status == 0 ? status = 3 : status == 1 ? status = 1 : status == 2 ? status = 0 : status == 3 ? status = 2 : null;
    $.ajax({
        url: "/api/BmemberAPI/Search",
        method: "get",
        data: { fb: fb, google: google, gender: gender, status: status },
        success: e => {
            e.length == 0 ? $("#alert").text("查無資料").show().delay(2000).fadeOut() : Table(e);
        }, error: e => {
            console.log(e);
        }
    })
}

function Selected(e) {
    if (confirm("確定更改帳號狀態嗎")) {
        $.ajax({
            url: "/api/BmemberAPI/changestatus",
            method: "get",
            data: { Id: e.data.Id, Status: e.target.options.selectedIndex },
            success: () => {
                $("#alert").text("更改成功").show().delay(2000).fadeOut();
                //更改成功=>覆寫befor值為當前值
                $("#" + e.target.id).attr("data-befor", e.target.options.selectedIndex);
            },
            error: m => {
                alert(m.responseJSON.Message);
            }
        })
    }
    else {
        //Confirm==false=>將選單回歸成選擇前
        $("#" + e.target.id).val($(this).attr("data-befor"));
    }
}

function GotoOrder(e) {
    Cookies.set("data", e.data.Id);//透過Cookie傳遞訂單Id
    location.href = "/backstage/order/index";
}