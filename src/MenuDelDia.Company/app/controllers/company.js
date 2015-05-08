(function () {
    'use strict';

    angular
        .module('menudeldia')
        .controller('companyCtrl', company);

    company.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'FileUploader', 'companyService', 'configService', 'authService', 'companyInfo', 'tags', 'localStorageService', 'toaster', 'helperService', '$log'];

    function company($scope, $rootScope, $state, $stateParams, $timeout, FileUploader, companyService, configService, authService, companyInfo, tags, localStorageService, toaster, helperService, $log) {

        //function definition
        $scope.nextStep = nextStep;
        $scope.saveCompany = saveCompany;
        $rootScope.isAuth = isAuth;
        $scope.companySubmit = false;
        //initialize controller
        activate();

        //functions

        function activate() {
            $scope.existingCompany = false;
            $scope.loadingSave = false;
            $scope.loadingNextStep = false;
            $scope.showNextStep = false;

            authService.fillAuthData();

            initImageUpload();

            $scope.tags = tags;
            $scope.company = companyInfo;

            _.map($scope.tags, function (item) {
                item.selected = _.indexOf($scope.company.tags, item.id) !== -1;
                return item;
            });

            if (authService.authentication.isAuth == false) { $state.go('account'); }


            $rootScope.enabledStores = true;
            $rootScope.enabledMenu = true;
            $scope.existingCompany = true;

        }

        function uploadImage() {
            $scope.uploader.queue[0].upload(); //Manage errors
        }

        function isCompanyValid(isValid) {
            if (isValid === false) {
                $scope.companySubmit = true;
                toaster.error("Campos incompletos", "Completa los campos marcados en rojo para continuar", 5000, 'trustedHtml');
                return false;
            }

            if ($scope.company.tags.length === 0) {
                toaster.error("Ha ocurrido un error", "Debes seleccionar al menos una característica", 5000, 'trustedHtml');
                return false;
            }
            if ($scope.company.tags.length > 3) {
                toaster.error("Ha ocurrido un error", "Debes seleccionar como máximo 3 características", 5000, 'trustedHtml');
                return false;
            }
            if ($scope.uploader.queue.length == 0) {
                toaster.error("Logo de tu empresa", "Debes subir un logo de tu empresa", 5000, 'trustedHtml');
                return false;
            }
            return true;
        }

        function saveCompany(isValid) {
            $scope.company.tags = _.pluck(_.filter($scope.tags, function (i) { return i.selected }), 'id');

            if (!isCompanyValid(isValid))
                return;

            $scope.loadingSave = true;

            companyService.save($scope.company)
                .then(
                    function (result) {
                        if ($scope.uploader.queue.length != 0) {
                            uploadImage();
                        } else {
                            $scope.loadingSave = false;
                        }
                    },
                    function (result) {
                        helperService.processError(result, toaster);
                    });
        }

        function nextStep(isValid) {

            $scope.company.tags = _.pluck(_.filter($scope.tags, function (i) { return i.selected }), 'id');

            if (!isCompanyValid(isValid))
                return;

            $scope.loadingNextStep = true;
            
            companyService.save($scope.company)
                .then(
                    function (result) {
                        if ($scope.uploader.queue.length != 0) {
                            //uploadImage();
                            $state.go('stores');
                            $scope.loadingNextStep = false;
                        } else {
                            $scope.loadingNextStep = false;
                        }
                    },
                    function (result) {

                    });
            
        }

        function initImageUpload() {
            //Image upload
            var authData = localStorageService.get('authorizationData');
            $scope.uploader = new FileUploader({
                url: "http://localhost:45291/api/site/file/upload",
                headers: { Authorization: 'Bearer ' + authData.token }
            });

            $scope.uploader.onSuccessItem = function (item, response, status, headers) {
                $scope.loadingSave = false;
                //$state.reload();
            }
            $scope.uploader.onErrorItem = function (item, response, status, headers) {
                $scope.loadingSave = false;
                ///$state.reload();
                helperService.processError(response, toaster);
            }
            $scope.uploader.onCompleteItem = function (item, response, status, headers) {
                $scope.loadingSave = false;
                //$state.reload();
            }
            $scope.uploader.filters.push({
                name: 'imageFilter',
                fn: function (item, options) {
                    var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                    return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1;
                }
            });
        }

        function isAuth() {
            return authService.authentication.isAuth;
        }

        $scope.$watch('tags', function() {

            var tags = _.pluck(_.filter($scope.tags, function (i) { return i.selected }), 'id');
            if (tags.length === 3)
                $scope.tagsDisabled = true;
            else
                $scope.tagsDisabled = false;

        },true);
    }
})();

