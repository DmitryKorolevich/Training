angular.module('app.core.utils.parts.starsDirective', [])
   .directive('stars', ['$parse', '$timeout', function ($parse, $timeout) {
       return {
           restrict: 'E',
           transclude: true,
           template: '<div class="stars"></div>',
           require: 'ngModel',
           link: function (scope, elem, attr, modelCtrl) {
               var maxStarCount = 5;
               var wrapper = elem.find(".stars");
               modelCtrl.$render = function () {
                   wrapper.empty();
                   if (modelCtrl.$modelValue) {
                       var currentValue = modelCtrl.$modelValue;
                       var usedStars=0;
                       while (currentValue > 0)
                       {
                           if (currentValue > 1) {
                               currentValue=currentValue-1;
                               wrapper.append($('<img src="/assets/images/stars/fullstar.gif">'));
                           }
                           else if (currentValue > 0.5)
                           {
                               currentValue = 0;
                               wrapper.append($('<img src="/assets/images/stars/fullstar.gif">'));
                           }
                           else if (currentValue <= 0.5)
                           {
                               currentValue=0;
                               wrapper.append($('<img src="/assets/images/stars/halfstar.gif">'));
                           }
                           usedStars++;
                       }
                       while(usedStars<maxStarCount)
                       {
                           wrapper.append($('<img src="/assets/images/stars/emptystar.gif">'));
                           usedStars++;
                       }
                   };
               };
           }
       };
   }]);