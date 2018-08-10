//region -- LocalStorage --
var LocalStorage = {
    set: function (key, value) {
        localStorage.setItem(key, JSON.stringify(value));
    },
    get: function(key) {
        var saved = localStorage.getItem(key);
        saved = JSON.parse(saved);
        return saved;
    },
    remove: function(key){
        if(key){
            localStorage.removeItem(key);
        }
    }
};
//endregion

var notify_read = [];

chrome.runtime.onMessage.addListener(
    function(request, sender, sendResponse) {
        switch (request.action)
        {
            case "test":
                sendResponse('');
                break;
            case "getExchangeRate":
            case "addToCart":
            case "addToFavorite":
            case "getCategory":
            case "translate":
                getUrls(request, sender, sendResponse);
                break;
            case "setTranslateValue":
                setTranslateValue(request, sender, sendResponse);
                break;
            case "getTranslateValue":
                getTranslateValue(request, sender, sendResponse);
                break;
            default :
                //todo
                break;

        }
    }
);

function setTranslateValue(request, sender, sendResponse){
    var value = request.value;
    LocalStorage.set("translate", value);

    chrome.tabs.sendMessage(sender.tab.id, { action: request.callback }, function(response) {

    });
}

function getTranslateValue(request, sender, sendResponse){
    var value = LocalStorage.get("translate") == undefined ? 0 : LocalStorage.get("translate");

    chrome.tabs.sendMessage(sender.tab.id, { action: request.callback, value: value }, function(response) {

    });
}

function getUrls(request, sender, sendResponse){
    //var resp = sendResponse;
    $.ajax({
        url: request.url,
        data: request.data == undefined ? {} : request.data,
        method: request.method == undefined ? 'GET' : request.method,
        contentType: 'application/x-www-form-urlencoded',
        xhrFields: {
            withCredentials: true
        },
        headers: {'X-Requested-With': 'XMLHttpRequest'},
        success: function(d){
            chrome.tabs.sendMessage(sender.tab.id, { action: request.callback, response: d }, function(response) {

            });
        },
        error: function(){
            chrome.tabs.sendMessage(sender.tab.id, { action: request.callback }, function(response) {

            });
        }
    });

}

var service, last_sync_time, user_id;
var time_sync = 1000 * 60 * 5;//5 phut
var id_group_unread_notify = 0;
var number_group_notify = 2;
var prefix = "addon_notify_";

LocalStorage.set(prefix + "is_login", false);

