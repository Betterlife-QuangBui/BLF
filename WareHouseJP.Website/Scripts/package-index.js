$(function () {
    $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
});
//edit shipment
$(document).on("click", ".shipment_item-edit-child", function (e) {
    $(".shipment_edit_body").html("");
    $(".package_add_body").html("");
    $(".wasehouse-info").html(""); $(".package_edit_body").html("");
    $(".shipment_edit_body").load($(this).attr("href"));
    return false;
});
$(function () {
    $("th.datepicker input").datepicker({
        inline: true,
        showOn: 'both', buttonImageOnly: true, buttonImage: '/images/ui-icon-calendar.png',
        dateFormat: "yy-mm-dd"
    });
});
//default column
$(document).on("click", ".default-column", function (e) {
    $(".hiden-column-content").hide(); $(".hiden-column-title").show();
    $(".hiden-column input[type=checkbox]").prop('checked', false);
    $(".hiden-column input[type=checkbox]").each(function (i, e) {
        $(this).prop('checked', true);
        if (i == 6 || i == 8 ) {
            $(this).prop('checked', false);
        }
    });
    $(".hiden-column input[type=checkbox]").each(function (i, e) {
        var index = parseInt($(e).attr("data-index")) + 1;
        if (e.checked) {
            $('#storejp-table tr > *:nth-child(' + index + ')').show();
        }
        else {
            $('#storejp-table tr > *:nth-child(' + index + ')').hide();
        }
    });
    return false;
    e.preventDefault();
});
//pagging jp
$(document).on("click", "#storejp-table .storejp-pagging-index a", function (e) {
    $(".wasehouse-info,.package_edit_body,.package_add_body").html("");
    
    var href = $(this).attr("href");
    var data_sort = $("#sort-jp").val();
    var ShipmentId = $("#ShipmentId").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var delivery = $("#storejp-table thead tr select:eq(0)").val();
    var weigh = $("#storejp-table thead tr input:eq(1)").val();
    var date_send = $("#storejp-table thead tr input:eq(3)").val();
    var hour_send = $("#storejp-table thead tr input:eq(4)").val();
    var date_recive = $("#storejp-table thead tr input:eq(5)").val();
    var hour_recive = $("#storejp-table thead tr input:eq(6)").val();
    var status = $("#storejp-table thead tr select:eq(1)").val();
    var notes = $("#storejp-table thead tr input:eq(2)").val();
    if (delivery == "") { delivery = "0"; }
    if (weigh == "") { weigh = "-1"; }
    if (href === undefined || href === null) { }
    else {
        $("#storejp-table tbody").html("");
        href = href + "&trackingcode=" + trackingcode + "&delivery=" + delivery + "&weigh=" + weigh + "&date_send=" + date_send + "&hour_send=" + hour_send + "&date_recive="
            + date_recive + "&hour_recive=" + hour_recive + "&status=" + status + "&notes=" + notes+ "&data_sort=" + data_sort;
        $("#storejp-table tbody").load(href);
    }
    
   
    e.preventDefault();
});
//sort-jp
$(document).on("click", "#storejp-table .sort-row .sort", function (e) {
    $(".wasehouse-info,.package_edit_body,.package_add_body").html("");
    loading();
    var data_sort = $(this).parent().attr("data-sort");
    $("#sort-jp").val(data_sort);
    var data_sort_return = data_sort.split('-')[0] + "-" + (data_sort.split('-')[1] == "asc" ? "desc" : "asc");
    $(this).parent().attr("data-sort", data_sort_return);
    
    var data_sort = $("#sort-jp").val();
    var ShipmentId = $("#ShipmentId").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var delivery = $("#storejp-table thead tr select:eq(0)").val();
    var weigh = $("#storejp-table thead tr input:eq(1)").val();
    var date_send = $("#storejp-table thead tr input:eq(3)").val();
    var hour_send = $("#storejp-table thead tr input:eq(4)").val();
    var date_recive = $("#storejp-table thead tr input:eq(5)").val();
    var hour_recive = $("#storejp-table thead tr input:eq(6)").val();
    var status = $("#storejp-table thead tr select:eq(1)").val();
    var notes = $("#storejp-table thead tr input:eq(2)").val();
    if (delivery == "") { delivery = "0"; }
    if (weigh == "") { weigh = "-1"; }

    var href = "/ajax/Package?id=" + ShipmentId + "&page=1&trackingcode=" + trackingcode + "&delivery=" + delivery + "&weigh=" + weigh + "&date_send=" + date_send + "&hour_send=" + hour_send + "&date_recive="
        + date_recive + "&hour_recive=" + hour_recive + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;

    //display sort
    $("#storejp-table .sort-row th").each(function (i, item) {
        $(this).find((".sort-asc")).show();
        $(this).find((".sort-desc")).hide();
    });
    $(this).hide();
    if (data_sort.split('-')[1] == "asc") {
        $(this).next().show();
    }
    if (data_sort.split('-')[1] == "desc") {
        $(this).prev().show();
    }

    $("#storejp-table tbody").html("");
    $("#storejp-table tbody").load(href);
});
//filter-jp
$(document).on("keyup", "#storejp-table .filter-jp input", function (e) {
    $(".wasehouse-info,.package_edit_body,.package_add_body").html("");
    loading()
    var data_sort = $("#sort-jp").val();
    var ShipmentId = $("#ShipmentId").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var delivery = $("#storejp-table thead tr select:eq(0)").val();
    var weigh = $("#storejp-table thead tr input:eq(1)").val();
    var date_send = $("#storejp-table thead tr input:eq(3)").val();
    var hour_send = $("#storejp-table thead tr input:eq(4)").val();
    var date_recive = $("#storejp-table thead tr input:eq(5)").val();
    var hour_recive = $("#storejp-table thead tr input:eq(6)").val();
    var status = $("#storejp-table thead tr select:eq(1)").val();
    var notes = $("#storejp-table thead tr input:eq(2)").val();
    if (delivery == "") { delivery = "0"; }
    if (weigh == "") { weigh = "-1"; }

    var href = "/ajax/Package?id=" + ShipmentId + "&page=1&trackingcode=" + trackingcode + "&delivery=" + delivery + "&weigh=" + weigh + "&date_send=" + date_send + "&hour_send=" + hour_send + "&date_recive="
        + date_recive + "&hour_recive=" + hour_recive + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
    $("#storejp-table tbody").html("");
    $("#storejp-table tbody").load(href);
});
$(document).on("change", "#storejp-table .filter-jp select,#storejp-table .filter-jp input:eq(3),#storejp-table .filter-jp input:eq(5)", function (e) {
    $(".wasehouse-info,.package_edit_body,.package_add_body").html("");
    loading()
    var data_sort = $("#sort-jp").val();
    var ShipmentId = $("#ShipmentId").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var delivery = $("#storejp-table thead tr select:eq(0)").val();
    var weigh = $("#storejp-table thead tr input:eq(1)").val();
    var date_send = $("#storejp-table thead tr input:eq(3)").val();
    var hour_send = $("#storejp-table thead tr input:eq(4)").val();
    var date_recive = $("#storejp-table thead tr input:eq(5)").val();
    var hour_recive = $("#storejp-table thead tr input:eq(6)").val();
    var status = $("#storejp-table thead tr select:eq(1)").val();
    var notes = $("#storejp-table thead tr input:eq(2)").val();
    if (delivery == "") { delivery = "0"; }
    if (weigh == "") { weigh = "-1"; }

    var href = "/ajax/Package?id=" + ShipmentId + "&page=1&trackingcode=" + trackingcode + "&delivery=" + delivery + "&weigh=" + weigh + "&date_send=" + date_send + "&hour_send=" + hour_send + "&date_recive="
        + date_recive + "&hour_recive=" + hour_recive + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
    $("#storejp-table tbody").html("");
    $("#storejp-table tbody").load(href);
});

