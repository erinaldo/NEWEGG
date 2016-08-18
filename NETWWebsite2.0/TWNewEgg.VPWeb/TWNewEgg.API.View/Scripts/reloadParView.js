function showAccountSettingsViewJS(controllername, actionName, viewName, divname) {
    $("div[Category='" + controllername + "']").each(function () {
        $(this).attr("class", "nav");
    });
    //var firstSubView = "";
    //if (actionName == "AccountInformation")
    //{
    //    firstSubView = "AccountInformation";
    //}
    var urlRefresh = "/" + controllername + "/" + actionName;
    $("#" + viewName).attr("class", "nav active");
    $.ajax({
        type: "POST",
        url: urlRefresh,
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify({
            "viewName": viewName
        }),
        success: function (res) {
            if (res.IsSuccess == true) {
                $("#" + divname).html(res.ViewHtml);
                //$("#AccountSettingDetail").html(res.ViewHtml);
            }
            else {
                $("#" + divname).html("");
                //$("#AccountSettingDetail").html("");
            }
        }
    });
}
