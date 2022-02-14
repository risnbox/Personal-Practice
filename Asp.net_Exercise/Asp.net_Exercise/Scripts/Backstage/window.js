$(function () {
    for (let i = 0; data.length > i; i++) {
        $("table tbody").append(
            "<tr><td style='width:40%'>" + data[i].name + "</td>" +
            "<td style='width:20%'>" + data[i].color_size + "</td>" +
            "<td style='width:20%'>" + data[i].price + "</td>" +
            "<td style='width:20%'>" + data[i].quantity + "</td></tr>" 
        )
    }
    $("#total").text(data[0].total);
})