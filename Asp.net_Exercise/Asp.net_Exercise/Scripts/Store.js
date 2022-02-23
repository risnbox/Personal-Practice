$(function () {

    $("#CitySelect").append(
        "<option value='0' disabled selected>-請選擇-</option>"
    )

    $("#TownSelect").append(
        "<option value='0' disabled selected>-請先選擇縣市-</option>"
    )
    if (err) {
        $('#alert').text(err).show().delay(2000).fadeOut();
    }
    //選市後產生該市所有行政區
    $("#CitySelect").change(() => {
        $("#TownSelect").empty();
        let city = $("#CitySelect").find("option:selected").text();
        $.ajax({
            url: "/store/Gettown?city=" + city,
            type: "get",
            success: e => {
                e = JSON.parse(e);
                for (let i in e) {
                    $("#TownSelect").append(
                        "<option>" + e[i] + "</option>"
                    )
                }
            }
        });
    })
    //透過下拉選單得到地區後取得711資料
    $("#Openbtn").on('click', function () {
        $("#CitySelect").val() == 0 ? $('#alert').text("請先選擇地區").show().delay(2000).fadeOut() : null;
        $('table #S').remove();//清空舊資料
        let city = $('#CitySelect').find('option:selected').text();
        let town = $('#TownSelect').find('option:selected').text();
        Getdata(city, town);
    })
    //取得711資料的Function
    function Getdata(city, town) {
        $.ajax({
            url: "/api/storeapi/Get7111?city=" + city + "&town=" + town,
            type: "get",
            success: e => {
                for (let i = 0; e.iMapSDKOutput.GeoPosition.length > i; i++) {//將資料新增到Table
                    $("#Ltable").append(
                        "<tr id='S'><td id='N" + i + "'>" + e.iMapSDKOutput.GeoPosition[i].POIName + "</td>" +
                        "<td id='A" + i + "'>" + e.iMapSDKOutput.GeoPosition[i].Address + "</td>" +
                        "<td id='I" + i + "'>" + e.iMapSDKOutput.GeoPosition[i].POIID + "</td>" +
                        "<td id='T" + i + "'>" + e.iMapSDKOutput.GeoPosition[i].Telno + "</td>" +
                        "<td><a href='https://www.google.com.tw/maps/place/" + e.iMapSDKOutput.GeoPosition[i].Address + " 'target='_blank' class='btn btn-info'>地圖</td>" +
                        "<td><input class='btn btn-default' type='button' value='選擇' id='Selectbtn" + i + "'/></td></tr>"
                    )
                    $("#Selectbtn" + i).on('click', { Id: i }, SelectStore)//傳值到Function
                }
            }
        });
    }

    function SelectStore(e) {
        let i = e.data.Id,name = $("#N" + i).text(),Address = $('#A' + i).text(),Id = $('#I' + i).text(),TelNo = $('#T' + i).text();
        $.ajax({
            url: "/api/storeapi/SelectStore",
            type: 'POST',
            dataType: 'text',
            data: { StoreName: name, StoreAddress: Address, StoreId: Id, StoreTelNo: TelNo },
            success: () => {
                $('#alert').text("成功選擇門市").show().delay(1000).fadeOut();
                setTimeout(() => {
                    viewstore();
                }, 1000);
            },
            error: e => {
                r = JSON.parse(e.responseText);
                $('#alert').text(r.ExceptionMessage).show().delay(2500).fadeOut();
            }
        });
    }
    $("#ViewStore").on('click', viewstore);

    function viewstore() {
        $.ajax({
            url: "/api/storeapi/ViewStore",
            type: "GET",
            success: e => {
                $("table #S").remove();
                if (!e.length) {
                    $('#alert').text("您還未曾選取門市").show().delay(2500).fadeOut();
                }
                else {
                    for (let i = 0; e.length > i; i++) {
                        $("#Ltable").append(
                            "<tr id='S'><td id='N" + i + "'>" + e[i].StoreName + "</td>" +
                            "<td id='A" + i + "'>" + e[i].StoreAddress + "</td>" +
                            "<td id='I" + i + "'>" + e[i].StoreId + "</td>" +
                            "<td id='T" + i + "'>" + e[i].StoreTelNo + "</td>" +
                            "<td><a href='https://www.google.com.tw/maps/place/" + e[i].StoreAddress + " 'target='_blank' class='btn btn-info'>地圖</td>" +
                            "<td><input type='button' id='Sbtn" + i + "' value='刪除' class='btn btn-danger'/></tr>"
                        )
                        $("#Sbtn" + i).on('click', { Sid: e[i].StoreId }, DeleteStore);
                    }
                }
            }
        });
    }

    function DeleteStore(e) {
        let Cf = confirm("確定要移除該門市嗎?");
        if (Cf) {
            $.ajax({
                url: "/api/storeapi/DeleteStore?store=" + e.data.Sid,
                type: "Get",
                success: () => {
                    $('#alert').text("刪除成功").show().delay(2000).fadeOut();
                    viewstore();
                }
            });
        }
    }
})