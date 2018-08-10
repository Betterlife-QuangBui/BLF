﻿//add new package
$(document).on("click", ".package_add", function (e) {
    $(".package_add_body").html("");
    $(".wasehouse-info").html(""); $(".package_edit_body").html("");
    $(".package_add_body").load($(this).attr("href"));
    return false;
    e.preventDefault();
});
//edit package
$(document).on("click", ".package_item-edit", function (e) {
    $(".package_add_body").html(""); $(".package_edit_body").html("");
    $(".wasehouse-info").html("");
    $(this).parent().parent().parent().parent().find(".package_edit_body").load($(this).attr("href"));
    return false;
});
$(document).on("click", ".close-package", function (e) {
    $(".package_add_body").html(""); $(".package_edit_body").html("");
    $(".wasehouse-info").html("");
    return false;
    e.preventDefault();
});

$(document).on("keyup", "#txt-package", function (e) {
    key = $(this).val();
    ShipmentId = $("#ShipmentId").val();
    $(".package_add_body").html(""); $(".package_edit_body").html("");
    $(".package_content").html(""); $(".wasehouse-info").html("");
    $(".package_content").load("/ajax/Package/" + ShipmentId + "?key=" + key + "&page=1&sort=" + $("#PackageStatusId").val());
    return false;
    e.preventDefault();
});


$(document).on("click", "a.package_item-delete", function (e) {
    if (confirm("Bạn có chắc muốn xóa kiện hàng này?")) {
        id = $(this).attr("data-id")
        $.ajax({
            method: 'POST', async: false,
            url: $(this).attr("href"),
            success: function (result) {
                if (result.status == true) {
                    $(".wasehouse-info").html("");
                    var href = "/ajax/package/" + id + "?page=1&sort=" + $("#PackageStatusId").val();


                    $(".package_add_body").html(""); $(".package_edit_body").html("");
                    $(".package-detail_content,.package_content").html("");
                    $(".package_content").load(href);
                    notify('white', 'Xóa dữ liệu thành công', "1");
                }
                else {
                    notify('error', "Xóa dữ liệu thất bại", "2");
                }
            }
        });
    }
    return false;
    e.preventDefault();
});
//Import shipment
$(document).on("click", ".shipment_item-ixport", function (e) {
    $(".package_add_body").html("");
    $(".wasehouse-info").html(""); $(".package_edit_body").html("");
    $(".package_add_body").load($(this).attr("href"));
    return false;
    e.preventDefault();
});
$(document).on("click", ".close-package", function (e) {
    $(".package_add_body").html(""); $(".package_edit_body").html("");
    $(".wasehouse-info").html("");
    return false;
    e.preventDefault();
});


