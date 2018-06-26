var app = angular.module('history-app', ['ui.bootstrap']);

app.service('pickerService', function () {
    var date_from, date_to;

    var setDates = function (new_date_from, new_date_to) {
        date_from = new_date_from;
        date_to = new_date_to;
    }

    var getDates = function () {
        return { date_from: date_from, date_to: date_to };
    }

    var daysInMonth = function (m, y) {
        return 32 - moment(y, m, 32);
    }

    var getFromDateString = function () {
        return moment(date_from).format('YYYY-MM-DD') + ' 00:00:00';
    }

    var getToDateString = function () {
        return moment(date_to).format('YYYY-MM-DD') + ' 23:59:59';
    }

    return {
        setDates: setDates,
        getDates: getDates,
        daysInMonth: daysInMonth,
        getFromDateString: getFromDateString,
        getToDateString: getToDateString
    };
});

app.controller('history-ctrl', function ($scope, $http, pickerService) {
    var dt = moment();

    $('#overlay').hide();

    $scope.init = function () {
        $http.get('/home/getContragentCurrency').then(function (resp) {
            $scope.currency = resp.data;
        });

        $scope.getHistory = function () {
            $('#overlay').show();
            $http.post('/home/getHistory', { dateFrom: moment($scope.date_from).format('YYYY-MM-DD'), dateTo: moment($scope.date_to).format('YYYY-MM-DD'), currencyId: $scope.currency === undefined ? 1 : $scope.currency.id }).then(function (resp) {
                $scope.historyItems = resp.data.Value;
                $scope.startRest = formatNumber(resp.data.Key);

                angular.forEach($scope.historyItems, function (hi) {
                    hi.tdate = moment(hi.tdate).format('YYYY-MM-DD');                    
                    hi.number = hi.waybillNum === null ? hi.docNum : hi.waybillNum;


                    hi.amountIn = formatNumber(hi.amountIn);
                    hi.amountOut = formatNumber(hi.amountOut);
                    hi.rest = formatNumber(hi.rest);
                });

                $('#overlay').hide();
            });
        }



        $scope.generateWaybillPdf = function (generalId) {
            window.open('/home/generateWaybillPdf?generalId=' + generalId, '_blank');
            //window.location.href = '/home/generateWaybillPdf?generalId=' + generalId;
        }


    }

    $scope.generateHistoryExcel = function () {
        window.location.href = '/home/generateHistoryExcel?dateFrom=' + moment($scope.date_from).format('YYYY-MM-DD') + '&dateTo=' + moment($scope.date_to).format('YYYY-MM-DD') + '&currencyId=' + ($scope.currency === undefined ? 1 : $scope.currency.id);
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
                name: 'მიღება ზრდადობით',
                field: 'amountIn'
            },
            {
                name: 'მიღება კლებადობით',
                field: '-amountIn'
            },
            {
                name: 'გადახდა ზრდადობით',
                field: 'amountOut'
            },
            {
                name: 'გადახდა კლებადობით',
                field: '-amountOut'
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
                name: 'Amount in Ascending',
                field: 'amountIn'
            },
            {
                name: 'Amount in Descending',
                field: '-amountIn'
            },
            {
                name: 'Amount out Ascending',
                field: 'amountOut'
            },
            {
                name: 'Amount out Descending',
                field: '-amountOut'
            }
        ]
    }

    $scope.selectedOrderByOption = $scope.orderByOptions[0];
    
    $scope.orderByFilter = function () {
        return $scope.selectedOrderByOption.field;
    }
    //---

    $scope.openFromDatepicker = function ($event) {
        $scope.from_opened = true;
    };

    $scope.openToDatepicker = function ($event) {
        $scope.to_opened = true;
    };

    $scope.dateOptions = {
        startingDay: 1,
        showWeeks: false
    };

    $scope.menuChoiceClick = function (sign) {
        var year = dt.year();
        switch (sign) {
            case "today":
                $scope.date_from = filterDate(moment());
                $scope.date_to = filterDate(moment());
                break;
            case "year":
                $scope.date_from = filterDate(moment([year, 0, 1]));
                $scope.date_to = filterDate(moment([year, 11, 31]));
                break;
            case "kvartali1":
                $scope.date_from = filterDate(moment([year, 0, 1]));
                $scope.date_to = filterDate(moment([year, 2, 31]));
                break;
            case "kvartali2":
                $scope.date_from = filterDate(moment([year, 3, 1]));
                $scope.date_to = filterDate(moment([year, 5, 30]));
                break;
            case "kvartali3":
                $scope.date_from = filterDate(moment([year, 6, 1]));
                $scope.date_to = filterDate(moment([year, 8, 30]));
                break;
            case "kvartali4":
                $scope.date_from = filterDate(moment([year, 9, 1]));
                $scope.date_to = filterDate(moment([year, 11, 31]));
                break;
            case "yan":
                $scope.date_from = filterDate(moment([year, 0, 1]));
                $scope.date_to = filterDate(moment([year, 0, 31]));
                break;
            case "feb":
                $scope.date_from = filterDate(moment([year, 1, 1]));
                $scope.date_to = filterDate(moment([year, 1, moment([year, 1]).daysInMonth()]));
                break;
            case "mar":
                $scope.date_from = filterDate(moment([year, 2, 1]));
                $scope.date_to = filterDate(moment([year, 2, 31]));
                break;
            case "apr":
                $scope.date_from = filterDate(moment([year, 3, 1]));
                $scope.date_to = filterDate(moment([year, 3, 30]));
                break;
            case "may":
                $scope.date_from = filterDate(moment([year, 4, 1]));
                $scope.date_to = filterDate(moment([year, 4, 31]));
                break
            case "jun":
                $scope.date_from = filterDate(moment([year, 5, 1]));
                $scope.date_to = filterDate(moment([year, 5, 30]));
                break;
            case "jul":
                $scope.date_from = filterDate(moment([year, 6, 1]));
                $scope.date_to = filterDate(moment([year, 6, 31]));
                break;
            case "aug":
                $scope.date_from = filterDate(moment([year, 7, 1]));
                $scope.date_to = filterDate(moment([year, 7, 31]));
                break;
            case "sep":
                $scope.date_from = filterDate(moment([year, 8, 1]));
                $scope.date_to = filterDate(moment([year, 8, 30]));
                break;
            case "oct":
                $scope.date_from = filterDate(moment([year, 9, 1]));
                $scope.date_to = filterDate(moment([year, 9, 31]));
                break;
            case "nov":
                $scope.date_from = filterDate(moment([year, 10, 1]));
                $scope.date_to = filterDate(moment([year, 10, 30]));
                break;
            case "dec":
                $scope.date_from = filterDate(moment([year, 11, 1]));
                $scope.date_to = filterDate(moment([year, 11, 31]));
                break;
        }

        pickerService.setDates($scope.date_from, $scope.date_to);
    };

    var filterDate = function (dat) {
        return dat.toDate();
    }

    $scope.date_from = filterDate(dt);
    $scope.date_to = filterDate(dt);

    $scope.$watch('date_from', function (newval, oldval) {
        pickerService.setDates($scope.date_from, $scope.date_to);
        if (newval !== oldval) {
        }
    }, true);

    $scope.$watch('date_to', function (newval, oldval) {
        pickerService.setDates($scope.date_from, $scope.date_to);
        if (newval !== oldval) {
        }
    }, true);
});