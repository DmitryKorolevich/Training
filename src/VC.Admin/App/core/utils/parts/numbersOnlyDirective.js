angular.module('app.core.utils.parts.numbersOnlyDirective', [])
    .directive('numbersOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            modelCtrl.$parsers.push(function (inputValue) {
                // this next if is necessary for when using ng-required on your input. 
                // In such cases, when a letter is typed first, this parser will be called
                // again, and the 2nd time, the value will be undefined
                if (inputValue == undefined) return ''
                var transformedInput = inputValue.replace(/[^0-9]/g, '');
                if (transformedInput != inputValue) {
                    modelCtrl.$setViewValue(transformedInput);
                    modelCtrl.$render();
                }

                return transformedInput;
            });

            if (isDefined(attrs.min) || attrs.ngMin) {
                var minVal;
                modelCtrl.$validators.min = function (value) {
                    return modelCtrl.$isEmpty(value) || isUndefined(minVal) || value >= minVal;
                };

                attrs.$observe('min', function (val) {
                    if (isDefined(val) && !isNumber(val)) {
                        val = parseFloat(val, 10);
                    }
                    minVal = isNumber(val) && !isNaN(val) ? val : undefined;

                    modelCtrl.$validate();
                });
            }

            if (isDefined(attrs.max) || attrs.ngMax) {
                var maxVal;
                modelCtrl.$validators.max = function (value) {
                    return modelCtrl.$isEmpty(value) || isUndefined(maxVal) || value <= maxVal;
                };

                attrs.$observe('max', function (val) {
                    if (isDefined(val) && !isNumber(val)) {
                        val = parseFloat(val, 10);
                    }
                    maxVal = isNumber(val) && !isNaN(val) ? val : undefined;

                    modelCtrl.$validate();
                });
            }

            function isDefined(value) { return typeof value !== 'undefined'; }
            function isUndefined(value) { return typeof value === 'undefined'; }
            function isNumber(value) { return typeof value === 'number'; }
        }
    };
});