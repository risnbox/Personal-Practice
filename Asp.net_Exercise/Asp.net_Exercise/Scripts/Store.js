
$(function () {

    $("#CitySelect").append(
        "<option value='0' disabled selected>-請選擇-</option>"
    )

    $("#TownSelect").append(
        "<option value='0' disabled selected>-請先選擇縣市-</option>"
    )

    //選市後產生該市所有行政區
    $("#CitySelect").change(function () {
        $("#TownSelect").empty();
        var city = $("#CitySelect").find("option:selected").text();
        $.ajax({
            url: "/store/Gettown?city=" + city,
            type: "get",
            success: function (data) {
                data = JSON.parse(data);
                for (var i in data) {
                    $("#TownSelect").append(
                        "<option>" + data[i] + "</option>"
                    )
                }
            }
        })
    })
    //透過下拉選單得到地區後取得711資料
    $("#Openbtn").on('click', function () {
        $('table #S').remove();//清空舊資料
        var city = $('#CitySelect').find('option:selected').text();
        var town = $('#TownSelect').find('option:selected').text();
        Getdata(city, town);
    })
    //取得711資料的Function
    function Getdata(city, town) {
        $.ajax({
            url: "/store/Get711?city=" + city + "&town=" + town,
            type: "get",
            success: function (data) {
                data = JSON.parse(data);
                var L = data.iMapSDKOutput.GeoPosition.length;
                for (var i = 0; L > i; i++) {//將資料新增到Table
                    $("#Ltable").append(
                        "<tr id='S'><td id='N" + i + "'>" + data.iMapSDKOutput.GeoPosition[i].POIName + "</td>" +
                        "<td id='A" + i + "'>" + data.iMapSDKOutput.GeoPosition[i].Address + "</td>" +
                        "<td id='I" + i + "'>" + data.iMapSDKOutput.GeoPosition[i].POIID + "</td>" +
                        "<td id='T" + i + "'>" + data.iMapSDKOutput.GeoPosition[i].Telno + "</td>" +
                        "<td><a href='https://www.google.com.tw/maps/place/" + data.iMapSDKOutput.GeoPosition[i].Address + " 'target='_blank' class='btn btn-info'>地圖</td>" +
                        "<td><input class='btn btn-default' type='button' value='選擇' id='Selectbtn" + i + "'/></td></tr>"
                    )
                    $("#Selectbtn" + i).on('click', { Id: i }, SelectStore)//傳值到Function

                }
            }
        })
    }

    function SelectStore(e) {

        var i = e.data.Id;
        var name = $("#N" + i).text();
        var Address = $('#A' + i).text();
        var Id = $('#I' + i).text();
        var TelNo = $('#T' + i).text();
        $.ajax({
            url: "/api/storeapi/SelectStore",
            type: 'POST',
            dataType: 'text',
            data: { Name: name, Address: Address, ID: Id, TelNo: TelNo },
            success: e => {
                return e;
            },
            error: e => {
                return e;
            }
        }).then(e => {
            console.log(e);
        }).catch(e => {
            console.log(e);
            alert(e.JSONreqirect.Message);
            location.reload();
        })
    }
    $("#ViewStore").on('click', viewstore)

    function viewstore() {
        $.ajax({
            url: "/store/ViewStore",
            type: "GET",
            success: function (data) {
                data = JSON.parse(data);
                $("table #S").remove();
                var L = data.length;
                if (L == 0) {
                    alert("您還未曾選取門市");
                }
                else {
                    for (var i = 0; L > i; i++) {
                        $("#Ltable").append(
                            "<tr id='S'><td id='N" + i + "'>" + data[i].StoreName + "</td>" +
                            "<td id='A" + i + "'>" + data[i].StoreAddress + "</td>" +
                            "<td id='I" + i + "'>" + data[i].StoreId + "</td>" +
                            "<td id='T" + i + "'>" + data[i].StoreTelNo + "</td>" +
                            "<td><a href='https://www.google.com.tw/maps/place/" + data[i].StoreAddress + " 'target='_blank' class='btn btn-info'>地圖</td>" +
                            "<td><input type='button' id='Sbtn" + i + "' value='刪除' class='btn btn-danger'/></tr>"
                        )
                        $("#Sbtn" + i).on('click', { Sid: data[i].StoreId }, DeleteStore);
                    }
                }
            }
        })
    }

    function DeleteStore(e) {
        var Cf = confirm("確定要移除該門市嗎?");
        if (Cf == true) {
            $.ajax({
                url: "/store/DeleteStore?store=" + e.data.Sid,
                type: "Get",
                success: function () {
                    alert("刪除成功");
                    viewstore();
                }
            });
        }
    }
})