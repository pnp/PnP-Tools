(function () {
    'use strict';
    var controllerId = 'reports.apps.expired90';

    angular
        .module('app.reports')
        .controller('ReportsAppsExpired90Controller', ReportsAppsExpired90Controller);

    ReportsAppsExpired90Controller.$inject = ['$q', 'ApplicationDataService', 'usSpinnerService', 'logger', '$log', '$timeout'];

    function ReportsAppsExpired90Controller($q, ApplicationDataService, usSpinnerService, logger, $log, $timeout) {
        var vm = this;
        vm.applications = [];
        vm.pageSize = 10;
        vm.query = "";
        vm.currentPage = 1;
        vm.loading = false;
        vm.isMenuOpen = false;
        vm.csvExportFileName = "ApplicationsExpired90days";
        vm.reportFields = { appId: 'Application ID', displayName: 'Display Name', replyUrls: 'Reply Url', identifierUris: 'IdentifierUris', endDate: 'End Date' };


        vm.getExpiredApplications = getExpiredApplications;
        vm.getExpiredApplicationCount = getExpiredApplicationCount;
        vm.setPageSize = setPageSize;
        vm.openMenu = openMenu;

        /*Have to do this for spinner because $broadcast loads first */
        $timeout(function () {
            usSpinnerService.spin('spinner');
        }, 100);

        activate();

        function activate() {
            logger.info('Activating Applications Expired in 90 ');
            vm.loading = true;
            usSpinnerService.spin('spinner');
            getExpiredApplications();
        }

        function getExpiredApplications() {

            $log.info('Info ' + controllerId, 'Entering getExpiredApplications');
            return ApplicationDataService.getExpiredApplicationInDays(90)
           .then(function (data) {
               vm.applications = data;
               vm.loading = false;
               usSpinnerService.stop('spinner');
               return vm.applications;
           });
        }

        function openMenu() {
            vm.isMenuOpen = !vm.isMenuOpen;
        }

        function setPageSize(pageSize) {
            vm.pageSize = pageSize;
        }

        function getExpiredApplicationCount() {
            return vm.applications.length;
        }
    }
})();