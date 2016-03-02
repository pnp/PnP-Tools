// dataservice factory
(function () {
    'use strict';

    var controllerId = 'ApplicationDataService';

    angular
        .module('app.core')
        .factory('ApplicationDataService', ApplicationDataService);

    ApplicationDataService.$inject = ['$http', '$q', 'exception', 'logger', '$log'];

    function ApplicationDataService($http, $q, exception, logger, $log) {
        var isPrimed = false;
        var primePromis;

        var service = {
            getAllApplications: getAllApplications,
            getExpiredApplicationInDays: getExpiredApplicationInDays,
            getExpiredApplications : getExpiredApplications,
            ready: ready
        };

        return service;

        function getAllApplications() {
            $log.info('Getting all applications');
            return $http.get('/api/applications')
               .then(getAllApplicationsComplete)
               .catch(getAllApplicationsFailed);

            function getAllApplicationsComplete(response) {
                return response.data;
            }

            function getAllApplicationsFailed(error) {
                $log.error(error.data.error.message, error.data, "All Applications Failed");
            }
        }

        function getExpiredApplicationInDays(days) {
            $log.info('Getting expired applications in ' + days);
            return $http.get('/api/applications/getExpired/' + days)
                .then(geExpiredApplicationsInDaysComplete)
                .catch(getExpiredApplicationsInDaysFailed);

            function geExpiredApplicationsInDaysComplete(response) {
                return response.data;
            }

            function getExpiredApplicationsInDaysFailed(error) {
                $log.error(error.data.error.message, error.data, "Get Expired Applications Failed");
            }
        }

        function getExpiredApplications() {
            $log.info('Getting all expired principals');
            return $http.get('/api/applications/expired')
                .then(getExpiredApplicationsComplete)
                .catch(getExpiredApplicationsFailed);

            function getExpiredApplicationsComplete(response) {
                return response.data;
            }

            function getExpiredApplicationsFailed(error) {
                //message,data, title
                $log.error(error.data.error.message, error.data, "All Expired Applications Failed");
            }
        }

        /*
        * 
        */
        function getMe(){
    
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