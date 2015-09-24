Object.clone = function (fObj) {
    if (null == fObj || "object" != typeof fObj)
        return fObj;

    // Handle Date
    if (fObj instanceof Date) {
        var tCopy = new Date();
        tCopy.setTime(fObj.getTime());
        return tCopy;
    }

    // Handle Array
    if (fObj instanceof Array) {
        var tCopy = [];
        for (var tI = 0; tI < fObj.length; tI++) {
            tCopy[tI] = Object.clone(fObj[tI]);
        }
        return tCopy;
    }

    // Handle Object
    if (fObj instanceof Object) {
        var tCopy = {};
        for (var tAttr in fObj) {
            if (fObj.hasOwnProperty(tAttr)) tCopy[tAttr] = Object.clone(fObj[tAttr]);
        }
        return tCopy;
    }

    throw new Error("Unable to copy object! Its type isn't supported.");
}

Boolean.parse = function (str) {
    if (str) {
        switch (str.toLowerCase()) {
            case "true":
                return true;
            case "false":
                return false;
            case "1":
                return true;
            case "0":
                return false;
        }
    }
    return false;
};

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

Date.doPlaceDate = function (fString, fDate) {

    var tResult = fString;
    var tDate = fDate || new Date();
    tResult = tResult.replace("{dd}", tDate.getDate());
    tResult = tResult.replace("{DD}", (tDate.getDate() < 10 ? '0' + tDate.getDate() : tDate.getDate()));
    tResult = tResult.replace("{mm}", tDate.getMonth() + 1);
    tResult = tResult.replace("{MM}", (tDate.getMonth() < 9 ? '0' + (tDate.getMonth() + 1) : (tDate.getMonth() + 1)));
    tResult = tResult.replace("{yy}", tDate.getYear() > 100 ? tDate.getYear() - 100 : tDate.getYear());
    tResult = tResult.replace("{yyyy}", tDate.getFullYear());
    tResult = tResult.replace("{hh}", tDate.getHours());
    tResult = tResult.replace("{HH}", (tDate.getHours() < 10 ? '0' + tDate.getHours() : tDate.getHours()));
    tResult = tResult.replace("{MN}", (tDate.getMinutes() < 10 ? '0' + tDate.getMinutes() : tDate.getMinutes()));
    tResult = tResult.replace("{SS}", (tDate.getSeconds() < 10 ? '0' + tDate.getSeconds() : tDate.getSeconds()));
    tResult = tResult.replace("{AP}", tDate.getHours() >= 12 ? 'AM' : 'PM');
    return tResult;
};

String.prototype.doPlaceDate = function (fDate) {
    return Date.doPlaceDate(this, fDate);
};

if (!('format' in Date.prototype))
    Date.prototype.format = function (fFormat) {
        return Date.doPlaceDate(fFormat, this);
};

Date.prototype.toServerDateTime = function () {
    return Date.doPlaceDate("{yyyy}-{MM}-{DD}T{HH}:{MN}:{SS}.000Z", this);
};

Date.parseDateTime = function (fDateString, fUTC)
{
    if (!String.isString(fDateString))
        return fDateString;

    var tParts = (fDateString || '').split('T');
    if (tParts.length != 2)
        return null;

    var tTimezonePosition = tParts[1].lastIndexOf('+');
    if (tTimezonePosition > 0)
        tParts[1] = tParts[1].substr(0, tTimezonePosition);

    var tYMD = tParts[0].split('-');
    var tHMS = tParts[1].split(':');

    if (tYMD.length != 3 || tHMS.length != 3)
        return null;


    return fUTC ?
       new Date(Date.UTC(tYMD[0], parseInt(tYMD[1], 10) - 1, tYMD[2], tHMS[0], tHMS[1], tHMS[2])) :
       new Date(tYMD[0], parseInt(tYMD[1], 10) - 1, tYMD[2], tHMS[0], tHMS[1], tHMS[2]);
};

