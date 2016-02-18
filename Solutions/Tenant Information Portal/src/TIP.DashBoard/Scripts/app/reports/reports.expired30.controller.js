(function () {
    'use strict';
    var controllerId = 'reports.principals.allexpired.controller';

    angular
        .module('app.reports')
        .controller('ReportsExpired30Controller', ReportsExpired30Controller);

    ReportsExpired30Controller.$inject = ['$q', 'PrincipalDataService', 'usSpinnerService', 'logger', '$log', '$timeout'];

    function ReportsExpired30Controller($q, PrincipalDataService, usSpinnerService, logger, $log, $timeout) {
        var vm = this;
        vm.allPrincipals = [];
        vm.pageSize = 50;
        vm.query = "";
        vm.currentPage = 1;
        vm.loading = false;
        vm.csvExportFileName = "PrincipalsExpiredin30Days";
        vm.reportFields = {appId: 'Application ID', displayName: 'Display Name', principalNames: 'Principal Names', replyUrls: 'Reply Url', endDate: 'End Date'};
    

        vm.getAllExpiredPrincipals = getAllExpiredPrincipals;
        vm.getPrincipalCount = getPrincipalCount;

        /*Have to do this for spinner because $broadcast loads first */
        $timeout(function () {
            usSpinnerService.spin('spinner');
        }, 100);

        activate();

        function activate() {
            logger.info('Activating Principals');
            vm.loading = true;
            usSpinnerService.spin('spinner');
            getAllExpiredPrincipals();
        }

        function getAllExpiredPrincipals() {
            $log.info('Info ' + controllerId, 'Entering getAllExpiredPrincipals');
            return PrincipalDataService.getExpiredPrincipalsInDays(30)
           .then(function (data) {
               vm.allPrincipals = data;
               vm.loading = false;
               usSpinnerService.stop('spinner');
               return vm.allPrincipals;
           });
        }

        function getPrincipalCount() {
            return vm.allPrincipals.length;
        }
    };
})();