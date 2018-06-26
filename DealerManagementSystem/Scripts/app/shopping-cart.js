var app = angular.module('shoppingCart-app', []);

var contragentShoppingCartAmountSum = $('#shoppingCart').text();

app.controller('shoppingCart-ctrl', function ($scope, $http) {
    $('#overlay').hide();

    $scope.init = function () {
        $('#overlay').show();
        $http.get('/home/getContragentShoppingCartProducts').then(function (resp) {
            $scope.products = resp.data.products;
            setProductRowClass($scope.products);
            setProductStockCellClass($scope.products);

            $scope.subTotalSum = formatNumber(resp.data.subTotalSum);

            angular.forEach($scope.products, function (p) {
                p.price = formatNumber(p.price);
                p.totalSum = formatNumber(p.totalSum);
            });


            $http.get('/home/getContragentAddresses').then(function (resp) {
                $scope.contragentAddresses = resp.data;
                $scope.selectedContragentAddress = $scope.contragentAddresses[0];

                $('#overlay').hide();
            });            
        });
    }

    $scope.updateShoppingCart = function (product) {
        $('#overlay').show();

        var productOrdered = parseInt(product.ordered);

        if (productOrdered > 0 && productOrdered <= product.total) {
            var shoppingCart = {
                productId: product.id,
                quantity: productOrdered
            }

            $http.post('/home/updateShoppingCart', { shoppingCart: shoppingCart }).then(function (resp) {
                if (resp.data) {
                    window.location.reload();
                }
            });
        }
    }

    $scope.removeShoppingCart = function (product) {
        $('#overlay').show();

        $http.post('/home/removeShoppingCart', { productId: product.id }).then(function (resp) {
            window.location.reload();
        });
    }

    $scope.placeOrder = function () {
        $('#overlay').show();

        $http.post('/home/placeOrder', { contragentAddressId: $scope.selectedContragentAddress.id }).then(function (resp) {
            if (resp.data) {
                alert('შეკვეთა მიღებულია, ჩვენ დაგიკავშირდებით');
                window.location.reload();
            }
        });
    }

    //order by
    if (currentLanguage === 'ka-GE') {
        $scope.orderByOptions = [
            {
                name: 'დასახელება ზრდადობით',
                field: 'name'
            },
            {
                name: 'დასახელება კლებადობით',
                field: '-name'
            },
            {
                name: 'კოდი ზრდადობით',
                field: 'code'
            },
            {
                name: 'კოდი კლებადობით',
                field: '-code'
            },
            {
                name: 'ფასი ზრდადობით',
                field: 'price'
            },
            {
                name: 'ფასი კლებადობით',
                field: '-price'
            }
        ]
    }

    if (currentLanguage === 'en-US') {
        $scope.orderByOptions = [
            {
                name: 'Name Ascending',
                field: 'name'
            },
            {
                name: 'Name Descending',
                field: '-name'
            },
            {
                name: 'Code Ascending',
                field: 'code'
            },
            {
                name: 'Code Descending',
                field: '-code'
            },
            {
                name: 'Price Ascending',
                field: 'price'
            },
            {
                name: 'Price Descending',
                field: '-price'
            }
        ]
    }

    $scope.selectedOrderByOption = $scope.orderByOptions[0];
    
    $scope.orderByFilter = function () {
        return $scope.selectedOrderByOption.field;
    }
    //---

    $scope.showProductDetails = function (productId) {
        window.open('/home/productdetails/' + productId, '_blank');
    }
});