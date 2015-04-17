angular.module('app.modules.content.controllers.mastersController', [])
.controller('mastersController', ['$scope', '$state', 'contentService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
function ($scope, $state, contentService, toaster, modalUtil, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("refresh");
    $scope.deleteTracker = promiseTracker("delete");

    function errorHandler(result) {
        var messages = "";
        if (result.Messages) {
            $.each(result.Messages, function (index, value) {
                messages += value.Message + "<br />";
            });
        }
        toaster.pop('error', "Error!", messages, null, 'trustedHtml');
    };

    function refreshMasters() {
        contentService.getMasterContentItems($scope.filter,$scope.refreshTracker)
			.success(function (result) {
			    if (result.Success) {
			        $scope.masters = result.Data;
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
        //Should be loaded with loockups and basic settings on app opening
        $scope.types = [
            { Id: null, Name: 'All' },
	        { Id: 1, Name: 'Recipe Category' },
	        { Id: 2, Name: 'Recipe' },
	        { Id: 3, Name: 'Article Category' },
	        { Id: 4, Name: 'Article' },
	        { Id: 5, Name: 'FAQ Category' },
	        { Id: 6, Name: 'FAQ' },
	        { Id: 7, Name: 'Content Page Category' },
	        { Id: 8, Name: 'Content Page' },
	    ];

	    $scope.filter = {
            Type: null,
	    };
        $scope.loaded=false;

        refreshMasters();
	}

	$scope.filterMasters = function () {
	    refreshMasters();
	};

	$scope.open = function (id) {
        if(id)
        {
            $state.go('index.oneCol.masterDetail',{ id: id});
        }
        else
        {
            $state.go('index.oneCol.addNewMaster');
        }
	};

	$scope.delete = function (id) {
		confirmUtil.confirm(function() {
		    contentService.deleteMasterContentItem(id,$scope.deleteTracker)
			    .success(function (result) {
			        if (result.Success) {
			            toaster.pop('success', "Success!", "Successfully deleted");
			            refreshMasters();
			        } else {
			            errorHandler(result);
			        }
			    })
			    .error(function (result) {
			        errorHandler(result);
			    });
		}, 'Are you sure you want to delete this master template?');
	};

	initialize();
}]);