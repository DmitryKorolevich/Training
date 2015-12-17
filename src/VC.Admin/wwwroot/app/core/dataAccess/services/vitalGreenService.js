'use strict';

angular.module('app.core.dataAccess.services.vitalGreenService', [])
.service('vitalGreenService', ['$http', function ($http) {
    var baseUrl = '/Api/VitalGreen/';

    function getConfig(tracker) {
        var config = {};
        if (tracker) {
            config.tracker = tracker;
        }
        return config;
    };

    return {
        getVitalGreenReport: function (filter, tracker) {
            return $http.post(baseUrl + 'GetVitalGreenReport', filter, getConfig(tracker));
        },
        getVitalGreenReportFileUrl: function (filter, buildNumber) {
            return baseUrl + 'GetVitalGreenReportFile?year={0}&month={1}&buildNumber={2}'.format(filter.Year, filter.Month, buildNumber);
        },
    };
}]);