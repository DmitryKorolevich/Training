'use strict';

angular.module('app.modules.content.controllers.manageRecipeController', [])
.controller('manageRecipeController', ['$scope','$state','$stateParams', 'contentService', 'toaster', 'confirmUtil', function ($scope,$state,$stateParams, contentService, toaster, confirmUtil) {

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
            $scope.recipe.Id = result.Id;
            $scope.recipe.MasterContentItemId = result.MasterContentItemId;
		} else {
            var messages=""; 
            if(result.Messages)
            {
                $scope.forms.recipeForm.submitted = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $.each(result.Messages, function (index, value) {                    
                    $scope.forms.recipeForm[value.Field].$setValidity("server", false);
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
	    $scope.descriptionExpanded = false;

	    $scope.toogleEditorState = function (property) {
	        $scope[property] = !$scope[property];
	    };

        $scope.recipe=
        {
            Name:'',
            Url:'',
            Template: '@default()',
            Description:'',
            Title:null,
            MetaKeywords:null,
            MetaDescription: null,
            MasterContentItemId: 0,
        };
        $scope.loaded = false;
        $scope.forms={};

        $scope.save = function () {
            $.each($scope.forms.recipeForm, function (index, element) {
                if (index == 'Url' && element.$name==index)
                {
                    element.$setValidity("server", true);
                }
            });

            if ($scope.forms.recipeForm.$valid) {
                var categoryIds = [];
                getSelected($scope.rootCategory, categoryIds);
                $scope.recipe.CategoryIds = categoryIds;

                contentService.updateRecipe($scope.recipe).success(function (result) {
                    successSaveHandler(result);
                }).
                    error(function (result) {
                        errorHandler(result);
                    });
            } else {
                $scope.forms.recipeForm.submitted = true;
            }
        };

        contentService.getCategoriesTree({Type: 2})//recipes
			.success(function (result) {
				if (result.Success) {
					$scope.rootCategory=result.Data;
                    if($scope.id)
                    {
                        contentService.getRecipe($scope.id)
			                .success(function (result) {
				                if (result.Success) {
					                $scope.recipe=result.Data;
                                    setSelected($scope.rootCategory, $scope.recipe.CategoryIds);
                                    $scope.loaded=true;
				                } else {
					                errorHandler(result);
				                }
			                }).
			                error(function(result) {
				                errorHandler(result);
			                });
                    }
                    else
                    {
                        $scope.loaded=true;
                    }
				} else {
					errorHandler(result);
				}
			}).
			error(function(result) {
				errorHandler(result);
			});
	}

    function setSelected(category , ids){
        $.each(ids, function( index, id ) {
            if(category.Id==id)
            {
                category.IsSelected=true;
            }
        });
        $.each(category.SubItems, function( index, value ) {
            setSelected(value,ids)
        });
    }

    function getSelected(category , ids){
        if(category.IsSelected)
        {
            ids.push(category.Id);
        }
        $.each(category.SubItems, function( index, value ) {
            getSelected(value,ids)
        });
    }

	$scope.hasChildren = function (scope) {
		return scope.childNodesCount() > 0;
	};

	$scope.deleteAssignedProduct = function(index) {
		confirmUtil.confirm(function() {
            $scope.recipe.AssignedProducts.splice(index, 1);
		}, 'Are you sure you want to delete the given assigned product?');
	};

	$scope.goToMaster = function (id) {
	    $state.go('index.oneCol.masterDetail', { id: id });
	};

	initialize();
}]);