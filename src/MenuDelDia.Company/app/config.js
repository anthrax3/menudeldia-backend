angular
    .module("menudeldia")
    .config(function ($stateProvider, $urlRouterProvider, $httpProvider, uiGmapGoogleMapApiProvider, localStorageServiceProvider) {

        $urlRouterProvider.otherwise('account');

        $stateProvider
            .state('account', {
                url: '/account',
                templateUrl: 'app/templates/account.html',
                controller: 'accountCtrl'
            })
            .state('account-logout', {
                url: '/account/logout',
                templateUrl: 'app/templates/account.html',
                controller: 'accountCtrl',
                resolve: {
                    logoff: function (authService) { return authService.logOut(); }
                }
            })
            .state('company', {
                url: '/company',
                templateUrl: 'app/templates/company.html',
                controller: 'companyCtrl',
                resolve: {
                    companyInfo: function (companyService) {
                        return companyService.getCompanyName();
                    },
                    tags: function (configService) {
                        return configService.getTags();
                    }
                }
            })
            .state('stores', {
                url: '/stores',
                templateUrl: 'app/templates/stores.html',
                controller: 'storesCtrl',
                resolve: {
                    companyInfo: function (companyService) {
                        return companyService.getCompanyName();
                    },
                    stores: function (storesService) {
                        return storesService.stores();
                    }
                }
            })
            .state('menu', {
                url: '/menu',
                templateUrl: 'app/templates/menu.html',
                controller: 'menuCtrl',
                resolve: {
                    restaurantMenus: function (menuService) {
                        return menuService.menus();
                    }
                }
            });

        uiGmapGoogleMapApiProvider.configure({
            //    key: 'your api key',
            v: '3.17',
            libraries: 'weather,geometry,visualization'
        });
        localStorageServiceProvider.setPrefix('mdd');
        localStorageServiceProvider.setStorageType('localStorage');
        localStorageServiceProvider.setNotify(true, true);
        $httpProvider.interceptors.push('authInterceptorService');
    })
    .run(function ($rootScope,$state, authService) {
        $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
            document.body.scrollTop = document.documentElement.scrollTop = 0;
        });

        $rootScope.$on('$stateChangeStart', function (event,toState,a,b,c) {
            debugger;
            
            if (toState.name != 'account' && authService.isLoggedIn() == false)
                $state.go("account");

        });

    });