// TenantDataService factory
(function () {
	'use strict';

	var controllerId = 'TenantDataService';

	angular
        .module('app.core')
        .factory('TenantDataService', PrincipalDataService);

	PrincipalDataService.$inject = ['$http', '$q', 'exception', 'logger', '$log'];

	function PrincipalDataService($http, $q, exception, logger, $log) {
		var isPrimed = false;
		var primePromis;

		var service = {
			getTenantInformation: getTenantInformation,
			ready: ready
		};

		return service;

		function getTenantInformation() {
			logger.info('Getting Tenant Inforomation');
			return $http.get('/api/tenant')
               .then(getTenantInfoComplete)
               .catch(getTenantInfoFailed);

		    function getTenantInfoComplete(response) {
                logger.success(controllerId, response, 'TenantInfo');
		        return response.data;
			}

			function getTenantInfoFailed(error) {
				logger.error('Failed getting Tenant Details.' + error)
			}
		}

		function prime() {
			// This function can only be called once.
			if (primePromise) {
				return primePromise;
			}

			primePromise = $q.when(true).then(success);
			return primePromise;

			function success() {
				isPrimed = true;
				logger.info('Primed data');
			}
		}

		function ready(nextPromises) {
			var readyPromise = primePromise || prime();

			return readyPromise
                .then(function () { return $q.all(nextPromises); })
                .catch(exception.catcher('"ready" function failed'));
		}
	}
})();