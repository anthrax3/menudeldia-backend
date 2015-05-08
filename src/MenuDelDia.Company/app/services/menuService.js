(function () {
    'use strict';

    angular
        .module('menudeldia')
        .factory('menuService', menuService);

    menuService.$inject = ['$q', '$http'];

    function menuService($q, $http) {
        var service = {
            addMenu:addMenu,
            updateMenu:updateMenu,
            menus:menus
        };

        return service;

        function menus() {
            var deferred = $q.defer();

            //call webapi service
            $http.get('http://localhost:45291/api/site/menus/')
                .success(function (data, status, headers, config) { deferred.resolve(data); })
                .error(function (data, status, headers, config) { deferred.reject({ data: data, status: status }); });

            return deferred.promise;
        }

        function addMenu(menu) {
            var deferred = $q.defer();

            //call webapi service
            $http.post('http://localhost:45291/api/site/menu', menu)
                .success(function (data, status, headers, config) { deferred.resolve(data); })
                .error(function (data, status, headers, config) { deferred.reject({ data: data, status: status }); });

            return deferred.promise;
        }

        function updateMenu(menu) {
            var deferred = $q.defer();

            //call webapi service
            $http.post('http://localhost:45291/api/site/updatemenu', menu)
                .success(function (data, status, headers, config) { deferred.resolve(data); })
                .error(function (data, status, headers, config) { deferred.reject({ data: data, status: status }); });

            return deferred.promise;
        }
    }
})();

//(function () {
//    'use strict';

//    angular
//        .module('menudeldia')
//        .factory('menuService', menuService);

//    menuService.$inject = ['$q', '$http'];

//    function menuService($q, $http) {
//        var service = {
//            addMenu: addMenu,
//            updateMenu: updateMenu,
//            menus: menus
//        };

//        return service;

//        function menus() {
//            var deferred = $q.defer();

//            //call webapi service
//            $http.get('http://mddservice.azurewebsites.net/api/site/menus/')
//                .success(function (data, status, headers, config) { deferred.resolve(data); })
//                .error(function (data, status, headers, config) { deferred.reject({ data: data, status: status }); });

//            return deferred.promise;
//        }

//        function addMenu(menu) {
//            var deferred = $q.defer();

//            //call webapi service
//            $http.post('http://mddservice.azurewebsites.net/api/site/menu', menu)
//                .success(function (data, status, headers, config) { deferred.resolve(data); })
//                .error(function (data, status, headers, config) { deferred.reject({ data: data, status: status }); });

//            return deferred.promise;
//        }

//        function updateMenu(menu) {
//            var deferred = $q.defer();

//            //call webapi service
//            $http.post('http://mddservice.azurewebsites.net/api/site/updatemenu', menu)
//                .success(function (data, status, headers, config) { deferred.resolve(data); })
//                .error(function (data, status, headers, config) { deferred.reject({ data: data, status: status }); });

//            return deferred.promise;
//        }
//    }
//})();