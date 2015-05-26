'use strict';

angular.module('app.modules.content.controllers.masterManageController', [])
.controller('masterManageController', ['$scope', '$rootScope','$stateParams', 'contentService', 'toaster', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $stateParams, contentService, toaster, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function Processor(data, ids)
    {
        var self = this;
        self.Id = data.Id;
        self.Name = data.Name;
        self.IsSelected=false;
        $.each(ids, function (index, id) {
            if (self.Id == id)
            {
                self.IsSelected=true;
                return false;
            }
        });
    }

	function successSaveHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved.");
            $scope.id=result.Data;
            $scope.master.Id = result.Data;
            $scope.IsDefaultInDB=$scope.master.IsDefault;
		} else {
            var messages="";
            if(result.Messages)
            {
                $scope.detailsTab.active = true;
                $.each(result.Messages, function( index, value ) {
                    messages+=value.Message +"<br />";
                });
            }
    	    toaster.pop('error', "Error!", messages,null,'trustedHtml');
		}
	};
    
	function errorHandler(result) {
		toaster.pop('error', "Error!", "Server error occured");
	};

	function initialize() {
	    $scope.id = $stateParams.id;

        $scope.types = Object.clone($rootScope.ReferenceData.ContentTypes);
        $scope.сontentProcessors = Object.clone($rootScope.ReferenceData.ContentProcessors);

        $scope.master=
        {
            Name:'',
            Template: '',
            IsDefault:false,
            Type:1,
        };
        $scope.detailsTab = {
            active: true
        };
        $scope.loaded = false;
        $scope.forms = {};

        if ($scope.id) {
            contentService.getMasterContentItem($scope.id,$scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.master = result.Data;
                        $scope.IsDefaultInDB=$scope.master.IsDefault;
                        setProcessors($scope.master.ProcessorIds);
                        $scope.loaded = true;
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function (result) {
                    errorHandler(result);
                });
        }
        else {
            setProcessors([]);
            $scope.loaded = true;
        }
	}

    function setProcessors(ids){
        var processors=[];
        $.each($scope.сontentProcessors, function (index, processor) {
            processors.push(new Processor(processor, ids));
        });
        $scope.processors=processors;
    }

    function getProcessorIds(){
        var ids=[];
        $.each($scope.processors, function (index, processor) {
            if(processor.IsSelected)
            {
                ids.push(processor.Id);
            }
        });
        return ids;
    }

    $scope.save = function () {
        if ($scope.forms.masterForm.$valid) {
            $scope.master.ProcessorIds = getProcessorIds();
            contentService.updateMasterContentItem($scope.master,$scope.editTracker).success(function (result) {
                successSaveHandler(result);
            }).
                error(function (result) {
                    errorHandler(result);
                });
        } else {
            $scope.forms.masterForm.submitted = true;
            $scope.detailsTab.active = true;
        }
    };

	$scope.cancel = function () {
	};

	initialize();
}]);