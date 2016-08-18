return;
var oXmlHttp = new XMLHttpRequest();
var oXmlDoc = null;
var oXmlElement = null;

var listCityDictionary = new Array();
var listCntryDictionary = new Array();
var listBranchDictionary = new Array();
oXmlHttp.open("GET", "/XML/HiLifeStoreMap.xml",false);
oXmlHttp.send(null);
//oXmlHttp.onreadystatechange = callbackFunction;
//console.log(oXmlHttp);
//console.log("oXmlHttp.responseXML : " + oXmlHttp.responseXML);
oXmlDoc = oXmlHttp.responseXML;
//console.log("oXmlDoc : " + oXmlDoc);
//console.log(oXmlDoc.documentElement);
oXmlElement = oXmlDoc.documentElement;
//function callbackFunction() {
//    if (oXmlHttp.readyState == 4) {
//        console.log(oXmlHttp);
//        oXmlDoc = oXmlHttp.responseXML;
//        console.log(oXmlDoc);
//        oXmlElement = oXmlDoc.documentElement;
//        console.log(oXmlElement);
//        getDocDataTest();
//    }
//}

function getDocDataTest() {
    var nodeCity = oXmlElement.getElementsByTagName("Stcity");
    var strSelectCity = "";
    var strSelectCntry = "";
    var strSelectBranch = "";
    var defaultSelectOption = "<option value=''> --請選擇-- </option>";
    for (var i = 0; i < nodeCity.length; i++) {
        var cityOption = "<option value='" + nodeCity[i].getAttribute("Name") + "'>" + nodeCity[i].getAttribute("Name") + "</option>";
        if (i == 0) {
            strSelectCity += "<select id='selectHiLifeCity' class='select_sm' style='width:100px;' onchange='showSelectCntry(this)'>";
            strSelectCity += defaultSelectOption;
            strSelectCity += cityOption;
        }
        else if (i == nodeCity.length - 1) {
            strSelectCity += cityOption;
            strSelectCity += "</select>";
        }
        else {
            strSelectCity += cityOption;
        }

        listCityDictionary.push(nodeCity[i].getAttribute("Name"));
        nodeCntry = nodeCity[i].getElementsByTagName("Stcntry");
        if (typeof (nodeCntry) != "undefined" || nodeCntry.length > 0) {
            for (var j = 0; j < nodeCntry.length; j++) {
                var cntryOption = "<option value='" + nodeCntry[j].getAttribute("Name") + "'>" + nodeCntry[j].getAttribute("Name") + "</option>";
                if (j == 0) {
                    strSelectCntry += "<select id='selectHiLifeCntry' style='width:100px;' onchange='showSelectBranch(this)'>";
                    strSelectCntry += defaultSelectOption;
                    strSelectCntry += cntryOption;
                }
                else if (j == nodeCntry.length - 1) {
                    strSelectCntry += cntryOption;
                    strSelectCntry += "</select>";
                }
                else {
                    strSelectCntry += cntryOption;
                }
                nodeBranch = nodeCntry[j].getElementsByTagName("Branch");
                if (typeof (nodeBranch) != "undefined" || nodeBranch.length > 0) {
                    for (var k = 0; k < nodeBranch.length; k++) {
                        hiLifeStoreAddr = nodeBranch[k].getAttribute("Stadr").replace(nodeCity[i].getAttribute("Name"), "").replace(nodeCntry[j].getAttribute("Name"), "");
                        var branchOption = "<option value='" + nodeBranch[k].getAttribute("Dcrono") + "|" + nodeBranch[k].getAttribute("Zipcd") + "|" + nodeBranch[k].getAttribute("Stno") + "'>" + hiLifeStoreAddr + "[" + nodeBranch[k].getAttribute("Stnm") + "]" + "</option>";
                        if (k == 0) {
                            strSelectBranch += "<select id='selectHiLifeBranch' style='width:370px; background: url(/Themes/2014/img/btn2014.png) -14px -252px;' onchange='chooseBranch(this);'>";
                            strSelectBranch += defaultSelectOption;
                            strSelectBranch += branchOption;
                        }
                        else if (k == nodeBranch.length - 1) {
                            strSelectBranch += branchOption;
                            strSelectBranch += "</select>";
                        }
                        else {
                            strSelectBranch += branchOption;
                        }
                    }
                    listBranchDictionary.push({ cntryName: nodeCntry[j].getAttribute("Name"), branchSet: strSelectBranch });
                    strSelectBranch = "";
                }
            }
            listCntryDictionary.push({ cityName: nodeCity[i].getAttribute("Name"), cntrySet: strSelectCntry });
            strSelectCntry = "";
        }
    }

    function initHiLifeStoreRoute() {
        undefinedSetting = "<option value=''> --請選擇-- </option>";
        if (typeof (defaultSelectOption) == "undefined") {
            defaultSelectOption = undefinedSetting;
        }
        $("#hiLifeCity").html("");
        if (typeof (strSelectCity) == "undefined") {
            $("#hiLifeCity").html("<select id='selectHiLifeCity' class='select_sm' style='width:100px;' onchange='showSelectCntry(this)'>" + defaultSelectOption + "</select>");
        }
        else if (strSelectCity.length > 0) {
            $("#hiLifeCity").html(strSelectCity);
        }
        $("#hiLifeCntry").html("<select id='selectHiLifeCntry' style='width:100px;' onchange='showSelectBranch(this)'>" + defaultSelectOption + "</select>");
        $("#hiLifeBranch").html("<select id='selectHiLifeBranch' style='width:370px; background: url(/Themes/2014/img/btn2014.png) -14px -252px;' onchange='chooseBranch(this);'>" + defaultSelectOption + "</select>");
    }

    function initHiLifeStoreRouteOff() {
        undefinedSetting = "<option value=''> --請選擇-- </option>";
        if (typeof (defaultSelectOption) == "undefined") {
            defaultSelectOption = undefinedSetting;
        }
        $("#hiLifeCity").html("");
        $("#hiLifeCity").html("<select id='selectHiLifeCity' class='select_sm' style='width:100px;' onchange='showSelectCntry(this)'>" + defaultSelectOption + "</select>");
        $("#hiLifeCntry").html("<select id='selectHiLifeCntry' style='width:100px;' onchange='showSelectBranch(this)'>" + defaultSelectOption + "</select>");
        $("#hiLifeBranch").html("<select id='selectHiLifeBranch' style='width:370px; background: url(/Themes/2014/img/btn2014.png) -14px -252px;' onchange='chooseBranch(this);'>" + defaultSelectOption + "</select>");
    }

    function showSelectCntry(objCity) {
        if (objCity.value.length > 1) {
            for (var cityi = 0; cityi < listCntryDictionary.length ; cityi++) {
                if (listCntryDictionary[cityi].cityName == objCity.value) {
                    $("#hiLifeCntry").html("");
                    $("#hiLifeCntry").html(listCntryDictionary[cityi].cntrySet);
                }
            }
        }
        else {
            $("#hiLifeCntry").html("<select id='selectHiLifeCntry' style='width:100px;' onchange='showSelectBranch(this)'>" + defaultSelectOption + "</select>");
        }
        $("#hiLifeBranch").html("<select id='selectHiLifeBranch' style='width:370px; background: url(/Themes/2014/img/btn2014.png) -14px -252px;' onchange='chooseBranch(this);'>" + defaultSelectOption + "</select>");
        $("#showHiLifeStoreName").html("");
        $("#showHiLifeStoreAddr").html("");
        // 將店名回填至CustomerInfo頁中的ConvenienceStoreName input裡
        $("#ConvenienceStoreName").val("");
        $("#storeRoute").val("");
        $("#storeStno").val("");
        //
        delivAddrClear();
    }

    function showSelectBranch(objCntry) {
        if (objCntry.value.length > 1) {
            for (var cntryi = 0; cntryi < listBranchDictionary.length ; cntryi++) {
                if (listBranchDictionary[cntryi].cntryName == objCntry.value) {
                    $("#hiLifeBranch").html("");
                    $("#hiLifeBranch").html(listBranchDictionary[cntryi].branchSet);
                }
            }
        }
        else {
            $("#hiLifeBranch").html("<select id='selectHiLifeBranch' style='width:370px; background: url(/Themes/2014/img/btn2014.png) -14px -252px;' onchange='chooseBranch(this);'>" + defaultSelectOption + "</select>");
        }
        $("#showHiLifeStoreName").html("");
        $("#showHiLifeStoreAddr").html("");
        // 將店名回填至CustomerInfo頁中的ConvenienceStoreName input裡
        $("#ConvenienceStoreName").val("");
        $("#storeRoute").val("");
        $("#storeStno").val("");
        //
        delivAddrClear();
    }

    function chooseBranch(objBranch) {
        if (objBranch.value.length > 1) {
            var branchText = document.getElementById("selectHiLifeBranch").options[document.getElementById("selectHiLifeBranch").selectedIndex].text;
            var hiLifeDcrono = objBranch.value.split("|")[0];
            var zipCode = objBranch.value.split("|")[1];
            var hiLifeStno = objBranch.value.split("|")[2];
            var convenienceName = branchText.split("[")[1].split("]")[0];
            var storeAddress = $("#selectHiLifeCity").val() + $("#selectHiLifeCntry").val() + branchText.split("[")[0];
            $("#showHiLifeStoreName").html(convenienceName);
            $("#showHiLifeStoreAddr").html(storeAddress);
            // 將店名回填至CustomerInfo頁中的ConvenienceStoreName input裡
            $("#ConvenienceStoreName").val(convenienceName);
            $("#storeRoute").val(hiLifeDcrono);
            $("#storeStno").val(hiLifeStno);
            //
            $("#salesorder_delivloc").val($("#selectHiLifeCity").val());
            CityChoose($("#selectHiLifeCity").val(), "delivCity", "salesorder_delivzip", 0);
            $("#delivCity").val(zipCode + " " + $("#selectHiLifeCntry").val());
            $("#delivaddr").val(branchText.split("[")[0]);
            $("#salesorder_delivaddr").val(branchText.split("[")[0]);
            $("#salesorder_delivzip").val(zipCode);
            selectChange();
        }
        else {
            $("#showHiLifeStoreName").html("");
            $("#showHiLifeStoreAddr").html("");
            // 將店名回填至CustomerInfo頁中的ConvenienceStoreName input裡
            $("#ConvenienceStoreName").val("");
            $("#storeRoute").val("");
            $("#storeStno").val("");
            //
            delivAddrClear();
        }
    }

    function delivAddrClear() {
        $("#salesorder_delivloc").val("");
        $("#delivCity").find("optgroup").remove();
        $("#delivCity").find("option").remove();
        $("#delivCity").append('<option value = "" selected="selected">請選擇鄉鎮市區</option>');
        $("#salesorder_delivzip").val("");
        $("#delivaddr").val("");
    }
}