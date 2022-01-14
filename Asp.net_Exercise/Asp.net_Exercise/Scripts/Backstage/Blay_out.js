$(function() {
    $("#button_1").on('click', Pullback);
    $("#"+active).addClass('active');
})
function Pullback() {
    if ($("body").hasClass("sidebar-mini sidebar-collapse")) {
        $("#icon_1").attr("class", "fas fa-angle-double-left")
    }
    else {
        $("#icon_1").attr("class", "fas fa-angle-double-right")
    }
}