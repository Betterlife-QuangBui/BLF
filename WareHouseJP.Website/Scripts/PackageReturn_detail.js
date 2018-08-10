//pagging jp
$(document).on("click", "#storejp-table .storejp-pagging a", function (e) {
    var href = $(this).attr("href");
    var data_sort = $("#sort-jp").val();
    var tracking = $("#storejp-table thead tr input:eq(0)").val();
    var split = $("#storejp-table thead tr input:eq(1)").val();
    if (href === undefined || href === null) { }
    else {
        $("#storejp-table tbody").html("");
        href = href + "&tracking=" + tracking + "&split=" + split + "&data_sort=" + data_sort;
        $("#storejp-table tbody").load(href);
    }
    e.preventDefault();
});
//sort-jp
$(document).on("click", "#storejp-table .sort-row .sort", function (e) {
    loading();
    var data_sort = $(this).parent().attr("data-sort");
    $("#sort-jp").val(data_sort);
    var data_sort_return = data_sort.split('-')[0] + "-" + (data_sort.split('-')[1] == "asc" ? "desc" : "asc");
    $(this).parent().attr("data-sort", data_sort_return);
    data_sort = $("#sort-jp").val();
    var tracking = $("#storejp-table thead tr input:eq(0)").val();
    var split = $("#storejp-table thead tr input:eq(1)").val();
    var href = "/ajax/PackageReturnJP?page=1&tracking=" + tracking + "&split=" + split + "&data_sort=" + data_sort;
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
    loading();
    var data_sort = $("#sort-jp").val();
    var tracking = $("#storejp-table thead tr input:eq(0)").val();
    var split = $("#storejp-table thead tr input:eq(1)").val();
    var href = "/ajax/PackageReturnJP?page=1&tracking=" + tracking + "&split=" + split + "&data_sort=" + data_sort;
    $("#storejp-table tbody").html("");
    $("#storejp-table tbody").load(href);
});
//add to vn
$(document).on("click", "#storejp-table input[type='checkbox']", function (e) {
    $("#storevn-table input[type='checkbox']").prop('checked', false);
    var arr = [];
    $("#storejp-table input[type='checkbox']").each(function (i, e) {
        if ($(this).prop("checked")) {
            arr.push($(this).attr("id"));
        }
    });
    $(".action-export-detail button").attr("disabled", "disabled");
    if (arr.length > 0) {
        $("#tracking-jp-add").val(arr.toString());
        $(".action-right-to-left").removeAttr("disabled");
    }
    else {
        $(".action-export-detail button").attr("disabled", "disabled");
        $("#tracking-jp-add").val("");
    }
});
$(document).on("click", ".action-right-to-left", function (e) {
    loading();
    var ids = $("#tracking-jp-add").val();
    $("#tracking-jp-add").val("");
    var PackageReturnId = $("#PackageReturnId").val();
    $.ajax({
        method: 'POST', async: false,
        url: "/PackageReturn/AddItems",
        data: { Id: PackageReturnId, ids: ids },
        success: function (result) {
            if (result.status == true) {
                notify('white', result.message, "1");
                location.reload();
                //var data_sort = $("#sort-jp").val();
                //var tracking = $("#storejp-table thead tr input:eq(0)").val();
                //var split = $("#storejp-table thead tr input:eq(1)").val();
                //var href = "/ajax/PackageReturnJP?page=1&tracking=" + tracking + "&split=" + split + "&data_sort=" + data_sort;
                //$("#storejp-table tbody").html("");
                //$("#storejp-table tbody").load(href);
                ////update vn
                //var PackageReturnId = $("#PackageReturnId").val();
                //var data_sort = $("#sort-packagereturn").val();
                //var tracking = $("#storejp-table thead tr input:eq(0)").val();
                //var split = $("#storejp-table thead tr input:eq(1)").val();
                //var href = "/ajax/PackageReturnDetail?id=" + PackageReturnId + "&page=1&tracking=" + tracking + "&split=" + split + "&data_sort=" + data_sort;
                //$("#storevn-table tbody").html("");
                //$("#storevn-table tbody").load(href);
            }
            else {
                notify('error', result.message, "2");
            }
            $(".action-export-detail button").attr("disabled", "disabled");
            $("#storejp-table input[type='checkbox']").prop('checked', false);
            $("#storevn-table input[type='checkbox']").prop('checked', false);
        }
    });
});

