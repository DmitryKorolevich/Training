'use strict';

angular.module('app.core.dataAccess.services.templateService', [])
.service('templateService', ['$http', function ($http) {
    var baseUrl = '/Api/Template/';

    function getConfig(tracker) {
        var config = {};
        if (tracker) {
            config.tracker = tracker;
        }
        return config;
    };

    return {
        //categories
        tryCompileTemplate: function (template, tracker) {
            return $http.post(baseUrl + 'TryCompileTemplate', template, getConfig(tracker));
        }
    };
}]);