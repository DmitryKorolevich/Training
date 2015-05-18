angular.module('app.modules.gc.controllers.gcsController', [])
.controller('gcsController', ['$scope', '$rootScope', '$state', 'gcService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, gcService, toaster, modalUtil, confirmUtil, promiseTracker) {
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

    function refreshItems() {
        gcService.getGiftCertificates($scope.filter, $scope.refreshTracker)
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
        $scope.types = Object.clone($rootScope.ReferenceData.GCTypes);
        $scope.types.splice(0, 0, { Key: null, Text: 'All' });

	    $scope.filter = {
	        Type: null,
	        Code: null,
	        Paging: { PageIndex: 1, PageItemCount: 100 }
	    };

        refreshItems();
	}

	$scope.filterItems = function () {
	    $scope.filter.Paging.PageIndex = 1;
	    refreshItems();
	};

	$scope.pageChanged = function () {
	    refreshItems();
	};

	$scope.add = function () {
	    $state.go('index.oneCol.gcsAdd', { });
	};

	$scope.edit = function (id) {
	    $state.go('index.oneCol.gcDetail', { id: id });
	};

	$scope.send = function (item) {
	    var data =
            {
                ToName: item.FirstName || item.LastName ? item.FirstName + ' ' + item.LastName : null,
                ToEmail: item.Email,
                FromName: 'Vital Choice',
                Codes: [ item ],
            };
	    modalUtil.open('app/modules/gc/partials/sendEmail.html', 'sendEmailController', data);
	};

	$scope.delete = function (id) {
		confirmUtil.confirm(function() {
		    gcService.deleteGiftCertificate(id, $scope.deleteTracker)
			    .success(function (result) {
			        if (result.Success) {
			            toaster.pop('success', "Success!", "Successfully deleted.");
			            refreshItems();
			        } else {
			            errorHandler(result);
			        }
			    })
			    .error(function (result) {
			        errorHandler(result);
			    });
		}, 'Are you sure you want to delete this gift certificate?');
	};

	initialize();
}]);