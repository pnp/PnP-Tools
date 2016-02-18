(function () {
    'use strict';
    var controllerId = 'principal';

    angular
        .module('app.principal')
        .controller('PrincipalController', PrincipalController);

    PrincipalController.$inject = ['PrincipalDataService', 'logger', '$log', '$scope'];

    function PrincipalController(PrincipalDataService, logger, $log, $scope) {
        var vm = this;
        vm.expiredPrincipals = [];
        vm.expiredPrincipals30 = [];
        vm.expiredPrincipals60 = [];
        vm.expiredPrincipals90 = [];

        //Pagination
        $scope.currentPage = 1;
        $scope.pageSize = 5,

        vm.getAllExpiredPrincipals = getAllExpiredPrincipals;
        vm.getExpiredPrincipals30Days = getExpiredPrincipals30Days;
        vm.getExpiredPrincipals60Days = getExpiredPrincipals60Days;
        vm.getExpiredPrincipals90Days = getExpiredPrincipals90Days;

        activate();

        function activate() {
            logger.info('Activating Principals');
        }

        function getAllExpiredPrincipals() {
            $log.info('Info ' + controllerId, 'Entering getAllExpiredPrincipals');
            return PrincipalDataService.getAllExpiredPrincipals()
           .then(function (data) {
               vm.expiredPrincipals = data;
               return vm.expiredPrincipals;
           });
        }
        
        function getExpiredPrincipals30Days() {
            $log.info('Info ' + controllerId, 'Entering getExpiredPrincipals30Days');
            return PrincipalDataService.getExpiredPrincipalsInDays(30)
            .then(function (data) {
                vm.expiredPrincipals30 = data;
                return vm.expiredPrincipals30;
            });
        }

        function getExpiredPrincipals60Days() {
            $log.info('Info ' + controllerId, 'Entering getExpiredPrincipals60Days');
            return PrincipalDataService.getExpiredPrincipalsInDays(60)
            .then(function (data) {
                vm.expiredPrincipals60 = data;
                return vm.expiredPrincipals60;
            });
        }

        function getExpiredPrincipals90Days() {
            $log.info('Info ' + controllerId, 'Entering getExpiredPrincipals90Days');
            return PrincipalDataService.getExpiredPrincipalsInDays(90)
           .then(function (data) {
               vm.expiredPrincipals90 = data;
               return vm.expiredPrincipals90;
           });
        }

    };
})();