//pagging return
$(document).on("click", "#storevn-table .storejp-pagging a", function (e) {

    var href = $(this).attr("href");
    var PackageReturnId = $("#PackageReturnId").val();
    var data_sort = $("#sort-packagereturn").val();
    var tracking = $("#storejp-table thead tr input:eq(0)").val();
    var split = $("#storejp-table thead tr input:eq(1)").val();
    if (href === undefined || href === null) { }
    else {
        loading();
        $("#storevn-table tbody").html("");
        href = href + "&tracking=" + tracking + "&split=" + split + "&data_sort=" + data_sort;
        $("#storevn-table tbody").load(href);
    }
    e.preventDefault();
});
//sort-return
$(document).on("click", "#storevn-table .sort-row .sort", function (e) {
    loading();
    var PackageReturnId = $("#PackageReturnId").val();
    var data_sort = $(this).parent().attr("data-sort");
    $("#sort-packagereturn").val(data_sort);
    var data_sort_return = data_sort.split('-')[0] + "-" + (data_sort.split('-')[1] == "asc" ? "desc" : "asc");
    $(this).parent().attr("data-sort", data_sort_return);
    var tracking = $("#storejp-table thead tr input:eq(0)").val();
    var split = $("#storejp-table thead tr input:eq(1)").val();
    var href = "/ajax/PackageReturnDetail?id=" + PackageReturnId + "&page=1&tracking=" + tracking + "&split=" + split + "&data_sort=" + data_sort;
    //display sort
    $("#storevn-table .sort-row th").each(function (i, item) {
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
    $("#storevn-table tbody").html("");
    $("#storevn-table tbody").load(href);
});
//filter-return
$(document).on("keyup", "#storevn-table .filter-shipping input", function (e) {
    loading();
    var PackageReturnId = $("#PackageReturnId").val();
    var data_sort = $("#sort-packagereturn").val();
    var tracking = $("#storejp-table thead tr input:eq(0)").val();
    var split = $("#storejp-table thead tr input:eq(1)").val();
    var href = "/ajax/PackageReturnDetail?id=" + PackageReturnId + "&page=1&tracking=" + tracking + "&split=" + split + "&data_sort=" + data_sort;
    $("#storevn-table tbody").html("");
    $("#storevn-table tbody").load(href);
});
//remote vn to jp
//check box
$(document).on("click", ".checkall", function (e) {
    var check = $(this).prop("checked");
    $("#storevn-table tr.item input[type='checkbox']").prop('checked', check);
    var arr = [];
    $("#storevn-table tr.item input[type='checkbox']").each(function (i, e) {
        if ($(this).prop("checked")) {
            arr.push($(this).attr("id"));
        }
    });
    $(".action-export-detail button").attr("disabled", "disabled");
    if (arr.length > 0) {
        $(".action-table").find("select.select-action-table").removeAttr("disabled");
        $("#tracking-shipping").val(arr.toString());
        $(".action-left-to-right").removeAttr("disabled");
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("#tracking-shipping").val("");
    }
});
$(document).on("click", ".chk-parent", function (e) {
    var check = $(this).prop("checked");
    $("#storevn-table tr.item input[data-pre=" + $(this).attr("data-id") + "]").prop('checked', check);
    var arr = [];
    $("#storevn-table tr.item input[type='checkbox']").each(function (i, e) {
        if ($(this).prop("checked")) {
            arr.push($(this).attr("id"));
        }
    });
    $(".action-export-detail button").attr("disabled", "disabled");
    if (arr.length > 0) {
        $(".action-table").find("select.select-action-table").removeAttr("disabled");
        $("#tracking-shipping").val(arr.toString());
        $(".action-left-to-right").removeAttr("disabled");
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("#tracking-shipping").val("");
    }
});
$(document).on("click", "#col-List", function (e) {
    $("#storevn-table tr.parent").hide();
});
$(document).on("click", "#col-split", function (e) {
    $("#storevn-table tr.parent").show();
});
$(document).on("change", "select.select-action-table", function (e) {
    if ($(this).val() == 2) {
        var arr = [];
        $("#storevn-table tr.item input[type='checkbox']").each(function (i, e) {
            if ($(this).prop("checked")) {
                arr.push($(this).attr("id"));
            }
        });
        $(".action-export-detail button").attr("disabled", "disabled");
        if (arr.length > 0) {
            $(".action-table").find("select.select-hawb,button").removeAttr("disabled");
            $("#tracking-shipping").val(arr.toString());
            $(".action-left-to-right").removeAttr("disabled");
        }
        else {
            $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
            $("#tracking-shipping").val("");
        }
    }
});
$(document).on("click", "#storevn-table tr.item input[type='checkbox']", function (e) {
    $("#storejp-table input[type='checkbox']").prop('checked', false);
    var arr = [];
    $("#storevn-table tr.item input[type='checkbox']").each(function (i, e) {
        if ($(this).prop("checked")) {
            arr.push($(this).attr("id"));
        }
    });
    $(".action-export-detail button").attr("disabled", "disabled");
    if (arr.length > 0) {
        $("#tracking-shipping").val(arr.toString());
        $(".action-left-to-right").removeAttr("disabled");
        $(".action-table").find("input,button,textarea,select.select-action-table").removeAttr("disabled");
    }
    else {
        $(".action-export-detail button").attr("disabled", "disabled");
        $("#tracking-shipping").val("");
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
    }
});
$(document).on("click", ".action-left-to-right", function (e) {
    loading();
    var ids = $("#tracking-shipping").val();
    $("#tracking-shipping").val("");
    var PackageReturnId = $("#PackageReturnId").val();
    $.ajax({
        method: 'POST', async: false,
        url: "/PackageReturn/RemoveItems",
        data: { id: PackageReturnId, ids: ids },
        success: function (result) {
            if (result.status == true) {
                notify('white', result.message, "1");location.reload();
                //var data_sort = $("#sort-jp").val();
                //var tracking = $("#storejp-table thead tr input:eq(0)").val();
                //var split = $("#storejp-table thead tr input:eq(1)").val();
                //var href = "/ajax/PackageReturnJP?page=1&tracking=" + tracking + "&split=" + split + "&data_sort=" + data_sort;
                //$("#storejp-table tbody").html("");
                //$("#storejp-table tbody").load(href);
                ////update vn
                //var PackageReturnId = $("#PackageReturnId").val();
                //var data_sort = $("#sort-packagereturn").val();
                //var tracking = $("#storejp-table thead tr input:eq(0)").val();
                //var split = $("#storejp-table thead tr input:eq(1)").val();
                //var href = "/ajax/PackageReturnDetail?id=" + PackageReturnId + "&page=1&tracking=" + tracking + "&split=" + split + "&data_sort=" + data_sort;
                //$("#storevn-table tbody").html("");
                //$("#storevn-table tbody").load(href);
            }
            else {
                notify('error', result.message, "2");
            }
            $(".action-export-detail button").attr("disabled", "disabled");
            $("#storejp-table input[type='checkbox']").prop('checked', false);
            $("#storevn-table input[type='checkbox']").prop('checked', false);
        }
    });
});
$(document).on("click", ".btn-action-table", function (e) {
    loading();
    var ids = $("#tracking-shipping").val();
    $("#tracking-shipping").val("");
    var ShippingId = $("#ShippingId").val();
    var Hawb = $(".select-hawb").val();
    $.ajax({
        method: 'POST', async: false,
        url: "/Shipping/ChangeHAWB",
        data: { id: ShippingId, ids: ids, hawb: Hawb },
        success: function (result) {
            if (result.status == true) {
                notify('white', result.message.mess, "1");
                $(".shipping-package-weigh").html(result.message.weigh)
                var data_sort = $("#sort-shipping").val();
                var shippingmask = $("#storevn-table thead tr input:eq(0)").val();
                var hawb = $("#storevn-table thead tr select").val();

                var href = "/ajax/shippinghawb?id=" + ShippingId + "&page=1&shippingmask=" + shippingmask + "&hawb=" + hawb + "&data_sort=" + data_sort;
                $("#storevn-table tbody").html("");
                $("#storevn-table tbody").load(href);
                //update vn
                data_sort = $("#sort-jp").val();
                var shippingmask = $("#storejp-table thead tr input:eq(0)").val();
                var weigh = $("#storejp-table thead tr input:eq(1)").val();
                href = "/ajax/shippingexportgoodsvn?page=1&shippingmask=" + shippingmask + "&weigh=" + weigh + "&data_sort=" + data_sort;
                $("#storejp-table tbody").html("");
                $("#storejp-table tbody").load(href);
            }
            else {
                notify('error', result.message, "2");
            }
            $(".action-export-detail button").attr("disabled", "disabled");
            $("#storejp-table input[type='checkbox']").prop('checked', false);
            $("#storevn-table input[type='checkbox']").prop('checked', false);
        }
    });
});
$(document).on("click", ".btn-action-cancel", function (e) {
    $(".action-export-detail button").attr("disabled", "disabled");
    $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
    $(".action-table").find("select.select-hawb").val("");
    $(".action-table").find("select.select-action-table").val("-1");
    $("#storevn-table input[type='checkbox']").prop('checked', false);
    $("#storejp-table input[type='checkbox']").prop('checked', false);
});
function updateExportTracking(id, number, option) {
    var count = $("span[data-id=" + id + "]").attr("data-count");
    if (option == "add") {
        $("span[data-id=" + id + "]").attr("data-count", parseInt(count) + number);
        $("span[data-id=" + id + "]").html(parseInt(count) + number)
    }
    else {
        $("span[data-id=" + id + "]").attr("data-count", parseInt(count) - number);
        $("span[data-id=" + id + "]").html(parseInt(count) - number)
    }
}
//update status
$(document).on("click", ".btn-update-status", function (e) {
    var ModelId = $("#ModelId").val();
    status = $(".col-xs-5 #StatusId").val();
    $.ajax({
        method: 'POST', async: false,
        url: "/PackageReturn/UpdateStatus",
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