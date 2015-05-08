(function () {
    'use strict';

    angular
        .module('menudeldia')
        .factory('storesService', storesService);

    storesService.$inject = ['$q', '$http', 'appSettings'];

    function storesService($q, $http, appSettings) {
        var service = {
            addStore:addStore,
            updateStore:updateStore,
            stores:stores
        };

        return service;

        function stores() {
            var deferred = $q.defer();

            //call webapi service
            $http.get(appSettings.url + 'api/site/stores/')
                .success(function (data, status, headers, config) { deferred.resolve(data); })
                .error(function (data, status, headers, config) { deferred.reject({ data: data, status: status }); });

            return deferred.promise;
        }

        function addStore(store) {
            var deferred = $q.defer();

            //call webapi service
            $http.post(appSettings.url + 'api/site/store', store)
                .success(function (data, status, headers, config) { deferred.resolve(data); })
                .error(function (data, status, headers, config) { deferred.reject({ data: data, status: status }); });

            return deferred.promise;
        }

        function updateStore(store) {
            var deferred = $q.defer();

            //call webapi service
            $http.post(appSettings.url + 'api/site/updatestore', store)
                .success(function (data, status, headers, config) { deferred.resolve(data); })
                .error(function (data, status, headers, config) { deferred.reject({ data: data, status: status }); });

            return deferred.promise;
        }
    }
})();