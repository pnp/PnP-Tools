(function () {
    'use strict';
    var controllerId = 'dashboard';

    angular
        .module('app.dashboard')
        .controller('DashBoardController', DashBoardController);
       
    DashBoardController.$inject = ['$q', 'PrincipalDataService', 'TenantDataService', 'logger', '$log', '$scope'];

    function DashBoardController($q, PrincipalDataService, TenantDataService, logger, $log, $scope) {
        var vm = this;
        vm.expiredPrincipals = [];
        vm.expiredPrincipals30 = [];
        vm.expiredPrincipals60 = [];
        vm.expiredPrincipals90 = [];
        vm.chartServicePrincipalsLabels = [];
        vm.chartServicePrincipalsData = [];
        vm.chartServicePrincipalsColours = [];
        vm.tenantInformation = [];

        vm.getAllExpiredPrincipals = getAllExpiredPrincipals;
        vm.getExpiredPrincipals30Days = getExpiredPrincipals30Days;
        vm.getExpiredPrincipals60Days = getExpiredPrincipals60Days;
        vm.getExpiredPrincipals90Days = getExpiredPrincipals90Days;
        vm.configureChartServicePrincipals = configureChartServicePrincipals;
        vm.getChartServicePrincipalData = getChartServicePrincipalData;
        vm.getTenantInformation = getTenantInformation;

        activate();
        
        /**
        * @desc Activate the DashBoardController
        */
        function activate() {
            logger.info('Activating Dashboard');
            configureChartServicePrincipals();
            getData();
        }

        /**
        *
        */
        function getData() {
            logger.info('Getting Principals');
            getTenantInformation();
            getAllExpiredPrincipals();
            getExpiredPrincipals30Days();
            getExpiredPrincipals60Days();
            getExpiredPrincipals90Days()
       } 
        /**
        *
        */
        function getTenantInformation() {
            $log.info('Info ' + controllerId, 'Entering getTenantInformation');
            return TenantDataService.getTenantInformation()
                   .then(function (data) {
                       vm.tenantInformation = data;
                       return vm.tenantInformation;
                   });
        }
        /**
        *
        */
        function getAllExpiredPrincipals() {
            $log.info('Info ' + controllerId, 'Entering getAllExpiredPrincipals');
            return PrincipalDataService.getAllExpiredPrincipals()
           .then(function (data) {
               vm.expiredPrincipals = data;
               return vm.expiredPrincipals;
           });
        }

        /**
        *
        */
        function getExpiredPrincipals30Days() {
            $log.info('Info ' + controllerId, 'Entering getExpiredPrincipals30Days');
            return PrincipalDataService.getExpiredPrincipalsInDays(30)
            .then(function (data) {
                vm.expiredPrincipals30 = data;
                return vm.expiredPrincipals30;
            });
        }

        /**
        *
        */
        function getExpiredPrincipals60Days() {
            $log.info('Info ' + controllerId, 'Entering getExpiredPrincipals60Days');
            return PrincipalDataService.getExpiredPrincipalsInDays(60)
            .then(function (data) {
                vm.expiredPrincipals60 = data;
                return vm.expiredPrincipals60;
            });
        }

        /**
        *
        */
        function getExpiredPrincipals90Days() {
            $log.info('Info ' + controllerId, 'Entering getExpiredPrincipals90Days');
            return PrincipalDataService.getExpiredPrincipalsInDays(90)
           .then(function (data) {
               vm.expiredPrincipals90 = data;
               return vm.expiredPrincipals90;
           });
        }

        /**
        *
        */
        function getChartServicePrincipalData() {
            vm.chartServicePrincipalsData = [vm.expiredPrincipals.length, 10, 15, 20];
            return vm.chartServicePrincipalsData;

        }

        /**
        *
        */
        function configureChartServicePrincipals() {
            vm.chartServicePrincipalsLabels = ["Expired Principals", "Expiring in 30 days", "Expiring in 60 days", "Expiring in 90 days"];
            vm.chartServicePrincipalsColours = ['#d73925', '#FDB45C', '#949FB1', '#5cb85c'];
            return vm.chartServicePrincipalsLabels;
        }
    };

})();