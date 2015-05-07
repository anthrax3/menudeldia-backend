(function () {
    'use strict';

    angular
        .module('menudeldia')
        .controller('storesCtrl', storesCtrl);

    storesCtrl.$inject = [
        '$scope', '$rootScope', '$state', '$stateParams', 'companyService', '$timeout', '$log', 'companyInfo', 'storesService', 'stores', 'helperService', 'toaster'];

    function storesCtrl($scope, $rootScope, $state, $stateParams, companyService, $timeout, $log, companyInfo, storesService, stores, helperService, toaster) {

        $scope.showStore = showStore;
        $scope.addStore = addStore;
        $scope.save = save;
        $scope.nextStep = nextStep;
        $scope.toggleOpenDay = toggleOpenDay;
        $scope.storeSubmit = false;

        $scope.companyName = companyInfo.name;

        activate();

        function activate(){
            $rootScope.enabledStores = true;
            $scope.loadingSave = false;
            $scope.loadingNextStep = false;
            initMap();
            loadCompanyStores();
        }

        function loadCompanyStores(){
            $scope.stores = stores;

            $scope.showForm = false;
            if($scope.stores.length == 0) {
                $scope.showForm = true;
                $scope.store = newStore();
            }
        }

        function addStore() {
            $scope.store = newStore();
            $scope.showForm = true;
        }

        function showStore(store){
            $scope.showForm = true;
            $scope.store = {
                id: store.id,
                identifier: store.identifier,
                zone: store.zone,
                address: store.address,
                phone: store.phone,
                features: store.features,
                delivery: store.delivery,
                days : store.days,
                location: store.location
            };

            $scope.markerOn = true;
            $scope.marker = {
                id: 0,
                coords: {
                    latitude: store.location.latitude,
                    longitude: store.location.longitude
                },
                options: { draggable: true }
            };
        }

        function save(isValid){
            $scope.loadingSave = true;
            if (!isStoreValid(isValid)) {
                $scope.loadingSave = false;
                return;
            }

            $scope.store.location.latitude = $scope.marker.coords.latitude;
            $scope.store.location.longitude = $scope.marker.coords.longitude;

            if($scope.store.id == null){
                storesService.addStore($scope.store)
                    .then(
                    function(result) {
                        $scope.store.id = result.id;
                        $scope.stores.push($scope.store);
                        
                        $scope.loadingSave = false;
                        $scope.showForm = false;

                        //clear marker
                        $scope.marker = null;
                        $scope.markerOn = false;
                    },
                    function(result){
                        $scope.loadingSave = false;
                    });
            }
            else{
                storesService.updateStore($scope.store)
                    .then(
                    function(resultUpdate) {
                        storesService.stores($stateParams.id).then(
                            function (resultStores){
                                $scope.stores = resultStores;
                                $scope.loadingSave = false;
                                $scope.showForm = false;

                                //clear marker
                                $scope.marker = null;
                                $scope.markerOn = false;
                        },
                            function(){
                                $scope.loadingSave = false;
                                $scope.showForm = false;

                                //clear marker
                                $scope.marker = null;
                                $scope.markerOn = false;
                        });
                    },
                    function(result){
                        $scope.loadingSave = false;
                    });
            }
        }

        function nextStep(isValid){
            $scope.loadingNextStep = true;
            $state.go('menu');
            $scope.loadingNextStep = false;
        }

        function toggleOpenDay(day){
            day.open = !day.open;
        }

        function initMap(){
            $scope.map = {
                center: { latitude: -34.8976001, longitude: -56.1419506 },
                zoom: 13,
                events: {
                    click: function (map, ev, ev2) {
                        $scope.$apply(function () {
                            $scope.mapInstance = map;
                            $scope.markerOn = true;
                            $scope.marker = {
                                id: 0,
                                coords: {
                                    latitude: ev2[0].latLng.lat(),
                                    longitude: ev2[0].latLng.lng()
                                },
                                options: { draggable: true }
                            };
                            $scope.store.location.latitude = ev2[0].latLng.lat();
                            $scope.store.location.longitude = ev2[0].latLng.lng();
                        });
                    }
                }};
        }

        function newStore(){
            return {
                identifier: '',
                zone: '',
                address: '',
                phone: '',
                features: '',
                delivery: true,
                days : [
                    { dayOfWeek:1, name: 'Lunes', from: '', to:'', open: true},
                    { dayOfWeek:2, name: 'Martes', from: '', to:'', open: true},
                    { dayOfWeek:3, name: 'Miércoles', from: '', to:'', open: true},
                    { dayOfWeek:4, name: 'Jueves', from: '', to:'', open: true},
                    { dayOfWeek:5, name: 'Viernes', from: '', to:'', open: true},
                    { dayOfWeek:6, name: 'Sábado', from: '', to:'', open: true},
                    { dayOfWeek:0, name: 'Domingo', from: '', to:'', open: true}
                ],
                location: {
                    latitude: null,
                    longitude: null
                }
            };
        }

        function isStoreValid(isValid) {
            if (isValid === false) {
                $scope.storeSubmit = true;
                toaster.error("Campos incompletos", "Completa los campos marcados en rojo para continuar", 5000, 'trustedHtml');
                return false;
            }
            if ($scope.marker == undefined) {
                toaster.error("Falta ubicacion", "Selecciona la ubicacion del local en el mapa", 5000, 'trustedHtml');
                return false;
            }

            if (_.every($scope.store.days, { open: false })) {
                toaster.error("No hay dias abierto", "No abres ningun dia! No podemos cargar tu menu semanal :)", 5000, 'trustedHtml');
                return false;
            }

            /*
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
            }*/
            return true;
        }

        //$scope.$watch('stores', function() {
        //        $scope.showNextStep =
        //            ($scope.showForm &&
        //                (
        //                    $scope.store != null &&
        //                    $scope.store.zone != '' &&
        //                    $scope.store.address != '' &&
        //                    $scope.store.phone != '' &&
        //                    ($scope.store.location != null &&
        //                        $scope.store.location.latitude != null &&
        //                        $scope.store.location.longitude != null)
        //                    )
        //                ) ||
        //            (!$scope.showForm && ($scope.stores != null && $scope.stores.length > 0));
        //},
        //true);

        //$scope.$watch('store', function() {
        //        $scope.showNextStep =
        //            ($scope.showForm &&
        //            (
        //                $scope.store != null &&
        //                $scope.store.zone != '' &&
        //                $scope.store.address != '' &&
        //                $scope.store.phone != '' &&
        //                ($scope.store.location != null &&
        //                    $scope.store.location.latitude != null &&
        //                    $scope.store.location.longitude != null)
        //                )
        //             ) ||
        //            (!$scope.showForm && ($scope.stores != null && $scope.stores.length > 0));
        //    },
        //    true);
    }
})();

