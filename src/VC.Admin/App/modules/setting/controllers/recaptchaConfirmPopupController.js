'use strict';

angular.module('app.modules.setting.controllers.recaptchaConfirmPopupController', [])
.controller('recaptchaConfirmPopupController', ['$scope', '$rootScope', '$uibModalInstance', '$timeout', 'data', 'toaster',
    function ($scope, $rootScope, $uibModalInstance, $timeout, data, toaster)
{
    function initialize()
    {
        $scope.Header = "Captcha Challenge";
        if (data.Header)
        {
            $scope.Header = data.Header;
        }
        $scope.CancelButton = {
            Label:'Cancel'
        };
        if (data.CancelButton)
        {
            $scope.CancelButton = data.CancelButton;
        }
        if (data.OkButton)
        {
            $scope.OkButton = data.OkButton;
        }

        $scope.cancel = function ()
        {
            if($scope.CancelButton && $scope.CancelButton.Handler)
            {
                $scope.CancelButton.Handler();
            }
            $uibModalInstance.close();
        };

        $scope.ok = function ()
        {
            if($scope.OkButton && $scope.OkButton.Handler)
            {
                $scope.OkButton.Handler(grecaptcha.getResponse($rootScope.ReferenceData.GoogleCaptchaWidget));
            }
            $uibModalInstance.close();
        };

        $timeout(function ()
        {
            $rootScope.ReferenceData.GoogleCaptchaWidget =
                grecaptcha.render('googleCaptcha', {
                    'sitekey': $rootScope.ReferenceData.GoogleCaptchaPublicKey
                });
        }, 200);
    }

    initialize();
}]);