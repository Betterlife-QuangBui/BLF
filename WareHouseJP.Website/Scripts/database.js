//Import shipment
$(document).on("click", ".database-import", function (e) {
    $(".database-body-crud").html("");
    $(".database-body-crud").load($(this).attr("href"));
    return false;
    e.preventDefault();
});
$(document).on("change", "#fileImport", function (e) {
    if ($(this).val().split('\\').pop() != "") {
        $(".btn-action-import").css("display", "inline-block");
    }
});
$(document).on("click", ".btn-import,#inputfileImport", function (e) {
    $("#fileImport").click();
    return false;
    e.preventDefault();
});
$(document).on("click", ".btn-action-import", function (e) {
    $(".se-pre-con").show();
    uploadFile();
    return false;
    e.preventDefault();
});
function uploadFile() {
    var file = document.getElementById('fileImport').files[0];
    var formData = new FormData();
    formData.append("fileName", file);
    ajax = new XMLHttpRequest();
    ajax.upload.addEventListener("progress", progressHandler, false);
    ajax.addEventListener("load", completeHandler, false);
    //ajax.open("POST", "/Import/Shipment/" + id + "?option=" + option);
    ajax.open("POST", "/Database/Import");
    ajax.send(formData);
}

function progressHandler(event) {
    var percent = (event.loaded / event.total) * 100;
    $(".progress-bar").html(percent + "%");
    $(".progress-bar").css("width", percent + "%");
}

function completeHandler() {
    $(".progress-bar").css("width", "100%");
    alert("Đã import dự liệu thành công");
    location.reload();

}