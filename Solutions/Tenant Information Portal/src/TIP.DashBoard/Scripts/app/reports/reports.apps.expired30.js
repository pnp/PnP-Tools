(function () {
	'use strict';
	var controllerId = 'reports.apps.expired30';

	angular
        .module('app.reports')
        .controller('ReportsAppsExpired30Controller', ReportsAppsExpired30Controller);

	ReportsAppsExpired30Controller.$inject = ['$q', 'ApplicationDataService', 'usSpinnerService', 'logger', '$log', '$timeout'];

	function ReportsAppsExpired30Controller($q, ApplicationDataService, usSpinnerService, logger, $log, $timeout) {
		var vm = this;
		vm.applications = [];
		vm.pageSize = 10;
		vm.query = "";
		vm.currentPage = 1;
		vm.loading = false;
		vm.csvExportFileName = "ApplicationsExpired30Days";
		vm.reportFields = { appId: 'Application ID', displayName: 'Display Name', replyUrls: 'Reply Url', identifierUris: 'IdentifierUris', endDate: 'End Date' };


		vm.getExpiredApplications = getExpiredApplications;
		vm.getExpiredApplicationCount = getExpiredApplicationCount;

		/*Have to do this for spinner because $broadcast loads first */
		$timeout(function () {
			usSpinnerService.spin('spinner');
		}, 100);

		activate();

		function activate() {
			logger.info('Activating Applications Expired in 30 Days ');
			vm.loading = true;
			usSpinnerService.spin('spinner');
			getExpiredApplications();
		}

		function getExpiredApplications() {

			$log.info('Info ' + controllerId, 'Entering getExpiredApplications');
			return ApplicationDataService.getExpiredApplicationInDays(30)
           .then(function (data) {
           	vm.applications = data;
           	vm.loading = false;
           	usSpinnerService.stop('spinner');
           	return vm.applications;
           });
		}

		function getExpiredApplicationCount() {
			return vm.applications.length;
		}
	};
})();