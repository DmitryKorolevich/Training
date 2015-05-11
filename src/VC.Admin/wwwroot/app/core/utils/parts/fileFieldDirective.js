angular.module('app.core.utils.parts.fileFieldDirective', [])
.directive('filefield', ['$parse', function ($parse) {
    return {
        restrict: 'EA',
        replace: true,
        template: '<p class="input-group">' +
                    '<input type="text" placeholder="{{placeHolder}}" class="form-control disabled" ng-model="inputValue" disabled/>' +
                    '<span class="input-group-btn">' +
                        '<button type="button" class="btn btn-default" data-ng-show="inputValue" data-ng-click="openPreview()"><i class="glyphicon glyphicon-eye-open"></i></button>' +
                        '<button type="button" class="btn btn-default" data-ng-show="inputValue" data-ng-click="clear()"><i class="glyphicon glyphicon-remove-circle"></i></button>' +
                        '<button type="button" class="btn btn-default" data-ng-click="openFileManagement()"><i class="glyphicon glyphicon-folder-open"></i></button>' +
                    '</span>' +
                  '</p>',
        scope: {
            Name: '=?'
        },
        require: ['filefield', '?^ngModel'],
        controller: 'fileFieldController',
        link: function (scope, element, attrs, ctrls) {
            var fileFieldCtrl = ctrls[0], ngModelCtrl = ctrls[1];

            if (ngModelCtrl) {
                fileFieldCtrl.init(ngModelCtrl);
            }
        }
    };
}])

.controller('fileFieldController', ['$scope', '$rootScope', '$attrs', '$parse', '$timeout', '$log', 'modalUtil', 'appBootstrap', 'filesConfig', 'confirmUtil',
    function ($scope, $rootScope, $attrs, $parse, $timeout, $log, modalUtil, appBootstrap, filesConfig, confirmUtil) {
        var self = this;

        this.init = function (ngModelCtrl_) {
            ngModelCtrl = ngModelCtrl_;
            self.baseUrl = $rootScope.ReferenceData.PublicHost + filesConfig.urlPrefix + '{0}';
            $scope.placeHolder = $attrs.placeholder;

            ngModelCtrl.$render = function () {
                self.render();
            };
        };        

        this.render = function () {
            if (ngModelCtrl.$viewValue) {
                $scope.inputValue = self.baseUrl.format(ngModelCtrl.$viewValue);
            }
            else
            {
                $scope.inputValue = null;
            }
            if ($attrs.required) {
                ngModelCtrl.$setValidity('required', ngModelCtrl.$viewValue);
            }
        };

        $scope.openFileManagement = function () {
            var url = ngModelCtrl.$viewValue;
            if (!url)
            {
                url = null;
            }
            var data = {
                fileUrl: url,
                thenCallback: function (data) {
                    if(data)
                    {
                        ngModelCtrl.$setViewValue(data);
                        ngModelCtrl.$render();
                    }
                }
            };
            appBootstrap.setData('FILES_POPUP_DATA', data);
            popup = modalUtil.open('app/modules/file/partials/selectFile.html', 'filesController', data, { size: 'lg' });
        };

        $scope.openPreview = function () {
            var url = ngModelCtrl.$viewValue;
            if (url) {
                var data = {
                    fileUrl: url,
                };
                popup = modalUtil.open('app/modules/file/partials/previewFile.html', 'previewFileController', data, { size: 'sm' });
            };
        };

        $scope.clear = function () {
            confirmUtil.confirm(function () {
                ngModelCtrl.$setViewValue(null);
                ngModelCtrl.$render();
            }, 'Are you sure you want to clear this image input field?');
        };
}])