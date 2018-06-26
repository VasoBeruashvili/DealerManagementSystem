var app = angular.module('search-catalog-app', []);

var contragentShoppingCartAmountSum = $('#shoppingCart').text();

app.controller('search-catalog-ctrl', function ($scope, $http) {
    $('#overlay').hide();

    $scope.init = function () {
        if (!isUndefinedNullOrEmpty(searchPhrase)) {
            $('#overlay').show();
            $http.post('/home/searchProducts', { searchPhrase: searchPhrase }).then(function (resp) {
                $scope.products = resp.data;
                setProductRowClass($scope.products);
                setProductStockCellClass($scope.products);


                angular.forEach($scope.products, function (p) {
                    p.price = formatNumber(p.price);
                });


                $('#searchPhrase').val(searchPhrase);
                $('#overlay').hide();
            });
        }
    }

    $scope.addToShoppingCart = function (product) {
        var productQuantity = parseInt(product.quantity);

        if (productQuantity > 0 && productQuantity <= product.stock) {
            var shoppingCart = {
                productId: product.id,
                quantity: productQuantity
            }

            $('#overlay').show();
            $http.post('/home/addToShoppingCart', { shoppingCart: shoppingCart }).then(function (resp) {
                if (resp.data) {
                    $('#shoppingCart').text(Math.round((parseFloat(isUndefinedNullOrEmpty(contragentShoppingCartAmountSum) ? 0 : contragentShoppingCartAmountSum) + parseFloat(clearString(product.price)) * productQuantity) * 100) / 100);
                    contragentShoppingCartAmountSum = $('#shoppingCart').text();

                    product.stock -= productQuantity;
                    product.ordered += productQuantity;

                    product.quantity = 0;
                    $('#overlay').hide();
                }
            });
        }
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