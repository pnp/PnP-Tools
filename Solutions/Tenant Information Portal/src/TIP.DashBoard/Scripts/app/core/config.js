(function () {
    'use strict';

    var app = angular.module('app.core');

    app.config(toastrConfig);

    /* @ngInject */
    function toastrConfig(toastr) {
        toastr.options.timeOut = 4000;
        toastr.options.positionClass = 'toast-bottom-right';
    }

    var config = {
        appErrorPrefix: '[NG-Modular Error] ', //Configure the exceptionHandler decorator
        appTitle: 'Principal DashBoard',
        version: '1.0.0'
    };

    app.value('config', config);

    app.config(configure);

    app.config(['$translateProvider', function ($translateProvider) {
     
        $translateProvider.useStaticFilesLoader({
            prefix: '/scripts/languages/',
            suffix: '.json'
        });
        $translateProvider.preferredLanguage('en-US').fallbackLanguage('en-US');
    }]);

    //app.config(function (ChartJsProvider) {
    //    // Configure all charts
    //    ChartJsProvider.setOptions({
    //        colours: ['#97BBCD', '#DCDCDC', '#F7464A', '#46BFBD', '#FDB45C', '#949FB1', '#4D5360'],
    //        responsive: true
    //    });
    //    // Configure all doughnut charts
    //    ChartJsProvider.setOptions('Doughnut', {
    //        animateScale: true
    //    });
    //});

    /* @ngInject */
    function configure($logProvider, exceptionHandlerProvider) {
        // turn debugging off/on (no info or warn)
        if ($logProvider.debugEnabled) {
            $logProvider.debugEnabled(true);
        }
        // Configure the common exception handler
        exceptionHandlerProvider.configure(config.appErrorPrefix);
    }
})();