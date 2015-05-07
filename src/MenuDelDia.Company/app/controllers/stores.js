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


        $scope.hours = [
            '00:00',
            '00:30',
            '01:00',
            '01:30',
            '02:00',
            '02:30',
            '03:00',
            '03:30',
            '04:00',
            '04:30',
            '05:00',
            '05:30',
            '06:00',
            '06:30',
            '07:00',
            '07:30',
            '08:00',
            '08:30',
            '09:00',
            '09:30',
            '10:00',
            '10:30',
            '11:00',
            '11:30',
            '12:00',
            '12:30',
            '13:00',
            '13:30',
            '14:00',
            '14:30',
            '15:00',
            '15:30',
            '16:00',
            '16:30',
            '17:00',
            '17:30',
            '18:00',
            '18:30',
            '19:00',
            '19:30',
            '20:00',
            '20:30',
            '21:00',
            '21:30',
            '22:00',
            '22:30',
            '23:00',
            '23:30'
        ];


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
            var s = $scope.store;
            debugger;
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
                    { dayOfWeek:1, name: 'Lunes', from: '09:00', to:'22:00', open: true},
                    { dayOfWeek: 2, name: 'Martes', from: '09:00', to: '22:00', open: true },
                    { dayOfWeek: 3, name: 'Miércoles', from: '09:00', to: '22:00', open: true },
                    { dayOfWeek: 4, name: 'Jueves', from: '09:00', to: '22:00', open: true },
                    { dayOfWeek: 5, name: 'Viernes', from: '09:00', to: '22:00', open: true },
                    { dayOfWeek: 6, name: 'Sábado', from: '09:00', to: '22:00', open: true },
                    { dayOfWeek: 0, name: 'Domingo', from: '09:00', to: '22:00', open: true }
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

