var app = angular.module('orders-app', []);

app.controller('orders-ctrl', function ($scope, $http) {
    $('#overlay').hide();

    $scope.init = function () {
        $('#overlay').show();
        $http.get('/home/getFinaOrders').then(function (resp) {
            $scope.orders = resp.data;

            angular.forEach($scope.orders, function (o) {
                o.tdate = moment(o.tdate).format('YYYY-MM-DD');
                o.dueDate = o.dueDate === null ? '' : moment(o.dueDate).format('YYYY-MM-DD');               


                //set statuses
                if (o.statusId === 1) {
                    o.status = 'მუშავდება';
                } else if (o.statusId === 2) {
                    o.status = 'მიღებულია';
                } else if (o.statusId === 3) {
                    o.status = 'გზაშია';
                } else if (o.statusId === 4) {
                    o.status = 'მიწოდებულია';
                } else if (o.statusId === 5) {
                    o.status = 'ვადაგადაცილებულია';
                }
                //---


                if (o.overdueDays >= 6) {
                    o.restStyle = 'color: green;';
                } else if (o.overdueDays >= 0 && o.overdueDays <= 5) {
                    o.restStyle = 'color: orange;';
                } else if(o.overdueDays < 0) {
                    o.restStyle = 'color: red;';
                }


                o.debit = formatNumber(o.debit);
                o.credit = formatNumber(o.credit);
                o.rest = formatNumber(o.rest);
            });

            $('#overlay').hide();
        });
    }

    $scope.generateInvoicePdf = function (generalId) {
        window.open('/home/generateInvoicePdf?generalId=' + generalId, '_blank');
        //window.location.href = '/home/generateInvoicePdf?generalId=' + generalId;
    }

    //order by
    if (currentLanguage === 'ka-GE') {
        $scope.orderByOptions = [
            {
                name: 'თარიღი ზრდადობით',
                field: 'tdate'
            },
            {
                name: 'თარიღი კლებადობით',
                field: '-tdate'
            },
            {
                name: 'გადაცილების თარიღი ზრდადობით',
                field: 'dueDate'
            },
            {
                name: 'გადაცილების თარიღი კლებადობით',
                field: '-dueDate'
            },
            {
                name: 'გადაცილებული დღეები ზრდადობით',
                field: 'overdueDays'
            },
            {
                name: 'გადაცილებული დღეები კლებადობით',
                field: '-overdueDays'
            }
        ]
    }

    if (currentLanguage === 'en-US') {
        $scope.orderByOptions = [
            {
                name: 'Date Ascending',
                field: 'tdate'
            },
            {
                name: 'Date Descending',
                field: '-tdate'
            },
            {
                name: 'Due date Ascending',
                field: 'dueDate'
            },
            {
                name: 'Due date Descending',
                field: '-dueDate'
            },
            {
                name: 'Overdue days Ascending',
                field: 'overdueDays'
            },
            {
                name: 'Overdue days Descending',
                field: '-overdueDays'
            }
        ]
    }

    $scope.selectedOrderByOption = $scope.orderByOptions[0];
    
    $scope.orderByFilter = function () {
        return $scope.selectedOrderByOption.field;
    }
    //---
});