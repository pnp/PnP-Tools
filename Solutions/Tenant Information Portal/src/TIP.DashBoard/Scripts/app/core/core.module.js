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
        'common.logger', 'common.exception',

         /*
         * 3rd Party Modules
         */
        'ui.bootstrap', 'pascalprecht.translate', 'angularSpinner', 'ngJsonExportExcel', 'chart.js', 'officeuifabric.core', 'officeuifabric.components', 'officeuifabric.components.commandbar',
        
        /*
        * Directives
        */
        'angularUtils.directives.dirPagination'
    ]);
})();