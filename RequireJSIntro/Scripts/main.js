/// <reference path="require.js" />

(function () {

    require.config({
        baseUrl: "/Scripts"
    });

    function boot() {
        require(['md2'], function (md2) { md2.alert() });
    }

    requirejs([], boot);

}());