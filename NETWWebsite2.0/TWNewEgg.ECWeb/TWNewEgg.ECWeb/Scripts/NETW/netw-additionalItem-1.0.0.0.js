function AdditionalItemList() {
    if ($(".cartAddBuy li.box") == null || $(".cartAddBuy li.box").length == 0) { }
    else {
        //點開小賣場

        $(function() {
            var $prev = $(".cartAddBuy .arrow.prev"), $next = $(".cartAddBuy .arrow.next");
            var $list = $(".cartAddBuy ul.list"), $box = $(".cartAddBuy li.box");
            var move = ($box.width() + 1) * 4;

            //將加購商品排成一列
            $list.width($box.width() * $box.length + $box.length);

            //箭頭動作
            $next.on("click", clickNext);
            $prev.on("click", clickPrev);

            //右箭頭 大於4個時才出現
            if ($box.length > 4) {
                $next.show();
            }

            function clickNext() {
                $next.off("click").addClass("active");
                $prev.off("click");

                $prev.show(500).css("left", "10px");
                var left = parseInt($list.css("left"));

                $list.animate({
                    left: left - move,
                }, 700,
                function () {
                    if ($list.position().left <= -($list.width()) + move) {
                        $next.hide(300).css("right", "-50px");
                    }
                    $next.on("click", clickNext).removeClass("active");
                    $prev.on("click", clickPrev);
                });
            }

            function clickPrev() {
                $prev.off("click").addClass("active");
                $next.off("click");

                $next.show(500).css("right", "10px");
                var left = parseInt($list.css("left"));

                $list.animate({
                    left: left + move,
                }, 700,
                function () {
                    if ($list.position().left >= 0) {
                        $prev.hide(300).css("left", "-50px");
                    }
                    $prev.on("click", clickPrev).removeClass("active");
                    $next.on("click", clickNext);
                });
            }
        });
    }
}