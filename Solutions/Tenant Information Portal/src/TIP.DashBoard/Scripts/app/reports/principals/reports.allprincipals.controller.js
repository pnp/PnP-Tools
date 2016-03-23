(function () {
    'use strict';
    var controllerId = 'reports.allprincipals.controller';

    angular
        .module('app.reports')
        .controller('ReportsAllPrincipalsController', ReportsAllPrincipalsController);

    ReportsAllPrincipalsController.$inject = ['$q', 'PrincipalDataService', 'usSpinnerService', 'logger', '$log', '$timeout'];

    function ReportsAllPrincipalsController($q, PrincipalDataService, usSpinnerService, logger, $log, $timeout) {
        var vm = this;
        vm.principals = [];
        vm.pageSize = 10;
        vm.query = "";
        vm.currentPage = 1;
        vm.loading = false;
        vm.csvExportFileName = "AllPrincipals";
        vm.reportFields = {appId: 'Application ID', displayName: 'Display Name', principalNames: 'Principal Names', replyUrls: 'Reply Url', endDate: 'End Date'};

        vm.getPrincipals = getPrincipals;
        vm.getPrincipalCount = getPrincipalCount;
        vm.setPageSize = setPageSize;
        vm.openMenu = openMenu;

        /*Have to do this for spinner because $broadcast loads first */
        $timeout(function () {
            usSpinnerService.spin('spinner');
        }, 100);

        activate();

        function activate() {
            logger.info('Activating All Principals');
            vm.loading = true;
            usSpinnerService.spin('spinner');
            getPrincipals();
        }

        function getPrincipals() {
            $log.info('Info ' + controllerId, 'Entering getPrincipals');
            return PrincipalDataService.getAllPrincipals()
           .then(function (data) {
               vm.principals = data;
               vm.loading = false;
               usSpinnerService.stop('spinner');
               return vm.principals;
           });
        }

        function getPrincipalCount() {
            return vm.principals.length;
        }

        function openMenu() {
            vm.isMenuOpen = !vm.isMenuOpen;
        }

        function setPageSize(pageSize) {
            vm.pageSize = pageSize;
        }
    }
})();