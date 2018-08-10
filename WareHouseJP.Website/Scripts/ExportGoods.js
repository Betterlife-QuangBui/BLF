$(document).on("change", "#ExportGoodStatus", function (e) {
    key = $("#txt-export-good").val();
    $(".package_add_body,.package_edit_body,.wasehouse-info,.package_content").html("");
    $(".package_content").load("/ajax/ExportGood?key=" + key + "&page=1&sort=" + $(this).val());
    return false;
    e.preventDefault();
});
$(document).on("change", "#barcode", function (e) {
    barcode = $(this).val();
    if (barcode.indexOf('-') >= 0) {
        $(".barcode-button").show();
    }
    else {
        $(".barcode-button").hide();
    }
    return false;
    e.preventDefault();
});
$(document).on("click", ".barcode-button", function (e) {
    barcode = $("#barcode").val(); var ExportId = $("#ExportId").val();
    if (barcode.indexOf('-') >= 0) {
        $.ajax({
            method: 'POST', async: false,
            url: "/ExportGoods/AddBarCode",
            data: { Id: ExportId, barcode: barcode },
            success: function (result) {
                if (result.status == true) {
                    notify('white', 'Cập nhật liệu thành công', "1");
                    $(".barcode-button").hide();
                    //loading data jp
                    $("#storejp-table thead tr input:eq(0)").val("")
                    $("#storejp-table thead tr input:eq(1)").val("")
                    $("#storejp-table thead tr select").val("0")
                    $("#sort-jp").val("tracking-asc");
                    var data_sort = $("#sort-jp").val();
                    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
                    var spilt = $("#storejp-table thead tr input:eq(1)").val();
                    var status = $("#storejp-table thead tr select").val();
                    var href = "/ajax/exportgoodsjp?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&status=" + status + "&data_sort=" + data_sort;
                    $("#storejp-table tbody").html("");
                    $("#storejp-table tbody").load(href);
                    //loading data vn
                    $("#sort-vn").val("");
                    $("#storevn-table thead tr input:eq(0)").val("")
                    $("#storevn-table thead tr input:eq(1)").val("")
                    var data_sort_vn = $("#sort-vn").val();
                    var trackingcode_vn = $("#storevn-table thead tr input:eq(0)").val();
                    var spilt_vn = $("#storevn-table thead tr input:eq(1)").val();

                    var href = "/ajax/ExportGoodsVN?id=" + ExportId + "&page=1&trackingcode=" + trackingcode_vn + "&spilt=" + spilt_vn + "&data_sort=" + data_sort_vn;
                    $("#storevn-table tbody").html("");
                    $("#storevn-table tbody").load(href);
                }
                else {
                    notify('error', result.message, "2");
                }
                $(".action-export-detail button").attr("disabled", "disabled");
                $("#storejp-table input[type='checkbox']").prop('checked', false);
                $("#storevn-table input[type='checkbox']").prop('checked', false);
            }
        });
    } else {
        $(".barcode-button").hide();
    }
    return false;
    e.preventDefault();
});
//add new package
$(document).on("click", ".package_add", function (e) {
    loading();
    $(".package_add_body,.package_edit_body,.wasehouse-info").html("");
    $(".package_add_body").load($(this).attr("href"));
    return false;
    e.preventDefault();
});
//edit package
$(document).on("click", ".package_item-edit", function (e) {
    loading();
    $(".package_add_body,.package_edit_body,.wasehouse-info").html("");
    $(this).parent().parent().parent().parent().find(".package_edit_body").load($(this).attr("href"));
    return false;
});
$(document).on("click", ".close-package", function (e) {
    $(".package_add_body,.package_edit_body,.wasehouse-info").html("");
    return false;
    e.preventDefault();
});

$(document).on("keyup", "#txt-export-good", function (e) {
    key = $(this).val();
    $(".package_add_body,.package_edit_body,.wasehouse-info,.package_content").html("");
    $(".package_content").load("/ajax/ExportGood?key=" + key + "&page=1&sort=" + $('#ExportGoodStatus').val());
    return false;
    e.preventDefault();
});

