$(function () {

    $("#Search").on('click', SearchProd)
    function SearchProd() {
        $("table .S").remove();
        var T = $('#SType').find("option:selected").text();
        var C = $('#SClass').find("option:selected").text();
        $.ajax({
            url: "/api/prodapir/SearchProd?Stype=" + T + "&Sclass=" + C,
            type: "GET",
            success: function (data) {
                for (var i = 0; data.length > i; i++) {
                    $("table").append(
                        "<tr id='S" + i + "' class='S'><td id='N" + i + "'>" + data[i].Prod.Name + "</td>" +
                        "<td id='P" + i + "'>" + data[i].Prod.Price + "</td>" +
                        "<td id='T" + i + "'>" + data[i].type.TypeName + "</td>" +
                        "<td id='C" + i + "'>" + data[i].Clas.ClassName + "</td>" +
                        "<td><input type='button' id='DelProd" + i + "' value='刪除' class='btn btn-danger' /></td><tr/>"
                    )
                    $("#DelProd" + i).on('click', { name: data[i].Prod.Name, Id: i }, Delete);
                }
            }
        })
    }

    function Delete(e) {
        var Name = e.data.name;
        if (confirm("確定刪除嗎?")) {
            $.ajax({
                url: "/api/prodapir/DelProd?name=" + Name,
                type: "Get",
                success: function (data) {
                    if (data == 0) {
                        alert("刪除失敗")
                    }
                    alert("刪除成功")
                    $("#S" + e.data.Id).remove();
                }
            })
        }
    }

})