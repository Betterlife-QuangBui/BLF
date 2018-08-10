$(document).on("click", ".spritTracking", function () {
    $(".wasehouse-info").load($(this).attr("data-href"));
    return false;
});
$(document).on("change", ".update-weigh", function () {
    $.ajax({
        url: '/StorageJP/UpdateWeigh',
        data: { id: $(this).attr("data-id"), weigh: $(this).val() },
        method: 'POST',async :false,
        success: function (result) {
            if (result.status == true) {
                $(".total-weigh").html(result.message.weigh);
            }
        }
    });
})
$(document).on("click", ".conform-check", function () {
    $.ajax({
        url: '/StorageJP/UpdateIsCheck',async :false,
        data: { id: $(this).attr("data-id"), isCheck: $(this).is(":checked") },
        method: 'POST',
        success: function (result) {
            
        }
    });
})
$(document).on("click", ".copy-row", function (e) {
    // var index = $("#simple-table tbody tr").index($(this).parent().parent());
    var thisRow = $(this).parent().parent();
    $(thisRow).clone().insertAfter(thisRow).find("td").each(function (e, i) {
        $(this).find("select[name=TrackingCode]").addClass("border")
        $(this).find("a[data-main=true]").attr("data-main", "false");
        $(this).find("input.quantity").attr("value", "0");
        $(this).find("input.quantity").addClass("border")
        $(this).find("select[name=TrackingCode]").focus();
    });
    return false;

    //var row = $(this).parent().parent();
    //.find("input.quantity").attr("value", "0").find("a[data-main=true]").attr("data-main", "false")
    //row = row.find("input").attr("value", "0");
    //var thisRow = row;
    //$(thisRow).clone().insertAfter(thisRow);
});
$(document).on("click", '.conform-close', function () {
    var id = $(this).attr('data-id'); $(".se-pre-con[data-id=" + id + "]").hide();
})
//update row
$(document).on("click", '.conform-save', function () {
    var id = $(this).attr('data-id');
    var el = $(this).parent().parent().parent();

    var trackingSub = $(el).find('td:eq(0) select').val();
    var trackingSub_Old = $(el).find('td:eq(0) input').val();
    var nameJP = $(el).find('td:eq(3) input').val();
    var nameEN = $(el).find('td:eq(4) input').val();
    var categoryId = $(el).find('td:eq(5) select').val();

    var quantity = $(el).find('td:eq(8) input').val();
    var price = $(el).find('td:eq(9)').html().trim();
    var amount = $(el).find('td.amount').html(parseFloat(price) * parseInt(quantity));
    //$(this).find('td.amount').number(true);
    var marital = $(el).find('td:eq(11) textarea').val();
    var madeIn = $(el).find('td:eq(12) select').val();
    var status = 1;//$(this).find('td:eq(11) select').val();
    var clone = $(el).find("a[data-main]").attr("data-main");
    var isDeny = $(el).find('td:eq(13) input[type=checkbox]').is(":checked");

    var dataTrackingparent = $(el).attr("data-trackingparent");
    $.ajax({
        url: '/StorageItemJP/UpdateItemV2',
        data: { id: id, trackingSub: trackingSub, categoryId: categoryId, nameJP: nameJP, nameEN: nameEN, price: price, quantity: quantity, marital: marital, madeIn: madeIn, status: status, trackingSub_Old: trackingSub_Old, clone: clone, isDeny: isDeny },
        method: 'POST', async: false,
        success: function (result) {
            if (result.status == true) {
                $(".total-quantity").html(result.message.count);
                $(".total-amount").html(result.message.amount);
                $(".se-pre-con[data-id=" + id + "]").hide();
                if ($(el).find('td:eq(0) select').hasClass("border")) {
                    $(el).find('td:eq(0) select').removeClass("border")
                }
                if ($(el).find('td:eq(8) input').hasClass("border")) {
                    $(el).find('td:eq(8) input').removeClass("border")
                }
                //if (trackingSub != dataTrackingparent) {
                //    var thisRow = $("tr[data-mainparent='" + dataTrackingparent + "']");
                //    $(thisRow).append($(el));
                //    $(thisRow).clone().insertAfter($(this).parent().parent().parent());
                //    return false;

                //}
               
            }
        }
    });
});

$(document).on("change", '.product-item', function () {
    var id = $(this).attr('data-id');
    var quantity = $(this).find('td:eq(8) input').val();
    if (quantity > 0) {
        var widtd = $("#simple-table").width();
        $(this).find(".se-pre-con").css("width", widtd + "px");
        $(this).find(".se-pre-con").show();
        return;
    }
});
//delete row
$(document).on("click", '.delete-row', function () {
    var id = $(this).attr('data-id');
    var removeIsRow = $(this).attr('data-main');
    if (removeIsRow == "true") {
        if (confirm('Bạn có chắc muốn xóa nó không')) {
            $.ajax({
                method: 'POST',async :false,
                url: '/StorageItemJP/DeleteItemRow', data: { id: id },
                success: function (result) {
                    if (result.status == true) {
                        $("tr#" + id).hide();
                        $(".total-quantity").html(result.message.count);
                        $(".total-amount").html(result.message.amount);
                    }
                }
            });
        }
    }
    else {
        $(this).parent().parent().remove();
    }
})
$(document).on("click", ".box-item", function () {
    var id = $(this).attr("data-id");
    $(".wasehouse-info").html("")
    $(".wasehouse-info").load($(this).attr("data-href"));

    $(".control").hide()
    $(".control[data-id=" + id + "]").show()

    $(".box-item").each(function (i, e) {
        var className = "active-item";
        if ($(this).hasClass(className)) {
            $(this).removeClass(className)
        }
    })
    $(this).addClass("active-item");
    return false;
})
$(document).on("click", ".pagging a", function () {
    var url = $(this).attr("href");
    $(".item-of-page").html("");
    $('html, body').animate({
        scrollTop: $(".page-content").offset().top
    }, 2000);
    $(".item-of-page").load(url);
    return false;
})
function inputSearch(key) {
    if ($(key).val().toString().length>2){
        $(".item-of-page").html("");
        $(".item-of-page").load('/Ajax/AgencyPackage?page=1&key=' + $(key).val());
    }
    if ($(key).val().toString().length ==0) {
        $(".item-of-page").html("");
        $(".item-of-page").load('/Ajax/AgencyPackage?page=1&key=' + $(key).val());
    }
}
$(window).load(function () {
    
});