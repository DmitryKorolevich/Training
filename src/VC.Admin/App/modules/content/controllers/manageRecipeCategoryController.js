'use strict';

angular.module('app.modules.content.controllers.manageRecipeCategoryController', [])
.controller('manageRecipeCategoryController', ['$scope', '$state', '$stateParams', 'contentService', 'toaster', 'confirmUtil', function ($scope, $state, $stateParams, contentService, toaster, confirmUtil) {

    function ServerMessages(data){
        var self = this;
        
        self.Messages = data;
    };

    ServerMessages.prototype.GetMessage = function(field) {
        var toReturn = '';
        $.each(this.Messages, function (index, message) {
            if (message.Field == field) {
                toReturn = message.Message;
                return false;
            }
        });
        return toReturn;
    };

	function successSaveHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved.");
            $scope.id=result.Data;
            $scope.recipeCategory.Id = result.Data;
		} else {
            var messages=""; 
            if(result.Messages)
            {
                $scope.forms.form.submitted = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $.each(result.Messages, function (index, value) {
                    if (value.Field) {
                        $scope.forms.form[value.Field].$setValidity("server", false);
                    }
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

	    $scope.recipeCategory =
        {
            Name: '',
            Url: '',
            Type: 1,//recipe category
            Template: '@default()',
        };
	    if ($stateParams.categoryid) {
	        $scope.recipeCategory.ParentId = $stateParams.categoryid;
	    }

	    $scope.loaded = false;
	    $scope.forms = {};

	    if ($scope.id) {
	        contentService.getCategory($scope.id)
                .success(function (result) {
                    if (result.Success) {
                        $scope.recipeCategory = result.Data;
                        $scope.loaded = true;
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function (result) {
                    errorHandler(result);
                });
	    }
	    else
	    {
	        $scope.loaded = true;
	    }
	}

	$scope.save = function () {
	    $.each($scope.forms.form, function (index, element) {
	        if (element.$name == index) {
	            element.$setValidity("server", true);
	        }
	    });

	    if ($scope.forms.form.$valid) {
	        contentService.updateCategory($scope.recipeCategory).success(function (result) {
	            successSaveHandler(result);
	        }).
                error(function (result) {
                    errorHandler(result);
                });
	    } else {
	        $scope.forms.form.submitted = true;
	    }
	};

	initialize();
}]);