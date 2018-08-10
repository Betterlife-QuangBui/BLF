$(function () {
    $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
    $("body").find(".export-item-btn").hide();
    //width = $(window).width();
    //if (width >= 1400) {
    //    $("#storejp-table,.package_header.warehouse_header").css("width", "1400px");
    //}
    //else $("#storejp-table").css("width", "auto");
});
$(function () {

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
    var trackingcode = $("#storejp-table thead tr.filter-jp input:eq(0)").val();
    var spilt = $("#storejp-table thead tr.filter-jp input:eq(6)").val();
    var weigh = $("#storejp-table thead tr.filter-jp input:eq(2)").val();
    var size = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
    var date_recive = $("#storejp-table thead tr.filter-jp input:eq(4)").val();
    var hour = $("#storejp-table thead tr.filter-jp input:eq(5)").val();
    var status = $("#storejp-table thead tr.filter-jp select:eq(2)").val();
    var notes = $("#storejp-table thead tr.filter-jp input:eq(1)").val();
    var error = $("#storejp-table thead tr.filter-jp input:eq(7)").val();

    var delivery = $("#storejp-table thead tr select:eq(0)").val();
    var address = $("#storejp-table thead tr.filter-jp select:eq(1)").val();
    

    if (spilt == "") { split = "-1";}
    if (weigh == "") { weigh = "-1"; }
    if (size == "") { size = "-1"; }
    if (error == "") { error = "-1"; }
    if (href === undefined || href === null) { }
    else {
        $("#storejp-table tbody").html("");
        href = href + "&delivery=" + delivery + "&address=" + address + "&trackingcode=" + trackingcode + "&spilt=" + spilt + "&weigh=" + weigh + "&size=" + size + "&date_recive="
            + date_recive + "&hour=" + hour + "&status=" + status + "&notes=" + notes
            + "&error=" + error + "&data_sort=" + data_sort;
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
    
    var data_sort = $("#sort-jp").val();
    var trackingcode = $("#storejp-table thead tr.filter-jp input:eq(0)").val();
    var spilt = $("#storejp-table thead tr.filter-jp input:eq(6)").val();
    var weigh = $("#storejp-table thead tr.filter-jp input:eq(2)").val();
    var size = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
    var date_recive = $("#storejp-table thead tr.filter-jp input:eq(4)").val();
    var hour = $("#storejp-table thead tr.filter-jp input:eq(5)").val();
    var status = $("#storejp-table thead tr.filter-jp select:eq(2)").val();
    var notes = $("#storejp-table thead tr.filter-jp input:eq(1)").val();
    var error = $("#storejp-table thead tr.filter-jp input:eq(7)").val();

    var delivery = $("#storejp-table thead tr select:eq(0)").val();
    var address = $("#storejp-table thead tr.filter-jp select:eq(1)").val();

    if (spilt == "") { split = "-1"; }
    if (weigh == "") { weigh = "-1"; }
    if (size == "") { size = "-1"; }
    if (error == "") { error = "-1"; }


    var href = "/ajax/StoreJPNew?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&weigh=" + weigh + "&size=" + size + "&date_recive="
            + date_recive + "&hour=" + hour + "&status=" + status + "&notes=" + notes
            + "&error=" + error + "&data_sort=" + data_sort + "&delivery=" + delivery + "&address=" + address;


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
$(document).on("keypress", "#storejp-table .filter-jp input:eq(0)", function (e) {
    $(this).autocomplete({
        source: "/Ajax/AutocompleteStoreJP",
        minLength: 1,
        select: function (event, ui) {
            loading();
            var data_sort = $("#sort-jp").val();
            var trackingcode = $("#storejp-table thead tr.filter-jp input:eq(0)").val();
            var spilt = $("#storejp-table thead tr.filter-jp input:eq(6)").val();
            var weigh = $("#storejp-table thead tr.filter-jp input:eq(2)").val();
            var size = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
            var date_recive = $("#storejp-table thead tr.filter-jp input:eq(4)").val();
            var hour = $("#storejp-table thead tr.filter-jp input:eq(5)").val();
            var status = $("#storejp-table thead tr.filter-jp select:eq(2)").val();
            var notes = $("#storejp-table thead tr.filter-jp input:eq(1)").val();
            var error = $("#storejp-table thead tr.filter-jp input:eq(7)").val();

            var delivery = $("#storejp-table thead tr select:eq(0)").val();
            var address = $("#storejp-table thead tr.filter-jp select:eq(1)").val();

            if (spilt == "") { split = "-1"; }
            if (weigh == "") { weigh = "-1"; }
            if (size == "") { size = "-1"; }
            if (error == "") { error = "-1"; }


            var href = "/ajax/StoreJPNew?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&weigh=" + weigh + "&size=" + size + "&date_recive="
                    + date_recive + "&hour=" + hour + "&status=" + status + "&notes=" + notes
                    + "&error=" + error + "&data_sort=" + data_sort + "&delivery=" + delivery + "&address=" + address;
            $("#storejp-table tbody").html("");
            $("#storejp-table tbody").load(href);
        }
    });
    
    if (e.which == 13) {
        $(this).autocomplete('close');
        loading();
        var data_sort = $("#sort-jp").val();
        var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
        var spilt = $("#storejp-table thead tr input:eq(6)").val();
        var weigh = $("#storejp-table thead tr input:eq(2)").val();
        var size = $("#storejp-table thead tr input:eq(3)").val();
        var date_recive = $("#storejp-table thead tr input:eq(4)").val();
        var hour = $("#storejp-table thead tr input:eq(5)").val();
        var status = $("#storejp-table thead tr select:eq(2)").val();
        var notes = $("#storejp-table thead tr input:eq(1)").val();
        var error = $("#storejp-table thead tr input:eq(7)").val();

        var delivery = $("#storejp-table thead tr select:eq(0)").val();
        var address = $("#storejp-table thead tr select:eq(0)").val();

        if (spilt == "") { split = "-1"; }
        if (weigh == "") { weigh = "-1"; }
        if (size == "") { size = "-1"; }
        if (error == "") { error = "-1"; }


        var href = "/ajax/StoreJPNew?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&weigh=" + weigh + "&size=" + size + "&date_recive="
                + date_recive + "&hour=" + hour + "&status=" + status + "&notes=" + notes
                + "&error=" + error + "&data_sort=" + data_sort + "&delivery=" + delivery + "&address" + address;
        $("#storejp-table tbody").html("");
        $("#storejp-table tbody").load(href);
        return false;
        e.preventDefault();
    }
    
});
$(document).on("keyup", "#storejp-table .filter-jp input:eq(1),#storejp-table .filter-jp input:eq(2),#storejp-table .filter-jp input:eq(3),#storejp-table .filter-jp input:eq(5),#storejp-table .filter-jp input:eq(6),#storejp-table .filter-jp input:eq(7)", function (e) {
    if ($(this).val().length > 0) {
        loading();
        var data_sort = $("#sort-jp").val();
        var trackingcode = $("#storejp-table thead tr.filter-jp input:eq(0)").val();
        var spilt = $("#storejp-table thead tr.filter-jp input:eq(6)").val();
        var weigh = $("#storejp-table thead tr.filter-jp input:eq(2)").val();
        var size = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
        var date_recive = $("#storejp-table thead tr.filter-jp input:eq(4)").val();
        var hour = $("#storejp-table thead tr.filter-jp input:eq(5)").val();
        var status = $("#storejp-table thead tr.filter-jp select:eq(2)").val();
        var notes = $("#storejp-table thead tr.filter-jp input:eq(1)").val();
        var error = $("#storejp-table thead tr.filter-jp input:eq(7)").val();

        var delivery = $("#storejp-table thead tr select:eq(0)").val();
        var address = $("#storejp-table thead tr.filter-jp select:eq(1)").val();

        if (spilt == "") { split = "-1"; }
        if (weigh == "") { weigh = "-1"; }
        if (size == "") { size = "-1"; }
        if (error == "") { error = "-1"; }


        var href = "/ajax/StoreJPNew?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&weigh=" + weigh + "&size=" + size + "&date_recive="
                + date_recive + "&hour=" + hour + "&status=" + status + "&notes=" + notes
                + "&error=" + error + "&data_sort=" + data_sort + "&delivery=" + delivery + "&address=" + address;
        $("#storejp-table tbody").html("");
        $("#storejp-table tbody").load(href);
    }
});
$(document).on("keyup", "#storejp-table .filter-jp input", function (e) {
    if ($(this).val().length == 0) {
        var data_sort = $("#sort-jp").val();
        var trackingcode = $("#storejp-table thead tr.filter-jp input:eq(0)").val();
        var spilt = $("#storejp-table thead tr.filter-jp input:eq(6)").val();
        var weigh = $("#storejp-table thead tr.filter-jp input:eq(2)").val();
        var size = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
        var date_recive = $("#storejp-table thead tr.filter-jp input:eq(4)").val();
        var hour = $("#storejp-table thead tr.filter-jp input:eq(5)").val();
        var status = $("#storejp-table thead tr.filter-jp select:eq(2)").val();
        var notes = $("#storejp-table thead tr.filter-jp input:eq(1)").val();
        var error = $("#storejp-table thead tr.filter-jp input:eq(7)").val();

        var delivery = $("#storejp-table thead tr select:eq(0)").val();
        var address = $("#storejp-table thead tr.filter-jp select:eq(1)").val();

        if (spilt == "") { split = "-1"; }
        if (weigh == "") { weigh = "-1"; }
        if (size == "") { size = "-1"; }
        if (error == "") { error = "-1"; }


        var href = "/ajax/StoreJPNew?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&weigh=" + weigh + "&size=" + size + "&date_recive="
                + date_recive + "&hour=" + hour + "&status=" + status + "&notes=" + notes
                + "&error=" + error + "&data_sort=" + data_sort + "&delivery=" + delivery + "&address=" + address;
        $("#storejp-table tbody").html("");
        $("#storejp-table tbody").load(href);
    }
});
$(document).on("change", "#storejp-table .filter-jp select,#storejp-table .filter-jp input:eq(4)", function (e) {
    loading();
    var data_sort = $("#sort-jp").val();
    var trackingcode = $("#storejp-table thead tr.filter-jp input:eq(0)").val();
    var spilt = $("#storejp-table thead tr.filter-jp input:eq(6)").val();
    var weigh = $("#storejp-table thead tr.filter-jp input:eq(2)").val();
    var size = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
    var date_recive = $("#storejp-table thead tr.filter-jp input:eq(4)").val();
    var hour = $("#storejp-table thead tr.filter-jp input:eq(5)").val();
    var status = $("#storejp-table thead tr.filter-jp select:eq(2)").val();
    var notes = $("#storejp-table thead tr.filter-jp input:eq(1)").val();
    var error = $("#storejp-table thead tr.filter-jp input:eq(7)").val();

    var delivery = $("#storejp-table thead tr select:eq(0)").val();
    var address = $("#storejp-table thead tr.filter-jp select:eq(1)").val();
    if (spilt == "") { split = "-1"; }
    if (weigh == "") { weigh = "-1"; }
    if (size == "") { size = "-1"; }
    if (error == "") { error = "-1"; }


    var href = "/ajax/StoreJPNew?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&weigh=" + weigh + "&size=" + size + "&date_recive="
            + date_recive + "&hour=" + hour + "&status=" + status + "&notes=" + notes
            + "&error=" + error + "&data_sort=" + data_sort + "&delivery=" + delivery + "&address=" + address;
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
                    var data_sort = $("#sort-jp").val();
                    var trackingcode = $("#storejp-table thead tr.filter-jp input:eq(0)").val();
                    var spilt = $("#storejp-table thead tr.filter-jp input:eq(6)").val();
                    var weigh = $("#storejp-table thead tr.filter-jp input:eq(2)").val();
                    var size = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
                    var date_recive = $("#storejp-table thead tr.filter-jp input:eq(4)").val();
                    var hour = $("#storejp-table thead tr.filter-jp input:eq(5)").val();
                    var status = $("#storejp-table thead tr.filter-jp select:eq(2)").val();
                    var notes = $("#storejp-table thead tr.filter-jp input:eq(1)").val();
                    var error = $("#storejp-table thead tr.filter-jp input:eq(7)").val();

                    var delivery = $("#storejp-table thead tr select:eq(0)").val();
                    var address = $("#storejp-table thead tr.filter-jp select:eq(1)").val();

                    if (spilt == "") { split = "-1"; }
                    if (weigh == "") { weigh = "-1"; }
                    if (size == "") { size = "-1"; }
                    if (error == "") { error = "-1"; }


                    var href = "/ajax/StoreJPNew?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&weigh=" + weigh + "&size=" + size + "&date_recive="
                            + date_recive + "&hour=" + hour + "&status=" + status + "&notes=" + notes
                            + "&error=" + error + "&data_sort=" + data_sort + "&delivery=" + delivery + "&address=" + address;
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
$(document).on("click", "a.package_item-delete", function (e) {
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
$(document).on("click", ".shipment_item-export", function (e) {
    $(".se-pre-con").show();
    storejp = $("#StoreJPId").val();
    href = $(this).attr("data-href");
    
    if (storejp != "") {
        location.href = "/Export/StorageJPFull?id="+storejp;
        $(".se-pre-con").fadeOut();
    }
    return false;
    e.preventDefault();
});

function deleteStore(storejp) {
    $.ajax({
        method: 'POST', async: false,
        url: "/StorageJP/DeleteMultiple/",data:{id:storejp},
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Xóa dữ liệu thành công', "1");
                var data_sort = $("#sort-jp").val();
                var trackingcode = $("#storejp-table thead tr.filter-jp input:eq(0)").val();
                var spilt = $("#storejp-table thead tr.filter-jp input:eq(6)").val();
                var weigh = $("#storejp-table thead tr.filter-jp input:eq(2)").val();
                var size = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
                var date_recive = $("#storejp-table thead tr.filter-jp input:eq(4)").val();
                var hour = $("#storejp-table thead tr.filter-jp input:eq(5)").val();
                var status = $("#storejp-table thead tr.filter-jp select:eq(2)").val();
                var notes = $("#storejp-table thead tr.filter-jp input:eq(1)").val();
                var error = $("#storejp-table thead tr.filter-jp input:eq(7)").val();

                var delivery = $("#storejp-table thead tr select:eq(0)").val();
                var address = $("#storejp-table thead tr.filter-jp select:eq(1)").val();

                if (spilt == "") { split = "-1"; }
                if (weigh == "") { weigh = "-1"; }
                if (size == "") { size = "-1"; }
                if (error == "") { error = "-1"; }


                var href = "/ajax/StoreJPNew?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&weigh=" + weigh + "&size=" + size + "&date_recive="
                        + date_recive + "&hour=" + hour + "&status=" + status + "&notes=" + notes
                        + "&error=" + error + "&data_sort=" + data_sort + "&delivery=" + delivery + "&address=" + address;
                $("#storejp-table tbody").html("");
                $("#storejp-table tbody").load(href);
            }
            else {
                notify('error', "Xóa dữ liệu thất bại", "2");
            }
        }
    });
}
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



$(document).on("click", ".storejp_add", function (e) {
    $(".package_add_body,.shipment_edit_body").html("");
    $("tr.add-item,tr.filter-jp-action").show();
    $("tr.filter-jp").hide();
    $("#btn-save-add").attr("disabled", "disabled");
    return false;
    e.preventDefault();
});
$(document).on("click", ".cancel-add-item", function (e) {
    $(".package_add_body,.shipment_edit_body").html("");
    $("tr.add-item,tr.filter-jp-action").hide();
    $("tr.filter-jp").show();
    $("#btn-save-add").attr("disabled", "disabled");

    $(".add-item").find("input[type=text]:eq(0),select:eq(0),select:eq(1),select:eq(2)").val("")
    $(".add-item").find("input[type=text]:eq(0),select:eq(0),select:eq(1),select:eq(2)").addClass("display-error-form")

    return false;
    e.preventDefault();
});
$(document).on("change", ".add-item", function (e) {
    var count = 0;
    //check tracking code
    if ($(this).find("#TrackingCode").val().length > 0) {
        //check exist tracking code
        $.ajax({
            method: 'POST', async: false,
            url: '/StorageJP/CheckExistTrackingCode',
            data: { tracking: $(this).find("#TrackingCode").val() },
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
    else {
        notify('white', 'Vui lòng nhập mã tracking', "1");
    }
    if (count == 1) { $(this).find("#TrackingCode").removeClass("display-error-form"); }
    else { $(this).find("#TrackingCode").addClass("display-error-form"); }
    //DeliveryId
    if ($(this).find("#DeliveryId").val().toString().trim().length > 0) {
        count += 1; $(this).find("#DeliveryId").removeClass("display-error-form");
    }
    else {
        notify('white', 'Vui lòng chọn công ty giao nhận', "1");
        $(this).find("#DeliveryId").addClass("display-error-form");
    }
    //DeliveryAddress
    if ($(this).find("#DeliveryAddress").val().toString().trim().length > 0) {
        count += 1; $(this).find("#DeliveryAddress").removeClass("display-error-form");
    }
    else {
        notify('white', 'Vui lòng chọn địa chỉ nhận hàng', "1");
        $(this).find("#DeliveryAddress").addClass("display-error-form");
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

$(document).on("click", ".default-column", function (e) {
    $(".hiden-column-content").hide(); $(".hiden-column-title").show();
    $(".hiden-column input[type=checkbox]").prop('checked', false);
    $(".hiden-column input[type=checkbox]").each(function (i, e) {
        if (i == 0 || i == 1 || i == 2 || i == 3 || i == 4 || i == 5||i == 6 || i == 7 || i == 10 || i == 11 || i == 12 || i == 13) {
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