(function () {
    'use strict';
    var controllerId = 'reports.principals.allexpired.controller';

    angular
        .module('app.reports')
        .controller('ReportsExpired30Controller', ReportsExpired30Controller);

    ReportsExpired30Controller.$inject = ['$q', 'PrincipalDataService', 'usSpinnerService', 'logger', '$log', '$timeout'];

    function ReportsExpired30Controller($q, PrincipalDataService, usSpinnerService, logger, $log, $timeout) {
        var vm = this;
        vm.principals = [];
        vm.pageSize = 10;
        vm.query = "";
        vm.currentPage = 1;
        vm.loading = false;
        vm.csvExportFileName = "PrincipalsExpiredin30Days";
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
            logger.info('Activating Principals');
            vm.loading = true;
            usSpinnerService.spin('spinner');
            getPrincipals();
        }

        function getPrincipals() {
            $log.info('Info ' + controllerId, 'Entering getPrincipals');
            return PrincipalDataService.getExpiredPrincipalsInDays(30)
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