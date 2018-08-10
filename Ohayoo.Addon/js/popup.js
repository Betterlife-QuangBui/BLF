//region -- LocalStorage --
var LocalStorage = {
    set: function (key, value) {
        localStorage.setItem(key, JSON.stringify(value));
    },
    get: function(key) {
        var saved = localStorage.getItem(key);
        saved = JSON.parse(saved);
        return saved;
    }
};
//endregion

var notify_item_tpl;
var prefix = "addon_notify_";
var is_login = LocalStorage.get(prefix + "is_login") == null ? false : LocalStorage.get(prefix + "is_login");
var $content;

$(document).ready(function(){
    console.info("ready");
    $content = $("#_list-notify-items");
    notify_item_tpl = $("#_notify-item-template").html();

    $("._slimScroll").slimScroll({
        height: '540px'
    });

    $content.html( _.template($("#_loading-template").html(), {}) );
    if(is_login){
        Popup.renderHtml();
    }else{
        $content.html( _.template($("#_alert-template").html(), { web_service_url: web_service_url }) );
    }

    Popup.viewAllNotifyOnlyView();

    /*** CĂC Sá»° KIá»†N ***/
    $(document).on('click', '#_list-notify-items > li', Popup.viewNotify);

});

var Popup = {
    viewAllNotifyOnlyView: function(){
        //reset notify
        Common.request({
            url: web_service_url + "CustomerNotification/ResetUnreadNotificationCount",
            type: 'POST',
            data: {}
        }).done(function (response) {

        });

        var notification_ids = [];
        $("._notification-row-view[data-type='VIEW']:not('.consumed')").each(function(i){
            var id = $(this).attr("id");
            if(id){
                notification_ids.push(id);
            }
        });
        if(!notification_ids.length) return;

        Common.request({
            url: web_service_url + "CustomerNotification/SetMoreNotificationIsConsumed",
            type: 'POST',
            data: { notification_ids: notification_ids }
        }).done(function (response) {

            $("._notification-row-view[data-type='VIEW']:not('.consumed')").each(function(i){
                var id = $(this).attr("id");
                $(this).addClass("consumed");
                var item_data = LocalStorage.get(prefix + "item_" + id);
                    item_data.isConsumed = true;
                    item_data.isViewed = true;

                    LocalStorage.set(prefix + "item_" + id, item_data);
            });

        });

    },

    viewNotify: function(){
        var id = $(this).attr("id");
        if(!id) return;

        var item_data = LocalStorage.get(prefix + "item_" + id);
        if(!item_data.isConsumed ){
            $(this).addClass("consumed");
            item_data.isConsumed = true;
            LocalStorage.set(prefix + "item_" + id, item_data);

            Popup.calTotalNotify();

            if(item_data.systemType == "READ") {

                Common.request({
                    url: web_service_url + "CustomerNotification/SetNotificationIsConsumed",
                    type: 'POST',
                    data: { notification_id: id }
                }).done(function (response) {

                    item_data.isRead = true;
                    LocalStorage.set(prefix + "item_" + id, item_data);

                    //Popup.calTotalNotify();

                });

            }
        }
    },

    calTotalNotify: function(){
        var total = -1;
        chrome.browserAction.getBadgeText({}, function(result) {
            total = parseInt(result);
        });

        if(total != -1){
            total--;
            chrome.browserAction.setBadgeText({
                text: '' + total
            });
        }
    },

    renderHtml: function(){
        var ids = LocalStorage.get(prefix + "ids");
        if(ids){
            $content.empty();

            var list_data = [];
            _.each(ids.split(","), function(id, idx){
                if(id){
                    var item_data = LocalStorage.get(prefix + "item_" + id);
                    if(item_data){
                        list_data.push(item_data);
                    }
                }
            });

            var list_data_sorted = list_data.sortBy("-createdTimeTimeStamp");
            _.each(list_data_sorted, function(item_data, idx){
                var html = _.template(notify_item_tpl, item_data);
                $content.append(html);
            });

            $("._time-ago").timeago();

        }else{
            $content.html("Hiá»‡n chÆ°a cĂ³ thĂ´ng bĂ¡o!");
        }

        chrome.browserAction.setBadgeText({
            text: ''
        });
    }

};

var Common = {
    request: function (params) {
        return $.ajax({
            url: params.url,
            type: params.type == undefined ? 'GET' : params.type,
            data: params.data == undefined ? {} : params.data
        });
    }
};

Array.prototype.sortBy = function() {
    function _sortByAttr(attr) {
        var sortOrder = 1;
        if (attr[0] == "-") {
            sortOrder = -1;
            attr = attr.substr(1);
        }
        return function(a, b) {
            var result = (a[attr] < b[attr]) ? -1 : (a[attr] > b[attr]) ? 1 : 0;
            return result * sortOrder;
        }
    }
    function _getSortFunc() {
        if (arguments.length == 0) {
            throw "Zero length arguments not allowed for Array.sortBy()";
        }
        var args = arguments;
        return function(a, b) {
            for (var result = 0, i = 0; result == 0 && i < args.length; i++) {
                result = _sortByAttr(args[i])(a, b);
            }
            return result;
        }
    }
    return this.sort(_getSortFunc.apply(null, arguments));
}