//check box
$(document).on("click", ".checkall", function (e) {
    var check = $(this).prop("checked");
    $("#storejp-table input[type='checkbox']").prop('checked', check);
    var arr = [];
    $("#storejp-table tr.item input[type='checkbox']").each(function (i, e) {
        if ($(this).prop("checked")) {
            arr.push($(this).attr("id"));
        }
    });

    if (arr.length > 0) {
        $(".action-table").find("input,button,textarea,select").removeAttr("disabled");
        $("#StoreJPId").val(arr.toString());
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("#StoreJPId").val("");
    }
});
$(document).on("click", "#storejp-table tr.item input[type='checkbox']", function (e) {
    var arr = [];
    $("#storejp-table tr.item input[type='checkbox']").each(function (i, e) {
        if ($(this).prop("checked")) {
            arr.push($(this).attr("id"));
        }
    });
    if (arr.length > 0) {
        $(".action-table").find("input,button,textarea,select").removeAttr("disabled");
       $("#StoreJPId").val(arr.toString());
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("#StoreJPId").val("");
    }
});
//delete one
$(document).on("click", ".conform-acion-index", function (e) {
    storejp = $("#StoreJPId").val();
    if (storejp == "") {
        $.ajax({
            method: 'POST', async: false,
            url: $(this).attr("data-url"),
            success: function (result) {
                if (result.status == true) {
                    notify('white', 'Xóa dữ liệu thành công', "1");
                    $(".wasehouse-info,.package_edit_body,.package_add_body").html("");
                    loading()
                    var data_sort = $("#sort-jp").val();
                    var ShipmentId = $("#ShipmentId").val();
                    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
                    var delivery = $("#storejp-table thead tr select:eq(0)").val();
                    var weigh = $("#storejp-table thead tr input:eq(1)").val();
                    var date_send = $("#storejp-table thead tr input:eq(3)").val();
                    var hour_send = $("#storejp-table thead tr input:eq(4)").val();
                    var date_recive = $("#storejp-table thead tr input:eq(5)").val();
                    var hour_recive = $("#storejp-table thead tr input:eq(6)").val();
                    var status = $("#storejp-table thead tr select:eq(1)").val();
                    var notes = $("#storejp-table thead tr input:eq(2)").val();
                    if (delivery == "") { delivery = "0"; }
                    if (weigh == "") { weigh = "-1"; }

                    var href = "/ajax/Package?id=" + ShipmentId + "&page=1&trackingcode=" + trackingcode + "&delivery=" + delivery + "&weigh=" + weigh + "&date_send=" + date_send + "&hour_send=" + hour_send + "&date_recive="
                        + date_recive + "&hour_recive=" + hour_recive + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
                    $("#storejp-table tbody").html("");
                    $("#storejp-table tbody").load(href);
                }
                else {
                    notify('error', "Xóa dữ liệu thất bại", "2");
                }
            }
        });
    }
    else {
        deleteStore(storejp);
    }
    $(".se-pre-con-crud-index").hide();
});
$(document).on("click", "a.package_item-delete-new", function (e) {
    showdelete("#storejp-table");
    $(".conform-acion-index").attr("data-url", $(this).attr("data-url"))
    return false;
    e.preventDefault();
});

