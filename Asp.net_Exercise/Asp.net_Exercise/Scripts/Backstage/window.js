$(function () {
    console.log(Tvalue);
    for (let i = 0; Tvalue.length > i; i++) {
        $("table tbody").append(
            "<tr><td style='width:40%'>" + Tvalue[i].name + "</td>" +
            "<td style='width:20%'>" + Tvalue[i].color_size + "</td>" +
            "<td style='width:20%'>" + Tvalue[i].price + "</td>" +
            "<td style='width:20%'>" + Tvalue[i].quantity + "</td></tr>" 
        )
    }
    $("#total").text(Tvalue[0].total);
})