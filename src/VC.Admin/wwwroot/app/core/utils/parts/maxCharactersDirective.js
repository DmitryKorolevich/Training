angular.module('app.core.utils.parts.maxCharactersDirective', [])
    .directive('maxCharacters', function ()
    {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl)
        {
            var maxCharacters = null;
            if (attrs.maxCharacters)
            {
                maxCharacters = attrs.maxCharacters;
            }
            if (attrs.ngMaxCharacters)
            {
                maxCharacters = attrs.ngMaxCharacters;
            }
            if (maxCharacters)
            {
                modelCtrl.$parsers.push(function (inputValue)
                {
                    // this next if is necessary for when using ng-required on your input. 
                    // In such cases, when a letter is typed first, this parser will be called
                    // again, and the 2nd time, the value will be undefined
                    if (inputValue == undefined) return ''
                    var transformedInput = inputValue.substring(0, maxCharacters);
                    if (transformedInput != inputValue)
                    {
                        modelCtrl.$setViewValue(transformedInput);
                        modelCtrl.$render();
                    }

                    return transformedInput;
                });
            }
        }
    };
});