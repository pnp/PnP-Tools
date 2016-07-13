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
        appTitle: 'TIP',
        version: '1.0.2'
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