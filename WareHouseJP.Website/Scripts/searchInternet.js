$(document).on("click", "#btnSearchJP", function (e) {
    $(".se-pre-con").show(); $("#search-form").submit();
});
function triggerParent() {
    id = $("input[name='StoregeJPId']").val();
    $.ajax({
        method: 'POST',async :false,
        url: '/StorageItemJP/UpdateParent', data: { id: id},
        success: function (result) {
            if (result.status == true) {
                //update kg
                $(parent.document.getElementById(id)).find(".info-tracking span:eq(0)").html("Số kiện: " + result.message.count + " - Số kg: " + result.message.weight);
                //update status
                $(parent.document.getElementById(id)).find("span#span-" + id).html(result.message.status);
                //call view detail
                $(parent.document.getElementById(id)).trigger('click');
            }
        }
    });
}
$(document).on("change", ".product-search", function () {
    id = $(this).attr("data-id");
    jancode = $(this).find('td:eq(3) input').val();
    namejp = $(this).find('td:eq(4) textarea').val();
    price = $(this).find('td:eq(5) input').val();
    $.ajax({
        method: 'POST',async :false,
        url: '/StorageItemJP/UpdateSearchProduct', data: { id: id, jancode: jancode, namejp: namejp, price: price },
        success: function (result) {}
    });
})
$(document).on("change", ".item-tracking-detail", function () {
    var id = $(this).attr('data-id');
    var weigh = $(this).find('input[name="sokg"]').val();
    var notes = $(this).find('textarea').val();
    $.ajax({
        method: 'POST',async :false,
        url: '/StorageItemJP/UpdateTrackingDetail', data: { id: id, weigh: weigh, size: "0", note: notes, status: 1 },
        success: function (result) {
            triggerParent()
        }
    });
});
$(document).on("click", ".removeTrackingMain", function () {
    if (confirm('Bạn có chắc muốn xóa dữ liệu này?')) {
        id = $(this).attr("data-id");
        dataParent = $(this).attr("data-parent");
        $.ajax({
            method: 'POST',async :false,
            url: '/StorageItemJP/DeleteTracking', data: { id: id, dataParent: dataParent },
            success: function (result) {
                if (result.status == true) {
                    $("#li" + id).remove();
                    $("#detail" + id).remove();
                    $('#myTab1s a:first').tab('show')
                    triggerParent()
                }
            }
        });
    }
});
$(function () {
    triggerParent();
});
$(document).on("click", ".tracking-sub", function () {
    var id = $(this).attr("data-id");
    var trackingdetail = $(this).attr("data-trackingdetail");
    $("#modaltrackingDetailId").val(trackingdetail);
    $("#dialogQuantity").val($(this).parent().parent().find('td:eq(8) input').val());
    $("#modalId").val(id);
});
$(document).on("click", ".delete-item", function () {
    if (confirm('Bạn có chắc muốn xóa dữ liệu này?')) {
        id = $(this).attr("data-id");
        $.ajax({
            method: 'POST',async :false,
            url: '/StorageItemJP/Delete', data: { id: id },
            success: function (result) {
                if (result.status == true) {
                    $("tr#" + id).hide();
                    $("tr#" + id).parents().find(".sum_amount").html(result.message.amount);
                    $("tr#" + id).parents().find(".sum_amount").number(true);
                    $("tr#" + id).parents().find(".sum_quantity").html(result.message.count);
                    triggerParent()
                }
            }
        });
    }
});
$(document).on("click", ".delete-item-parent", function () {
    if (confirm('Bạn có chắc muốn xóa dữ liệu này?')) {
        id = $(this).attr("data-id");
        parent = $(this).attr("data-parent");
        $.ajax({
            method: 'POST',async :false,
            url: '/StorageItemJP/DeleteItemChild', data: { id: id, parentJ: parent },
            success: function (result) {
                if (result.status == true) {
                    window.location.reload();
                }
            }
        });
    }
});

$(document).on("change", '.product-item', function () {
    var id = $(this).attr('data-id');
    var categoryId = $(this).find('td:eq(4) select').val();
    var nameJP = $(this).find('td:eq(2) input').val();
    var nameEN = $(this).find('td:eq(3) input').val();
    var price = $(this).find('td:eq(7) input').val();
    var quantity = $(this).find('td:eq(8) input').val();
    var amount = $(this).find('td.amount').html(parseFloat(price) * parseInt(quantity));
    $(this).find('td.amount').number(true);
    var marital = $(this).find('td:eq(10) textarea').val();
    var madeIn = $(this).find('td:eq(11) select').val();
    var status = 1;
    $.ajax({
        url: '/StorageItemJP/UpdateItem',async :false,
        data: { id: id, categoryId: categoryId, nameJP: nameJP, nameEN: nameEN, price: price, quantity: quantity, marital: marital, madeIn: madeIn, status: status },
        method: 'POST',
        success: function (result) {
            if (result.status == true) {
                $("tr#" + id).parents().find(".sum_amount").html(result.message.amount);
                $("tr#" + id).parents().find(".sum_amount").number(true);
                $("tr#" + id).parents().find(".sum_quantity").html(result.message.count);
                triggerParent()
            }
        }
    });
});
$(function () {
    $('#myTabs a').click(function (e) {
        e.preventDefault()
        $(this).tab('show')
    });
    $(".btnSaveTracking").click(function () {

        var subtracking = $("#modal-body #TrackingCode").val();
        var quantity = $("#dialogQuantity").val();
        var detailId = $("#modaltrackingDetailId").val();
        var id = $("#modalId").val();
        var subtracking = $("#gridSystemModal #TrackingCode").val();
        var modalStoregeJPId = $("#modalStoregeJPId").val();

        $.ajax({
            url: '/StorageItemJP/SaveTrackingSub',
            data: { id: id, subtracking: subtracking, quantity: quantity, detailId: detailId, modalStoregeJPId: modalStoregeJPId },
            method: 'POST',async :false,
            success: function (result) {
                if (result.status == true) {
                    window.location.reload();
                }
            }
        });

    });
});