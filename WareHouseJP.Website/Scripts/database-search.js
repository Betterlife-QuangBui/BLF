// Add database
$(document).on("click", ".storejp_add", function (e) {
    $('tr.database-add').css('display', 'table-row');
});
$(document).on("click", ".package_item-cancel", function (e) {
    $('tr.database-add').css('display', 'none');
    $("#storejp-table thead tr.database-add input:eq(0)").val('');
    $("#storejp-table thead tr.database-add input:eq(1)").val('');
    $("#storejp-table thead tr.database-add select:eq(0)").val(0);
    $("#storejp-table thead tr.database-add input:eq(2)").val('');
    $("#storejp-table thead tr.database-add input:eq(3)").val('');
    $("#storejp-table thead tr.database-add input:eq(4)").val('');
    $("#storejp-table thead tr.database-add input:eq(5)").val('');
    $("#storejp-table thead tr.database-add input:eq(6)").val('');
    $("#storejp-table thead tr.database-add input:eq(7)").val('');
    //$("#storejp-table thead tr.database-add select:eq(1)").val();
    $("#storejp-table thead tr.database-add input:eq(8)").val('');
    $("#storejp-table thead tr.database-add input:eq(9)").val('');
    $("#storejp-table thead tr.database-add input:eq(10)").val('');
    $("#storejp-table thead tr.database-add input:eq(11)").val('');
    $("#storejp-table thead tr.database-add input:eq(12)").val('');
    //$("#storejp-table thead tr.database-add select:eq(2)").val();
});
$(document).on("click", "#storejp_add", function (e) {
    var namejp = $("#storejp-table thead tr.database-add input:eq(0)").val();
    var nameen = $("#storejp-table thead tr.database-add input:eq(1)").val();
    var category = $("#storejp-table thead tr.database-add select:eq(0)").val();
    var categoryweb = $("#storejp-table thead tr.database-add input:eq(2)").val();
    //var image       = $("#storejp-table thead tr.database-add input:eq(3)").val();
    var jancode = $("#storejp-table thead tr.database-add input:eq(4)").val();
    var productcode = $("#storejp-table thead tr.database-add input:eq(5)").val();
    var quantity = $("#storejp-table thead tr.database-add input:eq(6)").val();
    var price = $("#storejp-table thead tr.database-add input:eq(7)").val();
    var amount = $("#storejp-table thead tr.database-add input:eq(8)").val();
    var origin = $("#storejp-table thead tr.database-add select:eq(1)").val();
    var material = $("#storejp-table thead tr.database-add input:eq(9)").val();
    var component = $("#storejp-table thead tr.database-add input:eq(10)").val();
    //var componentimage = $("#storejp-table thead tr.database-add input:eq(11)").val();
    var linkweb = $("#storejp-table thead tr.database-add input:eq(12)").val();
    var flightcode = $("#storejp-table thead tr.database-add input:eq(13)").val();
    var tracking = $("#storejp-table thead tr.database-add input:eq(14)").val();
    var producttype = $("#storejp-table thead tr.database-add select:eq(2)").val();
    var imgbase64   = $("#ImageBase64").val();
    var imgbase64component = $("#ComponentImage").val();
    
    var objDB = {
        NameJP: namejp, NameEN: nameen, CategoryId: category, PriceTax: price, JanCode: jancode, ProductCode: productcode,
        MadeIn: origin, Material: material, Component: component, CategoryWebName: categoryweb, LinkWeb: linkweb, FlightCode: flightcode,
        TrackingCode: tracking, ProductTypeId: producttype, Quantity: quantity, ImageBase64: imgbase64, ComponentImage: imgbase64component
    };
    $.ajax({
        type: "POST",
        url: "/Database/Add",
        data: objDB,
        datatype: "html",
        success: function (data) {
            alert('add success!');
            location.reload();
        }
    });
});
// delete database

