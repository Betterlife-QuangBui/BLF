$(document).on("click", ".btn-action-import-full", function (e) {
    uploadFileFull();
    return false;
    e.preventDefault();
});

function uploadFileFull() {
    $(".se-pre-con").show();
    //var option = $("input[name=option]").val();
    var file = document.getElementById('fileImport-full').files[0];
    var formData = new FormData();
    formData.append("fileName", file);
    ajax = new XMLHttpRequest();
    ajax.upload.addEventListener("progress", progressHandlerFull, false);
    ajax.addEventListener("load", completeHandlerFull, false);
    //ajax.open("POST", "/Import/Shipment/" + id + "?option=" + option);
    ajax.open("POST", "/Import/StorageJPFull");
    ajax.send(formData);
}

function progressHandlerFull(event) {
    var percent = (event.loaded / event.total) * 100;
    $(".progress-bar").html(percent + "%");
    $(".progress-bar").css("width", percent + "%");
}

function completeHandlerFull() {
    $(".progress-bar").css("width", "100%");
    alert("Đã import dự liệu thành công");
    location.reload();
}