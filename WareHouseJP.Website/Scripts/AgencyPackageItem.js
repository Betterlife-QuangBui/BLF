//edit shipment
$(document).on("click", ".shipment_item-edit-child", function (e) {
    $(".shipment_edit_body").html("");
    $(".shipment_edit_body").load($(this).attr("href"));
    return false;
});
$(document).on("click", ".close-package", function (e) {
    $(".shipment_edit_body").html(""); $(".shipment_edit_body").html("");
    $(".wasehouse-info").html("");
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
$(document).on("click", "a#btn-save-add,button#btn-save-add", function (e) {
    $(".se-pre-con").show();
    var idAgencyPackage = $(this).attr("data-id");
    var name = $("#name").val();
    var category = $("#CategoryId").val();
    var quantity = $("#quantity").val();
    var note = $("#note").val();
    var weblink = $("#weblink").val();
    var jancode = $("#jancode").val();
    var productId = $("#productcode").val();
    var price = $("#price").val();
    $.ajax({
        method: 'POST', async: false,
        url: "/AgencyPackageItem/Add",
        data: { id: idAgencyPackage, name: name, category: category, quantity: quantity, note: note, weblink: weblink, jancode: jancode, productId: productId, price: price },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Thêm dữ liệu thành công', "1");
                Cancel("false")
                $("#storejp-table tbody").html("");
                $("#storejp-table tbody").load("/ajax/AgencyPackageItem/" + idAgencyPackage + "?page=1"); $(".se-pre-con").fadeOut();
            }
            else {
                notify('error', "Thêm dữ liệu thất bại", "2"); $(".se-pre-con").fadeOut();
            }
        }
    });
    return false;
    e.preventDefault();
});
$(document).on("click", "a.update-one,button.update-one", function (e) {
    $(".se-pre-con").show();
    var el = $("#tr_" + $(this).attr("data-id"));
    var name = $(el).find("input:eq(1)").val();
    var category = $(el).find("select:eq(0)").val();
    var quantity = $(el).find("input:eq(5)").val();
    var note = $(el).find("input:eq(7)").val();
    var weblink = $(el).find("input:eq(2)").val();
    var jancode = $(el).find("input:eq(3)").val();
    var productId = $(el).find("input:eq(4)").val();
    var price = $(el).find("input:eq(6)").val();
    $.ajax({
        method: 'POST', async: false,
        url: $(this).attr("data-href"),
        data: { name: name, category: category, quantity: quantity, note: note, weblink: weblink, jancode: jancode, productId: productId, price: price },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Cập nhật dữ liệu thành công', "1");
                Cancel("true")
            }
            else {
                notify('error', "Cập nhật dữ liệu thất bại", "2"); $(".se-pre-con").fadeOut();
            }
        }
    });
    return false;
    e.preventDefault();
});
$(".filter-jp,.filter-jp-action").hide();
$(document).on("click", "#storejp-table .storejp-pagging-index a", function (e) {
    var href = $(this).attr("href");
    if (href === undefined || href === null) { }
    else {
        $("#storejp-table tbody").html("");
        $("#storejp-table tbody").load(href);
    }
    e.preventDefault();
});
$(document).on("click", ".btn-add-item", function (e) {
    $("#name").val(""); $("#CategoryId").val("1");
    $("#quantity").val("0"); $("#note").val("");
    $("#weblink").val(""); $("#jancode").val("");
    $("#productcode").val(""); $("#price").val("0");
    $(".filter-jp,.filter-jp-action").show();
    e.preventDefault();
});
$(document).on("click", ".cancel-add-item", function (e) {
    $("#name").val(""); $("#CategoryId").val("1");
    $("#quantity").val("0"); $("#note").val("");
    $("#weblink").val(""); $("#jancode").val("");
    $("#productcode").val(""); $("#price").val("0");
    $(".filter-jp,.filter-jp-action").hide();
    e.preventDefault();
});
$(document).on("click", ".cancel-grid", function (e) {
    Cancel("true")
    e.preventDefault();
});
$(document).on("click", ".cancel-add", function (e) {
    Cancel("false")
    e.preventDefault();
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
        $("#AgencyPackageItemId").val(arr.toString());
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("#AgencyPackageItemId").val("");
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
        $("#AgencyPackageItemId").val(arr.toString());
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("#AgencyPackageItemId").val("");
    }
});

