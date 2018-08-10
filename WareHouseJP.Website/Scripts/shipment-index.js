$(function () {
    $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
    $("th.datepicker input").datepicker({
        inline: true,
        showOn: 'both', buttonImageOnly: true, buttonImage: '/images/ui-icon-calendar.png',
        dateFormat: "yy-mm-dd"
    });
});
//pagging jp
$(document).on("click", "#storejp-table .storejp-pagging-index a", function (e) {
    var href = $(this).attr("href");
    var data_sort = $("#sort-jp").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var spilt = $("#storejp-table thead tr input:eq(4)").val();
    var weigh = $("#storejp-table thead tr input:eq(3)").val();
    var date_recive = $("#storejp-table thead tr input:eq(1)").val();
    var status = $("#storejp-table thead tr select").val();
    var notes = $("#storejp-table thead tr input:eq(2)").val();

    if (spilt == "") { split = "-1"; }
    if (weigh == "") { weigh = "-1"; }
    if (href === undefined || href === null) { }
    else {
        $("#storejp-table tbody").html("");
        href = href + "&trackingcode=" + trackingcode + "&spilt=" + spilt + "&weigh=" + weigh + "&date_recive="
            + date_recive + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
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

    loading();
    var data_sort = $("#sort-jp").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var spilt = $("#storejp-table thead tr input:eq(4)").val();
    var weigh = $("#storejp-table thead tr input:eq(3)").val();
    var date_recive = $("#storejp-table thead tr input:eq(1)").val();
    var status = $("#storejp-table thead tr select").val();
    var notes = $("#storejp-table thead tr input:eq(2)").val();

    if (spilt == "") { split = "-1"; }
    if (weigh == "") { weigh = "-1"; }
    var href = "/ajax/Shipment?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&weigh=" + weigh + "&date_recive="
            + date_recive + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;

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
    var data_sort = $("#sort-jp").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var spilt = $("#storejp-table thead tr input:eq(4)").val();
    var weigh = $("#storejp-table thead tr input:eq(3)").val();
    var date_recive = $("#storejp-table thead tr input:eq(1)").val();
    var status = $("#storejp-table thead tr select").val();
    var notes = $("#storejp-table thead tr input:eq(2)").val();

    if (spilt == "") { split = "-1"; }
    if (weigh == "") { weigh = "-1"; }
    var href = "/ajax/Shipment?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&weigh=" + weigh + "&date_recive="
            + date_recive + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
    $("#storejp-table tbody").html("");
    $("#storejp-table tbody").load(href);
});
$(document).on("change", "#storejp-table .filter-jp select,#storejp-table .filter-jp input:eq(1)", function (e) {
    loading();
    var data_sort = $("#sort-jp").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var spilt = $("#storejp-table thead tr input:eq(4)").val();
    var weigh = $("#storejp-table thead tr input:eq(3)").val();
    var date_recive = $("#storejp-table thead tr input:eq(1)").val();
    var status = $("#storejp-table thead tr select").val();
    var notes = $("#storejp-table thead tr input:eq(2)").val();

    if (spilt == "") { split = "-1"; }
    if (weigh == "") { weigh = "-1"; }
    var href = "/ajax/Shipment?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&weigh=" + weigh + "&date_recive="
            + date_recive + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
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
        $("body").find(".export-item-btn").show();
        $(".action-table").find("input,button,textarea,select").removeAttr("disabled");
        $("#StoreJPId").val(arr.toString());
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("body").find(".export-item-btn").hide();
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
        $("body").find(".export-item-btn").show();
        $(".action-table").find("input,button,textarea,select").removeAttr("disabled");
        $("#StoreJPId").val(arr.toString());
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("body").find(".export-item-btn").hide();
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
                    loading();
                    var data_sort = $("#sort-jp").val();
                    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
                    var spilt = $("#storejp-table thead tr input:eq(4)").val();
                    var weigh = $("#storejp-table thead tr input:eq(3)").val();
                    var date_recive = $("#storejp-table thead tr input:eq(1)").val();
                    var status = $("#storejp-table thead tr select").val();
                    var notes = $("#storejp-table thead tr input:eq(2)").val();

                    if (spilt == "") { split = "-1"; }
                    var href = "/ajax/Shipment?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&date_recive="
                            + date_recive + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
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
            showdelete("#storejp-table");
        }
    }
});

function deleteStore(storejp) {
    $.ajax({
        method: 'POST', async: false,
        url: "/Shipment/DeleteMultiple/", data: { id: storejp },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Xóa dữ liệu thành công', "1");
                loading();
                var data_sort = $("#sort-jp").val();
                var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
                var spilt = $("#storejp-table thead tr input:eq(4)").val();
                var weigh = $("#storejp-table thead tr input:eq(3)").val();
                var date_recive = $("#storejp-table thead tr input:eq(1)").val();
                var status = $("#storejp-table thead tr select").val();
                var notes = $("#storejp-table thead tr input:eq(2)").val();

                if (spilt == "") { split = "-1"; }
                var href = "/ajax/Shipment?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&date_recive="
                        + date_recive + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
                $("#storejp-table tbody").html("");
                $("#storejp-table tbody").load(href);
            }
            else {
                notify('error', "Xóa dữ liệu thất bại", "2");
            }
        }
    });
}