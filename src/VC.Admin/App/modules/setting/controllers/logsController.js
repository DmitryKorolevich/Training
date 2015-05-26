angular.module('app.modules.setting.controllers.logsController', [])
.controller('logsController', ['$scope', '$state', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'gridSorterUtil', 'promiseTracker', function ($scope, $state, settingService, toaster, modalUtil, confirmUtil, gridSorterUtil, promiseTracker) {

    function Filter() {
        var self = this;

        self.Message = '';          
        self.Source = '';                          
        self.LogLevel = null;
        var currentDate = new Date();
        currentDate.setHours(0, 0, 0, 0);
        self.ToDateObject = new DateObject(currentDate.shiftDate('+1d'));
        self.FromDateObject = new DateObject(currentDate.shiftDate('-1m'));
        self.Paging = { PageIndex: 1, PageItemCount: 100 };
        self.Sorting = gridSorterUtil.resolve(refreshLogs, "Date", "Desc")
    }

    Filter.prototype.clean = function () {
        var tClean = Object.clone(this);

        tClean.To = null;
        if (tClean.ToDateObject.Date)
            tClean.To = tClean.ToDateObject.Date.toServerDateTime();

        tClean.From = null;
        if (tClean.FromDateObject.Date)
            tClean.From = tClean.FromDateObject.Date.toServerDateTime();

        delete tClean.ToDateObject;
        delete tClean.FromDateObject;
        delete tClean.clean;
        return tClean;
    };

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function refreshLogs() {
        settingService.getLogItems($scope.filter.clean())
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