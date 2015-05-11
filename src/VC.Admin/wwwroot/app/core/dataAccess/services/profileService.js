'use strict';

angular.module('app.core.dataAccess.services.profileService', [])
.service('profileService', ['$http', function ($http) {
    var baseUrl = '/Api/Profile/';

    function getConfig(tracker) {
        var config = {};
        if (tracker) {
            config.tracker = tracker;
        }
        return config;
    };

    return {
        updateProfile: function (profile, tracker) {
            return $http.post(baseUrl + 'UpdateProfile', profile, getConfig(tracker));
        },
        getProfile: function (tracker) {
            return $http.get(baseUrl + 'GetProfile', getConfig(tracker));
        }
    };
}]);