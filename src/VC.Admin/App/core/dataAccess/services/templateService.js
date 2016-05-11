'use strict';

angular.module('app.core.dataAccess.services.templateService', [])
.service('templateService', ['$http', function ($http) {
    var baseUrl = '/Api/Content/';

    return {
        tryCompileTemplate: function (template) {
            return $http.post(baseUrl + 'TryCompileTemplate', { Template: template });
        }
    };
}]);