if (!('shiftDate' in Date.prototype)) {
    var pModifiers = {
        'y': function (fDate, fSign, fValue) {
            var tValue = fValue;
            switch (fSign) {
                case '!': break;
                case '+': tValue = fDate.getFullYear() + fValue; break;
                case '-': tValue = fDate.getFullYear() - fValue; break;
                default: throw 'UNKNOWN DATE SIGN FOR MODIFIER `Y`'
            }
            fDate.setFullYear(tValue);
        },
        'm': function (fDate, fSign, fValue) {
            var tValue = fValue;
            switch (fSign) {
                case '!': break;
                case '+': tValue = fDate.getMonth() + fValue; break;
                case '-': tValue = fDate.getMonth() - fValue; break;
                default: throw 'UNKNOWN DATE SIGN FOR MODIFIER `M`'
            }
            fDate.setMonth(tValue);
        },
        'd': function (fDate, fSign, fValue) {
            var tValue = fValue;
            switch (fSign) {
                case '!': break;
                case '+': tValue = fDate.getDate() + fValue; break;
                case '-': tValue = fDate.getDate() - fValue; break;
                default: throw 'UNKNOWN DATE SIGN FOR MODIFIER `D`'
            }
            fDate.setDate(tValue);
        },
        'h': function (fDate, fSign, fValue) {
            var tValue = fValue;
            switch (fSign) {
                case '!': break;
                case '+': tValue = fDate.getHours() + fValue; break;
                case '-': tValue = fDate.getHours() - fValue; break;
                default: throw 'UNKNOWN DATE SIGN FOR MODIFIER `H`'
            }
            fDate.setHours(tValue);
        }
    };
    Date.prototype.shiftDate = function (fRule) {
        if (!jQuery.isFunction(fRule.split))
            throw 'WRONG DATE DATE SHIFT RULE!';

        var tItems = fRule.split(/\s/);
        if (!tItems || tItems.length < 1)
            throw 'WRONG DATE DATE SHIFT RULE!';

        var tResult = new Date(this.getFullYear(), this.getMonth(), this.getDate(), this.getHours(), this.getMinutes(), this.getSeconds());

        for (var tC = 0; tC < tItems.length; tC++) {
            var tItem = tItems[tC];
            if (tItem.length < 3)
                throw 'WRONG DATE DATE SHIFT RULE!';

            var tSign = tItem.charAt(0);
            var tModifier = tItem.charAt(tItem.length - 1);
            var tLength = tItem.substring(1, tItem.length - 1);
            if (!(tModifier in pModifiers))
                throw 'WRONG MODIFIER IN DATE SHIFT RULE!';

            pModifiers[tModifier](tResult, tSign, parseInt(tLength));
        }
        return tResult;
    };
};

Date.parseDate = function (fDateString) {
   if(!String.isString(fDateString))
      return fDateString;

   var tYMD = fDateString.split('-');

   if(tYMD.length != 3)
      return null;

   return new Date(tYMD[0], parseInt(tYMD[1], 10) - 1, tYMD[2], 0, 0, 0);
};

Date.parseDateTime = function (fDateString, fUTC) {
   if(!String.isString(fDateString))
      return fDateString;

   var tParts = (fDateString || '').split('T');
   if(tParts.length != 2)
      return null;

   var tTimezonePosition = tParts[1].lastIndexOf('+');
   if(tTimezonePosition > 0)
      tParts[1] = tParts[1].substr(0, tTimezonePosition);

   var tYMD = tParts[0].split('-');
   var tHMS = tParts[1].split(':');

   if(tYMD.length != 3 || tHMS.length != 3)
      return null;


   return fUTC ?
      new Date(Date.UTC(tYMD[0], parseInt(tYMD[1], 10) - 1, tYMD[2], tHMS[0], tHMS[1], tHMS[2])) :
      new Date(tYMD[0], parseInt(tYMD[1], 10) - 1, tYMD[2], tHMS[0], tHMS[1], tHMS[2]);
};

function ServerMessages(data) {
    var self = this;

    self.Messages = data;
};

ServerMessages.prototype.GetMessage = function (field, collectionName, index, formName) {
    var toReturn = '';
    var fieldName = field;
    if (collectionName != undefined && index != undefined)
    {
        fieldName = '{0}.i{1}.{2}'.format(collectionName, index, field);
    }
    if (formName != undefined)
    {
        fieldName = formName + "::" + fieldName;
    }
    $.each(this.Messages, function (index, message) {
        if (message.Field == fieldName) {
            toReturn = message.Message;
            return false;
        }
    });
    return toReturn;
};