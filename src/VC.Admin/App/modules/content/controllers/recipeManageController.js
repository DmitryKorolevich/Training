'use strict';

angular.module('app.modules.content.controllers.recipeManageController', [])
.controller('recipeManageController', ['$scope', '$rootScope', '$state','$stateParams', 'contentService', 'toaster', 'confirmUtil','promiseTracker',
    function ($scope, $rootScope, $state,$stateParams, contentService, toaster, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

	function successSaveHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved.");
            $scope.id=result.Data.Id;
            $scope.recipe.Id = result.Data.Id;
            $scope.recipe.MasterContentItemId = result.Data.MasterContentItemId;
            $scope.previewUrl=$scope.baseUrl.format($scope.recipe.Url);
		} else {
            var messages=""; 
            if(result.Messages)
            {
                $scope.forms.recipeForm.submitted = true;                
	            $scope.detailsTab.active = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $.each(result.Messages, function (index, value) {
                    if (value.Field) {
                        $scope.forms.recipeForm[value.Field].$setValidity("server", false);
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
	    $scope.descriptionExpanded = false;

	    $scope.toogleEditorState = function (property) {
	        $scope[property] = !$scope[property];
	    };

	    $scope.baseUrl=$rootScope.ReferenceData.PublicHost + 'recipe/{0}?preview=true';
        $scope.previewUrl = null;

        $scope.recipe=
        {
            Name:'',
            Url:'',
            Template: '',
            Description:'',
            Title:null,
            MetaKeywords:null,
            MetaDescription: null,
            MasterContentItemId: 0,
        };
        $scope.detailsTab = {
            active: true
        };
        $scope.loaded = false;
        $scope.forms={};

        $scope.save = function () {
            $.each($scope.forms.recipeForm, function (index, element) {
            	if (element && element.$name == index) {
                    element.$setValidity("server", true);
                }
            });

            if ($scope.forms.recipeForm.$valid) {
                var categoryIds = [];
                getSelected($scope.rootCategory, categoryIds);
                $scope.recipe.CategoryIds = categoryIds;

                contentService.updateRecipe($scope.recipe,$scope.editTracker).success(function (result) {
                    successSaveHandler(result);
                }).
                    error(function (result) {
                        errorHandler(result);
                    });
            } else {
                $scope.forms.recipeForm.submitted = true;
                $scope.detailsTab.active = true;
            }
        };

	    contentService.getCategoriesTree({ Type: 1 }, $scope.refreshTracker)//recipe categories
			.success(function (result) {
				if (result.Success) {
					$scope.rootCategory=result.Data;
                    if($scope.id)
                    {
                        contentService.getRecipe($scope.id, $scope.refreshTracker)
			                .success(function (result) {
				                if (result.Success) {
					                $scope.recipe=result.Data;
					                $scope.previewUrl = $scope.baseUrl.format($scope.recipe.Url);
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

	function setSelected(category, ids) {
	    category.IsSelected = false;
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