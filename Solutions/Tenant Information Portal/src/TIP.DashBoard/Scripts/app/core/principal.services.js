// dataservice factory
(function () {
    'use strict';

    var controllerId = 'PrincipalDataService';

    angular
        .module('app.core')
        .factory('PrincipalDataService', PrincipalDataService);

    PrincipalDataService.$inject = ['$http', '$q', 'exception', 'logger', '$log'];

    function PrincipalDataService($http, $q, exception, logger, $log) {
        var isPrimed = false;
        var primePromis;
        
        var service = {
            getAllExpiredPrincipals: getAllExpiredPrincipals,
            getExpiredPrincipalsInDays: getExpiredPrincipalsInDays,
            getAllPrincipals: getAllPrincipals,
            ready: ready
        };

        return service;

        function getAllPrincipals() {
            logger.info('Getting all principals');
            return $http.get('/api/servicePrincipals')
               .then(getAllPrincipalsComplete)
               .catch(getAllPrincipalsFailed);

            function getAllPrincipalsComplete(response) {
                logger.success(controllerId, response, 'Principals');
                return response.data;
            }

            function getAllPrincipalsFailed(error) {
                logger.error('Failed getting all principals.' + error)
            }
        }

        function getExpiredPrincipalsInDays(days) {
            logger.info('Getting expired principals in ' + days );
            return $http.get('/api/servicePrincipal/getExpired/' + days)
                .then(getAllExpiredPrincipalsInDaysComplete)
                .catch(getAllExpiredPrincipalsInDaysFailed);
        }

        function getAllExpiredPrincipalsInDaysComplete(response) {
            logger.success(controllerId, response, 'Expired Principals');
            return response.data;
        }

        function getAllExpiredPrincipalsInDaysFailed(error) {
            logger.error('Failed getting expired princiapls.' + error)
        }

        function getAllExpiredPrincipals() {     
            logger.info('Getting all expired principals');
            return $http.get('/api/servicePrincipal/getAllExpired')
                .then(getAllExpiredPrincipalsComplete)
                .catch(getAllExpiredPrincipalsFailed);

            function getAllExpiredPrincipalsComplete(response) {
                logger.success(controllerId, response, 'Expired Principals');
                return response.data;
            }

            function getAllExpiredPrincipalsFailed(error) {
                logger.error('Failed getting all expired principals. ' + error.data.Error.message)
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