$(document).on("click", ".btn-action-table", function (e) {
    
    storejp = $("#StoreJPId").val();
    action = $(".select-action-table").val();
    if (action != "-1" && storejp != "") {
        if (action == "1") {
            //$(".se-pre-con").show();
            //deleteStore(storejp); $(".se-pre-con").fadeOut();
            showdelete("#storejp-table");
        }
    }
});
function deleteStore(storejp) {
    $.ajax({
        method: 'POST', async: false,
        url: "/AgencyPackage/DeleteMultiple/", data: { id: storejp },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Xóa dữ liệu thành công', "1");
                $(".wasehouse-info,.package_edit_body,.package_add_body").html("");
                loading()
                var data_sort = $("#sort-jp").val();
                var ShipmentId = $("#ShipmentId").val();
                var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
                var delivery = $("#storejp-table thead tr select:eq(0)").val();
                var weigh = $("#storejp-table thead tr input:eq(1)").val();
                var date_send = $("#storejp-table thead tr input:eq(3)").val();
                var hour_send = $("#storejp-table thead tr input:eq(4)").val();
                var date_recive = $("#storejp-table thead tr input:eq(5)").val();
                var hour_recive = $("#storejp-table thead tr input:eq(6)").val();
                var status = $("#storejp-table thead tr select:eq(1)").val();
                var notes = $("#storejp-table thead tr input:eq(2)").val();
                if (delivery == "") { delivery = "0"; }
                if (weigh == "") { weigh = "-1"; }

                var href = "/ajax/Package?id=" + ShipmentId + "&page=1&trackingcode=" + trackingcode + "&delivery=" + delivery + "&weigh=" + weigh + "&date_send=" + date_send + "&hour_send=" + hour_send + "&date_recive="
                    + date_recive + "&hour_recive=" + hour_recive + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
                $("#storejp-table tbody").html("");
                $("#storejp-table tbody").load(href);
            }
            else {
                notify('error', "Xóa dữ liệu thất bại", "2");
            }
        }
    });
}


