angular.module('app.modules.content.controllers.recipesController', [])
.controller('recipesController', ['$scope', '$state', 'contentService', 'toaster', 'modalUtil', 'confirmUtil', function ($scope, $state, contentService, toaster, modalUtil, confirmUtil) {
	function refreshRecipes() {

	};

	function initialize() {
		$scope.filter = { };

		refreshRecipes();
	}

	$scope.filterRecipes = function() {
		refreshRecipes();
	};

	$scope.open = function (id) {
        if(id)
        {
            $state.go('index.oneCol.recipeDetail',{ id: id});
        }
        else
        {
            $state.go('index.oneCol.addNewRecipe');
        }
	};

	$scope.delete = function(firstName, lastName, publicId) {
		confirmUtil.confirm(function() {
			
		}, 'Are you sure you want to delete this recipe?');
	};

	initialize();
}]);