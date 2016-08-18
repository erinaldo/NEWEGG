twNewegg.Media = {
	lightBox: $('.LightBoxUnderLay'),
	lightBoxMsg: $('.LightBoxMsg'),
	msgContent: $('.MsgContent'),
	floater: false,
	top: $(window).scrollTop(),
	left: $(window).scrollLeft(),

	GetValue: function (object) {
		var $obj = $(object).parent();
		var type = $obj.find('div.MediaBox').attr('type');//類型 (1圖片2影片)
		var path = $obj.find('div.MediaBox').attr('path');//來源連結
		var date = $obj.find('span.Date').text();//日期
		var title = $obj.find('div.Summary').text();//內文
		var link = $obj.find('div.MediaBox').attr('link');//圖檔or影片路徑
		while (title.indexOf(" ") > 0) { title = title.replace(" ", ""); }

		twNewegg.Media.lightBox.show();
		twNewegg.Media.lightBoxMsg.show();
		twNewegg.Media.showPop(type, path, date, title, link);
		twNewegg.Media.lockScroll(twNewegg.Media.floater);

	},

	showPop: function (type, path, date, title, link) {
		if (type == 2) {
			twNewegg.Media.msgContent.find('div.Media').append("<iframe src='" + link + "'></iframe>");
		}
		else {
			twNewegg.Media.msgContent.find('div.Media').append("<img src='" + link + "' />");
		}
		twNewegg.Media.msgContent.find('p.Date').text(date);
		twNewegg.Media.msgContent.find('div.MediaName').text(title);
		twNewegg.Media.lightBoxMsg.find('p > a').attr('href', path).text(path);
	},

	closePop: function () {
		$('.CloseBtn').click(function () {
			twNewegg.Media.lightBox.hide();
			twNewegg.Media.lightBoxMsg.hide();
			twNewegg.Media.msgContent.find('div.Media').html("");
			twNewegg.Media.lockScroll(twNewegg.Media.floater);
		});
	},

	lockScroll: function (state) {
		if (!state) {
			$('body').css('overflow', 'hidden');
			$(window).scroll(function () {
				$(this).scrollTop(twNewegg.Media.top).scrollLeft(twNewegg.Media.left);
			});
		} else {
			$('body').css('overflow', 'auto');
			$(window).unbind('scroll');
		}
		twNewegg.Media.floater = !twNewegg.Media.floater;
	}
};