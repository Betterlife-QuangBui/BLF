function activeSort(value) {
    $(".sort").val(value);
}
function sortItems(value) {
    var url = location.href;
    var index=url.indexOf("?")
    if (index>0) {
        url = url.substring(0, index);
    }
    url = url + "?page=1&sort=" + value;
    location.href = url;
}
$(function () {
    $("#MyCheckAvailable").find("table").addClass("table table-striped");
    $(".attr input[type=text],.attr select").addClass("form-control col-md-12");
    $(".sub-image").hover(function () {
        $("#main-image").attr("src", $(this).attr("src"));
    });
    $(".CheckAvailable").click(function () {
        $('html').css('overflow', 'hidden')
        var modal = document.getElementById('MyCheckAvailable')
        modal.style.display = 'block'
    });
    $(".close").click(function () {
        
        var modal = document.getElementById('MyCheckAvailable')
        modal.style.display = 'none'
        $('html').css('overflow', 'auto')
    });
});