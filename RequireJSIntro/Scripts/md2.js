/// <reference path="require.js" />
define("md2", ["md1"], function (md1) {

    var alertImpl = function () {
        alert(md1.getMsg());
    };

    return {
        alert: alertImpl
    };
});