$(document).on("click", ".pagging-package a", function (e) {
    var href = $(this).attr("href");
    if (href === undefined || href === null) { }
    else {
        $(".package_add_body,.package_edit_body,.wasehouse-info,.package_content").html("");
        $(".package_content").load($(this).attr("href"));
    }
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
                    $("#txt-export-good").val("");
                    var href = "/ajax/ExportGood?page=1&sort=" + $("#ExportGoodStatus").val();
                    $(".package_add_body,.package_edit_body,.wasehouse-info,.package_content").html("");
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
$(document).on("change", "#fileImport", function (e) {
    if ($(this).val().split('\\').pop() != "") {
        $(".btn-action-import").css("display", "inline-block");
    }
});
$(document).on("click", ".btn-import,#inputfileImport", function (e) {
    $("#fileImport").click();
    return false;
    e.preventDefault();
});
$(document).on("click", ".btn-backup", function (e) {
    $.ajax({
        async: false,
        url: $(this).attr("href"),
        success: function (result) {
        }
    }).done(function () {
        $(".btn-import").css("display", "inline-block");
    });
});


$(document).on("click", "input[name=size]", function (e) {
    $(".size input[type=text]").attr('disabled', 'disabled');
    $(".size select").attr('disabled', 'disabled');
    $(this).parent().next().find("select,input[type=text]").removeAttr('disabled');
});
$(document).on("click", ".ace-file-input", function (e) {
    $(".picture-option").css("display", "block");
});
$(document).on("click", ".picture-option .fa-times", function (e) {
    $(".picture-option").css("display", "none");
});
$(document).on("click", "#upload-computer", function (e) {
    $("#id-input-file-2").click();
});

$(document).on("change", "#id-input-file-2", function (e) {
    if (this.files && this.files[0]) {
        var FR = new FileReader();
        FR.onload = function (e) {
            $("#img-review img").attr("src", e.target.result);
            $("#ImageBase64").val(e.target.result)
        };
        FR.readAsDataURL(this.files[0]);
    }
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

//detail
$(document).on("click", "div.package_item", function (e) {
    loading();
    var href = $(this).attr("data-url");
    $(".wasehouse-info,.package_edit_body,.package_add_body").html("");

    $(".wasehouse-info").load(href);
    $(".package_item").each(function (i, e) {
        var className = "active";
        if ($(this).hasClass(className)) {
            $(this).removeClass(className)
        }
    })
    $(this).addClass("active");
    e.preventDefault();
});

//pagging jp
$(document).on("click", "#storejp-table .storejp-pagging a", function (e) {
    var href = $(this).attr("href");
    var data_sort = $("#sort-jp").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var spilt = $("#storejp-table thead tr input:eq(1)").val();
    var status = $("#storejp-table thead tr select").val();
    if (href === undefined || href === null) { }
    else {
        $("#storejp-table tbody").html("");
        href = href + "&trackingcode=" + trackingcode + "&spilt=" + spilt + "&status=" + status + "&data_sort=" + data_sort;
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
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var spilt = $("#storejp-table thead tr input:eq(1)").val();
    var status = $("#storejp-table thead tr select").val();

    var href = "/ajax/exportgoodsjp?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&status=" + status + "&data_sort=" + data_sort;
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
        source: "/ExportGoods/Autocomplete",
        minLength: 1,
        select: function (event, ui) {
            var data_sort = $("#sort-jp").val();
            var trackingcode = ui.item.value;
            var spilt = $("#storejp-table thead tr input:eq(1)").val();
            var status = $("#storejp-table thead tr select").val();
            var href = "/ajax/exportgoodsjp?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&status=" + status + "&data_sort=" + data_sort;
            $("#storejp-table tbody").html("");
            $("#storejp-table tbody").load(href);
        }
    });
    if (e.which == 13) {
        $(this).autocomplete('close');
        var data_sort = $("#sort-jp").val();
        var trackingcode = $(this).val();
        var spilt = $("#storejp-table thead tr input:eq(1)").val();
        var status = $("#storejp-table thead tr select").val();
        var href = "/ajax/exportgoodsjp?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&status=" + status + "&data_sort=" + data_sort;
        $("#storejp-table tbody").html("");
        $("#storejp-table tbody").load(href);
        return false;
        e.preventDefault();
    }
});
$(document).on("keyup", "#storejp-table .filter-jp input:eq(1)", function (e) {
    if ($(this).val().length > 0) {
        loading();
        var data_sort = $("#sort-jp").val();
        var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
        var spilt = $("#storejp-table thead tr input:eq(1)").val();
        var status = $("#storejp-table thead tr select").val();
        var href = "/ajax/exportgoodsjp?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&status=" + status + "&data_sort=" + data_sort;
        $("#storejp-table tbody").html("");
        $("#storejp-table tbody").load(href);
    }
});
$(document).on("keyup", "#storejp-table .filter-jp input", function (e) {
    if ($(this).val().length == 0) {
        loading();
        var data_sort = $("#sort-jp").val();
        var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
        var spilt = $("#storejp-table thead tr input:eq(1)").val();
        var status = $("#storejp-table thead tr select").val();
        var href = "/ajax/exportgoodsjp?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&status=" + status + "&data_sort=" + data_sort;
        $("#storejp-table tbody").html("");
        $("#storejp-table tbody").load(href);
    }
});
$(document).on("change", "#storejp-table .filter-jp select", function (e) {
    loading();
    var data_sort = $("#sort-jp").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var spilt = $("#storejp-table thead tr input:eq(1)").val();
    var status = $("#storejp-table thead tr select").val();

    var href = "/ajax/exportgoodsjp?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&status=" + status + "&data_sort=" + data_sort;
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
    var ExportId = $("#ExportId").val();
    $.ajax({
        method: 'POST', async: false,
        url: "/ExportGoods/AddItems",
        data: { Id: ExportId, ids: ids },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Cập nhật liệu thành công', "1"); location.reload();
                //updateExportTracking(ExportId, ids.split(',').length, "add")
                ////loading data jp
                //$("#storejp-table thead tr input:eq(0)").val("")
                //$("#storejp-table thead tr input:eq(1)").val("")
                //$("#storejp-table thead tr select").val("0")
                //$("#sort-jp").val("tracking-asc");
                //var data_sort = $("#sort-jp").val();
                //var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
                //var spilt = $("#storejp-table thead tr input:eq(1)").val();
                //var status = $("#storejp-table thead tr select").val();
                //var href = "/ajax/exportgoodsjp?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&status=" + status + "&data_sort=" + data_sort;
                //$("#storejp-table tbody").html("");
                //$("#storejp-table tbody").load(href);
                ////loading data vn
                //$("#sort-vn").val("");
                //$("#storevn-table thead tr input:eq(0)").val("")
                //$("#storevn-table thead tr input:eq(1)").val("")
                //var data_sort_vn = $("#sort-vn").val();
                //var trackingcode_vn = $("#storevn-table thead tr input:eq(0)").val();
                //var spilt_vn = $("#storevn-table thead tr input:eq(1)").val();

                //var href = "/ajax/ExportGoodsVN?id=" + ExportId + "&page=1&trackingcode=" + trackingcode_vn + "&spilt=" + spilt_vn + "&data_sort=" + data_sort_vn;
                //$("#storevn-table tbody").html("");
                //$("#storevn-table tbody").load(href);
            }
            else {
                notify('error', "Cập nhật liệu thất bại", "2");
            }
            $(".action-export-detail button").attr("disabled", "disabled");
            $("#storejp-table input[type='checkbox']").prop('checked', false);
            $("#storevn-table input[type='checkbox']").prop('checked', false);
        }
    });
});

//pagging vn
$(document).on("click", "#storevn-table .storejp-pagging a", function (e) {
    var href = $(this).attr("href");
    var ExportId = $("#ExportId").val();
    var data_sort = $("#sort-vn").val();
    var trackingcode = $("#storevn-table thead tr input:eq(0)").val();
    var spilt = $("#storevn-table thead tr input:eq(1)").val();
    if (href === undefined || href === null) { }
    else {
        $("#storevn-table tbody").html("");
        href = href + "&trackingcode=" + trackingcode + "&spilt=" + spilt + "&data_sort=" + data_sort + "&id=" + ExportId;
        $("#storevn-table tbody").load(href);
    }
    e.preventDefault();
});
//sort-vn
$(document).on("click", "#storevn-table .sort-row .sort", function (e) {
    loading();
    var ExportId = $("#ExportId").val();
    var data_sort = $(this).parent().attr("data-sort");
    $("#sort-vn").val(data_sort);
    var data_sort_return = data_sort.split('-')[0] + "-" + (data_sort.split('-')[1] == "asc" ? "desc" : "asc");
    $(this).parent().attr("data-sort", data_sort_return);
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var spilt = $("#storejp-table thead tr input:eq(1)").val();
    var status = $("#storejp-table thead tr select").val();

    var href = "/ajax/ExportGoodsVN?id=" + ExportId + "&page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&data_sort=" + data_sort;
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
//filter-vn
$(document).on("keypress", "#storevn-table .filter-vn input:eq(0)", function (e) {
    if ($(this).val().length == 0) { $(".barcode-button").hide(); }
    $(this).autocomplete({
        source: "/ExportGoods/AutocompleteVN",
        minLength: 1,
        select: function (event, ui) {
            var ExportId = $("#ExportId").val();
            var data_sort = $("#sort-vn").val();
            var trackingcode = ui.item.value;
            var spilt = $("#storevn-table thead tr input:eq(1)").val();

            var href = "/ajax/ExportGoodsVN?id=" + ExportId + "&page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&data_sort=" + data_sort;
            $("#storevn-table tbody").html("");
            $("#storevn-table tbody").load(href);
        }
    });
    if (e.which == 13) {
        $(this).autocomplete('close');
        var ExportId = $("#ExportId").val();
        var data_sort = $("#sort-vn").val();
        var trackingcode = $(this).val();
        var spilt = $("#storevn-table thead tr input:eq(1)").val();

        var href = "/ajax/ExportGoodsVN?id=" + ExportId + "&page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&data_sort=" + data_sort;
        $("#storevn-table tbody").html("");
        $("#storevn-table tbody").load(href);
        return false;
        e.preventDefault();
    }
});
$(document).on("keyup", "#storevn-table .filter-vn input:eq(1)", function (e) {
    if ($(this).val().length > 0) {
        var ExportId = $("#ExportId").val();
        var data_sort = $("#sort-vn").val();
        var trackingcode = $("#storevn-table thead tr input:eq(0)").val();
        var spilt = $("#storevn-table thead tr input:eq(1)").val();

        var href = "/ajax/ExportGoodsVN?id=" + ExportId + "&page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&data_sort=" + data_sort;
        $("#storevn-table tbody").html("");
        $("#storevn-table tbody").load(href);
    }
});
$(document).on("keyup", "#storevn-table .filter-vn input", function (e) {
    if ($(this).val().length == 0) {
        $(".barcode-button").hide();
        var ExportId = $("#ExportId").val();
        var data_sort = $("#sort-vn").val();
        var trackingcode = $("#storevn-table thead tr input:eq(0)").val();
        var spilt = $("#storevn-table thead tr input:eq(1)").val();

        var href = "/ajax/ExportGoodsVN?id=" + ExportId + "&page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&data_sort=" + data_sort;
        $("#storevn-table tbody").html("");
        $("#storevn-table tbody").load(href);
    }
});
//remote vn to jp
$(document).on("click", "#storevn-table input[type='checkbox']", function (e) {
    $("#storejp-table input[type='checkbox']").prop('checked', false);
    var arr = [];
    $("#storevn-table input[type='checkbox']").each(function (i, e) {
        if ($(this).prop("checked")) {
            arr.push($(this).attr("id"));
        }
    });
    $(".action-export-detail button").attr("disabled", "disabled");
    if (arr.length > 0) {
        $("#tracking-remove-vn").val(arr.toString());
        $(".action-left-to-right").removeAttr("disabled");
    }
    else {
        $(".action-export-detail button").attr("disabled", "disabled");
        $("#tracking-remove-vn").val("");
    }
});
$(document).on("click", ".action-left-to-right", function (e) {
    loading();
    var ids = $("#tracking-remove-vn").val();
    $("#tracking-remove-vn").val("");
    var ExportId = $("#ExportId").val();
    $.ajax({
        method: 'POST', async: false,
        url: "/ExportGoods/RemveItems",
        data: { ids: ids },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Cập nhật liệu thành công', "1");
                location.reload();
                //updateExportTracking(ExportId, ids.split(',').length, "remove")
                ////loading data jp
                //$("#storejp-table thead tr input:eq(0)").val("")
                //$("#storejp-table thead tr input:eq(1)").val("")
                //$("#storejp-table thead tr select").val("0")
                //$("#sort-jp").val("tracking-asc");
                //var data_sort = $("#sort-jp").val();
                //var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
                //var spilt = $("#storejp-table thead tr input:eq(1)").val();
                //var status = $("#storejp-table thead tr select").val();
                //var href = "/ajax/exportgoodsjp?page=1&trackingcode=" + trackingcode + "&spilt=" + spilt + "&status=" + status + "&data_sort=" + data_sort;
                //$("#storejp-table tbody").html("");
                //$("#storejp-table tbody").load(href);
                ////loading data vn
                //$("#sort-vn").val("");
                //$("#storevn-table thead tr input:eq(0)").val("")
                //$("#storevn-table thead tr input:eq(1)").val("")
                //var data_sort_vn = $("#sort-vn").val();
                //var trackingcode_vn = $("#storevn-table thead tr input:eq(0)").val();
                //var spilt_vn = $("#storevn-table thead tr input:eq(1)").val();

                //var href = "/ajax/ExportGoodsVN?id=" + ExportId + "&page=1&trackingcode=" + trackingcode_vn + "&spilt=" + spilt_vn + "&data_sort=" + data_sort_vn;
                //$("#storevn-table tbody").html("");
                //$("#storevn-table tbody").load(href);
            }
            else {
                notify('error', "Cập nhật liệu thất bại", "2");
            }
            $(".action-export-detail button").attr("disabled", "disabled");
            $("#storejp-table input[type='checkbox']").prop('checked', false);
            $("#storevn-table input[type='checkbox']").prop('checked', false);
        }
    });
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
    loading();
    var ModelId = $("#ModelId").val();
    status = $(".col-xs-5 #StatusId").val();
    $.ajax({
        method: 'POST', async: false,
        url: "/ExportGoods/UpdateStatus",
        data: { id: ModelId, status: status },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Cập nhật liệu thành công', "1");
                location.reload();
                $("input#StatusId").val(status);
                if ($("#StatusId").val() == "10") {
                    $(".ace,#StatusId").attr("disabled", "disabled");
                    $(".btn-update-status").hide();
                }
                $(".action-export-detail button").attr("disabled", "disabled");
                $("#storejp-table input[type='checkbox']").prop('checked', false);
                $("#storevn-table input[type='checkbox']").prop('checked', false);
                if ($("input#StatusId").val() == "10") {
                    $("#storejp-table input[type='checkbox']").attr("disabled", "disabled");
                    $("#storevn-table input[type='checkbox']").attr("disabled", "disabled");
                }
                else {
                    $("#storejp-table input[type='checkbox']").removeAttr("disabled");
                    $("#storevn-table input[type='checkbox']").removeAttr("disabled");
                }
                //update status html
                if (status == "10") {
                    $(".action-export-detail button").hide();
                    $("div#item-" + ModelId).attr("class", "package_item border-done");
                }
                else {
                    $(".action-export-detail button").show();
                    $("div#item-" + ModelId).attr("class", "package_item border-undone");
                }
                //location.reload();
            }
            else {
                notify('error', "Cập nhật liệu thất bại", "2");
                //location.reload();
            }
        }
    });
});
function updateStatus(status) {
    $(".action-export-detail button").attr("disabled", "disabled");
    $("#storejp-table input[type='checkbox']").prop('checked', false);
    $("#storevn-table input[type='checkbox']").prop('checked', false);

    if (status == "10") {

        $("#storejp-table input[type='checkbox']").attr("disabled", "disabled");
        $("#storevn-table input[type='checkbox']").attr("disabled", "disabled");
    }
    else {
        $("#storejp-table input[type='checkbox']").removeAttr("disabled");
        $("#storevn-table input[type='checkbox']").removeAttr("disabled");
    }
}
//disable checkbox
//autocomplete jp
