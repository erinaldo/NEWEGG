var m_listCoupon = null;

/*
$(document).ready(function ()
{
    var strTemp = "";
    var ni = 0;
    strTemp = $("#data_couponlist").val();
    if (strTemp != null && strTemp != "" && typeof (strTemp) != "undefined")
    {
        m_listCoupon = JSON.parse(strTemp);
    }

    //使用折價券
    $(".couponClick").bind("click", function () {RefreshCouponListDisplay(this); });
    //取消
    $("div.DialogBtn input:button[value='取消']").bind("click", function () { $("div.LightBoxUnderLay").hide(); $("div.couponUseDialog").hide();});
    //確定
    $("div.DialogBtn input:button[value='確定']").bind("click", function(){ AddUsedCoupon();});
});
*/

/* ------ 取得尚未設定使用的Coupon券 ------ */
function GetNonUseCouponList()
{
    if (m_listCoupon == null || typeof (m_listCoupon) == "undefined")
        return null;

    var ni = 0;
    var listCoupon = null;

    listCoupon = new Array();
    for (ni = 0; ni < m_listCoupon.length; ni++)
    {
        if(typeof(m_listCoupon[ni].ItemId) == "undefined" || m_listCoupon[ni].ItemId == null || m_listCoupon[ni].ItemId <= 0)
            listCoupon.push(m_listCoupon[ni]);
    }//end for i

    if (listCoupon.length <= 0)
        listCoupon = null;

    return listCoupon;
}

/* ------ 取得此商品已設定使用的coupon券 ------ */
function GetUsedCouponListByItemId(arg_numItemId)
{
    if (typeof (m_listCoupon) == "undefined" || m_listCoupon == null)
        return null;
    if (typeof (arg_numItemId) == "undefined" || arg_numItemId == null)
        return null;
    if (!$.isNumeric(arg_numItemId))
        return;

    var ni = 0;
    var listCoupon = null;

    listCoupon = new Array();
    for (ni = 0; ni < m_listCoupon.length; ni++)
    {
        if (typeof (m_listCoupon[ni].ItemId) == "undefined" || m_listCoupon[ni].ItemId == null)
            continue;
        if (m_listCoupon[ni].ItemId == arg_numItemId)
            listCoupon.push(m_listCoupon[ni]);
    }//end for

    if (listCoupon.length <= 0)
        listCoupon = null;

    return listCoupon;
}

