angular.module('app.core.utils.parts.fileFieldDirective', [])
.directive('filefield', ['$parse', function ($parse) {
    return {
        restrict: 'EA',
        replace: true,
        template: '<p class="input-group">' +
                    '<input type="text" placeholder="{{placeHolder}}" {{required}} class="form-control disabled" ng-model="inputValue" disabled/>' +
                    '<span class="input-group-btn">' +
                        '<button type="button" class="btn btn-default" data-ng-class="{disabled: disabled}" data-ng-show="inputValue" data-ng-click="clear()"><i class="glyphicon glyphicon-remove-circle"></i></button>' +
                        '<button type="button" class="btn btn-default" data-ng-show="inputValue" data-ng-click="openPreview()"><i class="glyphicon glyphicon-eye-open"></i></button>' +
                        '<button type="button" class="btn btn-default" data-ng-class="{disabled: disabled}" data-ng-click="openFileManagement()"><i class="glyphicon glyphicon-folder-open"></i></button>' +
                    '</span>' +
                  '</p>',
        scope: {
        	Name: '=?',
        	onSelected: '&'
        },
        require: ['filefield', '?^ngModel'],
        controller: 'fileFieldController',
        link: function (scope, element, attrs, ctrls) {
            var fileFieldCtrl = ctrls[0], ngModelCtrl = ctrls[1];

            if (ngModelCtrl) {
            	fileFieldCtrl.init(ngModelCtrl, scope);
            }
        }
    };
}])

.controller('fileFieldController', ['$scope', '$rootScope', '$attrs', '$parse', '$timeout', '$log', 'modalUtil', 'appBootstrap', 'confirmUtil',
    function ($scope, $rootScope, $attrs, $parse, $timeout, $log, modalUtil, appBootstrap, confirmUtil) {
        var self = this;
        var ngModelCtrl = { $setViewValue: angular.noop };
        self.fileManagementPopup = null;

        self.init = function (ngModelCtrl_) {
            ngModelCtrl = ngModelCtrl_;
            if ($rootScope.ReferenceData.PublicHost) {
                self.baseUrl = 'https://' + $rootScope.ReferenceData.PublicHost + '{0}';
            }
            $scope.placeHolder = $attrs.placeholder;
            if ($attrs.required)
            {
                $scope.required = "required";
            }
            else
            {
                $scope.required = "";
            }

            ngModelCtrl.$render = function () {
                self.render();
            };
        };        

        self.render = function () {
            if (ngModelCtrl.$viewValue) {
                $scope.inputValue = self.baseUrl.format(ngModelCtrl.$viewValue);
            }
            else
            {
                $scope.inputValue = null;
            }        
            if ($attrs.disabled!=undefined) {
                $scope.disabled = true;
            }
            else {
                $scope.disabled = false;
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
                    	if (data.FullRelativeName) {
                    		ngModelCtrl.$setViewValue(data.FullRelativeName);
                    		ngModelCtrl.$render();
	                    }

                    	if ($scope.onSelected) {
                    		$scope.onSelected({ file: data });
	                    }
                    }
                    if (self.fileManagementPopup)
                    {
                        self.fileManagementPopup.close();
                    }
                }
            };
            appBootstrap.setData('FILES_POPUP_DATA', data);
            self.fileManagementPopup = modalUtil.open('app/modules/file/partials/selectFile.html', 'filesController', data, { size: 'lg' });
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