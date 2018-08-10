$(document).on("click", ".storejp-pagging a", function (e) {
    $(".wasehouse-info").html("");
    var href = $(this).attr("href");
    if (href === undefined || href === null) { }
    else {
        $(".storejp-content").html("");
        $(".storejp-content").load($(this).attr("href"));
    }
    $('html, body').animate({
        scrollTop: $(".page-content").offset().top
    }, 2000);
    e.preventDefault();
});
$(document).on("change", ".storejp-select", function (e) {
    key = $("#storejp_search_key").val()
    $(".wasehouse-info").html("");
    $(".storejp-content").html("");
    $(".storejp-content").load("/ajax/storejp?page=1&key=" + key + "&sort=" + $(this).val());
    return false;
    e.preventDefault();
});
$(document).on("keyup", "#storejp_search_key", function (e) {
    key = $(this).val();
    sort = $("#StatusId").val();
    $(".wasehouse-info").html("");
    $(".storejp-content").html("");
    $(".storejp-content").load("/ajax/storejp?page=1&key=" + key + "&sort=" + sort);
    return false;
    e.preventDefault();
});
//$(document).on("click", ".storejp_add", function (e) {
//    $(".storejp_add_body").html(""); $(".shipment_edit_body").html("");
//    $(".storejp_add_body").load($(this).attr("href"));
//    return false;
//    e.preventDefault();
//});
$(document).on("click", ".close-shipment", function (e) {
    $(".storejp_add_body").html(""); $(".shipment_edit_body").html("");
    return false;
    e.preventDefault();
});
$(document).on("click", ".package_item-edit", function (e) {
    $(".storejp_add_body").html(""); $(".shipment_edit_body").html("");
    $("tr.add-item,tr.filter-jp-action").hide();
    $("tr.filter-jp").show(); $("#btn-save-add").attr("disabled", "disabled");
    $(this).parent().parent().parent().parent().find(".shipment_edit_body").load($(this).attr("href"));
    return false;
});
$(document).on("click", "input[name=size]", function (e) {
    $(".size input[type=text]").attr('disabled', 'disabled');
    $(".size select").attr('disabled', 'disabled');
    $(this).parent().next().find("select,input[type=text]").removeAttr('disabled');
    $("#SizeTableId").select2("val", " ");
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
        var FR = new FileReader();
        FR.onload = function (e) {
            $("#img-review img").attr("src", e.target.result);
            //$("#ImageBase64").val(e.target.result)
        };
        FR.readAsDataURL(this.files[0]);
    }
});
$(document).on("click", "div.storejp-box-item", function (e) {
    location.href = $(this).attr("data-href");
});
//detail
$(document).on("change", "#storejp-detail-action", function (e) {
    $("div.hiden").css("display", "none");
    $(".table_insert div[data-id='" + $(this).val() + "']").css("display", "inline-block");
});
//update status
$(document).on("click", ".btn-update-status", function (e) {
    ModelId = $("#ModelId").val();
    status = $("tr.statusId #StatusId").val();
    $.ajax({
        method: 'POST', async: false,
        url: "/StorageJP/UpdateStatus",
        data: { id: ModelId, status: status },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Cập nhật liệu thành công', "1");
                location.reload();
            }
            else {
                notify('error', "Cập nhật liệu thất bại", "2");
                location.reload();
            }
        }
    });
});
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
//thuc hien action tren form deatil
$(document).on("click", ".btn-action-storejp", function (e) {
    ids = $("#storejp-detail-id").val();
    option = $("#storejp-detail-action").val();
    if (option == "-1") return;
    MadeIn = $(".hiden #MadeIn").val();
    Category = $(".hiden #CategoryId").val();
    trackingTranfer = $(".hiden #TrackingCode").val();
    quantity = $(".hiden input[name='quantity']").val();
    item_quantity = $(".hiden input[name='item-quantity']").val();
    price = $(".hiden input[name='Price']").val();
    component = $(".hiden textarea[name='Component']").val();
    material = $(".hiden textarea[name='Material']").val();
    jancode = $(".hiden input[name='jancode']").val();
    nameJP = $(".hiden textarea[name='NameJP']").val();
    switch (option) {
        case "0":
            //delete
            deleteItems(ids)
            break;
        case "1":
            UpdateMadeIn(ids, MadeIn)
            break;
        case "2":
            UpdateComponentMaterialJancode(ids, item_quantity, "2")
            break;
        case "3":
            UpdateComponentMaterialJancode(ids, price, "3")
            break;
        case "3.1":
            UpdateComponentMaterialJancode(ids, Category, "3.1")
            break;
        case "4":
            SplitPackage(ids, trackingTranfer, quantity)
            break;
        case "5":
            UpdateComponentMaterialJancode(ids, component, "5");
            break;
        case "6":
            UpdateComponentMaterialJancode(ids, material, "6");
            break;
        case "7":
            UpdateComponentMaterialJancode(ids, jancode, 7);
            break;
        case "8":
            UpdateComponentMaterialJancode(ids, nameJP, 8)
            break;
    }
});
$(document).on("click", ".btn-cancel-action-storejp", function (e) {
    $("#storejp-detail-action").val(0);
    $("div.hiden").css("display", "none");
    $("div.hiden span").css("display", "none");
    $(".table_insert").find("input,button,textarea,select").attr("disabled", "disabled");
    $(".hiden #MadeIn").val("108");
    $(".hiden #CategoryId").val("1");
    $(".hiden #TrackingCode").val("");
    $(".hiden input[name='quantity']").val("1");
    $(".hiden input[name='Price']").val("1");
    $(".hiden input[name='item-quantity']").val("1");
    $(".hiden textarea[name='Component']").val("");
    $(".hiden textarea[name='Material']").val("");
    $(".hiden textarea[name='NameJP']").val("");
    $(".hiden input[name='jancode']").val("");
    $("#storejp-table input[type='checkbox']").prop('checked', false);
});
$(document).on("change", ".table_insert #TrackingCode", function (e) {
    if ($(this).val() == "21") {
        $(".table_insert input[name=quantity]").attr("readonly", "readonly");
    }
    else {
        $(".table_insert input[name=quantity]").removeAttr("readonly");
    }
});
function deleteItems(ids) {
    $.ajax({
        method: 'POST', async: false,
        url: "/StorageItemJP/DeleteItems",
        data: { ids: ids },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Xóa dữ liệu thành công', "1");
                location.reload();
            }
            else {
                notify('error', "Xóa dữ liệu thất bại", "2");
                location.reload();
            }
        }
    });
}
function UpdateMadeIn(ids, MadeIn) {
    $.ajax({
        method: 'POST', async: false,
        url: "/StorageItemJP/UpdateMadeIn",
        data: { ids: ids, MadeIn: MadeIn },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Cập nhật liệu thành công', "1");
                location.reload();
            }
            else {
                notify('error', "Cập nhật liệu thất bại", "2");
                location.reload();
            }
        }
    });
}
function SplitPackage(ids, trackingTranfer, quantity) {
    $.ajax({
        method: 'POST', async: false,
        url: "/StorageItemJP/SplitPackage",
        data: { ids: ids, trackingTranfer: trackingTranfer, quantity: quantity },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Cập nhật liệu thành công', "1");
                location.reload();
            }
            else {
                notify('error', "Cập nhật liệu thất bại", "2");
                location.reload();
            }
        }
    });
}
function UpdateComponentMaterialJancode(ids, data, option) {
    $.ajax({
        method: 'POST', async: false,
        url: "/StorageItemJP/UpdateComponentMaterialJancode",
        data: { ids: ids, data: data, option: option },
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Cập nhật liệu thành công', "1");
                location.reload();
            }
            else {
                notify('error', "Cập nhật liệu thất bại", "2");
                location.reload();
            }
        }
    });
}
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


