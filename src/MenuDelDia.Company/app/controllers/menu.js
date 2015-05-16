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

        function activate() {
            $rootScope.enabledStores = true;
            $rootScope.enabledMenu = true;
            $scope.loadingSave = false;
            loadMenu();
        }

        function loadMenu() {
            var menusTemplate = [
                { dayOfWeek: 1, name: "Lunes", },
                { dayOfWeek: 2, name: "Martes" },
                { dayOfWeek: 3, name: "Mi\u00e9rcoles" },
                { dayOfWeek: 4, name: "Jueves" },
                { dayOfWeek: 5, name: "Viernes" },
                { dayOfWeek: 6, name: "S\u00e1bado" },
                { dayOfWeek: 7, name: "Domingo" }
            ];

            var resultArray = [];

            _.each(menusTemplate, function (item) {

                var menusFromDay = _.where(restaurantMenus.menus, { dayOfWeek: item.dayOfWeek });
                if (menusFromDay.length) {
                    var menuFromDay = menusFromDay[0];
                    var menusForDay = menuFromDay.menus;
                    var cit = 3 - menuFromDay.menus.length;

                    for (var i = 0; i < cit; i++) { menusForDay.push({ name: '', price: 0, description: '' }); }

                    resultArray.push({
                        dayOfWeek: item.dayOfWeek,
                        name: item.name,
                        menus: menusForDay,
                        isDayOpen: true
                    });
                }
            });

            $scope.week = resultArray;
        }

        function save() {
            $scope.loadingSave = true;
            menuService.addMenu(
                {
                    menus: $scope.week
                }).then(
                function () {
                    $scope.loadingSave = false;
                    toaster.success("\u00c9xito", "El menu\u00fa se ha guardado correctamente.", 4000, 'trustedHtml');
                });


            $timeout(function () {
                $scope.loadingSave = false;
            }, 3000)
        }

        function isAuth() {
            return authService.authentication.isAuth;
        }

    }
})();

