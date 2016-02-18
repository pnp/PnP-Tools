(function () {
    'use strict';
    angular.module('app.core', [
         /*
         * Angular Modules
         */
        'ngAnimate', 'ngMessages', 'ngSanitize',

         /*
         * Reusable app modules
         */
        'common.exception', 'common.logger',

         /*
         * 3rd Party Modules
         */
        'ui.bootstrap', 'pascalprecht.translate', 'angularSpinner', 'ngJsonExportExcel', 'chart.js',

        /*
        * Directives
        */
        'angularUtils.directives.dirPagination'
    ]);
})();