//Import shipment
$(document).on("click", ".Import-StorageJP-DB", function (e) {

    $(".import-storejp").load($(this).attr("href"));
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
$(document).on("click", ".close-package", function (e) {
    $(".import-storejp").html("");
    return false;
    e.preventDefault();
});
$(document).on("click", ".delete-one-item", function (e) {
    var id = $(this).attr("data-id");
    deleteItems(id)
    return false;
    e.preventDefault();
});
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
            $(el).val($(el).attr("data-value")).trigger("change");
        });
        //restore image
        $(e).find("img").each(function (j, el) {
            $(el).attr("src",$(el).attr("data-value"));
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
    var tr = $(this).parent().parent().parent();
    amount = parseFloat($(tr).find("input[name=PriceTax]").val()) * parseFloat($(tr).find("input[name=Quantity]").val());
    $(tr).find(".amount").html(amount);
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
        url: "/StorageItemJP/UpdateItemProduct",
        data: JSON.stringify(json),
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            if (result.status == true) {
                notify('white', 'Cập nhật liệu thành công', "1");
                $(tr).find(".se-pre-con-crud").hide();
                $(tr).find(".se-pre-con-crud").css("display", "none");
                //lay tong loi

                //remove background
                NameJP = json["NameJP"]; JanCode = json["JanCode"];
                NameEN = json["NameEN"]; ImageBase64 = json["ImageBase64"];
                MadeIn = json["MadeIn"];
                //remove JanCode
                $(tr).find("input[name=JanCode]").attr("data-value", JanCode);
                if (JanCode == "") {
                    $(tr).find("input[name=JanCode]").parent().attr("class", "center");
                }
                else {
                    if (JanCode.toString().length != 13) {
                        $(tr).find("input[name=JanCode]").parent().attr("class", "center item-deny");
                    }
                    else if (JanCode.toString().indexOf('4') != 0) {
                        $(tr).find("input[name=JanCode]").parent().attr("class", "center item-deny");
                    }
                    else {
                        $(tr).find("input[name=JanCode]").parent().attr("class", "center");
                        if (MadeIn != "" && MadeIn == "108") {
                            if (JanCode.toString().indexOf('450') != 0)
                                if (JanCode.toString().indexOf('451') != 0)
                                    if (JanCode.toString().indexOf('452') != 0)
                                        if (JanCode.toString().indexOf('453') != 0)
                                            if (JanCode.toString().indexOf('454') != 0)
                                                if (JanCode.toString().indexOf('455') != 0)
                                                    if (JanCode.toString().indexOf('456') != 0)
                                                        if (JanCode.toString().indexOf('457') != 0)
                                                            if (JanCode.toString().indexOf('458') != 0)
                                                                if (JanCode.toString().indexOf('459') != 0)
                                                                    if (JanCode.toString().indexOf('490') != 0)
                                                                        if (JanCode.toString().indexOf('491') != 0)
                                                                            if (JanCode.toString().indexOf('492') != 0)
                                                                                if (JanCode.toString().indexOf('493') != 0)
                                                                                    if (JanCode.toString().indexOf('494') != 0)
                                                                                        if (JanCode.toString().indexOf('495') != 0)
                                                                                            if (JanCode.toString().indexOf('496') != 0)
                                                                                                if (JanCode.toString().indexOf('497') != 0)
                                                                                                    if (JanCode.toString().indexOf('498') != 0)
                                                                                                            if (JanCode.toString().indexOf('499') != 0)
                                                                                                                $(tr).find("input[name=JanCode]").parent().attr("class", "center item-deny");
                        }
                    }
                }
                //remove namjp & en
                var arrayName = "ガス,スプレー,オイル,バッテリー,漂白,ペイント,gas,spray,oil,battery,bleach,paint".split(',');
                var checkName = false;
                for (var i = 0; i < arrayName.length; i++) {
                    if (NameJP.toString().toLowerCase().indexOf(arrayName[i]) >= 0) {
                        checkName = true;
                        break;
                    }
                }
                $(tr).find("textarea[name=NameJP]").attr("data-value", NameJP);
                if (checkName == true || NameJP == "") {
                    $(tr).find("textarea[name=NameJP]").parent().attr("class", "center item-deny");
                    checkName = false;
                }
                else {
                    $(tr).find("textarea[name=NameJP]").parent().attr("class", "center");
                }
                //remove namjp & en
                for (var i = 0; i < arrayName.length; i++) {
                    if (NameEN.toString().toLowerCase().indexOf(arrayName[i]) >= 0) {
                        checkName = true;
                        break;
                    }
                }
                $(tr).find("textarea[name=NameEN]").attr("data-value", NameEN);
                if (checkName == true || NameEN == "") {
                    $(tr).find("textarea[name=NameEN]").parent().attr("class", "center item-deny");
                    checkName = false;
                }
                else {
                    $(tr).find("textarea[name=NameEN]").parent().attr("class", "center");
                }
                //image
                $(tr).find("input[name=ImageBase64]").attr("data-value", ImageBase64);
                if (ImageBase64 != "") {
                    $(tr).find("input[name=ImageBase64]").parent().attr("class", "center");
                    $(tr).find("img.ImageBase64").attr("data-value", ImageBase64);
                }
                else {
                    $(tr).find("input[name=ImageBase64]").parent().attr("class", "center item-deny");
                }

                Quantity = json["Quantity"]; PriceTax = json["PriceTax"];
                //quantity
                $(tr).find("input[name=Quantity]").attr("data-value", Quantity);
                if (Quantity == "" || parseFloat(Quantity) == 0) {
                    $(tr).find("input[name=Quantity]").parent().attr("class", "detail-right item-deny");
                }
                else {
                    $(tr).find("input[name=Quantity]").parent().attr("class", "detail-right");
                }
                //price
                $(tr).find("input[name=PriceTax]").attr("data-value", PriceTax);
                if (parseFloat(PriceTax) >= 50) {
                    $(tr).find("input[name=PriceTax]").parent().attr("class", "detail-right");
                }
                else {
                    $(tr).find("input[name=PriceTax]").parent().attr("class", "detail-right item-deny");
                }

                //category & made in
                CategoryId = json["CategoryId"];
                $(tr).find("select[name=CategoryId]").attr("data-value", CategoryId);
                if (CategoryId != "") {
                    $(tr).find("select[name=CategoryId]").parent().attr("class", "detail-right");
                }
                else {
                    $(tr).find("select[name=CategoryId]").parent().attr("class", "detail-right item-deny");
                }
                $(tr).find("select[name=MadeIn]").attr("data-value", MadeIn);
                if (MadeIn != "") {
                    $(tr).find("select[name=MadeIn]").parent().attr("class", "detail-right");
                }
                else {
                    $(tr).find("select[name=MadeIn]").parent().attr("class", "detail-right item-deny");
                }
                //link website
                LinkWeb = json["LinkWeb"];
                $(tr).find("input[name=LinkWeb]").attr("data-value", LinkWeb);
                if (LinkWeb != "") {
                    $(tr).find("input[name=LinkWeb]").parent().attr("class", "center");
                }
                else {
                    $(tr).find("input[name=LinkWeb]").parent().attr("class", "center item-deny");
                }
                //hien thi lai so loi

                var JanCode_error = 0; var NameJP_error = 0;
                var NameEN_error = 0; var CategoryId_error = 0; var ImageBase64_error = 0; var Quantity_error = 0;
                var PriceTax_error = 0; var MadeIn_error = 0; var LinkWeb_error = 0;
                $("#storejp-table tbody tr").each(function (i, e) {
                    if (($(e).find(" td:eq(2)").attr("class")) != undefined) {
                        if ($(e).find(" td:eq(2)").attr("class").indexOf('item-deny') >= 0) {
                            NameJP_error = NameJP_error + 1;
                        }
                    }
                });
                $("#storejp-table tbody tr").each(function (i, e) {
                    if (($(e).find(" td:eq(4)").attr("class")) != undefined) {
                        if ($(e).find(" td:eq(4)").attr("class").indexOf('item-deny') >= 0) {
                            NameEN_error = NameEN_error + 1;
                        }
                    }
                });
                $("#storejp-table tbody tr").each(function (i, e) {
                    if (($(e).find(" td:eq(5)").attr("class")) != undefined) {
                        if ($(e).find(" td:eq(5)").attr("class").indexOf('item-deny') >= 0) {
                            CategoryId_error = CategoryId_error + 1;
                        }
                    }
                });
                $("#storejp-table tbody tr").each(function (i, e) {
                    if (($(e).find(" td:eq(7)").attr("class")) != undefined) {
                        if ($(e).find(" td:eq(7)").attr("class").indexOf('item-deny') >= 0) {
                            ImageBase64_error = ImageBase64_error + 1;
                        }
                    }
                });
                $("#storejp-table tbody tr").each(function (i, e) {
                    if (($(e).find(" td:eq(8)").attr("class")) != undefined) {
                        if ($(e).find(" td:eq(8)").attr("class").indexOf('item-deny') >= 0) {
                            JanCode_error = JanCode_error + 1;
                        }
                    }
                });
                $("#storejp-table tbody tr").each(function (i, e) {
                    if (($(e).find(" td:eq(10)").attr("class")) != undefined) {
                        if ($(e).find(" td:eq(10)").attr("class").indexOf('item-deny') >= 0) {
                            Quantity_error = Quantity_error + 1;
                        }
                    }
                });
                $("#storejp-table tbody tr").each(function (i, e) {
                    if (($(e).find(" td:eq(11)").attr("class")) != undefined) {
                        if ($(e).find(" td:eq(11)").attr("class").indexOf('item-deny') >= 0) {
                            PriceTax_error = PriceTax_error + 1;
                        }
                    }
                });
                $("#storejp-table tbody tr").each(function (i, e) {
                    if (($(e).find(" td:eq(13)").attr("class")) != undefined) {
                        if ($(e).find(" td:eq(13)").attr("class").indexOf('item-deny') >= 0) {
                            MadeIn_error = MadeIn_error + 1;
                        }
                    }
                });
                $("#storejp-table tbody tr").each(function (i, e) {
                    if (($(e).find(" td:eq(117)").attr("class")) != undefined) {
                        if ($(e).find(" td:eq(17)").attr("class").indexOf('item-deny') >= 0) {
                            LinkWeb_error = LinkWeb_error + 1;
                        }
                    }
                });
                sum_error = NameJP_error + NameEN_error + CategoryId_error + ImageBase64_error + JanCode_error + Quantity_error + PriceTax_error + MadeIn_error + LinkWeb_error
                $(".sum-error").html(sum_error); $(".NameJP-error").html(NameJP_error);
                $(".NameEN-error").html(NameEN_error); $(".CategoryId-error").html(CategoryId_error);
                $(".ImageBase64-error").html(ImageBase64_error); $(".JanCode-error").html(JanCode_error);
                $(".Quantity-error").html(Quantity_error); $(".PriceTax-error").html(PriceTax_error);
                $(".MadeIn-error").html(MadeIn_error); $(".LinkWeb-error").html(LinkWeb_error);
            }
            else {
                $(tr).find(".se-pre-con-crud").hide();
                notify('error', "Cập nhật dữ liệu thất bại", "2");
            }

        }
    });
    return false;
    e.preventDefault();
});
$(document).on("click", "#storejp-table tbody .ImageBase64Image", function (e) {
    $(this).parent().next().click();
    //var url = $(this).attr("data-url");
    //window.open(url, '_blank');
    return false;
    e.preventDefault();
});
//$(document).on("dblclick", "#storejp-table tbody .ImageBase64Image", function (e) {
//    $(this).parent().next().click();
//    return false;
//    e.preventDefault();
//});

