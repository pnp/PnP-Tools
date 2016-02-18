(function () {
    'use strict';
    var controllerId = 'reports.allprincipals.controller';

    angular
        .module('app.reports')
        .controller('ReportsAllPrincipalsController', ReportsAllPrincipalsController);

    ReportsAllPrincipalsController.$inject = ['$q', 'PrincipalDataService', 'usSpinnerService', 'logger', '$log', '$timeout'];

    function ReportsAllPrincipalsController($q, PrincipalDataService, usSpinnerService, logger, $log, $timeout) {
        var vm = this;
        vm.allPrincipals = [];
        vm.pageSize = 50;
        vm.query = "";
        vm.currentPage = 1;
        vm.loading = false;
        vm.csvExportFileName = "AllPrincipals";
        vm.reportFields = {appId: 'Application ID', displayName: 'Display Name', principalNames: 'Principal Names', replyUrls: 'Reply Url', endDate: 'End Date'};
        vm.getAllPrincipals = getAllPrincipals;
        vm.getPrincipalCount = getPrincipalCount;

        /*Have to do this for spinner because $broadcast loads first */
        $timeout(function () {
            usSpinnerService.spin('spinner');
        }, 100);

        activate();

        function activate() {
            logger.info('Activating All Principals');
            vm.loading = true;
            usSpinnerService.spin('spinner');
            getAllPrincipals();
        }

        function getAllPrincipals() {
            $log.info('Info ' + controllerId, 'Entering getAllPrincipals');
            return PrincipalDataService.getAllPrincipals()
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