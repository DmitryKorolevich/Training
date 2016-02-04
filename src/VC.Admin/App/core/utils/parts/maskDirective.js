angular.module('app.core.utils.parts.maskDirective', [])
   .directive('mask', ['$parse', '$timeout', function ($parse,$timeout) {
       return {
           restrict: 'A',
           require: 'ngModel',
           link: function (scope, elem, attr, modelCtrl) {
               if (attr.mask)
               {
                   modelCtrl.$render = function () {
                       elem.val(modelCtrl.$viewValue || '');
                       elem.mask(attr.mask, { placeholder: attr.maskPlaceholder, autoclear: false });
                   };

                   if(attr.clean)
                   {
                       modelCtrl.$parsers.push(function (inputValue) {
                           var resultValues = inputValue.replaceAll('(', '').replaceAll(')', '').replaceAll(' ', '').replaceAll('-', '').replaceAll('x', '').replaceAll('_', '');
                           return resultValues;
                       });
                   }

                   modelCtrl.$validators.mask = function (modelValue, viewValue)
                   {
                       var value = modelValue || viewValue;
                       var testElem = $('<input type"text"/>');
                       testElem.val(value);
                       testElem.mask(attr.mask, { placeholder: attr.maskPlaceholder });
                       return modelCtrl.$isEmpty(value) || !modelCtrl.$isEmpty(testElem.val());
                   };
               }
           }
       };
   }]);