var service_host,
    add_to_cart_url,
    cart_url,
    catalog_scalar_url,
    isTranslate,
    link_detail_cart,
    add_to_favorite_url,
    button_add_to_cart_url;

//var site_using_https = false;
var site_using_https = true;

chrome.storage.sync.get({
    domain: site_using_https ? 'https://seudo.vn/' : 'http://seudo.vn/',
    is_translate: true
}, function(items) {

    var value = items.domain;

    if(site_using_https && items.domain.search("https") == -1){
        value = items.domain.replace("http", "https");
    }

    if(!site_using_https && items.domain.search("https") != -1){
        value = items.domain.replace("https", "http");
    }

    chrome.storage.sync.set({
        domain: value
    }, function() {

    });

    service_host = value;

    //============================================================

    isTranslate = items.is_translate;
    add_to_cart_url = service_host+'cart/add_cart_v2';
    cart_url = service_host+'checkout.html';
    link_detail_cart = service_host + "gio-hang";
    add_to_favorite_url = service_host + "i/favoriteLink/saveLink";
    button_add_to_cart_url = service_host + "images/icon-bkv1-small.png";

    var css_url = '';
    switch (items.domain){
        case 'http://cn.seudo.vn/':
            catalog_scalar_url = 'http://cnlogistic.seudo.vn/api/catalog/scalar';
            css_url = 'css/cn_main.css';
            break;
        default :
            css_url = 'css/main.css';
            catalog_scalar_url = 'http://logistic.seudo.vn/api/catalog/scalar';
            break;
    }

    if(css_url){
        var NewStyles = document.createElement("link");
        NewStyles.rel = "stylesheet";
        NewStyles.type = "text/css";
        NewStyles.href = chrome.extension.getURL(css_url);
        document.head.appendChild(NewStyles);
    }
});

var translate_url = 'http://apiohayoo.bannhac.com/api/Transalte';

//var translate_url = 'http://seudo.vn/i/goodies_util/translate';
var isUsingSetting = false;
var translate_keyword_url = "";
var version_tool = "4.11.73";
var exchange_rate;
var link_store_review_guidelines = "http://hotro.seudo.vn/post/huong-dan-cach-thuc-danh-gia-uy-tin-nha-cung-cap-71";//SEUDO
var web_service_url = "http://seudo.vn/i/";
var exchange_rate_url = 'http://seudo.vn/cart/GetExchangeRate';