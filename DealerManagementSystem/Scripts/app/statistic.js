$(function () {

    $('#overlay').hide();

    Highcharts.setOptions({
     colors: ['#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655']
    });

    $('#chart_category_cycle').highcharts({
        chart: {
            type: 'column'
        },
        title: {
            text: 'ბრუნვა, თვეების მიხედვით'
        },
        xAxis: {
            categories: ['იან', 'თებ', 'მარ', 'აპრ', 'მაი', 'ივნ', 'ივლ', 'აგვ', 'სექ', 'ოქტ', 'ნოემ', 'დეკ']
        },
        yAxis: {
            title: {
                text: 'თანხა'
            },
        },
        tooltip: {
            formatter: function () {
                return this.series.name + ': ' + this.y + '<br/>' +
                    'სულ: ' + this.point.stackTotal;
            }
        },
        plotOptions: {
            column: {
                stacking: 'normal'
            }
        },
        series: []
    });


    $('#chart_category_summary').highcharts({
        chart: {
            type: 'pie'
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.y}</b>'
        },
        plotOptions: {
            pie: {
                depth: 25,
                allowPointSelect: true,
                cursor: 'pointer'
            }
        },
        title: {
            text: 'ჯამური ბრუნვა კატეგორიებით'
        },
        series: [{
            name: 'თანხა',
            data: []
        }]
    });


    $('#chart_in_return').highcharts({
        chart: {
            type: 'bar'
        },
        title: {
            text: 'შესყიდვა/უკან დაბრუნება'
        },
        xAxis: {
            categories: ['იან', 'თებ', 'მარ', 'აპრ', 'მაი', 'ივნ', 'ივლ', 'აგვ', 'სექ', 'ოქტ', 'ნოემ', 'დეკ']
        },
        yAxis: {
            title: {
                text: 'თანხა'
            },
            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        tooltip: {
            formatter: function () {
                return this.series.name + ': ' + this.y + '<br/>';
            }
        },
        plotOptions: {
            column: {
                stacking: 'normal',
                dataLabels: {
                    enabled: true,
                    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white',
                    style: {
                        textShadow: '0 0 3px black'
                    }
                }
            }
        },
        series: []
    });

    $('#chart_debt').highcharts({
        chart: {
            type: 'column'
        },
        title: {
            text: 'მიმდინარე დავალიანება'
        },
        xAxis: {
            categories: ['საერთო დავალიანება/ბრუნვა', 'გადასახდელი დავალიანება']
        },
        yAxis: {
            title: {
                text: 'თანხა'
            },
            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        tooltip: {
            formatter: function () {
                return this.series.name + ': ' + this.y;
            }
        },
        series: [{ color: '#0066FF', name: 'საერთო დავალიანება/ბრუნვა', data: [] }, { color: '#e60000', name: 'გადასახდელი დავალიანება', data: [] }]
    });

    DateChange();

});


function DateChange() {

    var chart_category_cycle = $('#chart_category_cycle').highcharts();
    chart_category_cycle.showLoading(loading_text);
    $.post("/Statistic/CategoryCycleChange", { year: $('#year').val(), contragent_id: customer_id }, function (res) {

        var seriesLength = chart_category_cycle.series.length;
        for (var i = seriesLength - 1; i > -1; i--) {
            chart_category_cycle.series[i].remove();
        }

        res.forEach(function (r, i) {
            chart_category_cycle.addSeries({ 'name': r.group_name, 'data': $.map(r, function (val, index) { if (!isNaN(val)) return val; }) });
        });
        chart_category_cycle.redraw();
        chart_category_cycle.hideLoading();
    }, "json");

    var chart_category_summary = $('#chart_category_summary').highcharts();
    chart_category_summary.showLoading(loading_text);
    $.post("/Statistic/CategorySummaryChange", { year: $('#year').val(), contragent_id: customer_id }, function (res) {

        chart_category_summary.series[0].setData(res);
        chart_category_summary.hideLoading();
    }, "json");

    var chart_in_return = $('#chart_in_return').highcharts();
    chart_in_return.showLoading(loading_text);
    $.post("/Statistic/InReturnChange", { year: $('#year').val(), contragent_id: customer_id }, function (res) {

        var seriesLength = chart_in_return.series.length;
        for (var i = seriesLength - 1; i > -1; i--) {
            chart_in_return.series[i].remove();
        }

        res.forEach(function (r, i) {
            chart_in_return.addSeries({ 'name': r.oper_type, 'data': $.map(r, function (val, index) { if (!isNaN(val)) return val; }) });
        });
        chart_in_return.series[0].name = 'საქონლის დაბრუნება';
        chart_in_return.series[1].name = 'საქონლის შესყიდვა';

        chart_in_return.redraw();
        chart_in_return.hideLoading();
    }, "json");

    var chart_debt = $('#chart_debt').highcharts();
    chart_debt.showLoading(loading_text);
    $.post("/Statistic/DebtChange", { contragent_id: customer_id }, function (res) {

        chart_debt.series[0].setData([res.sruli_mimdinare_davalianeba, 0]);
        chart_debt.series[1].setData([0, res.mimdinare_gadasaxdeli]);

        console.log(res);

        $('#remaining_time').html(res.remaining_time + ' დღე');
        $('#contragentCurrency').html(res.contragentCurrency);
        $('#mimdinare_gadasaxdeli').html(formatNumber(res.mimdinare_gadasaxdeli) + (res.contragentCurrency === 'GEL' ? ' <i class="lari lari-normal"></i>' : ' <i class="fa fa-usd"></i>'));
        $('#sruli_mimdinare_davalianeba').html(formatNumber(res.sruli_mimdinare_davalianeba) + (res.contragentCurrency === 'GEL' ? ' <i class="lari lari-normal"></i>' : ' <i class="fa fa-usd"></i>'));
        $('#avg_overdue_amount').html(formatNumber(res.avg_overdue_amount) + (res.contragentCurrency === 'GEL' ? ' <i class="lari lari-normal"></i>' : ' <i class="fa fa-usd"></i>'));
        $('#avg_overdue_days').html(res.avg_overdue_days + ' დღე');

        chart_debt.hideLoading();
    }, "json");

}


var app = angular.module('statistic-app', ['ui.bootstrap']);

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

app.controller('statistic-ctrl', function ($scope, $http, pickerService) {

    var dt = moment();

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

    $scope.generateQueries = function () {
        $http.post('/statistic/generateQueries', { dateFrom: moment($scope.date_from).format('YYYY-MM-DD'), dateTo: moment($scope.date_to).format('YYYY-MM-DD') }).then(function (resp) {
            $scope.invoie_count = resp.data.invoie_count;
            $scope.avg_invoice_amount = formatNumber(resp.data.avg_invoice_amount);

            if (resp.data.contragentCurrency === 'GEL') {
                $scope.currencyClass = 'lari lari-normal';
            } else {
                $scope.currencyClass = 'fa fa-usd';
            }
        });
    }

    $scope.generateQueries();

});