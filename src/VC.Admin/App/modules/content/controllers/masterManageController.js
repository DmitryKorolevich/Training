'use strict';

angular.module('app.modules.content.controllers.masterManageController', [])
.controller('masterManageController', ['$scope', '$rootScope', '$stateParams', 'contentService', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $stateParams, contentService, settingService, toaster, confirmUtil, promiseTracker)
{
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function refreshHistory()
    {
        if ($scope.master && $scope.master.Id)
        {
            var data = {};
            data.service = settingService;
            data.tracker = $scope.refreshTracker;
            data.idObject = $scope.master.Id;
            data.idObjectType = 17//master
            $scope.$broadcast('objectHistorySection#in#refresh', data);
        }
    }

    function Processor(data, ids)
    {
        var self = this;
        self.Id = data.Id;
        self.Name = data.Name;
        self.IsSelected = false;
        $.each(ids, function (index, id)
        {
            if (self.Id == id)
            {
                self.IsSelected = true;
                return false;
            }
        });
    }

    function successSaveHandler(result)
    {
        if (result.Success)
        {
            toaster.pop('success', "Success!", "Successfully saved.");
            $scope.id = result.Data;
            $scope.master.Id = result.Data;
            $scope.IsDefaultInDB = $scope.master.IsDefault;
            refreshHistory();
        } else
        {
            $rootScope.fireServerValidation(result, $scope);
        }
    };

    function errorHandler(result)
    {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize()
    {
        $scope.id = $stateParams.id ? $stateParams.id : 0;

        $scope.types = Object.clone($rootScope.ReferenceData.ContentTypes);
        $scope.сontentProcessors = Object.clone($rootScope.ReferenceData.ContentProcessors);

        $scope.detailsTab = {
            active: true
        };
        $scope.loaded = false;
        $scope.forms = {};

        contentService.getMasterContentItem($scope.id, $scope.refreshTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.master = result.Data;
                    if ($scope.id)
                    {
                        $scope.IsDefaultInDB = $scope.master.IsDefault;
                    };
                    setProcessors($scope.master.ProcessorIds);
                    $scope.loaded = true;
                    refreshHistory();
                } else
                {
                    errorHandler(result);
                }
            }).
            error(function (result)
            {
                errorHandler(result);
            });

        $scope.aceChanged = function (e)
        {

        };
    }

    function setProcessors(ids)
    {
        var processors = [];
        $.each($scope.сontentProcessors, function (index, processor)
        {
            processors.push(new Processor(processor, ids));
        });
        $scope.processors = processors;
    }

    function getProcessorIds()
    {
        var ids = [];
        $.each($scope.processors, function (index, processor)
        {
            if (processor.IsSelected)
            {
                ids.push(processor.Id);
            }
        });
        return ids;
    }

    $scope.save = function ()
    {
        if ($scope.forms.masterForm.$valid)
        {
            $scope.master.ProcessorIds = getProcessorIds();
            contentService.updateMasterContentItem($scope.master, $scope.editTracker).success(function (result)
            {
                successSaveHandler(result);
            }).
                error(function (result)
                {
                    errorHandler(result);
                });
        } else
        {
            $scope.forms.submitted = true;
            $scope.detailsTab.active = true;
        }
    };

    $scope.cancel = function ()
    {
    };

    initialize();
}]);