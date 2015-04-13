String.prototype.format = function () {
    return String.format(this, arguments.length == 1 ? arguments[0] : arguments);
};

String.format = function (fSource, fParams) {
    var _toString = function (fObject, o) {
        var ctor = function (fObject) {
            if (typeof fObject == 'number')
                return Number;
            else if (typeof fObject == 'boolean')
                return Boolean;
            else if (typeof fObject == 'string')
                return String;
            else
                return fObject.constructor;
        }(fObject);
        var proto = ctor.prototype;
        var tFormatter = typeof fObject != 'string' ? proto ? proto.format || proto.toString : fObject.format || fObject.toString : fObject.toString;
        if (tFormatter) {
            if (typeof o == 'undefined' || o == "")
                return tFormatter.call(fObject);
            return tFormatter.call(fObject, o);
        }
        return "";
    };
    if (arguments.length == 1)
        return function () {
            return String.format.apply(null, [fSource].concat(Array.prototype.slice.call(arguments, 0)));
        };
    if (arguments.length == 2 && typeof fParams != 'object' && typeof fParams != 'array')
        fParams = [fParams];
    if (arguments.length > 2)
        fParams = Array.prototype.slice.call(arguments, 1);
    fSource = fSource.replace(/\{\{|\}\}|\{([^}: ]+?)(?::([^}]*?))?\}/g, function (match, num, format) {
        if (match == "{{") return "{";
        if (match == "}}") return "}";
        if (typeof fParams[num] != 'undefined' && fParams[num] !== null) {
            return _toString(fParams[num], format);
        } else {
            return "";
        }
    });
    return fSource;
};

String.isString = function (fVar) {
    return typeof fVar == 'string' || fVar instanceof String;
};

if (!('startsWith' in String.prototype))
    String.prototype.startsWith = function (fChars) {
        return this.length > fChars.length && this.indexOf(fChars) == 0;
    };

if (!('endsWith' in String.prototype))
    String.prototype.endsWith = function (fChars) {
        return this.length > fChars.length && this.lastIndexOf(fChars) == (this.length - fChars.length)
};

this.doPlaceDate = function (fString, fDate) {

    var tResult = fString;
    var tDate = fDate || new Date();
    tResult = tResult.replace("{dd}", tDate.getDate());
    tResult = tResult.replace("{DD}", (tDate.getDate() < 10 ? '0' + tDate.getDate() : tDate.getDate()));
    tResult = tResult.replace("{mm}", tDate.getMonth() + 1);
    tResult = tResult.replace("{MM}", (tDate.getMonth() < 9 ? '0' + (tDate.getMonth() + 1) : (tDate.getMonth() + 1)));
    tResult = tResult.replace("{yy}", tDate.getYear());
    tResult = tResult.replace("{yyyy}", tDate.getFullYear());
    tResult = tResult.replace("{hh}", tDate.getHours());
    tResult = tResult.replace("{HH}", (tDate.getHours() < 9 ? '0' + (tDate.getHours() + 1) : (tDate.getHours() + 1)));
    tResult = tResult.replace("{MN}", (tDate.getMinutes() < 9 ? '0' + (tDate.getMinutes() + 1) : (tDate.getMinutes() + 1)));
    tResult = tResult.replace("{AP}", '');
    //tResult = tResult.replace("{AP}", tDate.getHours() >= 12 ? 'AM' : 'PM');
    return tResult;
};

String.prototype.doPlaceDate = function (fDate) {
    return self.doPlaceDate(this, fDate);
};

if (!('format' in Date.prototype))
    Date.prototype.format = function (fFormat) {
        return self.doPlaceDate(fFormat, this);
};