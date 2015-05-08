(function () {
    'use strict';

    angular
        .module('menudeldia')
        .factory('configService', configService);

    configService.$inject = ['$q', '$http', 'appSettings'];

    function configService($q, $http, appSettings) {
        var service = {
            getTags: getTags
        };

        return service;

        function getTags() {
            var deferred = $q.defer();

            //call webapi service
            $http.get(appSettings.url + 'api/site/tags')
                .error(function(data, status, headers, config) { deferred.reject({ data: data, status: status }); })
                .success(function(data, status, headers, config) { deferred.resolve(data); });

            return deferred.promise;
        }
    }
})();