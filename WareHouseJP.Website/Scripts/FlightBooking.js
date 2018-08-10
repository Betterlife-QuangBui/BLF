$(document).on("click", ".box-item", function () {
    var id = $(this).attr("data-id");
    $(".wasehouse-info").html("")
    $(".wasehouse-info").load($(this).attr("data-href"));

    $(".control").hide()
    $(".control[data-id=" + id + "]").show()

    $(".box-item").each(function (i, e) {
        var className = "active-item";
        if ($(this).hasClass(className)) {
            $(this).removeClass(className)
        }
    })
    $(this).addClass("active-item");

    return false;
})
$(document).on("click", ".pagging a", function () {
    var url = $(this).attr("href");
    $(".item-of-page").html("");
    $('html, body').animate({
        scrollTop: $(".page-content").offset().top
    }, 2000);
    $(".item-of-page").load(url);
    return false;
})
function inputSearch(key) {
    if ($(key).val().toString().length>2){
        $(".item-of-page").html("");
        $(".item-of-page").load('/Ajax/FlightBooking?page=1&key=' + $(key).val());
    }
    if ($(key).val().toString().length ==0) {
        $(".item-of-page").html("");
        $(".item-of-page").load('/Ajax/FlightBooking?page=1&key=' + $(key).val());
    }
}
$(window).load(function () {
    
});