// edit database
$(document).on("change", "#storejp-table tbody tr.product-item td:not(:first-child, :empty)", function (e) {
    $(this).parent().find(".se-pre-con-crud").css("width", ($("#storejp-table").width() + 8) + "px");
    $(this).parent().find(".se-pre-con-crud").css("height", ($(this).height() + 9) + "px");
    $(this).parent().find(".se-pre-con-crud").show();
    return false;
    e.preventDefault();
});
$(document).on("click", ".conform-close", function (e) {
    var id = $(this).attr("data-id");
    
    $(this).parent().parent().parent().find("td").each(function (i, e) {
        $(e).find("textarea").each(function (j, el) {
            $(el).val($(el).attr("data-value"));
        });
        $(e).find("input[type=text]").each(function (j, el) {
            $(el).val($(el).attr("data-value"));
        });
        $(e).find("input[type=number]").each(function (j, el) {
            $(el).val($(el).attr("data-value"));
        });
        $(e).find("input[type=hidden]").each(function (j, el) {
            $(el).val($(el).attr("data-value"));
        });
        $(e).find("select").each(function (j, el) {
            $(el).val($(el).attr("data-value"));
        });
    });
    amount = parseFloat($("#" + id).find("input[name=PriceTax]").val()) * parseFloat($("#" + id).find("input[name=Quantity]").val());
    $("#" + id).find(".amount").html(amount);
    $(this).parent().hide();
    return false;
    e.preventDefault();
});
$(document).on("click", ".conform-save", function (e) {
    var json = {};
    var id = $(this).attr("data-id");
    amount = parseFloat($("#" + id).find("input[name=PriceTax]").val()) * parseFloat($("#" + id).find("input[name=Quantity]").val());
    $("#" + id).find(".amount").html(amount);
    $(this).parent().parent().parent().find("td").each(function (i, e) {
        $(e).find("textarea").each(function (j, el) {
            json[$(el).attr("name")] = $(el).val();
        });
        $(e).find("input[type=text]").each(function (j, el) {
            json[$(el).attr("name")] = $(el).val();
        });
        $(e).find("input[type=number]").each(function (j, el) {
            json[$(el).attr("name")] = $(el).val();
        });
        $(e).find("input[type=hidden]").each(function (j, el) {
            json[$(el).attr("name")] = $(el).val();
        });
        $(e).find("select").each(function (j, el) {
            json[$(el).attr("name")] = $(el).val();
        });
    });
    $.ajax({
        method: 'POST',
        url: "/Database/Update",
        data: JSON.stringify(json),
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Cập nhật liệu thành công', "1");
                $("#" + id).find(".se-pre-con-crud").hide();
                //lay tong loi

                //remove background
                NameJP = json["NameJP"]; //JanCode = json["JanCode"];
                NameEN = json["NameEN"]; //ImageBase64 = json["ImageBase64"];
                //remove JanCode
                //$("#" + id).find("input[name=JanCode]").attr("data-value", JanCode);
                //if (JanCode == "") {
                //    $("#" + id).find("input[name=JanCode]").parent().attr("class", "center");
                //}
                //else {
                //    if (JanCode.toString().length != 13 || JanCode.toString().indexOf('4') != 0) {
                //        $("#" + id).find("input[name=JanCode]").parent().attr("class", "center item-deny");
                //    }
                //    else {
                //        $("#" + id).find("input[name=JanCode]").parent().attr("class", "center");
                //    }
                //}
                //remove namjp & en
                var arrayName = "ガス,スプレー,オイル,バッテリー,漂白,ペイント,gas,spray,oil,battery,bleach,paint".split(',');
                var checkName = false;
                for (var i = 0; i < arrayName.length; i++) {
                    if (NameJP.toString().toLowerCase().indexOf(arrayName[i]) >= 0) {
                        checkName = true;
                        break;
                    }
                }
                $("#" + id).find("textarea[name=NameJP]").attr("data-value", NameJP);
                if (checkName == true || NameJP == "") {
                    $("#" + id).find("textarea[name=NameJP]").parent().attr("class", "center item-deny");
                    checkName = false;
                }
                else {
                    $("#" + id).find("textarea[name=NameJP]").parent().attr("class", "center");
                }
                //remove namjp & en
                for (var i = 0; i < arrayName.length; i++) {
                    if (NameEN.toString().toLowerCase().indexOf(arrayName[i]) >= 0) {
                        checkName = true;
                        break;
                    }
                }
                $("#" + id).find("textarea[name=NameEN]").attr("data-value", NameEN);
                if (checkName == true || NameEN == "") {
                    $("#" + id).find("textarea[name=NameEN]").parent().attr("class", "center item-deny");
                    checkName = false;
                }
                else {
                    $("#" + id).find("textarea[name=NameEN]").parent().attr("class", "center");
                }
                ////image
                //$("#" + id).find("input[name=ImageBase64]").attr("data-value", ImageBase64);
                //if (ImageBase64 != "") {
                //    $("#" + id).find("input[name=ImageBase64]").parent().attr("class", "center");
                //}
                //else {
                //    $("#" + id).find("input[name=ImageBase64]").parent().attr("class", "center item-deny");
                //}

                //Quantity = json["Quantity"]; PriceTax = json["PriceTax"];
                ////quantity
                //$("#" + id).find("input[name=Quantity]").attr("data-value", Quantity);
                //if (Quantity == "" || parseFloat(Quantity) == 0) {
                //    $("#" + id).find("input[name=Quantity]").parent().attr("class", "detail-right item-deny");
                //}
                //else {
                //    $("#" + id).find("input[name=Quantity]").parent().attr("class", "detail-right");
                //}
                ////price
                //$("#" + id).find("input[name=PriceTax]").attr("data-value", PriceTax);
                //if (parseFloat(PriceTax) >= 50) {
                //    $("#" + id).find("input[name=PriceTax]").parent().attr("class", "detail-right");
                //}
                //else {
                //    $("#" + id).find("input[name=PriceTax]").parent().attr("class", "detail-right item-deny");
                //}

                ////category & made in
                //CategoryId = json["CategoryId"]; MadeIn = json["MadeIn"];
                //$("#" + id).find("select[name=CategoryId]").attr("data-value", CategoryId);
                //if (CategoryId != "") {
                //    $("#" + id).find("select[name=CategoryId]").parent().attr("class", "detail-right");
                //}
                //else {
                //    $("#" + id).find("select[name=CategoryId]").parent().attr("class", "detail-right item-deny");
                //}
                //$("#" + id).find("select[name=MadeIn]").attr("data-value", MadeIn);
                //if (MadeIn != "") {
                //    $("#" + id).find("select[name=MadeIn]").parent().attr("class", "detail-right");
                //}
                //else {
                //    $("#" + id).find("select[name=MadeIn]").parent().attr("class", "detail-right item-deny");
                //}
                ////link website
                //LinkWeb = json["LinkWeb"];
                //$("#" + id).find("input[name=LinkWeb]").attr("data-value", LinkWeb);
                //if (LinkWeb != "") {
                //    $("#" + id).find("input[name=LinkWeb]").parent().attr("class", "center");
                //}
                //else {
                //    $("#" + id).find("input[name=LinkWeb]").parent().attr("class", "center item-deny");
                //}
                //hien thi lai so loi

                //var JanCode_error = 0; var NameJP_error = 0;
                //var NameEN_error = 0; var CategoryId_error = 0; var ImageBase64_error = 0; var Quantity_error = 0;
                //var PriceTax_error = 0; var MadeIn_error = 0; var LinkWeb_error = 0;
                //$("#storejp-table tbody tr").each(function (i, e) {
                //    if (($(e).find(" td:eq(2)").attr("class")) != undefined) {
                //        if ($(e).find(" td:eq(2)").attr("class").indexOf('item-deny') >= 0) {
                //            NameJP_error = NameJP_error + 1;
                //        }
                //    }
                //});
                //$("#storejp-table tbody tr").each(function (i, e) {
                //    if (($(e).find(" td:eq(4)").attr("class")) != undefined) {
                //        if ($(e).find(" td:eq(4)").attr("class").indexOf('item-deny') >= 0) {
                //            NameEN_error = NameEN_error + 1;
                //        }
                //    }
                //});
                //$("#storejp-table tbody tr").each(function (i, e) {
                //    if (($(e).find(" td:eq(5)").attr("class")) != undefined) {
                //        if ($(e).find(" td:eq(5)").attr("class").indexOf('item-deny') >= 0) {
                //            CategoryId_error = CategoryId_error + 1;
                //        }
                //    }
                //});
                //$("#storejp-table tbody tr").each(function (i, e) {
                //    if (($(e).find(" td:eq(7)").attr("class")) != undefined) {
                //        if ($(e).find(" td:eq(7)").attr("class").indexOf('item-deny') >= 0) {
                //            ImageBase64_error = ImageBase64_error + 1;
                //        }
                //    }
                //});
                //$("#storejp-table tbody tr").each(function (i, e) {
                //    if (($(e).find(" td:eq(8)").attr("class")) != undefined) {
                //        if ($(e).find(" td:eq(8)").attr("class").indexOf('item-deny') >= 0) {
                //            JanCode_error = JanCode_error + 1;
                //        }
                //    }
                //});
                //$("#storejp-table tbody tr").each(function (i, e) {
                //    if (($(e).find(" td:eq(10)").attr("class")) != undefined) {
                //        if ($(e).find(" td:eq(10)").attr("class").indexOf('item-deny') >= 0) {
                //            Quantity_error = Quantity_error + 1;
                //        }
                //    }
                //});
                //$("#storejp-table tbody tr").each(function (i, e) {
                //    if (($(e).find(" td:eq(11)").attr("class")) != undefined) {
                //        if ($(e).find(" td:eq(11)").attr("class").indexOf('item-deny') >= 0) {
                //            PriceTax_error = PriceTax_error + 1;
                //        }
                //    }
                //});
                //$("#storejp-table tbody tr").each(function (i, e) {
                //    if (($(e).find(" td:eq(13)").attr("class")) != undefined) {
                //        if ($(e).find(" td:eq(13)").attr("class").indexOf('item-deny') >= 0) {
                //            MadeIn_error = MadeIn_error + 1;
                //        }
                //    }
                //});
                //$("#storejp-table tbody tr").each(function (i, e) {
                //    if (($(e).find(" td:eq(117)").attr("class")) != undefined) {
                //        if ($(e).find(" td:eq(17)").attr("class").indexOf('item-deny') >= 0) {
                //            LinkWeb_error = LinkWeb_error + 1;
                //        }
                //    }
                //});
                //sum_error = NameJP_error + NameEN_error + CategoryId_error + ImageBase64_error + JanCode_error + Quantity_error + PriceTax_error + MadeIn_error + LinkWeb_error
                //$(".sum-error").html(sum_error); $(".NameJP-error").html(NameJP_error);
                //$(".NameEN-error").html(NameEN_error); $(".CategoryId-error").html(CategoryId_error);
                //$(".ImageBase64-error").html(ImageBase64_error); $(".JanCode-error").html(JanCode_error);
                //$(".Quantity-error").html(Quantity_error); $(".PriceTax-error").html(PriceTax_error);
                //$(".MadeIn-error").html(MadeIn_error); $(".LinkWeb-error").html(LinkWeb_error);
            }
            else {
                $("#" + id).find(".se-pre-con-crud").hide();
                notify('error', "Cập nhật dữ liệu thất bại", "2");
            }
        }
    });
    return false;
    e.preventDefault();
});
//$(document).on("click", ".package_item-edit", function (e) {
//    var trid = $(this).closest('tr').attr('id'); // table row ID

