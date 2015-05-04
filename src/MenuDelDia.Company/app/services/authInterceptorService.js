(function () {
    'use strict';

    angular
        .module('menudeldia')
        .factory('authInterceptorService', authInterceptorService);

    authInterceptorService.$inject = ['$q', 'localStorageService', '$injector'];

    function authInterceptorService($q, localStorageService, $injector) {

        var authInterceptorServiceFactory = {};

        var service = {
            request: request,
            responseError: responseError
        };

        function request(config) {
            config.headers = config.headers || {};
            var authData = localStorageService.get('authorizationData');
            if (authData) {
                config.headers.Authorization = 'Bearer ' + authData.token;
            }
            return config;
        }

        function responseError(rejection) {
            if (rejection.status === 401) {
                //var $state = $injector.get("$state");
                //var toaster = $injector.get("toaster");
                //var authServices = $injector.get("authServices");

                //authServices.logOut();
                //toaster.info("¡Atención!", rejection.data, 10000, 'trustedHtml');
                //$state.go('/');
            }
            return $q.reject(rejection);
        }

        return service;
    }
})();