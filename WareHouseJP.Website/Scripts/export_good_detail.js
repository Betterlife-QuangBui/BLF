$(function () {
    loadJP();
})
//research-text
$(document).on("keyup", ".research-text", function () {
    loadJP();
});
function loadJP() {
    status = $("#export_status").val();
    if (status != 10) {
        $.ajax({
            url: '/ExportGoodDetails/_Search',
            data: { key: $(".research-text").val() },
            method: 'POST', async: false,
            async: false,
            success: function (result) {
                $("#item_container").html(result);
            }
        });
    }
    else {
        $("#item_container").html("");
    }
}
$(document).on("mouseover", "body", function () {
    $("#item_container .item").draggable({
        revert: true
    });
    $("#cart_items").droppable({
        accept: ".item",
        activeClass: "drop-active",
        hoverClass: "drop-hover",
        drop: function (event, ui) {
            var item = ui.draggable.html();
            var itemid = ui.draggable.attr("id");
            var id = ui.draggable.attr("data-id");
            //Save into database
            var array = [];
            array.push(id);
            var exportId = $("#exportId").val();
            $.ajax({
                url: '/ExportGoodDetails/Add2New',
                data: { array: array, exportId: exportId },
                method: 'POST',
                async: false,
                success: function (result) {
                    if (result.status == true) {
                        array = [];

                        var html = '<div class="item" id="to' + itemid + '">';
                        html = html + '<div class="divrm">';
                        html = html + '<a data-new="' + result.message.idNew + '" data-export="@(export.Id)" data-ids=' + id + ' data-id=' + itemid + ' class="red delete-to remove  ' + itemid + '"><i class="fa fa-trash"></i></a>';
                        html = html + '<div/>' + item + '</div>';
                        if ($("#cart_items").find('.item .divrm a[data-id=' + itemid + ']').length <= 0) {
                            $("#cart_items").append(html);
                        }
                        $(".num-package").html(result.message.num);
                        $(".num-kg").html(result.message.kg);
                        $("div#" + exportId).find(".info-tracking span:eq(0)").html("Số kiện: " + result.message.num);
                        if (result.message.num <= 0) {
                            $(".check-isstaffconfirm").attr('disabled', 'disabled'); $("div.control[data-id='" + $("#exportId").val() + "'] a:eq(2)").hide();
                        }
                        else {
                            $(".check-isstaffconfirm").removeAttr('disabled'); $("div.control[data-id='" + $("#exportId").val() + "'] a:eq(2)").show();
                        }
                        $("#item_container #" + itemid).remove();
                        if ($("#cart_items").find('.item').length > 0) {
                            $(".mess-thongbao").hide();
                        }
                    }
                }
            });
        }
    });
});

$(document).on("click", '.delete-to', function () {
    var id = $(this).attr("data-id");
    var ids = $(this).attr("data-ids");
    var title = $(this).next().next().html();
    var html = '<div class="item" data-id="' + ids + '" id="' + id + '"><label class="title">' + title + '</label></div>';
    //$("#item_container").append(html);
    $(this).parent().parent().remove();
    id_new = $(this).attr("data-new");
    $.ajax({
        method: 'POST',async :false,
        url: '/ExportGoodDetails/DeleteExportGoodDetail2', data: { id: id_new },
        success: function (result) {
            if (result.status == true) {
                $(".num-package").html(result.message.num);
                $(".num-kg").html(result.message.kg);
                $("div#" + $("#exportId").val()).find(".info-tracking span:eq(0)").html("Số kiện: " + result.message.num);
                if (result.message.num <= 0) {
                    $(".check-isstaffconfirm").attr('disabled', 'disabled');
                    $("div.control[data-id='" + $("#exportId").val() + "'] a:eq(2)").hide();
                }
                else {
                    $(".check-isstaffconfirm").removeAttr('disabled');
                    $("div.control[data-id='" + $("#exportId").val() + "'] a:eq(2)").show();
                }
                loadJP();
            }
        }
    });
});


$(document).on("click", ".removeTrackingMain", function () {
    if (confirm('Bạn có chắc muốn xóa dữ liệu này?')) {
        id = $(this).attr("data-id");
        $.ajax({
            method: 'POST',async :false,
            url: '/ExportGoodDetails/DeleteExportGoodDetail', data: { id: id },
            success: function (result) {
                if (result.status == true) {
                    $("#li" + id).remove();
                    $("#detail" + id).remove();
                    $('#myTab1s a:first').tab('show')
                }
            }
        });
    }
});

$(document).on("click", ".btn_save_vn", function () {
    var array = [];
    $("#cart_items .item").each(function (index) {
        array.push($(this).find('.divrm a').attr('data-ids'));
    });
    if (array.length == 0) {
        alert('Vui lòng chon kiện hàng JP');
    }
    else {
        exportId = $("#exportId").val();
        $.ajax({
            url: '/ExportGoodDetails/AddNew',
            data: { array: array, exportId: exportId },
            method: 'POST',
            async: false,
            success: function (result) {
                if (result.status == true) {
                    array = [];
                    $("#cart_items").html("");
                    $.each(result.message, function (index, item) {
                        html = '<li class="" style="position:relative;" id="li' + item.Id + '">';
                        html += '<a data-toggle="tab" href="#detail' + item.Id + '" aria-expanded="false">';
                        html += '<i class="grey ace-icon fa fa-gavel"></i> ' + item.TrackingCode + '</a>';
                        html += '<a class="removeTrackingMain" style="position: absolute;right: -11px;top: -11px;padding:2px;border: none;z-index:9;background: none;box-shadow:none;cursor:pointer;" href="#" data-id="' + item.Id + '">';
                        html += '<i class="red ace-icon fa fa-trash"></i> </a>';
                        html += '</li>';
                        $("#myTab1s").append(html);
                        $.ajax({
                            url: '/ExportGoodDetails/DisplayData',
                            data: { id: item.Id },
                            method: 'POST',
                            async: false,
                            success: function (result) {
                                content = '<div id="detail' + item.Id + '" class="tab-pane fade">';
                                content += result;
                                content += '</div>';
                                $('.tab-content').append(content);
                            }
                        });

                    });
                }
            }
        });
    }
});
$(document).on("click", '.check-isstaffconfirm', function (e) {
    var id = $(this).attr("data-id");
    check = $(this).is(":checked");
    $.ajax({
        type: 'POST', async: false,
        data: { id: id, isCheck: check },
        url: '/ExportGoodDetails/IsCheckConfirm',
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Cập nhật dữ liệu thành công', "1");
                //update status
                $("#" + id).find("span#span-" + id).html(result.message);
                //call view detail
                $("#" + id).trigger('click');
                if (check == true) {
                    //disable control
                    $("div.control[data-id='" + id + "'] a:eq(0)").addClass("disabled");
                    $("div.control[data-id='" + id + "'] a:eq(1)").addClass("disabled");
                }
                else {
                    $("div.control[data-id='" + id + "'] a").removeClass("disabled");
                }
            }
            else {
                notify('error', 'Cập nhật dữ liệu thất bại', "2");
            }
        }
    })
    e.preventDefault();
})