$(document).on("change", "#storejp-table tbody input[name=fileImageBase64]", function (e) {
    id = $(this).attr("data-id");
    if (this.files && this.files[0]) {
        var filename = $(this).val().split('\\').pop();
        var FR = new FileReader(); 
        FR.onload = function (e) {
            $(".ImageBase64Image[data-id='" + id + "']").attr("src", e.target.result);
            $("input[name=ImageBase64][data-id='" + id + "']").val(e.target.result)
        };
        FR.readAsDataURL(this.files[0]);
    }
});
$(document).on("click", "#storejp-table tbody .ComponentImage", function (e) {
    $(this).next().click();
    return false;
    e.preventDefault();
});
$(document).on("change", "#storejp-table tbody input[name=fileComponentImage]", function (e) {
    id = $(this).attr("data-id");
    if (this.files && this.files[0]) {
        var FR = new FileReader();
        FR.onload = function (e) {
            $(".ComponentImage[data-id='" + id + "']").attr("src", e.target.result);
            $("input[name=ComponentImage][data-id='" + id + "']").val(e.target.result)
        };
        FR.readAsDataURL(this.files[0]);
    }
});
//import all
$(document).on("click", ".shipment_item-ixport", function (e) {
    $(".storejp_add_body,.shipment_edit_body").html("");
    $(".storejp_add_body").load($(this).attr("href"));
    return false;
    e.preventDefault();
});
$(document).on("click", ".close-package", function (e) {
    $(".storejp_add_body,.shipment_edit_body").html("");
    return false;
    e.preventDefault();
});
$(document).on("change", "#fileImport-full", function (e) {

    if ($(this).val().split('\\').pop() != "") {
        $(".btn-action-import-full").show();
        $(".btn-action-import-full").css("display", "inline-block !important");
    }
});
$(document).on("click", ".btn-import-full", function (e) {

    $("#fileImport-full").click();
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
            if (i == 0 || i == 1 || i == 2 || i == 3 || i == 5 || i == 7 || i == 8 || i == 9 || i == 10 || i == 11 || i == 12 || i == 13 || i == 14) {
                $(this).prop('checked', true);
            }
        }
        else {
            $("tr.main-tracking").show()
            if (i == 0 || i == 2 || i == 3 || i == 5 || i == 7 || i == 8 || i == 9 || i == 10 || i == 11 || i == 12 || i == 13 || i == 14) {
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
$(document).on("click", ".shipment_item-edit-child", function (e) {
    $(".shipment_edit_body").html("");
    $(".shipment_edit_body").load($(this).attr("href"));
    return false;
});
$(document).on("click", "input[name=displaytype]", function (e) {
    id = $(this).attr("id");
    if (id == "col-List") {
        $("tr.main-tracking").hide()
        $(".hiden-column input[type=checkbox]:eq(1)").prop('checked', true);
        $(".hiden-column input[type=checkbox]:eq(1)").attr('disabled', "disabled");
    }
    else {
        $("tr.main-tracking").show()
        $(".hiden-column input[type=checkbox]:eq(1)").prop('checked', false);
        $(".hiden-column input[type=checkbox]:eq(1)").removeAttr('disabled');
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
$(document).on("keyup", ".filter-jp input", function (e) {
    var index = $(this).parent().attr("data-index");
    value = $(this).val();
    $(".filter-jp input,.filter-jp select").val("");
    $(this).val(value);
    $("#storejp-table tbody tr.product-item").each(function (i, e) {
        flag = false;
        $(e).find("td").each(function (i, td) {
            if (i == index) {
                value_row = $(td).find("textarea,input,select").val();
                if (value_row.toString().indexOf(value) >= 0) {
                    flag = true;
                }
            }
        })
        if (flag == false) {
            $(e).hide();
        }
        else {
            $(e).show();
        }
    });
});
$(document).on("change", ".filter-jp select", function (e) {
    var index = $(this).parent().attr("data-index");
    value = $(this).val();
    $(".filter-jp input,.filter-jp select").val("");
    $(this).val(value);
    $("#storejp-table tbody tr.product-item").each(function (i, e) {
        flag = false;
        $(e).find("td").each(function (i, td) {
            if (i == index) {
                value_row = $(td).find("textarea,input,select").val();
                if (value_row.toString().indexOf(value) >= 0) {
                    flag = true;
                }
            }
        })
        if (flag == false) {
            $(e).hide();
        }
        else {
            $(e).show();
        }
    });
});


$(document).on("dblclick", "textarea[name='NameJP']", function (e) {
    var url = $(this).attr("data-url");
    window.open(url, '_blank');
});
//active edit form
RequestId = $("#RequestId").val();
if (RequestId != "") {
    href = $(".package_item-edit[data-id='" + RequestId + "']").attr("href");
    href = href + "?actionlink=Package";
    $(".package_item-edit[data-id='" + RequestId + "']").attr("href", href);
    $(".package_item-edit[data-id='" + RequestId + "']").click();
}
