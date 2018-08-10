//add new shipment
$(document).on("click", ".shipment_add", function (e) {
    $(".shipment_add_body").html(""); $(".shipment_edit_body").html("");
    $(".shipment_add_body").load("/ExportGoods/Add");
    return false;
    e.preventDefault();
});
$(document).on("click", ".close-shipment", function (e) {
    $(".shipment_add_body").html(""); $(".shipment_edit_body").html("");
    return false;
    e.preventDefault();
});
//edit shipment
$(document).on("click", ".shipment_item-edit", function (e) {
    $(".shipment_edit_body").html(""); $(".shipment_add_body").html("");
    $(this).parent().parent().parent().parent().find(".shipment_edit_body").load($(this).attr("href"));
    return false;
});
//add new package
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
//active edit form
RequestId = $("#RequestId").val();
if (RequestId != "") {
    href = $(".shipment_item-edit[data-id='" + RequestId + "']").attr("href");
    href = href + "?actionlink=Package";
    $(".shipment_item-edit[data-id='" + RequestId + "']").attr("href", href);
    $(".shipment_item-edit[data-id='" + RequestId + "']").click();
}
$(document).on("click", ".close-package", function (e) {
    $(".shipment_edit_body").html(""); $(".shipment_add_body").html("");
    return false;
    e.preventDefault();
});