$(function () {
    $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
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
        if (i == 0 || i == 1 || i == 2 || i == 3 || i == 7 || i == 8) {
            $(this).prop('checked', true);
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
    var data_sort = $("#sort-shipping").val();
    var shippingCode = $("#storejp-table thead tr input:eq(0)").val();
    var shipper = $("#storejp-table thead tr select:eq(0)").val();
    var mawb = $("#storejp-table thead tr select:eq(1)").val();
    var dateFlight = $("#storejp-table thead tr input:eq(1)").val();
    var from = $("#storejp-table thead tr select:eq(2)").val();
    var to = $("#storejp-table thead tr select:eq(3)").val();
    var status = $("#storejp-table thead tr select:eq(4)").val();
    var notes = $("#storejp-table thead tr input:eq(2)").val();

    if (shipper == "") { shipper = "0"; }
    if (href === undefined || href === null) { }
    else {
        $("#storejp-table tbody").html("");
        href = href + "&shippingCode=" + shippingCode + "&shipper=" + shipper + "&mawb=" + mawb + "&dateFlight=" + dateFlight + "&from=" + from + "&to="
            + to + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
        $("#storejp-table tbody").load(href);
    }
    e.preventDefault();
});
//sort-jp
$(document).on("click", "#storejp-table .sort-row .sort", function (e) {
    $(".wasehouse-info,.package_edit_body,.package_add_body").html("");
    loading();
    var data_sort = $(this).parent().attr("data-sort");
    $("#sort-shipping").val(data_sort);
    var data_sort_return = data_sort.split('-')[0] + "-" + (data_sort.split('-')[1] == "asc" ? "desc" : "asc");
    $(this).parent().attr("data-sort", data_sort_return);

    var data_sort = $("#sort-shipping").val();
    var shippingCode = $("#storejp-table thead tr input:eq(0)").val();
    var shipper = $("#storejp-table thead tr select:eq(0)").val();
    var mawb = $("#storejp-table thead tr select:eq(1)").val();
    var dateFlight = $("#storejp-table thead tr input:eq(1)").val();
    var from = $("#storejp-table thead tr select:eq(2)").val();
    var to = $("#storejp-table thead tr select:eq(3)").val();
    var status = $("#storejp-table thead tr select:eq(4)").val();
    var notes = $("#storejp-table thead tr input:eq(2)").val();

    if (shipper == "") { shipper = "0"; }

    var href = "/ajax/Shipping?page=1" + "&shippingCode=" + shippingCode + "&shipper=" + shipper + "&mawb=" + mawb + "&dateFlight=" + dateFlight + "&from=" + from + "&to="
            + to + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;

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

    var data_sort = $("#sort-shipping").val();
    var shippingCode = $("#storejp-table thead tr input:eq(0)").val();
    var shipper = $("#storejp-table thead tr select:eq(0)").val();
    var mawb = $("#storejp-table thead tr select:eq(1)").val();
    var dateFlight = $("#storejp-table thead tr input:eq(1)").val();
    var from = $("#storejp-table thead tr select:eq(2)").val();
    var to = $("#storejp-table thead tr select:eq(3)").val();
    var status = $("#storejp-table thead tr select:eq(4)").val();
    var notes = $("#storejp-table thead tr input:eq(2)").val();

    if (shipper == "") { shipper = "0"; }

    var href = "/ajax/Shipping?page=1" + "&shippingCode=" + shippingCode + "&shipper=" + shipper + "&mawb=" + mawb + "&dateFlight=" + dateFlight + "&from=" + from + "&to="
            + to + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
    loading()
    $("#storejp-table tbody").html("");
    $("#storejp-table tbody").load(href);
});
$(document).on("change", "#storejp-table .filter-jp select,#storejp-table .filter-jp input:eq(1)", function (e) {
    $(".wasehouse-info,.package_edit_body,.package_add_body").html("");
    loading()
    var data_sort = $("#sort-shipping").val();
    var shippingCode = $("#storejp-table thead tr input:eq(0)").val();
    var shipper = $("#storejp-table thead tr select:eq(0)").val();
    var mawb = $("#storejp-table thead tr select:eq(1)").val();
    var dateFlight = $("#storejp-table thead tr input:eq(1)").val();
    var from = $("#storejp-table thead tr select:eq(2)").val();
    var to = $("#storejp-table thead tr select:eq(3)").val();
    var status = $("#storejp-table thead tr select:eq(4)").val();
    var notes = $("#storejp-table thead tr input:eq(2)").val();

    if (shipper == "") { shipper = "0"; }

    var href = "/ajax/Shipping?page=1" + "&shippingCode=" + shippingCode + "&shipper=" + shipper + "&mawb=" + mawb + "&dateFlight=" + dateFlight + "&from=" + from + "&to="
            + to + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
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
        $("#ShippingId").val(arr.toString());
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("#ShippingId").val("");
    }
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
$(document).on("click", "#storejp-table tr.item input[type='checkbox']", function (e) {
    var arr = [];
    $("#storejp-table tr.item input[type='checkbox']").each(function (i, e) {
        if ($(this).prop("checked")) {
            arr.push($(this).attr("id"));
        }
    });
    if (arr.length > 0) {
        $(".action-table").find("input,button,textarea,select").removeAttr("disabled");
        $("#ShippingId").val(arr.toString());
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("#ShippingId").val("");
    }
});
//delete one
$(document).on("click", ".conform-acion-index", function (e) {
    storejp = $("#ShippingId").val();
    if (storejp == "") {
        $.ajax({
            method: 'POST', async: false,
            url: $(this).attr("data-url"),
            success: function (result) {
                if (result.status == true) {
                    notify('white', 'Xóa dữ liệu thành công', "1");
                    $(".wasehouse-info,.package_edit_body,.package_add_body").html("");
                    loading()
                    var data_sort = $("#sort-shipping").val();
                    var shippingCode = $("#storejp-table thead tr input:eq(0)").val();
                    var shipper = $("#storejp-table thead tr select:eq(0)").val();
                    var mawb = $("#storejp-table thead tr select:eq(1)").val();
                    var dateFlight = $("#storejp-table thead tr input:eq(1)").val();
                    var from = $("#storejp-table thead tr select:eq(2)").val();
                    var to = $("#storejp-table thead tr select:eq(3)").val();
                    var status = $("#storejp-table thead tr select:eq(4)").val();
                    var notes = $("#storejp-table thead tr input:eq(2)").val();

                    if (shipper == "") { shipper = "0"; }

                    var href = "/ajax/Shipping?page=1" + "&shippingCode=" + shippingCode + "&shipper=" + shipper + "&mawb=" + mawb + "&dateFlight=" + dateFlight + "&from=" + from + "&to="
                            + to + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
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
$(document).on("click", "a.delete-shipping-one", function (e) {
    showdelete("#storejp-table");
    $(".conform-acion-index").attr("data-url", $(this).attr("data-url"))
    return false;
    e.preventDefault();
});

$(document).on("click", ".btn-action-table", function (e) {
    storejp = $("#ShippingId").val();
    action = $(".select-action-table").val();
    if (action != "-1" && storejp != "") {
        if (action == "1") {
            showdelete("#storejp-table");
        }
    }
});
function deleteStore(storejp) {
    $.ajax({
        method: 'POST', async: false,
        url: "/Shipping/DeleteMultiple/", data: { id: storejp },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Xóa dữ liệu thành công', "1");
                $(".wasehouse-info,.package_edit_body,.package_add_body").html("");
                loading()
                var data_sort = $("#sort-shipping").val();
                var shippingCode = $("#storejp-table thead tr input:eq(0)").val();
                var shipper = $("#storejp-table thead tr select:eq(0)").val();
                var mawb = $("#storejp-table thead tr select:eq(1)").val();
                var dateFlight = $("#storejp-table thead tr input:eq(1)").val();
                var from = $("#storejp-table thead tr select:eq(2)").val();
                var to = $("#storejp-table thead tr select:eq(3)").val();
                var status = $("#storejp-table thead tr select:eq(4)").val();
                var notes = $("#storejp-table thead tr input:eq(2)").val();

                if (shipper == "") { shipper = "0"; }

                var href = "/ajax/Shipping?page=1" + "&shippingCode=" + shippingCode + "&shipper=" + shipper + "&mawb=" + mawb + "&dateFlight=" + dateFlight + "&from=" + from + "&to="
                        + to + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
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

    var href = $(this).attr("href");
    var id = $(this).attr("data-id");
    $(".wasehouse-info,.package_edit_body,.package_add_body").html("");

    $(".wasehouse-info").load(href);
    $("#storejp-table tbody tr").each(function (i, e) {
        var className = "active";
        if ($(this).hasClass(className)) {
            $(this).removeClass(className)
        }
    })
    $("#storejp-table tr[data-id=" + id + "]").addClass("active");
    e.preventDefault();
});