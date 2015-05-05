(function () {
    'use strict';

    angular
        .module('menudeldia')
        .controller('companyCtrl', company);

    company.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'FileUploader', 'companyService', 'configService', 'authService', 'companyInfo','tags'];

    function company($scope, $rootScope, $state, $stateParams, $timeout, FileUploader, companyService, configService, authService, companyInfo,tags) {

        //function definition
        $scope.nextStep = nextStep;
        $scope.saveCompany = saveCompany;

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
            debugger;
            $scope.uploader.queue[0].upload(); //Manage errors
        }

        function saveCompany() {
            $scope.loadingSave = true;

            

            //set new tags
            $scope.company.tags = _.pluck(_.filter($scope.tags, function (i) { return i.selected }), 'id');
            
            companyService.save($scope.company)
                .then(
                    function (result) {
                        uploadImage();
                        $scope.loadingSave = false;
                    },
                    function(result) {

                    });
        }

        function nextStep() {
            $scope.loadingNextStep = true;
            $state.go('stores');
            $scope.loadingNextStep = false;
        }

        function initImageUpload() {
            //Image upload
            debugger;
            $scope.uploader = new FileUploader({
                url:"http://localhost:45291/api/site/file/upload"
            });

            $scope.uploader.onSuccessItem = function (item, response, status, headers) {
                
            }
            $scope.uploader.onErrorItem = function (item, response, status, headers) {

            }
            $scope.uploader.onCompleteItem = function (item, response, status, headers) {

            }
            $scope.uploader.filters.push({
                name: 'imageFilter',
                fn: function (item, options) {
                    var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                    return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1;
                }
            });
        }

        //        $scope.$watch('company', function() {
        //            $scope.showNextStep = ($scope.company.name != "")
        //                && ($scope.company.description != "")
        //                && ($scope.company.url != "")
        //                && ($scope.company.email != "")
        //                && ($scope.company.phone != "");
        //                //&& (!$scope.existingCompany
        //                //    && $scope.user.userName != "" //if first time also check for user and password
        //                //    && $scope.user.password != "");
        //
        //        },
        //        true);
    }
})();

