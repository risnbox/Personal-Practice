function GotoView(id) {
    window.open("/Home/ProdDetails?Pid=" + id, "_blank");
}
$(function () {

    $.ajax({
        url: "/api/Homeapi/GetProd?TYPE=" + type,
        type: "GET",
        success: function (data) {
            for (var i = 0; data.length > i; i++) {
                $("#Father").append("<div id='Prod'><a id='P" + i + "' href='javascript:void(0);' onclick='GotoView(" + data[i].prod.Id + ")'>" +
                    "<img id='Pimg' src='/UpdataFiles/" + data[i].img.FileName + "' / ></a><p id='Ptext'>" + data[i].prod.Name +
                    "</p><p id='Ptext'>NT$" + data[i].prod.Price + "</p></div>"
                )
            }
        }
    })

})