$(document).on("click", ".spritTracking", function () {
    $(".wasehouse-info").load($(this).attr("data-href"));
    return false;
});

$(document).on("click", ".storejp-pagging a", function (e) {
    $(".wasehouse-info").html("");
    var href = $(this).attr("href");
    if (href === undefined || href === null) { }
    else {
        $(".storejp-content").html("");
        $(".storejp-content").load($(this).attr("href"));
    }
    e.preventDefault();
});
$(document).on("change", ".storejp-select", function (e) {
    key=$("#storejp_search_key").val()
    $(".wasehouse-info").html("");
    $(".storejp-content").html("");
    $(".storejp-content").load("/ajax/storejp?page=1&key="+key+"&sort=" + $(this).val());
    return false;
    e.preventDefault();
});

$(document).on("keyup", "#storejp_search_key", function (e) {
    key = $(this).val();
    sort = $(".storejp-select").val();
    $(".wasehouse-info").html("");
    $(".storejp-content").html("");
    $(".storejp-content").load("/ajax/storejp?page=1&key=" + key + "&sort=" + sort);
    return false;
    e.preventDefault();
});


function triggerParent() {
    id = $("input[name='leftId']").val();
    $.ajax({
        method: 'POST', async: false,
        url: '/StorageItemJP/UpdateParent', data: { id: id },
        success: function (result) {
            if (result.status == true) {
                //update kg
                $("#" + id).find(".info-tracking span:eq(0)").html("Số kiện: " + result.message.count + " - Số kg: " + result.message.weight);
                //update status
                $("#" + id).find("span#span-" + id).html(result.message.status);
                //call view detail
                $("#" + id).trigger('click');
            }
        }
    });
}
$(document).on("change", ".update-weigh", function () {
    $.ajax({
        url: '/StorageJP/UpdateWeigh', async: false,
        data: { id: $(this).attr("data-id"), weigh: $(this).val() },
        method: 'POST',
        success: function (result) {
            if (result.status == true) {
                $(".total-weigh").html(result.message.weigh);
                triggerParent()
            }
        }
    });
});
$(document).on("click", ".conform-check", function (e) {
    id = $(this).attr("data-id");
    check = $(this).is(":checked");
    $.ajax({
        url: '/StorageJP/UpdateIsCheck',
        data: { id: id, isCheck: $(this).is(":checked") },
        method: 'POST', async: false,
        success: function (result) {
            if (result.status == true) {
                $("span#span-" + id).html(result.message);
                notify('white', 'Cập nhật dữ liệu thành công', "1");
                if (check == true) {
                    //disable control
                    $("div.control[data-id='" + id + "'] a:eq(0)").addClass("disabled");
                    $("div.control[data-id='" + id + "'] a:eq(1)").addClass("disabled");
                    $("div.control[data-id='" + id + "'] a:eq(2)").addClass("disabled");
                    $("#simple-table a").addClass("disabled");
                    $("#simple-table input").attr("readonly", "readonly");
                    $("#simple-table input").attr("disabled", "disabled");
                    $("#simple-table select").attr('disabled', 'disabled');
                }
                else {
                    $("div.control[data-id='" + id + "'] a").removeClass("disabled");
                    $("#simple-table a").removeClass("disabled");
                    $("#simple-table input").removeAttr('readonly');
                    $("#simple-table input").removeAttr('disabled');
                    $("#simple-table select").removeAttr('disabled');
                }
                triggerParent()
            }
            else {
                notify('error', 'Cập nhật dữ liệu thất bại', "2");
            }
        }
    }).done();
})
$(document).on("click", ".copy-row", function (e) {
    var thisRow = $(this).parent().parent();
    $(thisRow).clone().insertAfter(thisRow).find("td").each(function (e, i) {
        $(this).find("select[name=TrackingCode]").addClass("border")
        $(this).find("a[data-main=true]").attr("data-main", "false");
        $(this).find("input.quantity").attr("value", "0");
        $(this).find("input.quantity").addClass("border")
        $(this).find("select[name=TrackingCode]").focus();
    });
    return false;
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
    //var jancode = $(el).find('td:eq(2) input').val();
    var nameJP = $(el).find('td:eq(3) input').val();
    var nameEN = $(el).find('td:eq(4) input').val();
    var categoryId = $(el).find('td:eq(5) select').val();
    var status = 1;
    var quantity = $(el).find('td:eq(8) input').val();
    var price = $(el).find('td:eq(9) input').val();
    var amount = $(el).find('td.amount').html(parseFloat(price) * parseInt(quantity));
    var marital = $(el).find('td:eq(11) textarea').val();
    var madeIn = $(el).find('td:eq(12) select').val();
    var clone = $(el).find("a[data-main]").attr("data-main");
    var isDeny = $(el).find('td:eq(13) input[type=checkbox]').is(":checked");

    var dataTrackingparent = $(el).attr("data-trackingparent");
    $.ajax({
        url: '/StorageItemJP/UpdateItemV2',
        data: { id: id, trackingSub: trackingSub, categoryId: categoryId, nameJP: nameJP, nameEN: nameEN, price: price, quantity: quantity, marital: marital, madeIn: madeIn, status: status, trackingSub_Old: trackingSub_Old, clone: clone, isDeny: isDeny },
        method: 'POST',
        async :false,
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
                notify('white', 'Cập nhật dữ liệu thành công', "1");
                triggerParent()
            }
            else {
                notify('error', result.message, "2");
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
        $.ajax({
            method: 'POST', async: false,
            url: '/StorageItemJP/DeleteItemRow', data: { id: id },
            success: function (result) {
                if (result.status == true) {
                    $("tr#" + id).hide();
                    $(".total-quantity").html(result.message.count);
                    $(".total-amount").html(result.message.amount);
                    notify('white', 'Xóa dữ liệu thành công', "1");
                    if (result.message.delete == "1") {
                        $("#tracking-detail-" + result.message.parent).hide();
                    }
                    triggerParent()
                }
                else {
                    notify('error', result.message, "2");
                }
            }
        });
    }
    else {
        $(this).parent().parent().remove();
    }
})
$(document).on("click", ".storejp-box-item", function () {
    var id = $(this).attr("data-id");
    $(".wasehouse-info").html("")
    $(".wasehouse-info").load($(this).attr("data-href"))
    $(".storejp-box-item").each(function (i, e) {
        var className = "active";
        if ($(this).hasClass(className)) {
            $(this).removeClass(className)
        }
    })
    $(this).addClass("active");
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
    if ($(key).val().toString().length > 2) {
        $(".item-of-page").html("");
        $(".item-of-page").load('/Ajax/StoreJP?page=1&key=' + $(key).val());
    }
    if ($(key).val().toString().length == 0) {
        $(".item-of-page").html("");
        $(".item-of-page").load('/Ajax/StoreJP?page=1&key=' + $(key).val());
    }
}
$(window).load(function () {

});