var Notify = {
    getListNotify: function(){
        last_sync_time = LocalStorage.get(prefix + "last_sync_time") == null ? "" : LocalStorage.get(prefix + "last_sync_time");
        Common.request({
            url: web_service_url + "CustomerNotification/GetAddonNotificationByUser",
            type: 'GET',
            data: { last_sync_time: last_sync_time }
        }).done(function (response) {
            if(response.type){

                var userInfo = response.user_info;
                var totalUnreadNotify = response.total_unread_notify;
                var arrayId = LocalStorage.get(prefix + "ids") == null ? [] : LocalStorage.get(prefix + "ids").split(",");
                service = response.service;

                if(userInfo){
                    user_id = parseInt(userInfo.user_id);
                }

                Notify.clearDataWhenChangeUser();

                if(response.data){

                    $.each(response.data, function(idx, item){
                        if(item.id && $.inArray(item.id, arrayId) == -1){
                            arrayId.push(item.id);
                        }

                        if(!item.isConsumed
                            && totalUnreadNotify <= number_group_notify){
                            Notify.createNotifyItem(item);
                        }

                        item.icon = Notify.renderIcon(item.businessType);
                        LocalStorage.set(prefix + "item_" + item.id, item);
                    });

                }

                if(totalUnreadNotify){
                    Notify.createNotifyGroup(totalUnreadNotify);

                    chrome.browserAction.setBadgeText({
                        text: '' + totalUnreadNotify
                    });
                }

                LocalStorage.set(prefix + "last_sync_time", response.last_sync_time);
                LocalStorage.set(prefix + "ids", arrayId.join(","));
                LocalStorage.set(prefix + "current_user_id", user_id);
                LocalStorage.set(prefix + "is_login", true);
            }

        }).fail(function (jqXHR, textStatus, errorThrown) {
            user_id = null;
            Notify.clearDataWhenChangeUser();
        });
    },
    createNotifyGroup: function(totalUnreadNotify){

        if(totalUnreadNotify > number_group_notify){
            id_group_unread_notify++;
            var id = prefix + user_id + "_group_" + id_group_unread_notify;

            var option = {
                type: 'basic',
                iconUrl: Common.getIconWebservice(service),
                title: "",
                message: "Bạn có " + totalUnreadNotify + " thông báo mới",
                priority: 0
            };

            chrome.notifications.create('notify' + id, option, function(id) {
            });
        }

    },

    createNotifyItem: function(item){

        var id = prefix + user_id + "_" + item.id;
        if(notify_read[ id ] == undefined){
            notify_read[ id ] = 1;
            var option = {
                type: 'basic',
                iconUrl: Common.getIconWebservice(service),
                title: item.isBefore12Hours ? $.timeago(item.time) : item.time,
                message: Common.convertingHTMLToPlainText(item.message),
                priority: 0
            };
            chrome.notifications.create('notify' + id, option, function(id) {
            });
        }
    },

    renderIcon: function(businessType){
        var icon = "";
        switch (businessType){
            case "COMPLAINT":
                icon = "fa fa-frown-o";
                break;
            case "CHAT":
                icon = "fa fa-comments-o";
                break;
            case "ORDER":
                icon = "fa fa-inbox";
                break;
            case "FINANCE":
                icon = "fa fa-money";
                break;
            default :
                icon = "fa fa-inbox";
                break;
        }
        return icon;
    },

    clearDataWhenChangeUser: function(){

        try{

            var localCurrentUserId = LocalStorage.get(prefix + "current_user_id") == null ? 0
                : parseInt(LocalStorage.get(prefix + "current_user_id"));

            if(user_id != localCurrentUserId){

                var ids = LocalStorage.get(prefix + "ids");
                if(ids){
                    $.each(ids.split(","), function(idx, id){
                        if(id){
                            LocalStorage.remove(prefix + "item_" + id);
                        }
                    });
                }
                LocalStorage.remove(prefix + "current_user_id");
                LocalStorage.remove(prefix + "is_login");
                LocalStorage.remove(prefix + "ids");
                LocalStorage.remove(prefix + "last_sync_time");

                chrome.browserAction.setBadgeText({
                    text: ''
                });
            }

        }catch (e){

        }

        return true;
    }

};

var Common = {
    request: function (params) {
        return $.ajax({
            url: params.url,
            type: params.type == undefined ? 'GET' : params.type,
            data: params.data == undefined ? {} : params.data
        });
    },

    getIconWebservice: function(service){
        var iconUrl = '';
        if(service){
            switch (service.toUpperCase()){
                case 'SEUDO':
                    iconUrl = 'images/logoSeuDo.png';
                    break;
                case 'ORDERHANG':
                    iconUrl = 'images/logoOrderHang.png';
                    break;
                default :
                    iconUrl = 'images/favicon.ico';
                    break;
            }
        }
        return iconUrl;
    },

    getCurrentDateTime: function() {
        var now     = new Date();
        var year    = now.getFullYear();
        var month   = now.getMonth()+1;
        var day     = now.getDate();
        var hour    = now.getHours();
        var minute  = now.getMinutes();
        var second  = now.getSeconds();
        if(month.toString().length == 1) {
            month = '0'+month;
        }
        if(day.toString().length == 1) {
            day = '0'+day;
        }
        if(hour.toString().length == 1) {
            hour = '0'+hour;
        }
        if(minute.toString().length == 1) {
            minute = '0'+minute;
        }
        if(second.toString().length == 1) {
            second = '0'+second;
        }
        var dateTime = year + '-' + month + '-' + day + ' ' + hour + ':' + minute + ':' + second;
        return dateTime;
    },

    convertingHTMLToPlainText: function(string){
        var s = '';
        if(string){
            string = string.replace(/<br>/gi, "\n");
            string = string.replace(/<p.*>/gi, "\n");
            string = string.replace(/<a.*href="(.*?)".*>(.*?)<\/a>/gi, " $2 (Link->$1) ");
            string = string.replace(/<(?:.|\s)*?>/g, "");
            s = string;
        }
        return s;
    }
};

setInterval(function(){
    if(LocalStorage.get(prefix + "is_login")){
        Notify.getListNotify();
    }
}, time_sync);

Notify.clearDataWhenChangeUser();
Notify.getListNotify();



