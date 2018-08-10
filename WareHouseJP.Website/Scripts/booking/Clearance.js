$(function () {
    $(".table_insert select").select2({
        theme: "bootstrap"
    });
});
jQuery(function ($) {
    $('#id-input-file-3').change(function () {
        $(this).next().find(".ace-file-name").attr("data-title", $(this).val().split('\\').pop());
        $(this).next().next().show();
        $(".ace-file-input .remove").css("right", "66px");
    });
    $("#remove-input-file-3").click(function () {
        $('#id-input-file-3').val("");
        $(this).hide();
        $("#id-input-file-name").attr("data-title", "Chưa có file");
    });
});
$(function () {
    $('#dt_ClearanceDate,#datetimepicker3,#clea-FlightDateFrom,#clea-FlightDateTo').datetimepicker({
        format: "YYYY-MM-DD"
    });
    $('#dt_ClearanceHour,#clea-FlightHourTo,#clea-FlightHourFrom,#dt_InvoiceHour').datetimepicker({
        format: 'LT'
    });
});

//Invoice
$(document).on("click", ".btnBookingInvoice", function (e) {
    var json = $("form#btnBookingInvoice").serializeArray();
    var jsonObject = {};
    for (var i = 0, l = json.length; i < l; i++) {
        jsonObject[json[i].name.toString().replace("Item2.", "").replace("Item1.", "")] = json[i].value;
    }
    $.ajax({
        method: "POST",
        url: '/Clearance/BookingInvoice',
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

//Clearance
$(document).on("click", ".btnSaveClearnace", function (e) {
    var json = $("form#btnSaveClearnace").serializeArray();
    var jsonObject = {};
    for (var i = 0, l = json.length; i < l; i++) {
        jsonObject[json[i].name.toString().replace("Item2.", "").replace("Item1.", "")] = json[i].value;
    }
    $.ajax({
        method: "POST",
        url: '/Clearance/SaveClearnace',
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

$(document).on("click", ".btnSaveClearnaceAir", function (e) {
    var json = $("form#btnSaveClearnaceAir").serializeArray();
    var jsonObject = {};
    for (var i = 0, l = json.length; i < l; i++) {
        jsonObject[json[i].name.toString().replace("Item1.", "")] = json[i].value;
    }
    //Booking_MAWB
    var arrMaWB = [];
    $("#MAWBClearance ul.select2-selection__rendered li.select2-selection__choice").each(function (idx, li) {
        arrMaWB.push($(li).attr("title"));
    });
    jsonObject.MAWB = arrMaWB.toString();
    var arrHaWB = [];
    $("#HAWBClearance ul.select2-selection__rendered li.select2-selection__choice").each(function (idx, li) {
        arrHaWB.push($(li).attr("title"));
    });
    jsonObject.HAWB = arrHaWB.toString();
    $.ajax({
        method: "POST",
        url: '/Clearance/SaveClearnaceAir',
        data: JSON.stringify(jsonObject),
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //loadBooking();
            //loadClearance();
        }
    });
    e.preventDefault();
    return;
});