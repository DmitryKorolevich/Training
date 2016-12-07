angular.module('app.modules.setting.controllers.addEditCountryController', [])
.controller('addEditCountryController', ['$scope', '$uibModalInstance', 'data', 'settingService', 'toaster', 'promiseTracker', '$rootScope', function ($scope, $uibModalInstance, data, settingService, toaster, promiseTracker, $rootScope) {
	$scope.saveTracker = promiseTracker("save");

	function successSaveHandler(result) {
	    if (result.Success) {
	        toaster.pop('success', "Success!", "Successfully saved.");

	        $scope.country = result.Data;
	        data.thenCallback($scope.country);
	        $uibModalInstance.close();
	    } else
	    {
	        $rootScope.fireServerValidation(result, $scope);
	    }
	};

	function errorHandler(result) {
		toaster.pop('error', "Error!", "Server error occured");
	};

	function initialize() {

	    $scope.customerTypes = [
            { Key: 1, Text: 'All' },
            { Key: 2, Text: 'Wholesale Only'}
	    ];

	    $scope.forms = {};

	    if (!data.country) {
	        $scope.country =
            {
                Id: 0,
                CountryCode: '',
                CountryName: '',
                StatusCode: 2,//Active,
                IdVisibility: 1,//all
            };
	    }
	    else
        {               
	        $scope.country = Object.clone(data.country);
	        $scope.country.StatusCode = $scope.country.StatusCode;
        }

	    $scope.save = function () {
	        $.each($scope.forms.form, function (index, element) {
	        	if (element && element.$name == index) {
	                element.$setValidity("server", true);
	            }
	        });

		    if ($scope.forms.form.$valid) {
		        settingService.updateCountry($scope.country, $scope.saveTracker).success(function (result) {
		            successSaveHandler(result);
		        }).
                error(function (result) {
                    errorHandler(result);
                });
		    } else {
		        $scope.forms.submitted = true;
			}
		};

		$scope.cancel = function () {
			$uibModalInstance.close();
		};
	}
    
	initialize();
}]);