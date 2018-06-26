Array.prototype.where = function (func) {
    var items = [],
        length = this.length;

    for (var i = 0; i < length; i++) {
        if (func(this[i]))
            items.push(this[i]);
    }

    return items.length === 0 ? null : items;
}


Array.prototype.any = function (func) {
    var length = this.length;

    for (var i = 0; i < length; i++) {
        if (func(this[i]))
            return true;
    }

    return false;
}


Array.prototype.first = function (func) {
    var length = this.length,
        item = null;

    for (var i = 0; i < length; i++) {
        if (func(this[i])) {
            return this[i];
        }
    }

    return item;
}


Array.prototype.last = function (func) {
    var length = this.length - 1,
        item = null;

    for (var i = length; i >= 0; i--) {
        if (func(this[i])) {
            item = this[i];
            break;
        }
    }

    return item;
}


Array.prototype.select = function (func) {
    var length = this.length,
        items = [];

    for (var i = 0; i < length; i++) {
        items.push(func(this[i]));
    }

    return items;
}


Array.prototype.max = function (func) {
    return func === undefined ? Math.max.apply(null, this) : Math.max.apply(null, this.select(func));
};


Array.prototype.min = function (func) {
    return func === undefined ? Math.min.apply(null, this) : Math.min.apply(null, this.select(func));
};


Array.prototype.sum = function (func) {
    var length = this.length,
        sum = 0;

    for (var i = 0; i < length; i++) {
        sum += func(this[i]);
    }

    return sum;
}


function isUndefinedNullOrEmpty(item) {
    return item === undefined || item === null || item === '';
}

function setProductRowClass(products) {
    angular.forEach(products, function (p) {
        if (p.stock === 0) {
            p.rowClass = 'not-available';
        }
    });
}

function searchCatalog() {
    window.location.href = '/home/searchcatalog?search=' + $('#searchPhrase').val();
}

function setProductStockCellClass(products) {
    angular.forEach(products, function (p) {
        if (p.stock < 5 && p.stock > 0) {
            p.displayStock = '5-';
            p.cellClass = 'five-minus';
        } else if (p.stock > 5) {
            p.displayStock = '5+';
            p.cellClass = 'five-plus';
        } else if(p.stock === 5) {
            p.cellClass = 'equals-five';
        }
    });
}

function formatNumber(number) {
    number = number.toFixed(2) + '';
    x = number.split(',');
    x1 = x[0];
    x2 = x.length > 1 ? ',' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ' ' + '$2');
    }
    return (x1 + x2).replace('.', ',');
}

function clearString(str) {
    return str.replace(' ', '').replace(',', '.');
}