//add new shipment
$(document).on("click", ".packageReturn_add", function (e) {
    $(".shipment_add_body").html(""); $(".shipment_edit_body").html("");
    $(".shipment_add_body").load("/PackageReturn/Add");
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

$(function () {

    $("th.datepicker input").datepicker({
        inline: true,
        showOn: 'both', buttonImageOnly: true, buttonImage: '/images/ui-icon-calendar.png',
        dateFormat: "yy-mm-dd"
    });
});
$(document).on("change", "#storejp-table .filter-jp select,#storejp-table .filter-jp input:eq(2)", function (e) {
    $(".shipment_add_body").html(""); $(".shipment_edit_body").html("");
    loading()
    var data_sort = $("#sort-packagereturn").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var reciver = $("#storejp-table thead tr input:eq(1)").val();
    var date = $("#storejp-table thead tr input:eq(2)").val();
    var hour = $("#storejp-table thead tr input:eq(3)").val();
    var status = $("#storejp-table thead tr select:eq(0)").val();
    var notes = $("#storejp-table thead tr input:eq(4)").val();
    var href = "/ajax/PackageReturn?page=1" + "&trackingcode=" + trackingcode + "&reciver=" + reciver + "&date=" + date + "&hour="
        + hour + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
    $("#storejp-table tbody").html("");
    $("#storejp-table tbody").load(href);
});

$(document).on("click", "#storejp-table .sort-row .sort", function (e) {
    $(".wasehouse-info,.package_edit_body,.package_add_body").html("");
    loading();
    var data_sort = $(this).parent().attr("data-sort");
    $("#sort-packagereturn").val(data_sort);
    var data_sort_return = data_sort.split('-')[0] + "-" + (data_sort.split('-')[1] == "asc" ? "desc" : "asc");
    $(this).parent().attr("data-sort", data_sort_return);

    var data_sort = $("#sort-packagereturn").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var reciver = $("#storejp-table thead tr input:eq(1)").val();
    var date = $("#storejp-table thead tr input:eq(2)").val();
    var hour = $("#storejp-table thead tr input:eq(3)").val();
    var status = $("#storejp-table thead tr select:eq(0)").val();
    var notes = $("#storejp-table thead tr input:eq(4)").val();

    var href = "/ajax/PackageReturn?page=1" + "&trackingcode=" + trackingcode + "&reciver=" + reciver + "&date=" + date + "&hour="
        + hour + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;

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
    var data_sort = $("#sort-packagereturn").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var reciver = $("#storejp-table thead tr input:eq(1)").val();
    var date = $("#storejp-table thead tr input:eq(2)").val();
    var hour = $("#storejp-table thead tr input:eq(3)").val();
    var status = $("#storejp-table thead tr select:eq(0)").val();
    var notes = $("#storejp-table thead tr input:eq(4)").val();
    var href = "/ajax/PackageReturn?page=1" + "&trackingcode=" + trackingcode + "&reciver=" + reciver + "&date=" + date + "&hour="
        + hour + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
    $("#storejp-table tbody").html("");
    $("#storejp-table tbody").load(href);
});
$(document).on("click", "#storejp-table .storejp-pagging-index a", function (e) {
    $(".shipment_add_body").html(""); $(".shipment_edit_body").html("");
    var href = $(this).attr("href");
    var data_sort = $("#sort-packagereturn").val();
    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
    var reciver = $("#storejp-table thead tr input:eq(1)").val();
    var date = $("#storejp-table thead tr input:eq(2)").val();
    var hour = $("#storejp-table thead tr input:eq(3)").val();
    var status = $("#storejp-table thead tr select:eq(0)").val();
    var notes = $("#storejp-table thead tr input:eq(4)").val();
    if (href === undefined || href === null) { }
    else {
        $("#storejp-table tbody").html("");
        href = href + "&trackingcode=" + trackingcode + "&reciver=" + reciver + "&date=" + date + "&hour="
        + hour + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
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
        $("#PackageReturnId").val(arr.toString());
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("#PackageReturnId").val("");
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
        $("#PackageReturnId").val(arr.toString());
    }
    else {
        $(".action-table").find("input[type=text],button,textarea,select").attr("disabled", "disabled");
        $("#PackageReturnId").val("");
    }
});

//delete one
$(document).on("click", ".conform-acion-index", function (e) {
    storejp = $("#PackageReturnId").val();
    if (storejp == "") {
        $.ajax({
            method: 'POST', async: false,
            url: $(this).attr("data-url"),
            success: function (result) {
                if (result.status == true) {
                    notify('white', 'Xóa dữ liệu thành công', "1");
                    var data_sort = $("#sort-packagereturn").val();
                    var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
                    var reciver = $("#storejp-table thead tr input:eq(1)").val();
                    var date = $("#storejp-table thead tr input:eq(2)").val();
                    var hour = $("#storejp-table thead tr input:eq(3)").val();
                    var status = $("#storejp-table thead tr select:eq(0)").val();
                    var notes = $("#storejp-table thead tr input:eq(4)").val();
                    var href = "/ajax/PackageReturn?page=1" + "&trackingcode=" + trackingcode + "&reciver=" + reciver + "&date=" + date + "&hour="
                        + hour + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
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
$(document).on("click", "a.delete-packagereturn-one", function (e) {
    showdelete("#storejp-table");
    $(".conform-acion-index").attr("data-url", $(this).attr("data-url"))
    return false;
    e.preventDefault();
});

$(document).on("click", ".btn-action-table", function (e) {
    storejp = $("#PackageReturnId").val();
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
        url: "/PackageReturn/DeleteMultiple/", data: { ids: storejp },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Xóa dữ liệu thành công', "1");
                var data_sort = $("#sort-packagereturn").val();
                var trackingcode = $("#storejp-table thead tr input:eq(0)").val();
                var reciver = $("#storejp-table thead tr input:eq(1)").val();
                var date = $("#storejp-table thead tr input:eq(2)").val();
                var hour = $("#storejp-table thead tr input:eq(3)").val();
                var status = $("#storejp-table thead tr select:eq(0)").val();
                var notes = $("#storejp-table thead tr input:eq(4)").val();
                var href = "/ajax/PackageReturn?page=1" + "&trackingcode=" + trackingcode + "&reciver=" + reciver + "&date=" + date + "&hour="
                    + hour + "&status=" + status + "&notes=" + notes + "&data_sort=" + data_sort;
                $("#storejp-table tbody").html("");
                $("#storejp-table tbody").load(href);
            }
            else {
                notify('error', "Xóa dữ liệu thất bại", "2");
            }
        }
    });
}
$(document).on("click", "input[name=address_choose]", function (e) {
    value = $(this).val();
    $(".hiden-address").hide();
    $("#HAWB").attr('disabled', 'disabled');
    $(".hiden-address .text-danger").hide();
    if (value == "address-input") {
        $(".hiden-address input[type='text']").attr("class", "form-control");
        $(".hiden-address").show();
    }
    else {
        $("#HAWB").removeAttr('disabled');
    }
});
$(document).on("change", "tr.hiden-address input[type='text']", function (e) {
    value = $("input[name=address_choose]:checked").val();
    if (value == "address-input" && $("#ReturnCode").val() != "") {
        //check ho ten
        name = $("tr.hiden-address #name").val();
        if (name == "") {
            $("tr.hiden-address #name").attr("class", "form-control input-validation-error");
            $("tr.hiden-address #name").next().show();
        }
        else {
            $("tr.hiden-address #name").attr("class", "form-control");
            $("tr.hiden-address #name").next().hide();
        }
        //check ma buu chinh
        PostalCode = $("tr.hiden-address #PostalCode").val()
        if (PostalCode == "") {
            $("tr.hiden-address #PostalCode").attr("class", "form-control input-validation-error");
            $("tr.hiden-address #PostalCode").next().show();
        }
        else {
            $("tr.hiden-address #PostalCode").attr("class", "form-control");
            $("tr.hiden-address #PostalCode").next().hide();
        }
        //check dia chi
        Address = $("tr.hiden-address #Address").val()
        if (Address == "") {
            $("tr.hiden-address #Address").attr("class", "form-control input-validation-error");
            $("tr.hiden-address #Address").next().show();
        }
        else {
            $("tr.hiden-address #Address").attr("class", "form-control");
            $("tr.hiden-address #Address").next().hide();
        }
        //check so dien thoai
        Phone = $("tr.hiden-address #Phone").val()
        if (Phone == "") {
            $("tr.hiden-address #Phone").attr("class", "form-control input-validation-error");
            $("tr.hiden-address #Phone").next().show();
        }
        else {
            $("tr.hiden-address #Phone").attr("class", "form-control");
            $("tr.hiden-address #Phone").next().hide();
        }
    }

});
$(document).on("click", ".btncreate", function (e) {
    value = $("input[name=address_choose]:checked").val();
    if (value == "address-input" && $("#ReturnCode").val() != "")
    {
        //check ho ten
        i = 0;
        name = $("tr.hiden-address #name").val();
        if (name == "") {
            $("tr.hiden-address #name").attr("class", "form-control input-validation-error");
            $("tr.hiden-address #name").next().show();
        }
        else {
            $("tr.hiden-address #name").attr("class", "form-control");
            $("tr.hiden-address #name").next().hide(); i += 1;
        }
        //check ma buu chinh
        PostalCode = $("tr.hiden-address #PostalCode").val()
        if (PostalCode == "") {
            $("tr.hiden-address #PostalCode").attr("class", "form-control input-validation-error");
            $("tr.hiden-address #PostalCode").next().show();
        }
        else {
            $("tr.hiden-address #PostalCode").attr("class", "form-control");
            $("tr.hiden-address #PostalCode").next().hide(); i += 1;
        }
        //check dia chi
        Address = $("tr.hiden-address #Address").val()
        if (Address == "") {
            $("tr.hiden-address #Address").attr("class", "form-control input-validation-error");
            $("tr.hiden-address #Address").next().show();
        }
        else {
            $("tr.hiden-address #Address").attr("class", "form-control");
            $("tr.hiden-address #Address").next().hide(); i += 1;
        }
        //check so dien thoai
        Phone = $("tr.hiden-address #Phone").val()
        if (Phone == "") {
            $("tr.hiden-address #Phone").attr("class", "form-control input-validation-error");
            $("tr.hiden-address #Phone").next().show();
        }
        else {
            $("tr.hiden-address #Phone").attr("class", "form-control");
            $("tr.hiden-address #Phone").next().hide(); i += 1;
        }
        if (i < 4) {
            return false;
            e.preventDefault();
        }
    }
    
});

$(document).on("click", ".ace-file-input", function (e) {
    $(".picture-option").css("display", "block");
    $("#webcam,#btnCapture").hide();
});
$(document).on("click", ".picture-option .fa-times", function (e) {
    $(".picture-option").css("display", "none");
    $("#webcam,#btnCapture").hide();
});
$(document).on("click", "#upload-computer", function (e) {
    $("#id-input-file-2").click();
    $("#webcam,#btnCapture").hide();
});
$(document).on("click", "#upload-webcam", function (e) {
    $("#webcam,#btnCapture").show();
});
$(document).on("change", "#id-input-file-2", function (e) {
    if (this.files && this.files[0]) {
        var filename = $(this).val().split('\\').pop();
        var FR = new FileReader();
        FR.onload = function (e) {
            $("#img-review img").attr("src", e.target.result);
            $("#ReturnImage").val(filename);
        };
        FR.readAsDataURL(this.files[0]);
    }
});
//active edit form
RequestId = $("#RequestId").val();
if (RequestId != "") {
    href = $(".shipment_item-edit[data-id='" + RequestId + "']").attr("href");
    href = href + "?actionlink=Package";
    $(".shipment_item-edit[data-id='" + RequestId + "']").attr("href", href);
    $(".shipment_item-edit[data-id='" + RequestId + "']").click();
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