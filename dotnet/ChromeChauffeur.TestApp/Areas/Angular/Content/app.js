var app = angular.module("cc", ["ngRoute"]);

app.config(['$routeProvider', function ($routeProvider) {
    $routeProvider
        .when('/items/', {
            templateUrl: cc.url.createContent('Content/views/list.html'),
            controller: 'itemsController'
        })
        .when('/items/add', {
            templateUrl: cc.url.createContent('Content/views/add.html'),
            controller: 'addItemController'
        })
        .otherwise({
            redirectTo: '/items'
        });;
}]);

angular.module("cc").controller("itemsController", function ($scope, $http) {

    $http.get(cc.url.create('api/angulartodo')).success(function (items) {
        $scope.items = items;
    });
});

angular.module("cc").controller("addItemController", function ($scope, $http, $location) {

    $scope.item = {};

    $scope.add = function () {
        $http.post(cc.url.create("api/angulartodo/"), $scope.item)
        .success(function () {
            $location.path("/items");
        }).error(function () {

        });
    }
});
