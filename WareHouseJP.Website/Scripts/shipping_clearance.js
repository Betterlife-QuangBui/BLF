$(document).on("click", ".btn-save-status", function (e) {
    ShippingId = $(this).attr("data-id");
    $(".se-pre-con").show();
        $.ajax({
            method: 'POST', async: false,
            url: "/Shipping/UpdateStatusShipping",
            data: { Id: ShippingId, statusId:$("#StatusId").val()},
            success: function (result) {
                if (result.status == true) {
                    notify('white', 'Update trạng thái thành công', "1");
                    $(".se-pre-con").fadeOut();
                    location.reload();
                }
                else {
                    notify('error', "Update trạng thái thất bại", "2");
                    $(".se-pre-con").fadeOut();
                }
            }
        });
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
            //$("#img-review img").attr("src", e.target.result);
            //$("#PreADFile").val(e.target.result)
        };
        FR.readAsDataURL(this.files[0]);
    }
});
var loadFile = function (event) {
    var output = document.getElementById('img-review-output');
    output.src = URL.createObjectURL(event.target.files[0]);
};