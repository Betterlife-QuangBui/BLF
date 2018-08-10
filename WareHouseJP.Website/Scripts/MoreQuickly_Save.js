$(document).on("click", "#tab-booking #storejp-table tbody input[type=radio]", function (e) {
    var arr = [];
    var count=$("#countKeySearch").val();
    $("#tab-booking #storejp-table tbody input[type=radio]").each(function (i, e) {
        if ($(this).prop("checked")) {
            arr.push($(this).attr("data-id"));
        }
    });
    if (parseInt(count) == arr.length) {
        $("#searchMoreQuickyId").val(arr.toString());
        $(".btnSaveMoreQuickyMain").show();
    }
    else {
        $("#searchMoreQuickyId").val("");
        $(".btnSaveMoreQuickyMain").hide();
    }
});
$(document).on("click", ".btnSaveMoreQuickyMain", function (e) {
    $(".se-pre-con").show();
    var StoreJPId = $(this).attr("data-id");
    var count = $("#countKeySearch").val();
    var searchMoreQuickyId = $("#searchMoreQuickyId").val();
    //update janecode search into db
    $.ajax({
        method: 'POST',
        url: '/StorageJP/SaveMoreQuicklyMain', data: { Id: $("#StoreJPId").val(), searchMoreQuickyId: searchMoreQuickyId},
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Thêm dữ liệu thành công', "1");
                location.href = '/StorageJP/Detail/' + $("#StoreJPId").val();
                $(".se-pre-con").fadeOut();
            }
            else {
                notify('error', 'Thêm dữ liệu thất bại', "2"); $(".se-pre-con").fadeOut();
            }
        }
    });

    return false;
    e.preventDefault();
});