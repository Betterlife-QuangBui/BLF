$(document).on("click", "#shipping_booking", function (e) {
    ShippingId = $(this).attr("data-id");
    trackingCount = $("#trackingCount").val();
    weigh = $("#weigh").val();
    notes = $("#notes").val();
    var check = $("#conform-booking").prop("checked");
    if (check == true) {
        $(".se-pre-con").show();
        $.ajax({
            method: 'POST', async: false,
            url: "/Shipping/AddBookingHistory",
            data: { Id: ShippingId, trackingCount: trackingCount, weigh: weigh,notes:notes },
            success: function (result) {
                if (result.status == true) {
                    notify('white', 'Booking thành công', "1");
                    location.reload();
                    $("#history-booking tbody").html("")
                    $("#history-booking tbody").load("/ajax/BookingHistory/" + ShippingId);
                    $(".se-pre-con").fadeOut();
                }
                else {
                    notify('error', "Booking thất bại", "2");
                    $(".se-pre-con").fadeOut();
                }
            }
        });
    }
});
$(document).on("click", "#conform-booking", function (e) {
    ShippingId = $(this).attr("data-id");
    trackingCount = $("#trackingCount").val();
    weigh = $("#weigh").val();
    var check = $(this).prop("checked");
    if (check == true) {
        $("#shipping_booking").removeAttr("disabled");
    }
    else {
        $("#shipping_booking").attr("disabled", "disabled");
    }
});

//update status
$(document).on("click", ".btn-update-status", function (e) {
    loading();
    var ModelId = $("#ModelId").val();
    status = $(".StatusId #StatusId").val();
    $.ajax({
        method: 'POST', async: false,
        url: "/Shipping/UpdateBookingStatus",
        data: { id: ModelId, status: status },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Cập nhật liệu thành công', "1");
                location.reload();
            }
            else {
                notify('error', "Cập nhật liệu thất bại", "2");
                //location.reload();
            }
        }
    });
});