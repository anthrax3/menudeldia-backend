(function () {
    'use strict';

    angular
        .module('menudeldia')
        .factory('helperService', helperService);

    helperService.$inject = ['$q', '$http'];

    function helperService($q, $http) {
        var service = {
            processError: processError
        };

        function processError(result, toaster) {
            if (result.status == 400) {
                toaster.error("Ha ocurrido un error", result.data, 10000, 'trustedHtml');
            }
            else if (result.status == 401) {
                //el error es procesado a nivel de authInterceptorService
            }
            else if (result.status == 403) {
                toaster.info("¡Atención!", result.data, 10000, 'trustedHtml');
            }
            else {
                toaster.error("Ha ocurrido un error", result.data, 10000, 'trustedHtml');
            }
        }

        return service;

    }
})();