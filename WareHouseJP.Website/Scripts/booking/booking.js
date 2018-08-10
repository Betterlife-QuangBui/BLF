$(function () {
    $(".table_insert select").select2({
        theme: "bootstrap"
    });
    $('#datetimepicker2').datetimepicker({
        format: "YYYY-MM-DD"
    });
    $('#datetimepicker_hour_2').datetimepicker({
        format: 'LT'
    });
});
$(function ($) {
    $('#id-input-file-2').change(function () {
        $(".ace-file-name").attr("data-title", $(this).val().split('\\').pop());
        $(".ace-file-input .remove").show();
        $(".ace-file-input .remove").css("right", "66px");
        var formData = new FormData();
        var totalFiles = document.getElementById("id-input-file-2").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("id-input-file-2").files[i];
            formData.append("FileUpload", file);
        }
        formData.append("FlightBookingId", $(this).attr("data-id"));
        $.ajax({
            method: "POST",
            url: '/FlightBooking/Upload',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
               
                
            }
        });
    });
    $(".ace-file-input .remove").click(function () {
        $('#id-input-file-2').val("");
        $(".ace-file-input .remove").hide();
        $(".ace-file-name").attr("data-title", "Chưa có hình");
    });
});
$(document).on("click", ".btnSaveBooking", function (e) {
    var json = $("form#btnSaveBooking").serializeArray();
    var jsonObject = {};
    for (var i = 0, l = json.length; i < l; i++) {
        jsonObject[json[i].name.toString().replace("Item1.","")] = json[i].value;
    }
    //Booking_MAWB
    var arrMaWB = [];
    $("#Booking_MAWB ul.select2-selection__rendered li.select2-selection__choice").each(function (idx, li) {
       arrMaWB.push($(li).attr("title"));
    });
    jsonObject.MAWB = arrMaWB.toString();
    var arrHaWB = [];
    $("#Booking_HAWB ul.select2-selection__rendered li.select2-selection__choice").each(function (idx, li) {
        arrHaWB.push($(li).attr("title"));
    });
    jsonObject.HAWB = arrHaWB.toString();
    $.ajax({
        method: "POST",
        url: '/FlightBooking/EditAir',
        data: JSON.stringify(jsonObject),
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            loadBooking(); loadClearance();
        }
    });
    e.preventDefault();
    return;
});

$(document).on("click", ".btnSaveAirBooking", function (e) {
    var json = $("form#btnSaveAirBooking").serializeArray();
    var jsonObject = {};
    for (var i = 0, l = json.length; i < l; i++) {
        jsonObject[json[i].name.toString().replace("Item2.", "").replace("Item1.", "")] = json[i].value;
    }
    $.ajax({
        method: "POST",
        url: '/FlightBooking/SaveAirBooking',
        data: JSON.stringify(jsonObject),
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            loadBooking(); loadClearance();
        }
    });
    e.preventDefault();
    return;
});