//    $('tr#' + trid).html('<tr><td><input type="text" value="nguyen" /></td></tr>');
//    $('tr#' + trid).css('display', 'none');
//    //alert(trid);
//});
// upload image
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
// upload image component
$(document).on("click", ".label-component", function (e) {
    $(".picture-option").css("display", "block");
});
$(document).on("click", ".div-op-component .icon-component", function (e) {
    $(".div-op-component").css("display", "none");
});
$(document).on("click", "#upload-computer-component", function (e) {
    $("#id-input-file-component").click();
});
$(document).on("change", "#id-input-file-component", function (e) {
    if (this.files && this.files[0]) {
        var FR = new FileReader();
        FR.onload = function (e) {
            $("#img-review-component img").attr("src", e.target.result);
            $("#ComponentImage").val(e.target.result)
        };
        FR.readAsDataURL(this.files[0]);
    }
});
//sort-jp
$(document).on("click", "#storejp-table .sort-row .sort", function (e) {
    loading();
    var data_sort = $(this).parent().attr("data-sort");
    $("#sort-jp").val(data_sort);
    var data_sort_return = data_sort.split('-')[0] + "-" + (data_sort.split('-')[1] == "asc" ? "desc" : "asc");
    $(this).parent().attr("data-sort", data_sort_return);

    var pagesize = $("#inPageSize").val();
    var pageno = $("#inPageNo").val();
    var totalcount = $("#inTotalCount").val();
    var data_sort = $("#sort-jp").val();
    var storejp_id = $("#StoreJPId").val();
    var namejp = $("#storejp-table thead tr.filter-jp input:eq(0)").val();
    var nameen = $("#storejp-table thead tr.filter-jp input:eq(1)").val();
    var category = $("#storejp-table thead tr.filter-jp select:eq(0)").val();
    var categoryweb = $("#storejp-table thead tr.filter-jp input:eq(2)").val();
    //var image     = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
    var jancode = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
    var productcode = $("#storejp-table thead tr.filter-jp input:eq(4)").val();
    var quantity = $("#storejp-table thead tr.filter-jp input:eq(5)").val();
    var price = $("#storejp-table thead tr.filter-jp input:eq(6)").val();
    var amount = $("#storejp-table thead tr.filter-jp input:eq(7)").val();
    var origin = $("#storejp-table thead tr.filter-jp select:eq(1)").val();
    var material = $("#storejp-table thead tr.filter-jp input:eq(8)").val();
    var component = $("#storejp-table thead tr.filter-jp input:eq(9)").val();
    //var componentimage = $("#storejp-table thead tr.filter-jp input:eq(11)").val();
    //var linkweb   = $("#storejp-table thead tr.filter-jp input:eq(10)").val();
    var flightcode = $("#storejp-table thead tr.filter-jp input:eq(10)").val();
    var tracking = $("#storejp-table thead tr.filter-jp input:eq(11)").val();
    var producttype = $("#storejp-table thead tr.filter-jp select:eq(2)").val();

    var href = "/ajax/dbstorage?pageno=" + pageno + "&pageSize=" + pagesize + "&totalCount=" + totalcount + "&data_sort=" + data_sort
                + "&namejp=" + namejp + "&nameen=" + nameen + "&category=" + category + "&categorywebname=" + categoryweb
                + "&jancode=" + jancode + "&productcode=" + productcode + "&quantity=" + quantity + "&price=" + price + "&amount=" + amount
                + "&origin=" + origin + "&flightcode=" + flightcode + "&tracking=" + tracking + "&producttype=" + producttype;
    //var href = "/ajax/DataBaseStorage?page=1&namejp=" + namejp + "&nameen=" + nameen + "&category=" + category + "&categorywebname=" + categoryweb
    //        + "&jancode=" + jancode + "&productcode=" + productcode + "&quantity=" + quantity + "&price=" + price + "&amount=" + amount
    //        + "&origin=" + origin + "&material=" + material + "&component=" + component + "&flightcode=" + flightcode
    //        + "&tracking=" + tracking + "&producttype=" + producttype + "&data_sort=" + data_sort;

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
$(document).on("keyup", "#storejp-table .filter-jp input:eq(0), #storejp-table thead tr input:eq(1), #storejp-table thead tr input:eq(2), #storejp-table thead tr input:eq(3), #storejp-table thead tr input:eq(4), #storejp-table thead tr input:eq(5), #storejp-table thead tr input:eq(6), #storejp-table thead tr input:eq(7), #storejp-table thead tr input:eq(8), #storejp-table thead tr input:eq(9), #storejp-table thead tr input:eq(10), #storejp-table thead tr input:eq(11)", function (e) {
    if ($(this).val().length > 0) {
        loading();
        
        var pagesize = $("#inPageSize").val();
        var pageno = $("#inPageNo").val();
        var totalcount = $("#inTotalCount").val();
        var data_sort = $("#sort-jp").val();
        var storejp_id = $("#StoreJPId").val();
        var namejp = $("#storejp-table thead tr.filter-jp input:eq(0)").val();
        var nameen = $("#storejp-table thead tr.filter-jp input:eq(1)").val();
        var category = $("#storejp-table thead tr.filter-jp select:eq(0)").val();
        var categoryweb = $("#storejp-table thead tr.filter-jp input:eq(2)").val();
        //var image     = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
        var jancode = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
        var productcode = $("#storejp-table thead tr.filter-jp input:eq(4)").val();
        var quantity = $("#storejp-table thead tr.filter-jp input:eq(5)").val();
        var price = $("#storejp-table thead tr.filter-jp input:eq(6)").val();
        var amount = $("#storejp-table thead tr.filter-jp input:eq(7)").val();
        var origin = $("#storejp-table thead tr.filter-jp select:eq(1)").val();
        var material = $("#storejp-table thead tr.filter-jp input:eq(8)").val();
        var component = $("#storejp-table thead tr.filter-jp input:eq(9)").val();
        //var componentimage = $("#storejp-table thead tr.filter-jp input:eq(11)").val();
        //var linkweb   = $("#storejp-table thead tr.filter-jp input:eq(10)").val();
        var flightcode = $("#storejp-table thead tr.filter-jp input:eq(10)").val();
        var tracking = $("#storejp-table thead tr.filter-jp input:eq(11)").val();
        var producttype = $("#storejp-table thead tr.filter-jp select:eq(2)").val();

        var href = "/ajax/DBStorage?pageno=" + pageno + "&pageSize=" + pagesize + "&totalCount=" + totalcount + "&data_sort=" + data_sort
                + "&namejp=" + namejp + "&nameen=" + nameen + "&category=" + category + "&categorywebname=" + categoryweb
                + "&jancode=" + jancode + "&productcode=" + productcode + "&quantity=" + quantity + "&price=" + price + "&amount=" + amount
                + "&origin=" + origin + "&flightcode=" + flightcode + "&tracking=" + tracking + "&producttype=" + producttype;
        //var href = "/ajax/DataBaseStorage?page=1&namejp=" + namejp + "&nameen=" + nameen + "&category=" + category + "&categorywebname=" + categoryweb
        //        + "&jancode=" + jancode + "&productcode=" + productcode + "&quantity=" + quantity + "&price=" + price + "&amount=" + amount
        //        + "&origin=" + origin + "&material=" + material + "&component=" + component + "&flightcode=" + flightcode
        //        + "&tracking=" + tracking + "&producttype=" + producttype + "&data_sort=" + data_sort;

        $("#storejp-table tbody").html("");
        $("#storejp-table tbody").load(href);
    }
});
$(document).on("change", "#storejp-table .filter-jp select:eq(0), #storejp-table .filter-jp select:eq(1), #storejp-table .filter-jp select:eq(2)", function (e) {
    loading();

    var pagesize = $("#inPageSize").val();
    var pageno = $("#inPageNo").val();
    var totalcount = $("#inTotalCount").val();
    var data_sort = $("#sort-jp").val();
    var storejp_id = $("#StoreJPId").val();
    var namejp = $("#storejp-table thead tr.filter-jp input:eq(0)").val();
    var nameen = $("#storejp-table thead tr.filter-jp input:eq(1)").val();
    var category = $("#storejp-table thead tr.filter-jp select:eq(0)").val();
    var categoryweb = $("#storejp-table thead tr.filter-jp input:eq(2)").val();
    //var image     = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
    var jancode = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
    var productcode = $("#storejp-table thead tr.filter-jp input:eq(4)").val();
    var quantity = $("#storejp-table thead tr.filter-jp input:eq(5)").val();
    var price = $("#storejp-table thead tr.filter-jp input:eq(6)").val();
    var amount = $("#storejp-table thead tr.filter-jp input:eq(7)").val();
    var origin = $("#storejp-table thead tr.filter-jp select:eq(1)").val();
    var material = $("#storejp-table thead tr.filter-jp input:eq(8)").val();
    var component = $("#storejp-table thead tr.filter-jp input:eq(9)").val();
    //var componentimage = $("#storejp-table thead tr.filter-jp input:eq(11)").val();
    //var linkweb = $("#storejp-table thead tr.filter-jp input:eq(10)").val();
    var flightcode = $("#storejp-table thead tr.filter-jp input:eq(10)").val();
    var tracking = $("#storejp-table thead tr.filter-jp input:eq(11)").val();
    var producttype = $("#storejp-table thead tr.filter-jp select:eq(2)").val();

    var href = "/ajax/DBStorage?pageno=" + pageno + "&pageSize=" + pagesize + "&totalCount=" + totalcount + "&data_sort=" + data_sort
                + "&namejp=" + namejp + "&nameen=" + nameen + "&category=" + category + "&categorywebname=" + categoryweb
                + "&jancode=" + jancode + "&productcode=" + productcode + "&quantity=" + quantity + "&price=" + price + "&amount=" + amount
                + "&origin=" + origin + "&flightcode=" + flightcode + "&tracking=" + tracking + "&producttype=" + producttype;

    //var href = "/ajax/DataBaseStorage?page=1&namejp=" + namejp + "&nameen=" + nameen + "&category=" + category + "&categorywebname=" + categoryweb
    //            + "&jancode=" + jancode + "&productcode=" + productcode + "&quantity=" + quantity + "&price=" + price + "&amount=" + amount
    //            + "&origin=" + origin + "&material=" + material + "&component=" + component + "&flightcode=" + flightcode
    //            + "&tracking=" + tracking + "&producttype=" + producttype + "&data_sort=" + data_sort;

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

//pagging jp
$(document).on("click", ".pagging-db a", function (e) {
    var href = $(this).attr("href");
    $(".se-pre-con").show(); 

    var pagesize = $("#inPageSize").val();
    var pageno = $("#inPageNo").val();
    var totalcount = $("#inTotalCount").val();
    var data_sort = $("#sort-jp").val();
    var storejp_id = $("#StoreJPId").val();
    var namejp = $("#storejp-table thead tr.filter-jp input:eq(0)").val();
    var nameen = $("#storejp-table thead tr.filter-jp input:eq(1)").val();
    var category = $("#storejp-table thead tr.filter-jp select:eq(0)").val();
    var categoryweb = $("#storejp-table thead tr.filter-jp input:eq(2)").val();
    //var image     = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
    var jancode = $("#storejp-table thead tr.filter-jp input:eq(3)").val();
    var productcode = $("#storejp-table thead tr.filter-jp input:eq(4)").val();
    var quantity = $("#storejp-table thead tr.filter-jp input:eq(5)").val();
    var price = $("#storejp-table thead tr.filter-jp input:eq(6)").val();
    var amount = $("#storejp-table thead tr.filter-jp input:eq(7)").val();
    var origin = $("#storejp-table thead tr.filter-jp select:eq(1)").val();
    var material = $("#storejp-table thead tr.filter-jp input:eq(8)").val();
    var component = $("#storejp-table thead tr.filter-jp input:eq(9)").val();
    //var componentimage = $("#storejp-table thead tr.filter-jp input:eq(11)").val();
    //var linkweb = $("#storejp-table thead tr.filter-jp input:eq(10)").val();
    var flightcode = $("#storejp-table thead tr.filter-jp input:eq(10)").val();
    var tracking = $("#storejp-table thead tr.filter-jp input:eq(11)").val();
    var producttype = $("#storejp-table thead tr.filter-jp select:eq(2)").val();

    if (href === undefined || href === null) { }
    else {
        $("#storejp-table tbody").html("");
        href = href + "&pageno=" + pageno + "&pageSize=" + pagesize + "&totalCount=" + totalcount + "&data_sort=" + data_sort
                + "&namejp=" + namejp + "&nameen=" + nameen + "&category=" + category + "&categorywebname=" + categoryweb
                + "&jancode=" + jancode + "&productcode=" + productcode + "&quantity=" + quantity + "&price=" + price + "&amount=" + amount
                + "&origin=" + origin + "&flightcode=" + flightcode + "&tracking=" + tracking + "&producttype=" + producttype;
        //href = href + "&namejp=" + namejp + "&nameen=" + nameen + "&category=" + category
        //        + "&price=" + price + "&jancode=" + jancode + "&productcode=" + productcode + "&origin=" + origin + "&material=" + material
        //        + "&component=" + component + "&data_sort=" + data_sort;
        $("#storejp-table tbody").load(href);
        
    }
    $(".se-pre-con").fadeOut();
    e.preventDefault();
});