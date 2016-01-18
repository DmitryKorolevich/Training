angular.module('app.modules.content.controllers.emailTemplatesController', [])
.controller('emailTemplatesController', ['$scope', '$state', 'contentService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $state, contentService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil) {
	$scope.refreshTracker = promiseTracker("refresh");

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function refresh() {
        contentService.getEmailTemplates($scope.filter, $scope.refreshTracker)
			.success(function (result) {
			    if (result.Success) {
			        $scope.items = result.Data.Items;
                    $scope.totalItems = result.Data.Count;
                    $scope.loaded=true;
			    } else {
			        errorHandler(result);
			    }
			})
			.error(function (result) {
			    errorHandler(result);
			});
	};


	function initialize() {
	    $scope.filter = {
            Name: '',
            CategoryId: null,
            Paging: { PageIndex: 1, PageItemCount: 100 },
            Sorting: gridSorterUtil.resolve(refresh, "Updated", "Desc")
	    };

        refresh();
	}

	$scope.filterItems = function () {
	    $scope.filter.Paging.PageIndex = 1;
	    refresh();
	};

	$scope.pageChanged = function () {
	    refresh();
	};

	$scope.open = function (id) {
        if(id)
        {
            $state.go('index.oneCol.emailTemplateDetail',{ id: id});
        }
	};

	initialize();
}]);