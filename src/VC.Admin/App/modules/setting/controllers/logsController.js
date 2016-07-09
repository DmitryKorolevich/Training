angular.module('app.modules.setting.controllers.logsController', [])
.controller('logsController', ['$scope', '$state', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'gridSorterUtil', 'promiseTracker', function ($scope, $state, settingService, toaster, modalUtil, confirmUtil, gridSorterUtil, promiseTracker) {

    function Filter() {
        var self = this;

        self.Message = '';
        self.Source = '';
        self.LogLevel = null;
        var currentDate = new Date();
        currentDate.setHours(0, 0, 0, 0);
        self.AppName = 'VC.Admin';
        self.To = currentDate.shiftDate('+1d');
        self.From = currentDate.shiftDate('-1m');
        self.Paging = { PageIndex: 1, PageItemCount: 100 };
        self.Sorting = gridSorterUtil.resolve(refreshLogs, "Date", "Desc")
    };

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function refreshLogs()
    {
        var data = {};
        angular.copy($scope.filter, data);
        if (data.From)
        {
            data.From = data.From.toServerDateTime();
        }
        if (data.To)
        {
            data.To = data.To.toServerDateTime();
        }

        settingService.getLogItems(data, $scope.refreshTracker)
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
    	$scope.refreshTracker = promiseTracker("refresh");

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

        $scope.appNames = [
            "VC.Admin",
            "VC.Public",
            "VitalChoice.Jobs",
            "VitalChoice.ExportService"
        ];
        
        $scope.filter = new Filter();
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

	$scope.open = function (item) {
	    modalUtil.open('app/modules/setting/partials/logDetails.html', 'logDetailsController', {
	        item: item });
	};

	initialize();
}]);