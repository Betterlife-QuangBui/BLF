$(document).on("click", "#storejp-table input[type='checkbox']", function (e) {
    var arr = [];
    var parent = $(this).attr("data-parent");

    var id = $(this).attr("id");

    var dataId = $(this).attr("data-id");
    var trackingcode = $(this).attr("data-text");
    var check = $(this).prop("checked");
    if (parent == "true") {
        $("input[data-id=" + id + "]").prop('checked', check);
    }

    $("#storejp-table input[type='checkbox']").each(function (i, e) {
        if ($(this).attr("data-parent") == "false" && $(this).prop("checked")) {
            arr.push($(this).attr("id"));
            $(".hiden #MadeIn").val($(this).attr("data-madein"))
            $(".hiden #CategoryId").val($(this).attr("data-cate"))
            $(".hiden input[name=item-quantity],.hiden input[name=quantity]").val($(this).attr("data-quantity"))
            $(".hiden input[name=Price]").val($(this).attr("data-price"))
            $(".hiden textarea[name=Component]").val($(this).attr("data-component"))
            $(".hiden textarea[name=Material]").val($(this).attr("data-material"))
            $(".hiden input[name=jancode]").val($(this).attr("data-jancode"))
            $(".hiden textarea[name=NameJP]").val($(this).attr("data-namejp"))
        }
    });
    $("tr.hiden").css("display", "none");
    if (arr.length > 0) {
        $(".table_insert").find("input,button,textarea,select").removeAttr("disabled");
    }
    else {
        $(".table_insert").find("input,button,textarea,select").attr("disabled", "disabled");
    }
    $("#storejp-detail-action option[value=0]").attr('selected', 'selected')
    $("#storejp-detail-action option").attr('disabled', 'disabled')
    if (arr.length > 0) {
        if (arr.length > 1) {
            $("#storejp-detail-action option[value=0]").removeAttr("disabled");
            $("#storejp-detail-action option[value=1]").removeAttr("disabled");
            $("#storejp-detail-action option[value=2]").removeAttr("disabled");
            $("#storejp-detail-action option[value=3]").removeAttr("disabled");
            $("#storejp-detail-action option[value='3.1']").removeAttr("disabled");
            $("#storejp-detail-action option[value='4']").removeAttr("disabled");
        }
        else {
            $("#storejp-detail-action option").removeAttr("disabled");
            $(".hiden #TrackingCode option[value=" + trackingcode + "]").attr("disabled", "disabled")
        }
        $("#storejp-detail-action option[value='7']").removeAttr("disabled");
        $("#storejp-detail-id").val(arr.toString())
        $("#storejp-detail-tracking").val(trackingcode);
    }
    else {
        $("#storejp-detail-id").val("")
    }
    $("#storejp-table").find("input[type='checkbox']").attr("data-check", "check");
    $("#storejp-detail-action").val("-1");
});
$(document).on("change", "#storejp-table tbody tr.product-item td:not(:first-child, :empty)", function (e) {
    $(this).parent().find(".se-pre-con-crud").css("width", ($("#storejp-table").width() + 8) + "px");
    $(this).parent().find(".se-pre-con-crud").css("height", ($(this).height()+9) + "px");
    $(this).parent().find(".se-pre-con-crud").show();
    return false;
    e.preventDefault();
});
$(document).on("click", "#col-Default", function (e) {
    $(".hiden-column-content").hide(); $(".hiden-column-title").show();
    id = $("input[name=displaytype]:checked").attr("id");
    $(".hiden-column input[type=checkbox]").prop('checked', false);
    $(".hiden-column input[type=checkbox]").each(function (i, e) {
        if (id == "col-List") {
            $("tr.main-tracking").hide()
            if (i == 0||i==1 || i == 2 || i == 3 || i == 5 || i == 7 || i == 8 || i == 9 || i == 10 || i == 11 || i == 12 || i == 13 || i == 14 || i == 17) {
                $(this).prop('checked', true);
            }
        }
        else {
            $("tr.main-tracking").show()
            if (i == 0 || i == 2 || i == 3 || i == 5 || i == 7 || i == 8 || i == 9 || i == 10 || i == 11 || i == 12 || i == 13 || i == 14 || i == 17) {
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
//autocomplete
var BLF =
    {
        BindData: function (obj, data, text) {
            $("#" + obj).empty();
            if (text != null)
                $("#" + obj).append(new Option(text, "0"));
            if (data == null) {
                $("#" + obj).empty();
            }
            else {
                $.each(data, function (index, item) {
                    $("#" + obj).append(new Option(item.Text, item.Value));
                });
            }
        },
        Refresh: function (obj) {
            //$("#" + obj).selectpicker('refresh');
        }
    }
$(document).on("keypress", "#search_chuyenhang", function (e) {
    if ($(this).val() == "") {
        BLF.BindData("ShippingMask", null, "-- Tất cả --");
        BLF.BindData("TrackingCode", null, "-- Tất cả --");
        BLF.BindData("SplitCode", null, "-- Tất cả --");
    }
    $(this).autocomplete({
        source: "/DatabaseSearch/AuComShippingCode",
        minLength: 0,
        select: function (event, ui) {
            var data_sort = $("#sort-database").val();
            var shippingCode = ui.item.value;
            ChooseShippingCode(shippingCode)
        }
    });
});
function ChooseShippingCode(shippingCode) {
    $.get('/DatabaseSearch/LoadShippingMaskByShipping', { shippingCode: shippingCode }, function (d) {
        BLF.BindData("ShippingMask", d, "-- Tất cả --");
        BLF.BindData("TrackingCode", null, "-- Tất cả --");
        BLF.BindData("SplitCode", null, "-- Tất cả --");
    }, 'json');
}
function ChangeShippingMask(that) {
    var self = $(that);
    if (self.val() != "0") {
        $.get('/DatabaseSearch/LoadTrackingCodeByShippingMask', { ShippingMask: self.val() }, function (d) {
            BLF.BindData("TrackingCode", d, "-- Tất cả --"); BLF.BindData("SplitCode", null, "-- Tất cả --");
        }, 'json');
    }
    else {
        BLF.BindData("TrackingCode", null, "-- Tất cả --"); BLF.BindData("SplitCode", null, "-- Tất cả --");
    }
}
function ChangeTrackingCode(that) {
    var self = $(that);
    if (self.val() != "0") {
        $.get('/DatabaseSearch/LoadSplitCodeByTrackingCode', { TrackingCode: self.val() }, function (d) {
            BLF.BindData("SplitCode", d, "-- Tất cả --");
        }, 'json');
    } else {
        BLF.BindData("SplitCode", null, "-- Tất cả --");
    }
}