function GoView(id) {
    window.open("/Home/ProdDetails?Pid=" + id, "_blank");
}
$(function () {
    $.ajax({
        url: "/api/Homeapi/GetProd?TYPE=" + type,
        type: "GET",
        success: e => {
            for (var i = 0; e.length > i; i++) {
                $("#Father").append("<div id='Prod'><a id='P" + i + "' href='/Home/ProdDetails?Pid=" + e[i].prod.Id + "'>" +
                    "<img id='Pimg' src='/UpdataFiles/" + e[i].img.FileName + "' / ></a><p id='Ptext'>" + e[i].prod.Name +
                    "</p><p id='Ptext'>NT$" + e[i].prod.Price + "</p></div>"
                )
            }
        }
    });
})