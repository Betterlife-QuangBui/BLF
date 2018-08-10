var rakuten =  function(cart_url) {
    this.source = 'rakuten';
    this.common_tool = new CommonTool();
    this.init = function () {
        var detail = $('#detail');
        detail.css('border','1px solid red');
        detail.css('font-size','11px');
        $('.tb-rmb').remove();

        // this.alert();
        // this.warning();
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
        var price_html = '<p class="ohayoo-append  ohayoo-large" style="margin: 10px 0;">' +
            'Tỷ giá: ' + common.currency_format(common.getExchangeRate(),true)+' VNĐ / 1 JPY</p>';
        var j_str_price = $('#rakutenLimitedId_cart');

        if(j_str_price != null && j_str_price != ""){
            j_str_price.prepend("<tr><th colspan='2'>"+price_html+"</th></tr>");
        }

        var title_content = this.getDescOrigin().replace('&', ' and ');

        //common.setIsTranslateToCookie();

        common.translate_title(title_content,'title', this);

        // this.translateProperties();

        return false;
    };

    this.set_translate = function(data) {
        var common = this.common_tool;
        var _title = this.getDomTitle();
        var jan_code = this.getJanCode();
        var time_sale = this.getTimeSale();
        var time_value = common.ajaxTranslate(time_sale);
        title_append = $('.catch_copy');
        proid_append = $('.item_number_title');
        time_append = $('.time_sale');

        if(_title != null && data.title != ""){
            // _title.setAttribute("data-text",_title.textContent);
            // _title.textContent = data.title;          
            title_append.append("<div class='ohayoo-append ohayoo-small'>" + data.title + "</div>");  
            proid_append.parent().parent().parent().append("<tr><td colspan='2' class='ohayoo-append ohayoo-small'>Jan code:  " + jan_code + "</td></tr>");
            time_append.parent().parent().parent().append("<tr><td colspan='2' class='ohayoo-append ohayoo-small'>" + time_value + "</td></tr>");           
        }
    };

    this.getDescOrigin = function(){

        try{
            var _title = this.getDomDesc();

            var title_origin = _title.getAttribute("data-text");
            if(title_origin == "" || typeof title_origin == "undefined" || title_origin == null){
                title_origin = _title.textContent;
            }
            return title_origin;
        }catch(ex){
            return "";
        }

    };

    this.getDomDesc = function(){
        try{
            var _title = null;

            if (document.getElementsByClassName("catch_copy").length > 0) {
                _title =  document.getElementsByClassName("catch_copy")[0];
            }
            return _title;

        }catch(ex){
            return null;
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

            if (document.getElementsByClassName("item_name").length > 0) {
                _title =  document.getElementsByClassName("item_name")[0];
            }
            return _title;

        }catch(ex){
            return null;
        }
    };

    this.getJanCode = function(){
        try {
            var _proid = null;
            if(document.getElementsByClassName("item_number").length > 0){
                _proid =  document.getElementsByClassName("item_number")[0].textContent;
            }
        return _proid;

        }catch(ex){
            return null;
        }
    }

    this.getTimeSale = function(){
        try {
            var time_sale1 = null;
            var time_sale2 = null;

            if (document.getElementsByClassName('time_sale').length > 0) {
                time_sale1 = document.getElementsByClassName('time_sale')[0].textContent;
                time_sale2 = document.getElementsByClassName('time_sale')[1].textContent;
            }
            return time_sale1 + time_sale2;
        }catch(e) {
            return "";
        }
    };

    this.getPromotionPrice = function(site){
        try{
            var span_price = null;
            var normal_price = document.getElementById('rakutenLimitedId_cart');

            var promotion_price = document.getElementById('rakutenLimitedId_cart');

            var price = 0;
            if(promotion_price == null){
                promotion_price = normal_price;
            }

            if(promotion_price != null) {
                try{

                    if(promotion_price.getElementsByClassName('price2').length > 0) {
                        span_price = promotion_price.getElementsByClassName('price2');
                        if(span_price != null && span_price != "" && span_price != "undefined"){
                            price = span_price[0].textContent.match(/[0-9]*[\.,]?[0-9]+/g);
                            
                        }
                    }else if(promotion_price.getElementsByClassName('tb-rmb-num').length > 0){
                        span_price = promotion_price.getElementsByClassName('tb-rmb-num');
                        if(span_price != null && span_price != "" && span_price != "undefined"){
                            price = span_price[0].textContent.match(/[0-9]*[\.,]?[0-9]+/g);
                        }
                    }

                    if(!price){
                        promotion_price = normal_price;
                    }
                    if(promotion_price.getElementsByClassName('price2').length > 0) {
                        span_price = promotion_price.getElementsByClassName('price2');
                        if(span_price != null && span_price != "" && span_price != "undefined"){
                            price = span_price[0].textContent.match(/[0-9]*[\.,]?[0-9]+/g);
                        }
                    }else if(promotion_price.getElementsByClassName('tb-rmb-num').length > 0){
                        span_price = promotion_price.getElementsByClassName('tb-rmb-num');
                        if(span_price != null && span_price != "" && span_price != "undefined"){
                            price = span_price[0].textContent.match(/[0-9]*[\.,]?[0-9]+/g);
                        }
                    }

                }catch(e){
                    price = document.querySelector('meta[property="apprakuten:price"]').getAttribute("content");
                }
            }
            return this.common_tool.processPrice(price,site);
        }catch(ex){
            throw Error(ex.message+ " Line:" +ex.lineNumber + " function getPromotionPrice");
        }
    };

    this.getTaxPostage1 = function(){
        try {
            var tax_postage1 = null;

            if (document.getElementsByClassName('tax_postage').length > 0) {
                tax_postage1 = document.getElementsByClassName('tax_postage')[0].textContent;
            }
            return tax_postage1;
        }catch(e) {
            return "";
        }
    };

    this.getTaxPostage2 = function(){
        try {
            var tax_postage2 = null;

            if (document.getElementsByClassName('tax_postage').length > 0) {
                tax_postage2 = document.getElementsByClassName('tax_postage')[1].textContent;
            }
            return tax_postage2;
        }catch(e) {
            return "";
        }
    };

    this.getQuantity = function(){
        try{
            var quantity = '';
            var element = document.getElementById("units");
            if (element) {
                quantity = element.value;
            } else quantity = '';

            if (quantity == '') {
                try {
                    quantity = document.getElementsByClassName('rItemUnits')[0].value;
                } catch (e) {
                    quantity = 1;
                }
            }

            return quantity;
        }catch(ex){
            throw Error(ex.message+ " Can't get origin price function getQuantity");
        }

    };

    this.getImgLink = function() {
        var img_src = '';
        var check = document.querySelector('meta[property="og:image"]');
        if(check){
                img_src = document.querySelector('meta[property="og:image"]').getAttribute("content");
        }
        return img_src;

    };

    this.getSiteName = function() {
        var site_name = '';
        var check = document.querySelector('meta[property="og:site_name"]');
        if(check){
                site_name = document.querySelector('meta[property="og:site_name"]').getAttribute("content");
        }
        return site_name;

    };

    this.getItemId = function(){
        var item_id = '';
        var check = document.querySelector('meta[property="apprakuten:item_id"]');
        if(check){
                item_id = document.querySelector('meta[property="apprakuten:item_id"]').getAttribute("content");
        }
        return item_id;
    };

    this.getItemCode = function(){
        var item_code = '';
        var check = document.querySelector('meta[property="apprakuten:item_code"]');
        if(check){
                item_code = document.querySelector('meta[property="apprakuten:item_code"]').getAttribute("content");
        }
        return item_code;
    };

    this.getShopId = function(){
        var shop_id = '';
        var check = document.querySelector('meta[property="apprakuten:shop_id"]');
        if(check){
                shop_id = document.querySelector('meta[property="apprakuten:shop_id"]').getAttribute("content");
        }
        return shop_id;
    };

    this.getShopName = function(){
        var shop_name = '';
        var check = document.querySelector('meta[property="apprakuten:shop_name"]');
        if(check){
                shop_name = document.querySelector('meta[property="apprakuten:shop_name"]').getAttribute("content");
        }
        return shop_name;

    };

    this.getShopCode = function(){
        var shop_code = '';
        var check = document.querySelector('meta[property="apprakuten:shop_code"]');
        if(check){
                shop_code = document.querySelector('meta[property="apprakuten:shop_code"]').getAttribute("content");
        }
        return shop_code;

    };

    this.getShopUrl = function(){
        var shop_url = '';
        var check = document.querySelector('meta[property="apprakuten:shop_url"]');
        if(check){
                shop_url = document.querySelector('meta[property="apprakuten:shop_url"]').getAttribute("content");
        }
        return shop_url;

    };

    



    // Not use(Không dùng)


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
            //console.info("RAKUTEN | Không lấy được địa điểm đăng bán của sản phẩm");
            //console.warn(e.message);
        }
        return location_sale;
    };

    this.getOriginPrice = function(){
        try{
            var str_price = $('#J_StrPrice');
            var origin_price = str_price.find('.price2');

            if(origin_price == null || origin_price.length == 0){
                origin_price = str_price.find('.tb-rmb-num');
            }

            if(origin_price == null || origin_price.length == 0){
                origin_price = $('#J_priceStd').find('.tb-rmb-num');
            }

            if(origin_price == null || origin_price.length == 0){
                origin_price = $('#J_priceStd').find('.price2');
            }

            if(origin_price == null || origin_price.length == 0){
                origin_price = $('#J_StrPriceModBox').find('.price2');
            }

            if(origin_price == null || origin_price.length == 0){
                origin_price = $('#J_StrPriceModBox').find('.tb-rmb-num');
            }

            if(origin_price == null || origin_price.length == 0){
                origin_price = $('#J_PromoPrice').find('.price2');
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
        //console.log("RAKUTEN | getProperties");
        //write by vanhs | edit_time: 13/06/2015
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

};