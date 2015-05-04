(function () {
    'use strict';

    angular
        .module('menudeldia')
        .controller('accountCtrl', account);

    account.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'authService'];

    function account($scope, $rootScope, $state, $stateParams, $timeout, authService) {

        //function definition
        $scope.signIn = signIn;
        $scope.logIn = logIn;

        $scope.signInSubmit = false;
        $scope.logInSubmit = false;


        //initialize controller
        activate();

        //functions

        function activate() {
            $scope.loadingSignIn = false;
            $scope.loadingLogIn = false;

            $scope.userSignIn = { email: '', password: '', validationPassword: '' };
            $scope.userLogIn = { email: '', password: '' };

            authService.fillAuthData();

            if (authService.authentication.isAuth) {
                $state.go('company');
            }
        }

        function signIn(isValid) {

            if (isValid == false) {
                $scope.signInSubmit = true;
                return;
            }

            $scope.loadingSignIn = true;

            authService.signIn($scope.userSignIn)
                       .then(function (resultRegister) {
                           debugger;
                           authService.login({ userName: $scope.userSignIn.email, password: $scope.userSignIn.password })
                                      .then(function (resultLogin) {
                                          $state.go('company');
                                          $scope.loadingSignIn = false;
                                      },
                                      function (result) {
                                          $scope.signInSubmit = false;
                                          $scope.loadingSignIn = false;
                                      });
                       },
                       function (resultRegister) {
                           $scope.signInSubmit = false;
                           $scope.loadingSignIn = false;
                       });
        }

        function logIn(isValid) {

            if (isValid == false) {
                $scope.logInSubmit = true;
                return;
            }

            $scope.loadingLogIn = true;
            authService.login({ userName: $scope.userLogIn.email, password: $scope.userLogIn.password })
                       .then(function (result) {
                           $state.go('company');
                           $scope.loadingLogIn = false;
                       },
                       function (result) {
                           $scope.logInSubmit = false;
                           $scope.loadingLogIn = false;
                       });
        }
    }
})();