function RefreshCouponListDisplay(argObj)
{
    var listNonUseCoupon = null;
    var listUsedCoupon = null;
    var numItemPrice = 0;
    var numItemId = 0;
    var numBuyQty = 0;
    var strItemCategory = "";
    var strContent = "";
    var strTemp = "";
    var ni = 0;
    var strTemplate = "";
    var dateEndDate = null;

    if (typeof (m_listCoupon) != "undefined" || m_listCoupon != null)
    {
        //取得商品資訊
        numItemPrice = $(argObj).attr("itemprice");
        numItemId = $(argObj).attr("itemid");
        strItemCategory = ";" + $(argObj).attr("itemcategory") + ";";

        if (typeof (numItemPrice) == "undefined" || numItemPrice == null || typeof (numItemId) == "undefined" || numItemId == null)
            return;

        if (!$.isNumeric(numItemPrice) || !$.isNumeric(numItemId))
            return;

        $("#chooseItemId").val(numItemId);

        //設定顯示樣式
        strTemplate = "<tr class='list'>";
        strTemplate += "<td><input class='couponCheck' type='checkbox' {$Checked} CouponId='{$CouponId}'></td>";
        strTemplate += "<td>";
        strTemplate += "<div class='couponTit'><div class='couponUseName'>{$CouponName}</div>";
        strTemplate += "<div class='couponUseInfo'><span class='red price'>{$CouponPrice}元折價券</span>有效日期：{$EndDate}</div>";
        strTemplate += "<td>";
        strTemplate += "</tr>";

        listUsedCoupon = GetUsedCouponListByItemId(numItemId);
        listNonUseCoupon = GetNonUseCouponList();

        //已使用的
        if (typeof (listUsedCoupon) != "undefined" && listUsedCoupon != null)
        {
            for (ni = 0; ni < listUsedCoupon.length; ni++)
            {
                dateEndDate = listUsedCoupon[ni].validend.replace(/\/Date\(/gi, "").replace(/\)\//gi, "");
                dateEndDate = parseInt(dateEndDate);
                dateEndDate = new Date(dateEndDate);

                strTemp = strTemplate;
                strTemp = strTemp.replace(/{\$Checked}/gi, "checked='checked'");
                strTemp = strTemp.replace(/{\$CouponId}/gi, listUsedCoupon[ni].id);
                strTemp = strTemp.replace(/{\$CouponName}/gi, listUsedCoupon[ni].title);
                strTemp = strTemp.replace(/{\$CouponPrice}/gi, listUsedCoupon[ni].value);
                strTemp = strTemp.replace(/{\$EndDate}/gi, dateEndDate.getFullYear() + "/" + (dateEndDate.getMonth() + 1) + "/" + dateEndDate.getDate() + " " + dateEndDate.getHours() + ":" + dateEndDate.getSeconds() + ":" + dateEndDate.getMilliseconds());

                strContent += strTemp;
            }
        }//end if

        //未使用的Coupon, 要比對商品單價及館架, 再決定是否可列出讓User選擇
        if (listNonUseCoupon != null && typeof (listNonUseCoupon) != "undefined" && typeof (listNonUseCoupon.length) != "undefined")
        {
            for (ni = 0; ni < listNonUseCoupon.length; ni++)
            {
                //商品單價限制不合
                if ((listNonUseCoupon[ni].limit != null || listNonUseCoupon[ni].limit != 0) && numItemPrice < listNonUseCoupon[ni].limit)
                    continue;

                //商品館架及品項必須符合
                if ((listNonUseCoupon[ni].items != null && typeof (listNonUseCoupon[ni].items) != "undefined"
                    && listNonUseCoupon[ni].items.indexOf(";" + numItemId + ";") >= 0)
                    || (listNonUseCoupon[ni].categories != null && typeof (listNonUseCoupon[ni].categories) != "undefined"
                    && (listNonUseCoupon[ni].categories.indexOf(";0;") >= 0 || listNonUseCoupon[ni].categories.indexOf(strItemCategory) >= 0))
                    )
                {
                    dateEndDate = listNonUseCoupon[ni].validend.replace(/\/Date\(/gi, "").replace(/\)\//gi, "");
                    dateEndDate = parseInt(dateEndDate);
                    dateEndDate = new Date(dateEndDate);

                    strTemp = strTemplate;
                    strTemp = strTemp.replace(/{\$Checked}/gi, "");
                    strTemp = strTemp.replace(/{\$CouponId}/gi, listNonUseCoupon[ni].id);
                    strTemp = strTemp.replace(/{\$CouponName}/gi, listNonUseCoupon[ni].title);
                    strTemp = strTemp.replace(/{\$CouponPrice}/gi, listNonUseCoupon[ni].value);
                    strTemp = strTemp.replace(/{\$EndDate}/gi, dateEndDate.getFullYear() + "/" + (dateEndDate.getMonth() + 1) + "/" + dateEndDate.getDate() + " " + dateEndDate.getHours() + ":" + dateEndDate.getSeconds() + ":" + dateEndDate.getMilliseconds());

                    strContent += strTemp;
                }
            }
        }
    }//end if (typeof (m_listCoupon) != "undefined" || m_listCoupon != null)

    //修改顯示畫面
    $("table.couponUseTable tbody").html(strContent);
    if (typeof ($("table.couponUseTable tbody tr")) != "undefined")
        numCouponCount = $("table.couponUseTable tbody tr").length;
    $("div.couponUseDialog div.tit span.red").html(numCouponCount);

    $("div.LightBoxUnderLay").fadeIn();
    $("div.couponUseDialog").fadeIn();
    LightBoxCenter(".couponUseDialog");
}

/* ------ 掃畫面得知哪幾張Coupon被設定使用 ------ */
function AddUsedCoupon(callback)
{
    var ni = 0;
    var nj = 0;
    var numItemId = 0;
    var listCoupon = null;
    var numBuyQty = 0;
    var listDisplayCoupon = null;
    var numAmount = 0;

    numItemId = $("#chooseItemId").val();
    if (typeof (numItemId) == "undefined" || typeof (m_listCoupon) == "undefined" || m_listCoupon == null)
    {
        $("div.LightBoxUnderLay").fadeOut();
        $("div.couponUseDialog").fadeOut();
        return;
    }

    numBuyQty = parseInt($("#" + numItemId + "_Qty").html());

    //掃所有的畫面, 設定Coupon使用
    listDisplayCoupon = $(":checkbox.couponCheck");
    listChooseCoupon = $(":checkbox.couponCheck:checked");

    //若沒有資料, 不作任何設定更動
    if (typeof (listDisplayCoupon) == "undefined" ||(listDisplayCoupon != null && listDisplayCoupon.length <= 0))
    {
        $("span#CouponDiscount_" + numItemId).html(numAmount);
        $("div.LightBoxUnderLay").hide();
        $("div.couponUseDialog").hide();
    }

    if (typeof(listChooseCoupon.length) != "undefined" && listChooseCoupon.length > numBuyQty)
    {
        alert("您最多只能選擇 " + numBuyQty + " 張折價券！");
        return false;
    }

    //比對Coupon進行設定
    numAmount = 0;
    for (ni = 0; ni < listDisplayCoupon.length; ni++)
    {
        for (nj = 0; nj < m_listCoupon.length; nj++)
        {
            //找到資料
            if ($(listDisplayCoupon[ni]).attr("couponid") == m_listCoupon[nj].id)
            {
                if ($(listDisplayCoupon[ni]).prop("checked"))
                {
                    m_listCoupon[nj].ItemId = numItemId;
                    numAmount += m_listCoupon[nj].value;
                }
                else
                {
                    m_listCoupon[nj].ItemId = null;
                }
                break;
            }
        }
    }

    //修改畫面
    $("span#CouponDiscount_" + numItemId).html(numAmount);
    $("div.couponUseDialog").hide();
    $("div.LightBoxUnderLay").hide();
    RefreshCouponTotal();
    RefeshNeedPay();
    if (callback) {
        callback();
    }
}

/* ------ 顯示Coupon券優惠總額 ------ */
function RefreshCouponTotal()
{
    //取得此頁面中所有使用Coupon券的總金額, 並修改折價券總計金額
    var numTotalCouponValue = 0;
    var numCouponValue = 0;
    var listValueTag = $(".couponClick").parent("p");
    var ni = 0;

    if (listValueTag != null && typeof (listValueTag) != "undefined" && listValueTag.length > 0)
    {
        for (ni = 0; ni < listValueTag.length; ni++)
        {
            //取得使用折價券的金額
            numCouponValue = parseInt($(listValueTag[ni]).children("span:eq(1)").html());
            if ($.isNumeric(numCouponValue))
            {
                numTotalCouponValue += numCouponValue;
            }
        }//end for ni

        if (numTotalCouponValue > 0)
        {
            $("#TotalCouponValue").html(numTotalCouponValue);
            $("#TotalCouponValue").parent("tr").removeAttr("style");
        }
        else
        {
            $("#TotalCouponValue").html(0);
            $("#TotalCouponValue").parent("tr").attr("style", "display:none;");
        }
        
    }
}

/* ------ 顯示應付金額 ------ */
function RefeshNeedPay()
{
    var numTotalPrice = 0;//總計
    var numNeedPayPrice = 0;//應付金額
    var numTotalDiscount = 0;//所有活動的折價金額
    var numSubDiscount = 0;
    var listTag = null;
    var ni = 0;

    //取得金額顯示區的所有列
    

    //取得總計
    numTotalPrice = parseInt($("#TotalPrice").html().replace(/,/gi, ""));

    //從金額顯示區的所有列當中, 取得所有優惠
    //listTag = $(".TotalTable tr");
    listTag = $("#CartTotal tr");
    if (listTag != null && typeof (listTag) != "undefined" && listTag.length > 1)
    {
        for (ni = 1; ni < listTag.length; ni++)
        {
            numSubDiscount = $(listTag[ni]).children("td").eq(0).html();
            if (numSubDiscount == null || typeof (numSubDiscount) == "undefined")
                continue;

            numSubDiscount = parseInt(numSubDiscount.replace(/,/gi, ""));
            if ($.isNumeric(numSubDiscount))
                numTotalDiscount += numSubDiscount;
        }//end for ni
    }

    //修改應付金額
    numNeedPayPrice = numTotalPrice - numTotalDiscount;
    $(".NeedPayMoneyPrice").html(Commafy(numNeedPayPrice));

}

/* ------ 驗證所有的Coupon使用狀況 ------ */
function VerifyCouponUsed()
{
    return false;
}

function GetCouponInfo()
{
    return m_listCoupon;
}

/* ------ 釋放該品項的折價券 ------ */
function RemoveCouponByItemId(argItemId)
{
    if (m_listCoupon == null)
        return;
    if (argItemId == null || typeof (argItemId) == "undefined")
        return;
    if (!$.isNumeric(argItemId))
        return;
    if (argItemId <= 0)
        return;

    var ni = 0;
    for (ni = 0; ni < m_listCoupon.length; ni++)
    {
        if (m_listCoupon[ni].ItemId != null && typeof (m_listCoupon[ni].ItemId) != "undefined" && m_listCoupon[ni].ItemId == argItemId)
            m_listCoupon[ni].ItemId = null;
    }

    //設定該品項的折價券相關畫面
    $("#CouponDiscount_" + argItemId).html("0");
}

/* ------ 數值千分位 ------ */
function Commafy(num)
{
    num = num + "";
    var re = /(-?\d+)(\d{3})/
    while (re.test(num))
    {
        num = num.replace(re, "$1,$2")
    }
    return num;
}