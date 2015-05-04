(function () {
    'use strict';

    angular
        .module('menudeldia')
        .factory('authService', authService);

    authService.$inject = ['$q', '$http', 'localStorageService', 'companyService'];

    function authService($q, $http, localStorageService) {
        var constAuthorizationData = 'authorizationData';

        var authentication = {
            isAuth: false,
            name: "",
            email: "",
        };

        var service = {
            authentication: authentication,
            saveRegistration: saveRegistration,
            login: login,
            logOut: logOut,
            fillAuthData: fillAuthData,
            isLoggedIn:isLoggedIn,
            signIn: signIn,
            //forgotPassword: forgotPassword,
        };

        return service;


        function saveRegistration(registration) {
            logOut();
            return $http.post('http://localhost:45291/token', registration).then(function (response) {
                return response;
            });
        };

        function login(loginData) {
            var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;
            var deferred = $q.defer();
            $http.post('http://localhost:45291/token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })

                .success(function (responseToken) {
                    if (localStorageService.isSupported) {
                        localStorageService.set(constAuthorizationData, {
                            token: responseToken.access_token,
                        });
                    } else {
                        localStorageService.cookie.set(constAuthorizationData,
                        {
                            token: responseToken.access_token,
                        });
                    }

                    authentication.isAuth = true;

                    deferred.resolve(responseToken);

                }).error(function (err, status, headers, config) {
                    logOut();
                    deferred.reject(err);
                });

            return deferred.promise;
        };

        function signIn(signInData) {
            var deferred = $q.defer();

            $http.post('http://localhost:45291/api/site/user/register', signInData)
                .success(function(response) { deferred.resolve(response); })
                .error(function(data, status, headers, config) {
                    deferred.reject({
                        data: data,
                        status: status,
                    });
                });
            return deferred.promise;
        };


        function logOut() {
            authentication.isAuth = false;
            authentication.userName = "";

            if (localStorageService.isSupported) {
                localStorageService.remove(constAuthorizationData);
            } else {
                localStorageService.cookie.remove(constAuthorizationData);
            }
        };


    
        function fillAuthData() {

            var authData;

            if (localStorageService.isSupported) {
                authData = localStorageService.get(constAuthorizationData);
            } else {
                authData = localStorageService.cookie.get(constAuthorizationData);
            }

            if (authData) {
                authentication.isAuth = true;
            }
        }

        function isLoggedIn() {
            fillAuthData();
            return authentication.isAuth;
        }
    }
})();