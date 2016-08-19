/* 密碼規則
 * (?=.*[a-z]) : 至少有一個小寫字母
 * (?=.*[A-Z]) : 至少有一個大寫字母
 * (?=.*\d) : 至少有一個數字
 * (?=.*[\x21-\x2F\x3A-\x40\x5B-\x60\x7B-\x7E]) : 至少有一個特殊符號
 * (?=.{8,30}) : 長度為8-30字元
 * (?!.*[^\x21-\x7e]) : 不允許數字、字母、特殊符號以外的字元出現
 * (?!.*[\u4e00-\u9fa5\x20\u3000\uff01-\uff5e\u3105-\u312c]) : 不允許輸入中文字、空白鍵、全形符號與注音符號
 */
/* ****** 驗證密碼強度 ******
 * 1. 至少8至30個字元(必要)
 * 2. 至少使用1個大寫字母
 * 3. 至少使用1個小寫字母
 * 4. 至少使用1個數字
 * 5. 至少使用1個特殊符號
 * 6. 密碼不得與帳號相同(必要)
 * 第1項及第6項為必要選項
 * 判斷密碼是否Pass:2個必要條件必須通過,4個選擇條件必須至少完2項
*/
var emailRule = /^([\w-\.\+\-\_]+)@@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4})$/;
// 暫無使用，參考用
var pwRule = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\x21-\x2F\x3A-\x40\x5B-\x60\x7B-\x7E])(?=.{8,30})(?!.*[^\x21-\x7e])(?!.*[\u4e00-\u9fa5\x20\u3000\uff01-\uff5e\u3105-\u312c]).*$/;
var pwdStrength = function (setpwd) {
    return document.getElementById(setpwd);
}

//var iss =
//{ /* FF0000:紅色 FF8540:橘色 FFA73D:淺橘色 BCBB28:芹菜色 299a0b:綠色 000000:黑色 */
//    color: ["#FF0000", "#FF8540", "#FFA73D", "#BCBB28", "#299a0b", "#000000"],
//    text: [" 16% ", " 33% ", " 50% ", " 67% ", " 84% ", " 100% ", " 0% "],
//    width: ["84%", "67%", "50%", "33%", "16%", "0%", "100%"],  /* 灰色區塊的%數 */
//    reset: function () /* 密碼強度:0% */ {
//        pwdStrength("showResult1").style.width = iss.width[6];
//        pwdStrength("showResult2").innerHTML = "密碼強度" + iss.text[6];
//        pwdStrength("showResult2").style.color = iss.color[5];
//    },
//    level0: function () /* 密碼強度:16% */ {
//        pwdStrength("showResult1").style.width = iss.width[0];
//        pwdStrength("showResult2").innerHTML = "密碼強度" + iss.text[0];
//        pwdStrength("showResult2").style.color = iss.color[0];
//    },
//    level1: function () /* 密碼強度:33% */ {
//        pwdStrength("showResult1").style.width = iss.width[1];
//        pwdStrength("showResult2").innerHTML = "密碼強度" + iss.text[1];
//        pwdStrength("showResult2").style.color = iss.color[1];
//    },
//    level2: function () /* 密碼強度:50% */ {
//        pwdStrength("showResult1").style.width = iss.width[2];
//        pwdStrength("showResult2").innerHTML = "密碼強度" + iss.text[2];
//        pwdStrength("showResult2").style.color = iss.color[2];
//    },
//    level3: function () /* 密碼強度:67% */ {
//        pwdStrength("showResult1").style.width = iss.width[3];
//        pwdStrength("showResult2").innerHTML = "密碼強度" + iss.text[3];
//        pwdStrength("showResult2").style.color = iss.color[2];
//    },
//    level4: function () /* 密碼強度:84% */ {
//        pwdStrength("showResult1").style.width = iss.width[4];
//        pwdStrength("showResult2").innerHTML = "密碼強度" + iss.text[4];
//        pwdStrength("showResult2").style.color = iss.color[3];
//    },
//    level5: function () /* 密碼強度:100% */ {
//        pwdStrength("showResult1").style.width = iss.width[5];
//        pwdStrength("showResult2").innerHTML = "密碼強度" + iss.text[5];
//        pwdStrength("showResult2").style.color = iss.color[4];
//    }
//}

function VerifyEmailRule(objEmail) {
    var strEmail = "";
    var boolVerifyEmail = false;


    if (typeof (objEmail) != "string")
        strEmail = $(objEmail).val();
    if (strEmail == null || typeof (objEmail) == "undefined")
        strEmail = "";

    if (strEmail.search(emailRule) >= 0)
        boolVerifyEmail = true;

    return boolVerifyEmail;
}

function VerifyPwdRule(strPwd, strEmail) {
    //iss.reset();
    //strAccountEmail = $("#account_email").val();
    var numMustRule = 0;//必要條件:密碼長度,不可與帳號相同
    var numConditionRule = 0;//選項條件:符合2項即可
    var boolVerifyResult = false;
    var level = 0;

    //驗證傳入資料
    if (strPwd == null || typeof (strPwd) == "undefined")
        strPwd = "";
    if (strEmail == null || typeof (strEmail) == "undefined")
        strEmail = "";

    var IsOK = false;
    var IsOKstrEmail = false;

    //至少6至16個字元(必要)
    if (strPwd.length >= 6 && strPwd.length <= 16) {
        level++;
        numMustRule++;
        $("#pwdcheck_length img").attr("src", "/Themes/img/Account/checked.png");
        IsOK = true;
    }
    else {
        $("#pwdcheck_length img").attr("src", "/Themes/img/Account/unchecked.png");
        IsOK = false;
    }

    ////需含有英文與數字
    //if (strPwd.match(/[0-9]/ig) && strPwd.match(/[A-Za-z]/)) {
    //    level++;
    //    numMustRule++;
    //    $("#pwdcheck_numeric img").attr("src", "/Themes/img/Account/checked.png");
    //}
    //else {
    //    $("#pwdcheck_numeric img").attr("src", "/Themes/img/Account/unchecked.png");
    //}

    //密碼不可與帳號相同
    if (strEmail != strPwd && strPwd.length > 0) {
        level++;
        numMustRule++;
        $("#pwdcheck_email img").attr("src", "/Themes/img/Account/checked.png");
        IsOKstrEmail = true;
    }                                                   
    else {
        $("#pwdcheck_email img").attr("src", "/Themes/img/Account/unchecked.png");
        IsOKstrEmail = false;
    }

    if (IsOK && IsOKstrEmail)
    {
        $(".registerArea .pswRuleBox").animate({
            opacity: 0,
            marginTop: 0,
            height: 0
        }, 500).fadeOut();
    }

    //密碼強度條顯示
    if (level == 1) {
        $("#showResult").attr('class', 'pswSecure step2');
        $("#showResult").css('display', '');
    } else if (level == 2) {
        $("#showResult").attr('class', 'pswSecure step3');
       $("#showResult").css('display', '');
    } else if (level == 3) {
        $("#showResult").attr('class', 'pswSecure step3');
        $("#showResult").css('display', '');
    } else {
        $("#showResult").css('display', 'none');

    }

    //判斷密碼是否Pass:3個必要條件必須通過
    if (numMustRule == 2 )
        boolVerifyResult = true;
    else
        boolVerifyResult = false;

    return boolVerifyResult;
}