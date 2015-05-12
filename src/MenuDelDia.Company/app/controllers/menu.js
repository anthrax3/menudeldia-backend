(function () {
    'use strict';

    angular
        .module('menudeldia')
        .controller('menuCtrl', menu);

    menu.$inject = ['$scope', '$rootScope', '$stateParams', '$timeout', 'menuService', 'restaurantMenus', 'authService', 'toaster'];

    function menu($scope, $rootScope, $stateParams, $timeout, menuService, restaurantMenus, authService, toaster) {

        $scope.save = save;
        $rootScope.isAuth = isAuth;

        activate();

        function activate(){
            $rootScope.enabledStores = true;
            $rootScope.enabledMenu= true;
            $scope.loadingSave = false;
            loadMenu();
        }

        function loadMenu(){

            var menus  = [
                {
                    dayOfWeek: 1,
                    name: "Lunes",
                    menus: [],
                    isDayOpen: true
                },
                {
                    dayOfWeek: 2,
                    name: "Martes",
                    menus: [],
                    isDayOpen: true
                },
                {
                    dayOfWeek: 3,
                    name: "Mi\u00e9rcoles",
                    menus: [],
                    isDayOpen: true
                },
                {
                    dayOfWeek: 4,
                    name: "Jueves",
                    menus: [],
                    isDayOpen: true
                },
                {
                    dayOfWeek: 5,
                    name: "Viernes",
                    menus: [],
                    isDayOpen: true
                },
                {
                    dayOfWeek: 6,
                    name: "S\u00e1bado",
                    menus: [],
                    isDayOpen: true
                },
                {
                    dayOfWeek: 7,
                    name: "Domingo",
                    menus: [],
                    isDayOpen: true
                }
            ];

            $scope.week = _.map(menus, function(item){

                var menusFromDay = _.where(restaurantMenus.menus,{dayOfWeek:item.dayOfWeek});
                if(menusFromDay.length)
                {debugger;
                    var menuFromDay = menusFromDay[0];

                    item.menus = menuFromDay.menus;

                    var cit = 3 - menuFromDay.menus.length;

                    for(var i=0; i < cit; i++){
                        item.menus.push(
                            {name: '', price: 0, description: ''}
                        );
                    }
                }
                else{
                    for(var i=0; i < 3; i++){
                        item.menus.push({name: '', price: 0, description: ''});
                    }
                }

                return item;
            });
        }

        function save(){
            $scope.loadingSave = true;
            menuService.addMenu(
                {
                    menus:$scope.week
                }).then(
                function(){
                    $scope.loadingSave = false;
                    toaster.success("\u00c9xito", "El menu\u00fa se ha guardado correctamente.", 4000, 'trustedHtml');
            });


            $timeout(function(){
                $scope.loadingSave = false;
            }, 3000)
        }

        function isAuth() {
            return authService.authentication.isAuth;
        }

    }
})();