$(document).on("click", "a.shipment_item-delete", function (e) {
    if (confirm("Bạn có chắc muốn xóa lô hàng này?")) {
        $.ajax({
            method: 'POST', async: false,
            url: $(this).attr("href"),
            success: function (result) {
                if (result.status == true) {
                    $(".wasehouse-info,.package-items").html("");
                    $(".shipment_content").html("");
                    $(".shipment_content").load("/ajax/shipment?page=1&sort=" + $(".shipment-select").val());
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
$(document).on("click", "input[name=size]", function (e) {
    $(".size input[type=text]").attr('disabled', 'disabled');
    $(".size select").attr('disabled', 'disabled');
    $(this).parent().next().find("select,input[type=text]").removeAttr('disabled');
    
    $("#SizeTableId").select2("val", " ");
});
$(document).on("change", "div.SizeTableId #SizeTableId", function (e) {
    value = $(this).val();
    switch (value) {
        case "2":
            //$("td.Weigh #Weigh").val("1.5");
            $("#WeighInput").val("1.5");
            $(".Weigh-Input").html("1.5");
            break;
        case "3":
            //$("td.Weigh #Weigh").val("3.5");
            $("#WeighInput").val("3.5");
            $(".Weigh-Input").html("3.5");
            break;
        case "4":
            //$("td.Weigh #Weigh").val("6.5");
            $("#WeighInput").val("6.5");
            $(".Weigh-Input").html("6.5");
            break;
        case "5":
            //$("td.Weigh #Weigh").val("11");
            $("#WeighInput").val("11");
            $(".Weigh-Input").html("11");
            break;
        case "6":
            //$("td.Weigh #Weigh").val("17");
            $("#WeighInput").val("17");
            $(".Weigh-Input").html("17");
            break;
        case "7":
            //$("td.Weigh #Weigh").val("25.5");
            $("#WeighInput").val("25.5");
            $(".Weigh-Input").html("25.5");
            break;
    }
});
$(document).on("keyup", "#SizeInput", function (e) {
    var value = $(this).val();
    if (value == "") {
        $("#WeighInput").val("");
        $(".Weigh-Input").html("XXX");
    }
    if (value.indexOf(' ') >= 0) {
        arrvalue = value.split(' ');
        if (arrvalue.length > 2) {
            if (arrvalue[2] != "") {
                var kg = (parseFloat(arrvalue[0]) * parseFloat(arrvalue[1]) * parseFloat(arrvalue[2])) / 6000
                $("#WeighInput").val(Math.round(kg * 100) / 100);
                $(".Weigh-Input").html(Math.round(kg * 100) / 100);
            }
        }
    }
    else if (value.indexOf(',') >= 0) {
        arrvalue = value.split(',');
        if (arrvalue.length > 2) {
            var kg = (parseFloat(arrvalue[0]) * parseFloat(arrvalue[1]) * parseFloat(arrvalue[2])) / 6000
            $(".Weigh-Input").html(Math.round(kg * 100) / 100); $("#WeighInput").val(Math.round(kg * 100) / 100);
        }
    }
});
$(function () {

    $("th.datepicker input").datepicker({
        inline: true,
        showOn: 'both', buttonImageOnly: true, buttonImage: '/images/ui-icon-calendar.png',
        dateFormat: "yy-mm-dd"
    });
});
$(document).on("change", "#storejp-table .filter-jp select,#storejp-table .filter-jp input:eq(3)", function (e) {
    $(".shipment_add_body").html(""); $(".shipment_edit_body").html("");
    loading()
    var data_sort = $("#sort-export").val();
    var shippingMask = $("#storejp-table thead tr input:eq(0)").val();
    var weigh = $("#storejp-table thead tr input:eq(1)").val();
    var size = $("#storejp-table thead tr input:eq(2)").val();
    var exportDate = $("#storejp-table thead tr input:eq(3)").val();
    var exportHour = $("#storejp-table thead tr input:eq(4)").val();
    var status = $("#storejp-table thead tr select:eq(0)").val();
    var notes = $("#storejp-table thead tr input:eq(5)").val();
    if (weigh == "") { weigh = "0"; }
    if (size == "") { size = "0"; }
    var href = "/ajax/ExportGoodIndexNews?page=1" + "&shippingMask=" + shippingMask + "&weigh=" + weigh + "&size=" + size + "&exportDate="
        + exportDate + "&exportHour=" + exportHour + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
    $("#storejp-table tbody").html("");
    $("#storejp-table tbody").load(href);
});

$(document).on("click", "#storejp-table .sort-row .sort", function (e) {
    $(".wasehouse-info,.package_edit_body,.package_add_body").html("");
    loading();
    var data_sort = $(this).parent().attr("data-sort");
    $("#sort-export").val(data_sort);
    var data_sort_return = data_sort.split('-')[0] + "-" + (data_sort.split('-')[1] == "asc" ? "desc" : "asc");
    $(this).parent().attr("data-sort", data_sort_return);

    var data_sort = $("#sort-export").val();
    var shippingMask = $("#storejp-table thead tr input:eq(0)").val();
    var weigh = $("#storejp-table thead tr input:eq(1)").val();
    var size = $("#storejp-table thead tr input:eq(2)").val();
    var exportDate = $("#storejp-table thead tr input:eq(3)").val();
    var exportHour = $("#storejp-table thead tr input:eq(4)").val();
    var status = $("#storejp-table thead tr select:eq(0)").val();
    var notes = $("#storejp-table thead tr input:eq(5)").val();

    if (weigh == "") { weigh ="0";}
    if (size == "") { size = "0"; }

    var href = "/ajax/ExportGoodIndexNews?page=1" + "&shippingMask=" + shippingMask + "&weigh=" + weigh + "&size=" + size + "&exportDate="
        + exportDate + "&exportHour=" + exportHour + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;

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
    $(".shipment_add_body").html(""); $(".shipment_edit_body").html("");
    loading()
    var data_sort = $("#sort-export").val();
    var shippingMask = $("#storejp-table thead tr input:eq(0)").val();
    var weigh = $("#storejp-table thead tr input:eq(1)").val();
    var size = $("#storejp-table thead tr input:eq(2)").val();
    var exportDate = $("#storejp-table thead tr input:eq(3)").val();
    var exportHour = $("#storejp-table thead tr input:eq(4)").val();
    var status = $("#storejp-table thead tr select:eq(0)").val();
    var notes = $("#storejp-table thead tr input:eq(5)").val();
    if (weigh == "") { weigh = "0"; }
    if (size == "") { size = "0"; }
    var href = "/ajax/ExportGoodIndexNews?page=1" + "&shippingMask=" + shippingMask + "&weigh=" + weigh + "&size=" + size + "&exportDate="
        + exportDate + "&exportHour=" + exportHour + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
    $("#storejp-table tbody").html("");
    $("#storejp-table tbody").load(href);
});
$(document).on("click", "#storejp-table .storejp-pagging-index a", function (e) {
    $(".shipment_add_body").html(""); $(".shipment_edit_body").html("");
    var href = $(this).attr("href");
    var data_sort = $("#sort-export").val();
    var shippingMask = $("#storejp-table thead tr input:eq(0)").val();
    var weigh = $("#storejp-table thead tr input:eq(1)").val();
    var size = $("#storejp-table thead tr input:eq(2)").val();
    var exportDate = $("#storejp-table thead tr input:eq(3)").val();
    var exportHour = $("#storejp-table thead tr input:eq(4)").val();
    var status = $("#storejp-table thead tr select:eq(0)").val();
    var notes = $("#storejp-table thead tr input:eq(5)").val();
    if (weigh == "") { weigh = "0"; }
    if (size == "") { size = "0"; }
    if (href === undefined || href === null) { }
    else {
        $("#storejp-table tbody").html("");
        href = href + "&shippingMask=" + shippingMask + "&weigh=" + weigh + "&size=" + size + "&exportDate="
        + exportDate + "&exportHour=" + exportHour + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
        $("#storejp-table tbody").load(href);
    }
    e.preventDefault();
});
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
        $("#ExportId").val(arr.toString());
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("#ExportId").val("");
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
        $("#ExportId").val(arr.toString());
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("#ExportId").val("");
    }
});

//delete one
$(document).on("click", ".conform-acion-index", function (e) {
    storejp = $("#ExportId").val();
    if (storejp == "") {
        $.ajax({
            method: 'POST', async: false,
            url: $(this).attr("data-url"),
            success: function (result) {
                if (result.status == true) {
                    notify('white', 'Xóa dữ liệu thành công', "1");
                    $(".shipment_add_body").html(""); $(".shipment_edit_body").html("");
                    loading()
                    var data_sort = $("#sort-export").val();
                    var shippingMask = $("#storejp-table thead tr input:eq(0)").val();
                    var weigh = $("#storejp-table thead tr input:eq(1)").val();
                    var size = $("#storejp-table thead tr input:eq(2)").val();
                    var exportDate = $("#storejp-table thead tr input:eq(3)").val();
                    var exportHour = $("#storejp-table thead tr input:eq(4)").val();
                    var status = $("#storejp-table thead tr select:eq(0)").val();
                    var notes = $("#storejp-table thead tr input:eq(5)").val();
                    if (weigh == "") { weigh = "0"; }
                    if (size == "") { size = "0"; }
                    var href = "/ajax/ExportGoodIndexNews?page=1" + "&shippingMask=" + shippingMask + "&weigh=" + weigh + "&size=" + size + "&exportDate="
                        + exportDate + "&exportHour=" + exportHour + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
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
$(document).on("click", "a.delete-export-one", function (e) {
    showdelete("#storejp-table");
    $(".conform-acion-index").attr("data-url", $(this).attr("data-url"))
    return false;
    e.preventDefault();
});

$(document).on("click", ".btn-action-table", function (e) {
    storejp = $("#ExportId").val();
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
        url: "/ExportGoods/DeleteMultiple/", data: { ids: storejp },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Xóa dữ liệu thành công', "1");
                $(".shipment_add_body").html(""); $(".shipment_edit_body").html("");
                loading()
                var data_sort = $("#sort-export").val();
                var shippingMask = $("#storejp-table thead tr input:eq(0)").val();
                var weigh = $("#storejp-table thead tr input:eq(1)").val();
                var size = $("#storejp-table thead tr input:eq(2)").val();
                var exportDate = $("#storejp-table thead tr input:eq(3)").val();
                var exportHour = $("#storejp-table thead tr input:eq(4)").val();
                var status = $("#storejp-table thead tr select:eq(0)").val();
                var notes = $("#storejp-table thead tr input:eq(5)").val();
                if (weigh == "") { weigh = "0"; }
                if (size == "") { size = "0"; }
                var href = "/ajax/ExportGoodIndexNews?page=1" + "&shippingMask=" + shippingMask + "&weigh=" + weigh + "&size=" + size + "&exportDate="
                    + exportDate + "&exportHour=" + exportHour + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
                $("#storejp-table tbody").html("");
                $("#storejp-table tbody").load(href);
            }
            else {
                notify('error', "Xóa dữ liệu thất bại", "2");
            }
        }
    });
}

$(document).on("click", ".default-column", function (e) {
    $(".hiden-column-content").hide(); $(".hiden-column-title").show();
    $(".hiden-column input[type=checkbox]").prop('checked', false);
    $(".hiden-column input[type=checkbox]").each(function (i, e) {
        $(this).prop('checked', true);
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