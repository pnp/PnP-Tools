(function () {
    'use strict';
    var controllerId = 'reports.apps.all.controller';

    angular
        .module('app.reports')
        .controller('ReportsAllApplicationsController', ReportsAllApplicationsController);

    ReportsAllApplicationsController.$inject = ['$q', 'ApplicationDataService', 'usSpinnerService', 'logger', '$log', '$timeout'];

    function ReportsAllApplicationsController($q, ApplicationDataService, usSpinnerService, logger, $log, $timeout) {
        var vm = this;
        vm.applications = [];
        vm.pageSize = 10;
        vm.query = "";
        vm.currentPage = 1;
        vm.loading = false;
        vm.csvExportFileName = "AllApplications";
        vm.reportFields = { appId: 'Application ID', displayName: 'Display Name', replyUrls: 'Reply Url', identifierUris: 'IdentifierUris', endDate: 'End Date' };

        vm.getAllApplications = getAllApplications;
       
        /*Have to do this for spinner because $broadcast loads first */
        $timeout(function () {
            usSpinnerService.spin('spinner');
        }, 100);

        activate();

        function activate() {
            logger.info('Activating All Applications');
            vm.loading = true;
            usSpinnerService.spin('spinner');
            getAllApplications();
        }

        function getAllApplications() {
            $log.info('Info ' + controllerId, 'Entering getAllApplications');
            return ApplicationDataService.getAllApplications()
           .then(function (data) {
               vm.applications = data;
               vm.loading = false;
               usSpinnerService.stop('spinner');
               return vm.applications;
           });
        }
    };
})();