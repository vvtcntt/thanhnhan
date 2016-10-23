(function (app) {
    app.filter('statusFilters', function () {
        return function (input) {
            if (input == true)
                return 'Kích hoạt';
            else
                return 'Khóa';
        }
    });

})(angular.module('tnitshop.common'));