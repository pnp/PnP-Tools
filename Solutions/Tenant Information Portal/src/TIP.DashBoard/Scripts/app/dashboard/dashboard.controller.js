/*
*
*/
(function () {
    'use strict';
    var controllerId = 'dashboard';

    angular
        .module('app.dashboard')
        .controller('DashBoardController', DashBoardController);
       
    DashBoardController.$inject = ['$q', 'PrincipalDataService', 'TenantDataService', 'ApplicationDataService', 'logger', '$log', '$translate'];

    function DashBoardController($q, PrincipalDataService, TenantDataService, ApplicationDataService, logger, $log, $translate) {
        var vm = this;
        vm.expiredPrincipals = [];
        vm.expiredPrincipals30 = [];
        vm.expiredPrincipals60 = [];
        vm.expiredPrincipals90 = [];

        vm.expiredApplications = [];
        vm.expiredApplications30 = [];
        vm.expiredApplications60 = [];
        vm.expiredApplications90 = [];
        vm.tenantInformation = [];

        vm.chartServicePrincipalsLabels = [];
        vm.chartApplicationsLabels = [];
        vm.chartServicePrincipalsData = [];
        vm.chartApplicationsData = [];
        vm.chartCharColors = ['#d73925', '#FDB45C', '#949FB1', '#5cb85c'];

        vm.getAllExpiredPrincipals = getAllExpiredPrincipals;
        vm.getExpiredPrincipals30Days = getExpiredPrincipals30Days;
        vm.getExpiredPrincipals60Days = getExpiredPrincipals60Days;
        vm.getExpiredPrincipals90Days = getExpiredPrincipals90Days;

        vm.getExpiredApplications = getAllExpiredApplications;
        vm.getExpiredApplications30Days = getExpiredApplications30Days;
        vm.getExpiredApplications60Days = getExpiredApplications30Days;
        vm.getExpiredApplications90Days = getExpiredApplications30Days;
        
        vm.getChartServicesPrincipalsLabels = getChartServicesPrincipalsLabels;
        vm.getApplicationChartLabels = getApplicationChartLabels;
        vm.getTenantInformation = getTenantInformation;

        activate();
        
        /**
        * @desc Activate the DashBoardController
        */
        function activate() {
            logger.info('Activating Dashboard');
            
            var promises = [
                getApplicationChartLabels(),
                getChartServicesPrincipalsLabels(),
                getTenantInformation(),
                getAllExpiredPrincipals(),
                getExpiredPrincipals30Days(),
                getExpiredPrincipals60Days(),
                getExpiredPrincipals90Days(),
                getAllExpiredApplications(),
                getExpiredApplications30Days(),
                getExpiredApplications60Days(),
                getExpiredApplications90Days()
            ];

            return $q.all(promises).then(function () {
                logger.info('Activated Dashboard View');
                vm.chartServicePrincipalsData = [
                    vm.expiredPrincipals.length,
                    vm.expiredPrincipals30.length,
                    vm.expiredPrincipals60.length,
                    vm.expiredPrincipals90.length
                ];

                vm.chartApplicationsData = [
                vm.expiredApplications.length,
                vm.expiredApplications30.length,
                vm.expiredApplications60.length,
                vm.expiredApplications90.length
                ];
            });
        }

        /**
        * @desc gets the TenantInformation
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
        * @desc Gets all the expired applications
        */
        function getAllExpiredApplications() {
            $log.info('Info ' + controllerId, 'Entering getExpiredApplications');
            return ApplicationDataService.getExpiredApplications()
           .then(function (data) {
               vm.expiredApplications = data;
               return vm.expiredApplications;
           });
        }

        /**
        *@desc Gets all Applications expiring in 30 days
        */
        function getExpiredApplications30Days() {
            $log.info('Info ' + controllerId, 'Entering getExpiredApplications30Days');
            return ApplicationDataService.getExpiredApplicationInDays(30)
            .then(function (data) {
                vm.expiredApplications30 = data;
                return vm.expiredApplications30;
            });
        }

        /**
        *@desc Gets all the applications expiring in 60 days
        */
        function getExpiredApplications60Days() {
            $log.info('Info ' + controllerId, 'Entering getExpiredApplications60Days');
            return ApplicationDataService.getExpiredApplicationInDays(60)
            .then(function (data) {
                vm.expiredApplications60 = data;
                return vm.expiredApplications60;
            });
        }

        /**
        *@desc Gets all the Applications expiring in 90 days
        */
        function getExpiredApplications90Days() {
            $log.info('Info ' + controllerId, 'Entering getExpiredApplications90Days');
            return ApplicationDataService.getExpiredApplicationInDays(90)
           .then(function (data) {
               vm.expiredApplications90 = data;
               return vm.expiredApplications90;
           });
        }

        /**
        *@desc Gets all the expired principals
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
        *@desc Gets all the Principals expiring in 30 days
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
        *@desc Gets all the Principals expiring in 60 days
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
        *@desc Gets all the Principals expiring in 90 days
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
        *@desc Translates the application chart labels
        */
        function getApplicationChartLabels() {
            $log.info('Info ' + controllerId, ' Entering getApplicationChartLabels');
            $translate('CHART_APPLICATION_LABEL')
                .then(function (translatedValue) {
                    vm.chartApplicationsLabels = translatedValue.split(",");
                    return vm.chartApplicationsLabels;
            });
        }

        /**
        *@desc Translates the sp chart labels
        */
        function getChartServicesPrincipalsLabels() {
            $log.info('Info ' + controllerId,  'Entering getChartServicesPrincipalsLabels');
            $translate('CHART_SERVICEPRINCIPALS_LABEL')
            .then(function (translatedValue) {
                vm.chartServicePrincipalsLabels = translatedValue.split(",");
            });
        }
    };

})();