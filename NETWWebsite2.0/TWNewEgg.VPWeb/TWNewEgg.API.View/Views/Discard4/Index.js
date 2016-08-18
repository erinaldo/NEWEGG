$(function () {

   

});

//檢查資料物件
function VerifyGrid(list) {

    var info;
    this.verify_msg = '';
    var r = /^[0-9]*[1-9][0-9]*$/;

    try {

        for (var i = 0; i < list.length; i++) {
            info = list[i];
            var item_num = i + 1;

            console.log('info.NumberCode:' + info.NumberCode);

            console.log('info.Discard4Flag:' + info.Discard4Flag);

            if (info.Discard4Flag == 'Y') {
                if (info.NumberCode == null || info.NumberCode == '') {
                    if ($.trim(this.verify_msg) != "") this.verify_msg += "<br>";
                    this.verify_msg = this.verify_msg.concat('第' + item_num + '筆，回收四聯單必填');
                }
                if (info.InstalledDate == null || info.InstalledDate == '') {
                    if ($.trim(this.verify_msg) != "") this.verify_msg += "<br>";
                    this.verify_msg = this.verify_msg.concat('第' + item_num + '筆，安裝日期必填');
                }
            }

        }

        console.log('verify_msg:' + this.verify_msg);

    }
    catch (ex) {
        console.log(ex);
    }
}
//正常傳回true
VerifyGrid.prototype.is_ok = function () {
    var is_ok = false;
    (this.verify_msg == '') ? is_ok = true : is_ok = false;
    return is_ok;
    //return this.verify_msg;
}
//取得檢查的信息
VerifyGrid.prototype.get_msg = function () {
    return this.verify_msg;
}