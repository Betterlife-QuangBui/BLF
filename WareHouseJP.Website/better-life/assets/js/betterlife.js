//show dialog delete
function showdelete(table) {
    var tb = $(table);
    width = tb.width();
    height = tb.height();
    var position = tb.offset();
    $(".se-pre-con-crud-index").css("width", (width + 8) + "px");
    $(".se-pre-con-crud-index").css("height", (height -105) + "px");
    $(".se-pre-con-crud-index").css("top", (position.top+45) + "px");
    $(".se-pre-con-crud-index").css("left", (position.left) + "px");
    $(".se-pre-con-crud-index button").css("margin-top", (height-145)/2 + "px");
    $(".se-pre-con-crud-index").show();

}

$(document).on("click", ".conform-close-index", function (e) {
    $(".se-pre-con-crud-index").hide();
});
$(window).bind('scroll', function () {
    if ($(window).scrollTop() > 50) {
        $('#sidebar').addClass('fixed');
    } else {
        $('#sidebar').removeClass('fixed');
    }
});
$(window).load(function () {
    $(".se-pre-con").show(); $(".se-pre-con").fadeOut();
    $("select.select2").select2({
            theme: "bootstrap"
        });
});
$(".shipment_add_body,.shipment_edit_body,.package_edit_body,.package_edit_body,.storejp_add_body,.package_add_body").css("cursor", "pointer");
$(".shipment_add_body,.shipment_edit_body,.package_edit_body,.package_edit_body,.storejp_add_body,.package_add_body,#img-capture-webcam").draggable();
setTimeout(function () {
    $('.message-alert').hide();
}, 5000);
function loading(){
$(".se-pre-con").show(); $(".se-pre-con").fadeOut();
}
function notify(style, message, icon) {
    if ($(".message-alert").hasClass("message-hidden")) {
        $(".message-alert").removeClass("message-hidde");
    }
    $(".message-alert").html("<div class='message-close'><i class='fa fa-times-circle-o fa-lg'></i></div><div class='message-body'>" + message + "</div>");
    $(".message-alert").show()
}
function notify1(style, message, icon) {
    $.notify({
        title: 'Thông báo',
        text: message,
        image: "<img src='/images/"+icon+".png'/>"
    }, {
        style: 'metro',
        className: style,
        autoHide: false,
        clickToHide: true
    });
}
$(document).on("click", ".message-close", function (e) {
    $(".message-alert").hide();
});
$(document).on("click", ".pagination a,button[type=submit].btncreate,a.add-item-btn,.shipment_item-edit,a.package_item-edit", function (e) {
    loading();
});
//inline page
jQuery(function ($) {
    //table checkboxes
    //$('th input[type=checkbox], td input[type=checkbox]').prop('checked', false);

    //select/deselect all rows according to table header checkbox
    $('#dynamic-table > thead > tr > th input[type=checkbox]').eq(0).on('click', function () {
        var th_checked = this.checked;//checkbox inside "TH" table header

        $(this).closest('table').find('tbody > tr').each(function () {
            var row = this;
            if (th_checked) tableTools_obj.fnSelect(row);
            else tableTools_obj.fnDeselect(row);
        });
    });

    //select/deselect a row when the checkbox is checked/unchecked
    $('#dynamic-table').on('click', 'td input[type=checkbox]', function () {
        var row = $(this).closest('tr').get(0);
        if (!this.checked) tableTools_obj.fnSelect(row);
        else tableTools_obj.fnDeselect($(this).closest('tr').get(0));
    });

    $(document).on('click', '#dynamic-table .dropdown-toggle', function (e) {
        e.stopImmediatePropagation();
        e.stopPropagation();
        e.preventDefault();
    });


    //And for the first simple table, which doesn't have TableTools or dataTables
    //select/deselect all rows according to table header checkbox
    var active_class = 'active';
    $('#simple-table > thead > tr > th input[type=checkbox]').eq(0).on('click', function () {
        var th_checked = this.checked;//checkbox inside "TH" table header

        $(this).closest('table').find('tbody > tr').each(function () {
            var row = this;
            if (th_checked) $(row).addClass(active_class).find('input[type=checkbox]').eq(0).prop('checked', true);
            else $(row).removeClass(active_class).find('input[type=checkbox]').eq(0).prop('checked', false);
        });
    });

    //select/deselect a row when the checkbox is checked/unchecked
    $('#simple-table').on('click', 'td input[type=checkbox]', function () {
        var $row = $(this).closest('tr');
        if (this.checked) $row.addClass(active_class);
        else $row.removeClass(active_class);
    });



    /********************************/
    //add tooltip for small view action buttons in dropdown menu
    $('[data-rel="tooltip"]').tooltip({ placement: tooltip_placement });

    //tooltip placement on right or left
    function tooltip_placement(context, source) {
        var $source = $(source);
        var $parent = $source.closest('table')
        var off1 = $parent.offset();
        var w1 = $parent.width();

        var off2 = $source.offset();
        //var w2 = $source.width();

        if (parseInt(off2.left) < parseInt(off1.left) + parseInt(w1 / 2)) return 'right';
        return 'left';
    }

});
$(document).on("mouseover", ".display-column,.navbar-buttons", function (e) {
    $(this).find(".dropdown-menu").show();
    return false;
    e.preventDefault();
});
$(document).on("mouseout", ".package_header,.navbar-buttons,.display-column-choose", function (e) {
    $(".dropdown-menu").hide();
    return false;
    e.preventDefault();
});
$(document).keypress(function (e) {
});
$(document).on("click", "div.btn-group a.delete-item", function (e) {
    //e.preventDefault();
    var id = $(this).attr("data-id");
    var href = $(this).attr("data-url");
    if (confirm("Bạn có chắc muốn xóa dữ liệu này")) {
        $.ajax({
            url: href,
            method: 'POST',
            success: function (result) {
                if (result.status == true) {
                    $("." + id).remove();
                    notify('white', 'Xóa dữ liệu thành công', "1");
                    e.preventDefault();
                }
                else {
                    notify('error', 'Xóa dữ liệu thất bại', "2"); e.preventDefault();
                }
            }
        }).done(function () { e.preventDefault(); });
    }
    e.preventDefault();
});
$(document).on("click", ".not-shipment_item-edit", function (e) {
    notify('white', $(this).attr("data-message"), "1");
});
$(document).on("click", "a.delete-item-main", function (e) {
    //e.preventDefault();
    var id = $(this).attr("data-id");
    var href = $(this).attr("data-url");
    if (confirm("Bạn có chắc muốn xóa dữ liệu này")) {
        $.ajax({
            url: href,
            method: 'POST',
            success: function (result) {
                if (result.status == true) {
                    $("#" + id.toString().replace("tr_", "")).remove();
                    notify('white', 'Xóa dữ liệu thành công', "1");
                    //location.reload();
                    $("body").find(".wasehouse-info").html("");
                    $("body").find(".control").html("");
                    e.preventDefault();
                }
                else {
                    notify('error', 'Xóa dữ liệu thất bại', "2"); e.preventDefault();
                }
            }
        }).done(function () { e.preventDefault(); });
    }
    e.preventDefault();
});
//dialog delete
$(function () {
    shortcut.add("F2", function () {
        $(".form-search a:first").click();
    });
    //override dialog's title function to allow for HTML titles
    $.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
        _title: function (title) {
            var $title = this.options.title || '&nbsp;'
            if (("title_html" in this.options) && this.options.title_html == true)
                title.html($title);
            else title.text($title);
        }
    }));
});
function sort() {
    $("input [name='sort']").val($("#sort-by").val());
    $("#AgencyPackage").submit();
}
function active(value) {
    $("#sort-by option[value='" + value + "']").attr("selected", "selected");
}
//dialog colorbox
$(document).ready(function () {
    //agency
    $(".iframe_agency_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 500, overlayClose: false });
    //HAWB
    $(".iframe_hawb_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 530, overlayClose: false });
    //MAWB
    $(".iframe_mawb_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 530, overlayClose: false });
    //agency account
    $(".iframe_agency_account_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 530, overlayClose: false });
    //Delivery Company
    $(".iframe_deliverycom_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 450, overlayClose: false });
    //AgencyPackage
    //$(".iframe_agencypackage_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 670, overlayClose: false });
    //AgencyPackageProduct
    $(".iframe_agencypackageproduct_add").colorbox({ iframe: true, innerWidth: "75%", innerHeight: "75%", overlayClose: false });
    //WareHouseInfo
    $(".iframe_warehouseinfo_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 430, overlayClose: false });
    //staff account
    $(".iframe_staff_account_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 500, overlayClose: false });

    //iframe_storagejp_add
    // $(".iframe_storagejp_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 680, overlayClose: false });

    //StorageItemJP
    // $(".iframe_storageitemjp_add").colorbox({ iframe: true, innerWidth: "90%", innerHeight: "80%", overlayClose: false });

    //StatusWareHouse
    $(".iframe_statuswarehouse_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 200, overlayClose: false });

    //airinfo
    $(".iframe_airinfo_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 500, overlayClose: false });
    //airinfo
    $(".iframe_fromair_add").colorbox({ iframe: true, innerWidth: 500, innerHeight: 200, overlayClose: false });
    $(".iframe_toair_add").colorbox({ iframe: true, innerWidth: 500, innerHeight: 200, overlayClose: false });
    //ExportGoods
    //$(".iframe_exportgood_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 640, overlayClose: false });

    //FlightBooking
    //$(".iframe_flightbooking_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 640, overlayClose: false });

    //TrackingStatus
    $(".iframe_trackingstatus_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 230, overlayClose: false });
    //Countries
    $(".iframe_countries_add").colorbox({ iframe: true, innerWidth: 600, innerHeight: 230, overlayClose: false });

    //Export Good Details
    $(".iframe_exportgooddetails_add").colorbox({ iframe: true, innerWidth: "90%", innerHeight: "80%", overlayClose: false });

    //iframe_exportinvoice_add
    $(".iframe_exportinvoice_add").colorbox({ iframe: true, innerWidth: 700, innerHeight: 500, overlayClose: false });

    //Update Info
    $(".iframe_updateinfo_edit").colorbox({ iframe: true, innerWidth: 700, innerHeight: 500, overlayClose: false });

    //DeliveryAddresses
    $(".iframe_deliveryaddresses_edit").colorbox({ iframe: true, innerWidth: 700, innerHeight: 300, overlayClose: false });

    //Examples of how to assign the Colorbox event to elements
    $(".group1").colorbox({ rel: 'group1' });
    $(".group2").colorbox({ rel: 'group2', transition: "fade" });
    $(".group3").colorbox({ rel: 'group3', transition: "none", width: "75%", height: "75%" });
    $(".group4").colorbox({ rel: 'group4', slideshow: true });
    $(".ajax").colorbox();
    $(".youtube").colorbox({ iframe: true, innerWidth: 640, innerHeight: 390 });
    $(".vimeo").colorbox({ iframe: true, innerWidth: 500, innerHeight: 409 });
    $(".iframe").colorbox({ iframe: true, width: "80%", height: "80%" });
    $(".inline").colorbox({ inline: true, width: "50%" });
    $(".callbacks").colorbox({
        onOpen: function () { alert('onOpen: colorbox is about to open'); },
        onLoad: function () { alert('onLoad: colorbox has started to load the targeted content'); },
        onComplete: function () { alert('onComplete: colorbox has displayed the loaded content'); },
        onCleanup: function () { alert('onCleanup: colorbox has begun the close process'); },
        onClosed: function () { alert('onClosed: colorbox has completely closed'); }
    });

    $('.non-retina').colorbox({ rel: 'group5', transition: 'none' })
    $('.retina').colorbox({ rel: 'group5', transition: 'none', retinaImage: true, retinaUrl: true });

    //Example of preserving a JavaScript event for inline calls.
    $("#click").click(function () {
        $('#click').css({ "background-color": "#f00", "color": "#fff", "cursor": "inherit" }).text("Open this window again and this message will still be here.");
        return false;
    });
});