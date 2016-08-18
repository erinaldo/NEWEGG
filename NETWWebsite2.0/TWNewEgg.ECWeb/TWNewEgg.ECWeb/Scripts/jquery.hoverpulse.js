/*
 * jQuery HoverPulse Plugin by M. Alsup
 * Examples and docs at: http://malsup.com/jquery/hoverpulse/
 * Dual licensed under the MIT and GPL
 * Requires: jQuery v1.2.6 or later
 * @version: 1.01  26-FEB-2009
 *
 * Patched to work with jQuery 1.10

 * Updated from Lynn.Y.Yeh for 產品縮放1.05倍
 */
(function ($)
{

    $.fn.hoverpulse = function (options)
    {
        // in 1.3+ we can fix mistakes with the ready state
        if (this.length == 0)
        {
            if (!$.isReady && this.selector)
            {
                var s = this.selector, c = this.context;
                $(function ()
                {
                    $(s, c).hoverpulse(options);
                });
            }
            return this;
        }

        var opts = $.extend({}, $.fn.hoverpulse.defaults, options);

        // parent must be relatively positioned
        this.parent().css({ position: 'relative' });
        // pulsing element must be absolutely positioned
        //this.css({ position: 'absolute', top: 0, left: 0 });
        
        this.each(function ()
        {
            var $this = $(this);
            var w = $this.width(), h = $this.height();
            /*
            var numMaxHeight = 0;
            var numMaxWidth = 0;

            numMaxHeight = parseInt($(this).css('max-height'));
            numMaxWidth = parseInt($(this).css('max-width'));

            //較寬
            
            if (w > h)
            {
                w = numMaxWidth;
                h = h * numMaxWidth / w;
            }
            else
            {
                h = numMaxHeight;
                w = w * numMaxHeight / h;
            }
            */

            $this.data('hoverpulse.size', { w: parseInt(w), h: parseInt(h) });
        });

        // bind hover event for behavior
        return this.hover(
          // hover over
          function ()
          {
              var $this = $(this);
              $this.parent().css('z-index', opts.zIndexActive);
              var size = $this.data('hoverpulse.size');
              var w = size.w, h = size.h
              var target = new Object({ "w": w * opts.Multiple, "h": h * opts.Multiple });
              //若原來的容器css有限定max, 則要重設原來容器的max, 及置中
              var numMaxHeight = parseInt($(this).css('max-height'));
              var numMaxWidth = parseInt($(this).css('max-width'));
              var numTop = "0px";
              var numLeft = "0px";

              if (numMaxHeight > 0)
              {
                  numTop = ((numMaxHeight - target.h) / 2) + "px";
              }
              if (numMaxWidth > 0)
              {
                  numLeft = ((numMaxWidth - target.w) / 2) + "px";
              }

              if ((numMaxHeight != null && typeof (numMaxHeight)) != "undefined" && numMaxHeight < target.h)
              {
                  numMaxHeight = target.h;
              }
              if (numMaxWidth != null && typeof (numMaxWidth) != "undefined" && numMaxWidth < target.w)
              {
                  numMaxWidth = target.w;
              }

              $this.stop().animate({
                  "margin-top": numTop,
                  "margin-left": numLeft,
                  height: target.h + "px",
                  width: target.w + "px",
                  "max-width": numMaxWidth + "px",
                  "max-height": numMaxHeight + "px"
                  
              }, opts.speed, function ()
              {
              });
          },
          // hover out
          function ()
          {
              var $this = $(this);
              var size = $this.data('hoverpulse.size');
              var w = size.w, h = size.h;
              var numTop = "0px";
              var numLeft = "0px";

              //重設原始的max, 及置中
              if (opts.OldMaxHeight > 0)
              {
                  numTop = ((opts.OldMaxWidth - h) / 2) + "px";
                  $this.css('max-width', opts.OldMaxWidth);
              }
              if (opts.OldMaxWidth > 0)
              {
                  numLeft = ((opts.OldMaxHeight - w) / 2) + "px";
                  $this.css('max-height', opts.OldMaxHeight);
              }

              $this.stop().animate({
                  "margin-top": numTop,
                  "margin-left": numLeft,
                  height: h + "px",
                  width: w + "px"
              }, opts.speed, function ()
              {

              });
          });
    };

    $.fn.hoverpulse.defaults = {
        size: 200,
        speed: 200,
        zIndexActive: 1,
        zIndexNormal: 1,
        Multiple: 1.05,
        OldMaxWidth:0,
        OldMaxHeight:0
    };

})(jQuery);
