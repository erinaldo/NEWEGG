
twNewegg.History = {
    pushState: function (obj, title, URL) {
        if (window.history && window.history.pushState) {
            history.pushState(obj, title, URL);
        } else {
            History.pushState(obj, title, URL);
        }
    },
    pushStateURLlocation: function (obj, title, URL) {
        if (window.history && window.history.pushState) {
            history.pushState(obj, title, URL);
        } else {
            // 因為與頁面處理的邏輯會衝突，故 Mark 使用 URL 跳轉方式做上下頁
            //History.pushState(obj, title, URL);
            window.location.search = URL;
        }
    },
    getState: function () {
        if (window.history && window.history.pushState) {
            return history.state;
        } else {
            return History.getState();
        }
    },
    onPopState: function (obj_html5, obj_historyjs) {
        if (window.history && window.history.pushState) {
            if (typeof (obj_html5) != "undefined" || obj_html5 != null) {
                window.onpopstate = obj_html5;
            }
        } else {
            if (typeof (obj_historyjs) != "undefined" || obj_historyjs != null) {
                window.onstatechange = obj_historyjs;
            } 
        }
    }
}