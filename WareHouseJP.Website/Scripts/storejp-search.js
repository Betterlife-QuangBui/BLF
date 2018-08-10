$(document).on("click", "#btnSearchJP", function (e) {
    $(".se-pre-con").show(); $("#search-form").submit();
});
$(function () {
    $(".table_insert").find("input,button,textarea,select,a").attr("disabled", "disabled");
    $(".table_insert").find("a").hide();
});
$(document).on("click", "input[name=saveprod]", function (e) {
    $("#IdItem").val($(this).attr("data-id"))
    $(".table_insert").find("input,button,textarea,select").removeAttr("disabled");
    $(".table_insert").find("a").show();
    $(".table_insert").find(".hiden").css("display", "none");
    //$(".picture-option").css("display", "none");
    
});
$(document).on("click", ".advan-add", function (e) {
    $(".table_insert").find(".hiden").show();
});
$(document).on("click", ".ace-file-input", function (e) {
    $(".picture-option").css("display", "block");
});
$(document).on("click", ".picture-option .fa-times", function (e) {
    $(".picture-option").css("display", "none");
});
$(document).on("click", "#upload-computer", function (e) {
    $("#id-input-file-2").click();
});

$(document).on("change", "#id-input-file-2", function (e) {
    if (this.files && this.files[0]) {
        var FR = new FileReader();
        FR.onload = function (e) {
            $("#img-review img").attr("src", e.target.result);
            $("#ComponentImage").val(e.target.result)
        };
        FR.readAsDataURL(this.files[0]);
    }
});