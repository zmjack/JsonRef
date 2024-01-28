var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
function eval_jsonref_refs($refs) {
    var pool = {};
    for (var key in $refs) {
        pool[key] = __assign({}, $refs[key].$);
    }
    function eval_value(value) {
        if (Array.isArray(value)) {
            return __spreadArray([], value.map(eval_value), true);
        }
        else if (typeof value === 'object') {
            if (value['$ref']) {
                return pool[value['$ref']];
            }
            for (var key in value) {
                value[key] = eval_value(value[key]);
            }
            return value;
        }
        else {
            return value;
        }
    }
    for (var key in pool) {
        var value = pool[key];
        pool[key] = eval_value(value);
    }
    return pool;
}
function eval_jsonref(jsonref) {
    var pool = eval_jsonref_refs(jsonref.$refs);
    return pool[jsonref.$.$ref];
}
