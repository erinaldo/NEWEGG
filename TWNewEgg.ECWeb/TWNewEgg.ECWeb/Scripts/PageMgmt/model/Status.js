angular.module('Status', [])
    .service('Status', function () {
        this.PageStatus = {};
        this.PageStatus["E"] = "編輯中";
        this.PageStatus["W"] = "待審核";
        this.PageStatus["R"] = "退回";
        this.PageStatus["A"] = "上線";
        this.PageStatus["D"] = "下線";
        this.PageStatus["Editing"] = "E";
        this.PageStatus["Waiting"] = "W";
        this.PageStatus["Reject"] = "R";
        this.PageStatus["Active"] = "A";
        this.PageStatus["Deactive"] = "D";
        this.ComponentStatus = { SAVED: "S", EDIT: "E", NEW: "N", DELETE: "D" };
    });