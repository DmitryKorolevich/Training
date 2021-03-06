﻿'use strict';

angular.module('app.modules.setting.controllers.logDetailsController', [])
.controller('logDetailsController', ['$scope', '$uibModalInstance', 'data', 'toaster', function ($scope, $uibModalInstance, data, toaster)
{

    function initialize()
    {
        $scope.cancel = function () {
            $uibModalInstance.close();
        };

        $scope.item = data.item;
        if (data.item && data.item.Message && data.item.Message[0] === '{') {
            try
            {
                data.item.Message = angular.fromJson(data.item.Message.replace(/\r?\n/g, ''));
                $scope.displayAsObject = true;
            }
            catch(error)
            {
                console.warn(error);
            }
        }
        else {
            $scope.displayAsObject = false;
        }
    }

    initialize();
}]);