$(document).on("click", ".detail-package-item", function (e) {

    //var href = $(this).attr("href");
    //var id = $(this).attr("data-id");
    //$(".wasehouse-info,.package_edit_body,.package_add_body").html("");

    //$(".wasehouse-info").load(href);
    //$("#storejp-table tbody tr").each(function (i, e) {
    //    var className = "active";
    //    if ($(this).hasClass(className)) {
    //        $(this).removeClass(className)
    //    }
    //})
    //$("#storejp-table tr[data-id=" + id + "]").addClass("active");
    //e.preventDefault();
    location.href = $(this).attr("href");
});
$(document).on("click", ".option-detail-crud a", function (e) {
    if ($(this).attr("data-index") == "1") {
        $(".tr-hidden").show();
        $(this).attr("data-index", "2");
        $(this).html("Đóng chi tiết");
    }
    else {
        $(".tr-hidden").hide();
        $(this).attr("data-index", "1");
        $(this).html("Chi tiết");
    }
    return false;
    e.preventDefault();
});

$(document).on("click", ".package_add", function (e) {
    $(".package_add_body").html(""); $(".package_edit_body").html("");$(".wasehouse-info").html("");
    $("tr.add-item,tr.filter-jp-action").show();
    $("tr.filter-jp").hide();
    $("#btn-save-add").attr("disabled", "disabled");
    return false;
    e.preventDefault();
});
$(document).on("click", ".cancel-add-item", function (e) {
    $(".package_add_body").html(""); $(".package_edit_body").html(""); $(".wasehouse-info").html("");
    $("tr.add-item,tr.filter-jp-action").hide();
    $("tr.filter-jp").show(); $("#btn-save-add").attr("disabled", "disabled");
    $(".add-item").find("input[type=text]:eq(0),input[type=number]:eq(0),select:eq(0),select:eq(1)").val("")
    $(".add-item").find("input[type=text]:eq(0),input[type=number]:eq(0),select:eq(0),select:eq(1)").addClass("display-error-form")
    return false;
    e.preventDefault();
});
$(document).on("change", ".add-item", function (e) {
    var count = 0;
    //check tracking code
    TrackingCode = $(this).find("#TrackingCode").val();
    if (TrackingCode.toString().trim().length > 0) {
        //check exist tracking code
        $.ajax({
            method: 'POST', async: false,
            url: '/AgencyPackage/CheckExistTrackingCode',
            data: { tracking: TrackingCode.toString().trim() },
            success: function (result) {
                if (result.status == true) {
                    count += 1;
                }
                else {
                    notify('white', 'Mã tracking đã tồn tại', "1");
                }
            }
        });
    }
    else
    {
        notify('white', 'Vui lòng nhập mã tracking', "1");
    }
    if (count == 1) { $(this).find("#TrackingCode").removeClass("display-error-form"); }
    else { $(this).find("#TrackingCode").addClass("display-error-form"); }
    //check weigh
    if ($(this).find("#Weigh").val().toString().trim().length > 0) {
        count += 1; $(this).find("#Weigh").removeClass("display-error-form");
    }
    else {
        notify('white', 'Vui lòng nhập số kg', "1");
        $(this).find("#Weigh").addClass("display-error-form");
    }
    //DeliveryId
    if ($(this).find("#DeliveryId").val().toString().trim().length > 0) {
        count += 1; $(this).find("#DeliveryId").removeClass("display-error-form");
    }
    else {
        notify('white', 'Vui lòng chọn công ty giao nhận', "1");
        $(this).find("#DeliveryId").addClass("display-error-form");
    }
    //StatusId
    if ($(this).find("#StatusId").val().toString().trim().length > 0) {
        count += 1; $(this).find("#StatusId").removeClass("display-error-form");
    }
    else {
        notify('white', 'Vui lòng chọn trạng thái', "1");
        $(this).find("#StatusId").addClass("display-error-form");
    }
    if (count >= 4) {
        $("#btn-save-add").removeAttr("disabled");
        $(".message-alert").hide()
    }
    else {
        $("#btn-save-add").attr("disabled", "disabled");
    }
    return false;
    e.preventDefault();
});