$(document).on("click", ".btn-action-table", function (e) {
    $(".se-pre-con").show();
    storejp = $("#AgencyPackageItemId").val();
    idAgencyPackage = $("#idAgencyPackage").val();
    action = $(".select-action-table").val();
    if (action != "-1" && storejp != "") {
        if (action == "1") {
            deleteStore(storejp, idAgencyPackage); $(".se-pre-con").fadeOut();
        }
    }
});
function deleteStore(storejp, idAgencyPackage) {
    $.ajax({
        method: 'POST', async: false,
        url: "/AgencyPackageItem/DeleteMultiple/", data: { ids: storejp },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Xóa dữ liệu thành công', "1");
                $("#storejp-table tbody").html("");
                $("#storejp-table tbody").load("/ajax/AgencyPackageItem/" + idAgencyPackage + "?page=1");
            }
            else {
                notify('error', "Xóa dữ liệu thất bại", "2");
            }
        }
    });
}
$(document).on("click", "a.remove-one", function (e) {
    if (confirm("Bạn có chắc muốn xóa kiện hàng này?")) {
        $(".se-pre-con").show();
        $.ajax({
            method: 'POST', async: false,
            url: $(this).attr("href"),
            success: function (result) {
                if (result.status == true) {
                    notify('white', 'Xóa dữ liệu thành công', "1");
                    $("#storejp-table tbody").html("");
                    $("#storejp-table tbody").load("/ajax/AgencyPackageItem/" + $("#idAgencyPackage").val() + "?page=1"); $(".se-pre-con").fadeOut();
                }
                else {
                    notify('error', "Xóa dữ liệu thất bại", "2"); $(".se-pre-con").fadeOut();
                }
            }
        });
    }
    return false;
    e.preventDefault();
});
function Cancel(flag) {
    $("#name").val(""); $("#CategoryId").val("1");
    $("#quantity").val("0"); $("#note").val("");
    $("#weblink").val(""); $("#jancode").val("");
    $("#productcode").val(""); $("#price").val("0");
    $(".filter-jp,.filter-jp-action").hide();
    if (flag == "true") {
        $("#storejp-table tbody").html("");
        $("#storejp-table tbody").load("/ajax/AgencyPackageItem/" + $("#idAgencyPackage").val() + "?page=" + $("#pageCurrent").val());
        $(".se-pre-con").fadeOut();
    }
}
//default column
$(document).on("click", ".default-column", function (e) {
    $(".hiden-column-content").hide(); $(".hiden-column-title").show();
    $(".hiden-column input[type=checkbox]").prop('checked', false);
    id = $("input[name=displaytype]:checked").attr("id");
    $(".hiden-column input[type=checkbox]").each(function (i, e) {
        if (id == "col-List") {
            $(".hiden-column input[type=checkbox]").prop('checked', true);

        }
        else {
            if (i == 0 || i == 1 || i == 5 || i == 7) {
                $(this).prop('checked', true);
            }
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
$(document).on("click", "input[name=displaytype]", function (e) {
    id = $(this).attr("id");
    $(".hiden-column input[type=checkbox]").prop('checked', false);
    if (id == "col-List") {
        $(".hiden-column input[type=checkbox]").prop('checked', true);
    }
    else {
        $(".hiden-column input[type=checkbox]").each(function (i, e) {
            if (i == 0 || i == 1 || i == 5 || i == 7) {
                $(this).prop('checked', true);
            }
        });
    }

    $(".hiden-column input[type=checkbox]").each(function (i, e) {
        var index = parseInt($(e).attr("data-index")) + 1;
        if (e.checked) {
            $('#storejp-table tr > *:nth-child(' + index + ')').show();
        }
        else {
            $('#storejp-table tr > *:nth-child(' + index + ')').hide();
        }
    });
});
$(document).on("change", "#storejp-table tbody tr.item td:not(:first-child, :empty)", function (e) {
    $(this).parent().find(".se-pre-con-crud").css("width", ($("#storejp-table").width() + 8) + "px");
    $(this).parent().find(".se-pre-con-crud").css("height", ($(this).height() + 9) + "px");
    $(this).parent().find(".se-pre-con-crud").show();
    return false;
    e.preventDefault();
});