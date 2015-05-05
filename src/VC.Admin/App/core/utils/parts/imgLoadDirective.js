angular.module('app.core.utils.parts.imgLoadDirective', [])
   .directive('imgLoad', ['$parse', function ($parse) {
       return {
           restrict: 'A',
           link: function (scope, elem, attrs) {
               var fn = $parse(attrs.imgLoad);
               elem.on('load', function (event) {
                   scope.$apply(function() {
                       fn(scope, { $event: event });
                   });
               });
           }
       };
   }]);