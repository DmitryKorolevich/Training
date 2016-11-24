'use strict';

angular.module('app.core.dataAccess.services.monitorService', [])
.service('monitorService', ['$http', function ($http)
{
    var baseUrl = '/Api/Monitor/';

    function getConfig(tracker) {
        var config = {};
        if (tracker) {
            config.tracker = tracker;
        }
        return config;
    };

    return {
        //edit locks
        editLockPing: function (model, tracker)
        {
            return $http.post(baseUrl + 'EditLockPing', model, getConfig(tracker));
        },
        editLockRequest: function (model, tracker)
        {
            return $http.post(baseUrl + 'EditLockRequest', model, getConfig(tracker));
        },

        //export
        getExportGeneralStatus: function (tracker)
        {
            return $http.get(baseUrl + 'GetExportGeneralStatus', getConfig(tracker));
        },
    };
}]);