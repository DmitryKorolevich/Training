angular.module('app.modules.setting.controllers.logsController', [])
.controller('logsController', ['$scope', '$state', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', function ($scope, $state, settingService, toaster, modalUtil, confirmUtil) {

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function refreshLogs() {
        settingService.getLogItems($scope.filter)
			.success(function (result) {
			    if (result.Success) {
			        $scope.logs = result.Data.Items;
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
        $scope.logLevels = [
        {
            Id: null, Name: 'All'
        },
        {
            Id: 'Fatal', Name: 'Fatal'
        },
        {
            Id: 'Error', Name: 'Error'
        },
        {
            Id: 'Warn', Name: 'Warn'
        },  
        {
            Id: 'Info', Name: 'Info'
        },
        {
            Id: 'Debug', Name: 'Debug'
        },
        ];

        var currentDate = new Date();
        var dateFormat = '{yyyy}-{MM}-{DD}';
        
	    $scope.filter = {
	        Message: '',               
	        Source: '',                          
	        LogLevel: null,    
	        To: new Date((new Date(currentDate)).setDate(currentDate.getDate() + 1)).format(dateFormat),
	        From: new Date((new Date(currentDate)).setDate(currentDate.getDate() - 30)).format(dateFormat),
	        Paging: { PageIndex: 1, PageItemCount: 100 }
	    };
        $scope.loaded=false;

		refreshLogs();
	}

	$scope.filterLogs = function () {
	    $scope.filter.Paging.PageIndex = 1;
		refreshLogs();
	};

	$scope.pageChanged = function () {
	    refreshLogs();
	};

	$scope.open = function (data) {
        
	};

	initialize();
}]);