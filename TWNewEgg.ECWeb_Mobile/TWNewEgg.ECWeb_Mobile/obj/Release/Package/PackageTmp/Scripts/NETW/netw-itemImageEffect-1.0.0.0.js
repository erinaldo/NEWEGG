//賣場頁用的圖片置中jQuery
function imgVerticalAlignCenter() {
    $(".itemImgCen").each(function () {
		$(this).attr("src", $(this).attr("src") + "?" + new Date().getTime());
	});
    $(".itemImgCen").load(function (index) {
		var imgHeight = $(this).height();
        var pic = $(this).closest('.pic');
		$(this).css("margin-top", (pic.height() - imgHeight) / 2);
		pic.css("textAlign", "center").css("overflow", "hidden");
    });
}

function sectionImgVerticalAlignCenter(section) {
    section.find(".itemImgCen").each(function () {
        $(this).attr("src", $(this).attr("src") + "?" + new Date().getTime());
    });
    section.find(".itemImgCen").load(function (index) {
        var imgHeight = $(this).height();
        var pic = $(this).closest('.pic');
        $(this).css("margin-top", (pic.height() - imgHeight) / 2);
    });
}