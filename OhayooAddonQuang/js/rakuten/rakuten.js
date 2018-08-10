var rakuten =  function(cart_url) {
    this.source = 'rakuten';
    this.common_tool = new CommonTool();
    this.init = function () {
        var detail = $('#detail');
        detail.css('border','1px solid red');
        detail.css('font-size','11px');
        $('.tb-rmb').remove();

        this.alert();
        this.warning();
        this.parse();
    };

    /**
     * SITE: RAKUTEN
     */
    this.warning = function(){
        try{

            //Nếu đã tồn tại cảnh báo rồi thì thôi ko hiển thị nữa
            var len = document.querySelectorAll("._warning-on-page").length;
            if(len) return;

            var $anchor = document.querySelectorAll("#J_SKU");
            if($anchor.length){
                var elem = document.createElement("div");
                elem.className = 'block-warning-on-page-addon _warning-on-page';
                elem.textContent = 'Vui lòng chọn đầy đủ thuộc tính của sản phẩm (màu sắc, kích thước,..), tiếp đó click vào nút "Đặt hàng"';
                $anchor[0].insertBefore(elem, null);
            }

        }catch (e){
            console.warn(e.message);
        }
    };

    //RAKUTEN | Hàm hiển thị cảnh báo trên addon
    this.alert = function(){
        //console.info("alert");
        //Cảnh báo về độ tin cậy của shop
        var text = "";

        try{

            var creditLevel = -1;
            var scripts = document.querySelectorAll("script");
            for(var i = 0; i < scripts.length; i++){
                var html = scripts[i].textContent.replace(/\s/g,'');
                var res = html.search("KISSY.merge");
                if(res != -1){
                    var n = html.match(/creditLevel:"([\s\S]*?)"/i);
                    creditLevel = n[1].trim();
                    break;
                }
            }

            //Nếu ng bán có 2 kim cương trở xuống thì hiển thị cảnh báo uy tín thấp
            if(creditLevel >= 1 && creditLevel <= 7){

                text += 'Người bán này có uy tín bán hàng thấp. Quý khách nên cân nhắc trước khi đặt hàng. Vui lòng tham khảo <a href="' +  link_store_review_guidelines + '" target="_blank">đánh giá điểm uy tín tại đây.</a>';

            }

        }catch(e){
            //console.warn(e.message);
        }

        if(text){
            $("._addon-message").removeClass("hidden").find("span:eq(0)").html(text);
        }

    };

    this.parse = function () {

        var common = this.common_tool;
        $('.tb-property-type').each(function (index, value) {
            var text = $(this).text();
            $(this).text(common_tool.key_translate_lib(text));
        });

        this.common_tool.loadOptionCategory();

        var price = this.getPromotionPrice("RAKUTEN");
        var price_html = '<p style="font-size: 24px; margin-top: 15px; font-weight: bold; color: #FF6600;">' +
            'Tỉ giá: ' + common.getExchangeRate().toFixed(2) + ' VNĐ / 1 JPY</p>';
        if ($('#rakutenLimitedId_cart').size()>0) {
            $('#rakutenLimitedId_cart').prepend("<tr><td colspan='2'>" + price_html + "</td></tr>");
        }
        else {
            $('.rakutenLimitedId_cart').prepend("<tr><td colspan='2'>" + price_html + "</td></tr>");
        }

        var title_content = this.getTitleOrigin();

        //common.setIsTranslateToCookie();

        common.translate_title(title_content,'title', this);

        this.translateProperties();

        return false;
    };

    this.translateProperties = function(){
        var common = this.common_tool;
        var span_pro = $('.J_TSaleProp li span');
        if(span_pro == null || span_pro.length == 0){
            span_pro = $('.J_SKU a span');
        }
        span_pro.each(function() {
            common.translate($(this),"properties");
        });
    };

    this.getLocationSale = function(){
        var location_sale = "";
        try{
            location_sale = document.querySelectorAll("#J-From")[0].textContent;
        }catch (e){
        }
        return location_sale;
    };

    this.getPriceInput = function(){
        return $('#_price').val();
    };

    this.getPropertiesInput = function(){
        return $('#_properties').val();
    };

    this.getQuantityInput = function(){
        return $('#_quantity').val();
    };

    this.getCommentInput = function(){
        return $('._comment_item').val();
    };

    this.set_translate = function(data) {
        var _title = this.getDomTitle();
        if (_title != null && data.title != "") {
            _title.setAttribute("data-text", _title.textContent);
            console.log(_title);
            $(_title).parent().prepend("<div id='addonTitleVN' style='color:blue;font-weight:bold;font-family:tahoma;font-size:22px;'>" + data.title + "</div>");
        }
        if ($(".item_number_title") && $(".item_number")) {
            $(".item_number_title").parent().parent().parent().append("<tr  style='color:#3366CC;font-weight:bold;font-family:tahoma;font-size:20px;'><td>Jan Code: </td><td>" + $(".item_number").html() + "</td></tr>")
        }
    };

    this.getPromotionPrice = function(site){
        try{
            var span_price = null;
            var normal_price = document.getElementsByClassName('price2');
            if (!$(".price2")) {
                normal_price = document.getElementsByClassName("double_price");
            }
            var price = normal_price[0].textContent.match(/[0-9]*[\.,]?[0-9]+/g);
            price = price.toString().replace(",",".");
            return this.common_tool.processPrice(price,site);
        }catch(ex){
        }
    };

    this.getStock = function(){
        try{
            var stock_id = document.getElementById('J_EmStock');
            var stock = 99;
            if(stock_id == null || stock_id == 'undefined'){
                stock_id = document.getElementById("J_SpanStock");
            }

            if(stock_id != null && stock_id != 'undefined'){
                stock = stock_id.textContent;
                stock = parseInt(stock.replace(/[^\d.]/g, ''));
            }
        }catch(ex){
            stock = 99;
        }


        return stock;
    };

    this.getOriginPrice = function(){
        try{
            var str_price = $('#J_StrPrice');
            var origin_price = str_price.find('.tm-price');

            if(origin_price == null || origin_price.length == 0){
                origin_price = str_price.find('.tb-rmb-num');
            }

            if(origin_price == null || origin_price.length == 0){
                origin_price = $('#J_priceStd').find('.tb-rmb-num');
            }

            if(origin_price == null || origin_price.length == 0){
                origin_price = $('#J_priceStd').find('.tm-price');
            }

            if(origin_price == null || origin_price.length == 0){
                origin_price = $('#J_StrPriceModBox').find('.tm-price');
            }

            if(origin_price == null || origin_price.length == 0){
                origin_price = $('#J_StrPriceModBox').find('.tb-rmb-num');
            }

            if(origin_price == null || origin_price.length == 0){
                origin_price = $('#J_PromoPrice').find('.tm-price');
            }

            if(origin_price == null || origin_price.length == 0){
                origin_price = $('#J_PromoPrice').find('.tb-rmb-num');
            }

            var price = origin_price.text();
            price = price.match(/[0-9]*[\.,]?[0-9]+/g);

            return this.common_tool.processPrice(price);
        }catch(ex){
            throw Error(ex.message+ " Can't get origin price function getOriginPrice");
        }
    };

    this.getOuterId = function(data_value){
        try{
            var scripts = document.getElementsByTagName('script');
            var skuId = "";
            var skuMap = null;
            if(scripts.length > 0) {
                for(var script = 0; script < scripts.length; script++) {
                    if(scripts[script].innerHTML.match(/Hub\.config\.set/)) {
                        try{
                            detailJsStart();
                            skuId = Hub.config.get('sku').valItemInfo.skuMap[";"+data_value+";"].skuId;
                        }catch(e){
                            skuMap = scripts[script].innerHTML.replace(/\s/g, '').substr(scripts[script].innerHTML.replace(/\s/g, '').indexOf(data_value) , 60);
                            skuId = skuMap.substr(skuMap.indexOf('skuId') + 8, 15).match(/[0-9]+/);
                        }
                    }else if(scripts[script].innerHTML.match(/TShop\.Setup/)){
                        skuMap = scripts[script].innerHTML.replace(/\s/g, '').substr(scripts[script].innerHTML.replace(/\s/g, '').indexOf(data_value) , 60);
                        skuId = skuMap.substr(skuMap.indexOf('skuId') + 8, 15).match(/[0-9]+/);
                    }
                }
            }

            return skuId;
        }catch(ex){
            return "";
        }
    };

    this.getTitleTranslate = function(){
        try{
            var _title = this.getDomTitle();
            var title_translate = _title.textContent;
            if(title_translate == ""){
                title_translate = _title.getAttribute("data-text");
            }
            return title_translate;
        }catch(ex){
            return "";
        }

    };

    this.getTitleOrigin = function(){

        try{
            var _title = this.getDomTitle();

            var title_origin = _title.getAttribute("data-text");
            if(title_origin == "" || typeof title_origin == "undefined" || title_origin == null){
                title_origin = _title.textContent;
            }
            return title_origin;
        }catch(ex){
            return "";
        }

    };
    this.getDomTitle = function(){
        try{
            var _title = null;
            if (document.getElementsByClassName("catch_copy").length > 0) {
                _title = document.getElementsByClassName("catch_copy")[0];
            }
            return _title;
        }catch(ex){
            return null;
        }
    };

    this.getShopName = function(){
        try{
            var shop_name = '';
            if(document.getElementsByClassName('tb-seller-name').length > 0){
                shop_name = document.getElementsByClassName('tb-seller-name')[0].textContent;

                if(shop_name == '' || shop_name == null) {

                    var shop_card = document.getElementsByClassName('shop-card');
                    var data_nick = shop_card.length > 0 ? shop_card[0].getElementsByClassName('ww-light') : '';
                    shop_name = (data_nick.length > 0 ? data_nick[0].getAttribute('data-nick') : '');
                    if(shop_name == '') {
                        /* Find base info*/
                        if( document.getElementsByClassName('base-info').length > 0) {
                            for(var i =0; i < document.getElementsByClassName('base-info').length; i++) {
                                if(document.getElementsByClassName('base-info')[i].getElementsByClassName('seller').length > 0) {
                                    if(document.getElementsByClassName('base-info')[i].getElementsByClassName('seller')[0].getElementsByClassName('J_WangWang').length > 0) {
                                        shop_name = document.getElementsByClassName('base-info')[i].getElementsByClassName('seller')[0].getElementsByClassName('J_WangWang')[0].getAttribute('data-nick');
                                        break;
                                    }
                                    if(document.getElementsByClassName('base-info')[i].getElementsByClassName('seller')[0].getElementsByClassName('ww-light').length > 0) {
                                        shop_name = document.getElementsByClassName('base-info')[i].getElementsByClassName('seller')[0].getElementsByClassName('ww-light')[0].getAttribute('data-nick');
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }else if($('#J_tab_shopDetail').length > 0){
                shop_name = $('#J_tab_shopDetail').find('span').first().data('nick');
            }
            shop_name = shop_name.trim();

            if(!shop_name){
                shop_name = document.querySelectorAll(".tb-shop-name")[0].getElementsByTagName("h3")[0].getElementsByTagName("a")[0].getAttribute("title");
            }

            return shop_name;
        }catch(ex){
            return "";
        }

    };

    this.getShopId = function(){
        var shop_id = '';
        var flag = document.querySelector('meta[name="microscope-data"]');
        if(flag){
            try{
                var string = document.querySelector('meta[name="microscope-data"]').getAttribute("content");
                if(string){
                    var array = string.split(';');
                    if(array.length > 0){
                        for(var i = 0; i < array.length; i++){
                            var str = array[i];
                            str = str.trim();
                            var params = str.split('=');
                            var key = params[0];
                            var value = params[1];
                            if(key == 'shopId'){
                                shop_id = value;
                                break;
                            }
                        }
                    }
                }
            }catch(ex){
                //console.info("Không lấy được shop_id." + ex.message);
            }
        }else{
            try{
                var shop_tilte_text;
                if(document.querySelector('.shop-title-text')){
                    shop_tilte_text = document.querySelector('.shop-title-text').getAttribute("href");
                }else{
                    shop_tilte_text = document.querySelectorAll(".tb-shop-name")[0].getElementsByTagName("h3")[0].getElementsByTagName("a")[0].getAttribute("href")
                }
                shop_tilte_text = shop_tilte_text.replace("//shop", "");
                var tmp = shop_tilte_text.split('.');
                shop_id = tmp[0];
            }catch(ex){
                //console.info("Không lấy được shop_id." + ex.message);
            }
        }
        return shop_id;
    };

    this.getSellerId = function(){
        var sellerId = "";
        try{
            var dataApi = document.querySelectorAll("#J_listBuyerOnView")[0].getAttribute("data-api");
            var a = dataApi.split("?");
            var b = a[1];
            var c = b.split("&");
            if(c.length){
                for(var i = 0; i < c.length; i++){
                    if(c[i]){
                        var tmp = c[i].split("=");
                        if(tmp[0].trim() == "seller_num_id"){
                            sellerId = tmp[1];
                            break;
                        }
                    }
                }
            }

        }catch(e){
            //console.info("RAKUTEN | không lấy được thông tin sellerId");
            //console.warn(e.message);
        }

        if(!sellerId){
            try{
                sellerId = document.querySelectorAll("#J_Pine")[0].getAttribute("data-sellerid");
            }catch (e){

            }
        }

        if(!sellerId){
            try{
                var content = document.querySelectorAll("[name='microscope-data']")[0].getAttribute("content");
                var arr = content.split(";");
                for(var k = 0; k < arr.length; k++){
                    if(arr[k]){
                        var temp = arr[k].split("=");
                        if(temp[0].trim() == "userid"){
                            sellerId = temp[1].trim();
                            break;
                        }
                    }
                }
            }catch (e){

            }
        }

        //console.info("sellerId: " + sellerId);

        return sellerId;
    };

    this.getProperties = function(){
        var color_size = '';
        try{
            var selected_props = document.getElementsByClassName('J_TSaleProp');
            if(!selected_props.length){
                selected_props = document.querySelectorAll("ul.tb-cleafix");
            }
            if(selected_props.length > 0){
                for(var i = 0; i < selected_props.length; i++) {
                    var li_origin = selected_props[i].getElementsByClassName('tb-selected')[0];
                    if(li_origin){
                        var c_s = li_origin.getElementsByTagName('span')[0].textContent;
                        if(c_s) { color_size += c_s.trim() + ';'; }
                    }
                }
            }
        }catch (e){
            console.warn("RAKUTEN | getProperties: " + e.message);
        }
        return color_size;
    };

    this.getPropertiesOrigin = function(){
        try{
            var selected_props = document.getElementsByClassName('J_TSaleProp');
            var color_size = '';

            if(!((typeof selected_props !== 'object' && selected_props != "" && selected_props != null)
                || (typeof selected_props === 'object' && selected_props.length > 0))){
                selected_props = document.querySelectorAll("ul.tb-cleafix");
            }
            if(selected_props.length > 0) {
                for(var i = 0; i < selected_props.length; i++) {
                    var li_origin = selected_props[i].getElementsByClassName('tb-selected')[0];
                    if(li_origin != null){
                        var c_s = li_origin.getElementsByTagName('span')[0].getAttribute("data-text");
                        if(c_s == "" || c_s == null || typeof c_s == "undefined"){
                            c_s = li_origin.getElementsByTagName('span')[0].textContent;
                        }
                        color_size+=c_s+';';
                    }
                }
            }
            return color_size;
        }catch(ex){
            throw Error(ex.message+ " Can't get origin price function getPropertiesOrigin");
        }

    };

    this.getDataValue = function(){
        try{

            var data_value = '';
            var tb_selected = document.getElementsByClassName('tb-selected');
            for(var i=0; i<tb_selected.length;i++) {
                var v = '';
                if(tb_selected[i].className.indexOf('J_SKU') > -1) {
                    v = tb_selected[i].getAttribute('data-pv');
                }else{
                    v = tb_selected[i].getAttribute('data-value');
                }
                if(v){
                    data_value += v + ';';
                }
            }

            return data_value;

        }catch(ex){
            return "";
        }

    };

    this.getWangwang = function(){
        try{
            var wangwang = "";

            var span_wangwang = $('.tb-shop-ww .ww-light');

            if(span_wangwang != null && span_wangwang != '' && span_wangwang.length > 0){
                wangwang = span_wangwang.attr('data-nick');
            }

            if(wangwang == ''){
                span_wangwang = document.querySelectorAll("span.seller");

                if(span_wangwang == null || span_wangwang == "" || span_wangwang == "undefined" || span_wangwang.length == 0){
                    var div_wangwang = document.getElementsByClassName('slogo-extraicon');
                    if(div_wangwang != null && div_wangwang != "" && div_wangwang != "undefined" && div_wangwang.length > 0){
                        span_wangwang = div_wangwang[0].getElementsByClassName("ww-light");
                    }
                }

                if(span_wangwang == null || span_wangwang == '' || span_wangwang.length == 0){
                    span_wangwang = document.querySelectorAll("div.hd-shop-desc span.ww-light");
                }


                if(span_wangwang.length > 0){
                    var sp_wangwang = span_wangwang[0].getElementsByTagName("span");
                    if(sp_wangwang != null && sp_wangwang != '' && sp_wangwang.length == 0){
                        wangwang = decodeURIComponent(sp_wangwang[0].getAttribute('data-nick'));
                    }else{
                        wangwang = decodeURIComponent(span_wangwang[0].getAttribute('data-nick'));
                    }
                }
            }
        }catch(ex){
            wangwang = "";
        }
        return wangwang;
    };

    this.checkSelectFull = function(){
        var props = document.getElementsByClassName('J_TSaleProp');
        if(!((typeof props != 'object' && props != "" && props != null)
            || (typeof props === 'object' && props.length > 0))){

            props = document.querySelectorAll("ul.tb-cleafix");
        }
        var full = true;
        if (props.length > 0) {
            /*            kiem tra so Thuộc tính da chon cua sp*/
            var count_selected = 0;
            for (var i = 0; i < props.length; i++) {
                var selected_props = props[i].getElementsByClassName('tb-selected');
                if (selected_props != null && selected_props != 'undefined')
                    count_selected += selected_props.length;
            }
            if (count_selected < props.length) {
                full = false;
            }

        }
        return full;
    };

    this.getQuantity = function(){
        try{
            var quantity = '';
            var element = document.getElementById("J_IptAmount");
            if (element) {
                quantity = element.value;
            } else quantity = '';

            if (quantity == '') {
                try {
                    quantity = document.getElementsByClassName('mui-amount-input')[0].value;
                } catch (e) {
                    console.log(e);
                }
            }

            return quantity;
        }catch(ex){
            throw Error(ex.message+ " Can't get origin price function getQuantity");
        }

    };

    /**
     * SITE: RAKUTEN
     * @returns {*}
     */
    this.getImgLink = function() {
        try{
            var img_src = "";
            try {
                var img_obj = document.getElementById('J_ImgBooth');
                if (img_obj != null) {
                    img_src = img_obj.getAttribute("src");
                    img_src = this.common_tool.resizeImage(img_src);
                    return encodeURIComponent(img_src);
                }

                img_obj = document.getElementById('J_ThumbView');

                if(img_obj != null && img_obj != ""){
                    img_src = img_obj.getAttribute("src");
                    img_src = this.common_tool.resizeImage(img_src);
                    return encodeURIComponent(img_src);
                }

                if (document.getElementById('J_ImgBooth').tagName == "IMG") {

                    var thumbs_img_tag = document.getElementById('J_UlThumb');
                    try {
                        if (thumbs_img_tag != null) {
                            img_src = thumbs_img_tag.getElementsByTagName("img")[0].src;
                        } else {
                            img_src = document.getElementById('J_ImgBooth').src;
                        }
                    } catch (e) {
                        console.log(e);
                    }
                } else {
                    /*                   Find thumb image*/
                    var thumbs_a_tag = document.getElementById('J_UlThumb');
                    if (thumbs_a_tag != null) {
                        img_src = thumbs_a_tag.getElementsByTagName("li")[0].style.backgroundImage.replace(/^url\(["']?/, '').replace(/["']?\)$/, '');
                    } else {
                        img_src = document.getElementById('J_ImgBooth').style.backgroundImage.replace(/^url\(["']?/, '').replace(/["']?\)$/, '');
                    }
                }

            } catch (e) {
                console.log("Image not found!" + e);
            }

            img_src = this.common_tool.resizeImage(img_src);
            return encodeURIComponent(img_src);
        }catch(ex){
            return "";
        }

    };

    this.getItemID = function(){

        var item_id = 0;
        var home = window.location.href;
        var dom;
        //1. Lấy trên dom
        if(!item_id){
            try{

                dom = document.getElementsByName("item_id");
                if(dom.length){
                    item_id = dom[0].value;
                }else{
                    item_id = document.getElementsByName("item_id_num")[0].value;
                }

            }catch(e){

            }
        }
        //2. Lấy trên link dạng http://www.rakuten.com/item/521828428176.htm?*
        if(!item_id){
            try{
                var temp = home.split('.htm')[0];
                item_id = temp.split('item/')[1];
            }catch(e){

            }
        }

        //3. Lấy params trên url dạng ?spm=a312a.7728556.2015080705.14.fbm100&amp;id=521985720964&amp;scm=1007.12006.12548.i522577577280&amp;pvid=34d586a7-1587-44bb-8b07-1fb66252488c
        if(!item_id){
            try{
                item_id = this.common_tool.getParamsUrl('id', home);
            }catch(e){

            }
        }

        //console.info("item_id: " + item_id);
        return item_id;

    };
};