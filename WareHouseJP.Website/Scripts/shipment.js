$(document).on("click", ".pagging-shipment a", function (e) {
    $(".wasehouse-info,.package-items").html("");
    var href = $(this).attr("href");
    if (href === undefined || href === null) { }
    else {
        $(".shipment_content").html(""); $(".shipment_add_body").html(""); $(".shipment_edit_body").html("");
        $(".shipment_content").load($(this).attr("href"));
    }
    e.preventDefault();
});
$(document).on("click", "div.shipment_item", function (e) {
    location.href = $(this).attr("data-url");
});

$(document).on("change", ".shipment-select", function (e) {
    $(".wasehouse-info,.package-items").html("");
    $(".shipment_content").html(""); $(".shipment_add_body").html(""); $(".shipment_edit_body").html("");
    $(".shipment_content").load("/ajax/shipment?page=1&sort=" + $(this).val());
    return false;
    e.preventDefault();
});

$(document).on("change", ".package-select", function (e) {
    key = $("#txt-package").val();
    ShipmentId = $("#ShipmentId").val(); $(".package_add_body").html(""); $(".package_edit_body").html("");
    $(".package_content").html(""); $(".wasehouse-info").html("");
    $(".package_content").load("/ajax/Package/" + ShipmentId + "?key=" + key + "&page=1&sort=" + $(this).val());
    return false;
    e.preventDefault();
});

//add new shipment
$(document).on("click", ".shipment_add", function (e) {
    $(".shipment_add_body").html(""); $(".shipment_edit_body").html("");
    $(".shipment_add_body").load("/Shipment/Add");
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

//edit package
$(document).on("click", ".package_item-edit", function (e) {
    $(".package_add_body").html(""); $(".package_edit_body").html("");
    $(".wasehouse-info").html("");
    $("tr.add-item,tr.filter-jp-action").hide();
    $("tr.filter-jp").show(); $("#btn-save-add").attr("disabled", "disabled");
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

$(document).on("click", ".pagging-package a", function (e) {
    var href = $(this).attr("href");
    if (href === undefined || href === null) { }
    else {
        $(".package_add_body").html(""); $(".package_edit_body").html("");
        $(".package_content").html(""); $(".wasehouse-info").html("");
        $(".package_content").load($(this).attr("href"));
    }
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
        id=$(this).attr("data-id")
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
$(document).on("click", ".default-column", function (e) {
    $(".hiden-column-content").hide(); $(".hiden-column-title").show();
    $(".hiden-column input[type=checkbox]").prop('checked', false);
    $(".hiden-column input[type=checkbox]").each(function (i, e) {
        if (i == 0 || i == 1 || i == 2 || i == 3 || i == 4 || i == 5) {
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