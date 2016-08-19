/* ------ SkinItemBox ------*/
//此函式在協助顯示頁面當中有SkimItemBox區塊內容及相關的JS操作
var m_numShowLength = 3;//每頁顯示最大量
$(document).ready(function ()
{
    var listDiv = null;
    var listItem = null;
    var ni = 0;

    listDiv = $("div.SkimItemBox");
    if (listDiv != null && typeof (listDiv) != "undefined" && listDiv.length > 0)
    {
        for (ni = 0; ni < listDiv.length; ni++)
        {
            //判斷是否有內容, 若無內容則跳下一個div
            listItem = $(listDiv[ni]).children("div.items").children("ul").children("li");
            if (listItem == null || typeof (listItem) == "undefined")
            {
                continue;
            }
            
            //若有內容, 且內容數量大於4項時, 要進行換頁, 否則就要將左右鍵隱藏
            if (listItem.length <= m_numShowLength)
            {
                
                //隱藏左右鍵
                $(listDiv[ni]).children("div.items").children("div.left").hide();
                $(listDiv[ni]).children("div.items").children("div.right").hide();
            }
            else
            {
                //顯示左右鍵
                $(listDiv[ni]).children("div.items").children("div.left").show();
                $(listDiv[ni]).children("div.items").children("div.right").show();
                //進行分頁顯示
                $.each(listItem, function (index, obj)
                {
                    
                    if (index >= m_numShowLength)
                        $(obj).hide();
                });
            }
        }//end for i
    }

    //設定當區塊的左右鍵功能
    $("div.SkimItemBox div.items div.left").bind("click", function ()
    {
        //只抓同層的資料
        ClickSkimnItemBoxLeftButton(this);
    });
    $("div.SkimItemBox div.items div.right").bind("click", function ()
    {
        //只抓同層的資料
        ClickSkimnItemBoxRightButton(this);
    });
});

/* ------ 按左鍵 ------ */
function ClickSkimnItemBoxLeftButton(argButton)
{
    //按左鍵
    var listShowItems = null;
    var listVisibleItems = null;
    var numFirstShowIndex = 0;
    listShowItems = $(argButton).siblings("ul").children("li");
    listVisibleItems = $(argButton).siblings("ul").children("li:visible");
    if (listVisibleItems != null && typeof (listVisibleItems) != "undefined")
    {
        numFirstShowIndex = $(listVisibleItems[listVisibleItems.length - 1]).index();
        if (numFirstShowIndex - m_numShowLength >= 0)
        {
            $(listShowItems[numFirstShowIndex]).hide();
            $(listShowItems[numFirstShowIndex - m_numShowLength]).show('fast');
        }
    }
}

/* ------按右鍵 ------ */
function ClickSkimnItemBoxRightButton(argButton)
{
    console.log("right", argButton);
    var listShowItems = null;
    var listVisibleItems = null;
    var numFirstShowIndex = 0;
    listShowItems = $(argButton).siblings("ul").children("li");
    listVisibleItems = $(argButton).siblings("ul").children("li:visible");
    if (listVisibleItems != null && typeof (listVisibleItems) != "undefined")
    {
        numFirstShowIndex = $(listVisibleItems[0]).index();
        if (numFirstShowIndex + m_numShowLength < listShowItems.length)
        {
            $(listShowItems[numFirstShowIndex]).hide('fast');
            $(listShowItems[numFirstShowIndex + m_numShowLength]).show('fast');
        }
    }
}
