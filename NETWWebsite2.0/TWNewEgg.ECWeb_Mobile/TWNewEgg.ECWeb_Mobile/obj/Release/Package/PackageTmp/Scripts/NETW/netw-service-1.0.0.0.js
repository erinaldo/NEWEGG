document.write('<script src="/Scripts/jquery.ba-hashchange.js"></script>');

$(function () {
    //取得錨點
    hashId = location.hash;//.replace('/^#/', '');
        
    var bar;
    $('.ColorBarTit').each(function () {
        bar = $(this);
        bar.attr('onclick', "location.href='#" + bar.attr('id') + "'");
    });
    
    //佈署 onclick 事件
    $('.ColorBarTit').click(function () {
        var nowItem = $(this);
        var nowDiv;

        //開啟點選的項目
        nowItem.filter(function () {
            var item = $(this);
            nowDiv = item.next('.AnsBox');

            if (item.hasClass('Close')) {
                item.removeClass('Close').addClass('Open');
                nowDiv.show();
            }
        });

        //關閉未點選的項目
        var nowPItem = nowItem.parent();

        nowPItem.find(".ColorBarTit[class~='Open']").not(nowItem)
            .removeClass('Open').addClass('Close');

        nowPItem.find('.AnsBox').not(nowDiv).hide();
    });
    
    var barZone = $('.ColorBarTit').parent();
    //hashId = '#' + $('.ColorBarTit').eq(0).attr('id');

    if (barZone.find(hashId).length > 0) {
        //點選指定項目
        barZone.find(hashId).click();
    }    

    //偵測錨點事件
    $(window).hashchange(function () {

        //取得錨點
        hashId = location.hash;

        //var barZone = $('.ColorBarTit').parent();
        if (barZone.find(hashId).length == 0) {
            hashId = '#' + barZone.find('.ColorBarTit').attr('id');
        }

        //點選指定項目
        barZone.find(hashId).click();
    });    
});