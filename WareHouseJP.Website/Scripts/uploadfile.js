$(document).on("click", ".btn-action-import", function (e) {
    uploadFile();
    return false;
    e.preventDefault();
});
function uploadFile() {
    var id = $("#Shipment").val();
    //var option = $("input[name=option]").val();
    var file = document.getElementById('fileImport').files[0];
    var formData = new FormData();
    formData.append("fileName", file);
    ajax = new XMLHttpRequest();
    ajax.upload.addEventListener("progress", progressHandler, false);
    ajax.addEventListener("load", completeHandler, false);
    //ajax.open("POST", "/Import/Shipment/" + id + "?option=" + option);
    ajax.open("POST", "/Import/Shipment/" + id);
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