'use strict';

angular.module('app.modules.content.controllers.masterManageController', [])
.controller('masterManageController', ['$scope','$stateParams', 'contentService', 'toaster', 'confirmUtil', function ($scope,$stateParams, contentService, toaster, confirmUtil) {

    function Processor(data, ids)
    {
        var self = this;
        self.Id = data.Id;
        self.Name = data.Name;
        self.IsSelected=false;
        $.each(ids, function (index, id) {
            if(self.Id==id)
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

        //Should be loaded with loockups and basic settings on app opening
	    $scope.types = [
	        { Id: 1, Name: 'Recipe Category' },
	        { Id: 2, Name: 'Recipe' },
	        { Id: 3, Name: 'Article Category' },
	        { Id: 4, Name: 'Article' },
	        { Id: 5, Name: 'FAQ Category' },
	        { Id: 6, Name: 'FAQ' },
	        { Id: 7, Name: 'Content' },
	    ];
        $scope.appProcessors = [
            {Id: 1, Name: 'Recipe root category processor'},
            {Id: 2, Name: 'Recipe sub-categories processor'},
            {Id: 3, Name: 'Recipes processor'},
        ];

        $scope.master=
        {
            Name:'',
            Template: '',
            IsDefault:false,
            Type:1,
        };
        $scope.loaded = false;
        $scope.forms = {};

        if ($scope.id) {
            contentService.getMasterContentItem($scope.id)
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
        $.each($scope.appProcessors, function (index, processor) {
            processors.push(new Processor(processor,ids))
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
            contentService.updateMasterContentItem($scope.master).success(function (result) {
                successSaveHandler(result);
            }).
                error(function (result) {
                    errorHandler(result);
                });
        } else {
            $scope.forms.masterForm.submitted = true;
        }
    };

	$scope.cancel = function () {
	};

	initialize();
}]);