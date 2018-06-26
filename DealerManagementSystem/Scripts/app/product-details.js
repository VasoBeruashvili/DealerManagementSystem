var app = angular.module('product-details-app', []);

app.controller('product-details-ctrl', function ($scope, $http) {
    $('#overlay').hide();

    $scope.updateShoppingCart = function (product) {
        var ord = $('#ordered').val();

        if (!isUndefinedNullOrEmpty(ord)) {
            var ordered = parseInt(ord);            

            if (ordered > 0 && ordered <= product.total) {
                var shoppingCart = {
                    productId: product.id,
                    quantity: ordered
                }

                $('#overlay').show();
                $http.post('/home/updateShoppingCart', { shoppingCart: shoppingCart }).then(function (resp) {
                    if (resp.data) {
                        window.location.reload();
                    }
                });
            }
        }
    }

    $scope.addToShoppingCart = function (product) {
        var ord = $('#ordered').val();

        if (!isUndefinedNullOrEmpty(ord)) {
            var productQuantity = parseInt(ord);

            if (productQuantity > 0 && productQuantity <= product.stock) {
                var shoppingCart = {
                    productId: product.id,
                    quantity: productQuantity
                }

                $('#overlay').show();
                $http.post('/home/addToShoppingCart', { shoppingCart: shoppingCart }).then(function (resp) {
                    window.location.reload();
                });
            }
        }
    }
});