'use strict';

angular.module('app.core.dataAccess.services.gcService', [])
.service('gcService', ['$http', function ($http) {
    var baseUrl = '/Api/GC/';

    function getConfig(tracker) {
        var config = {};
        if (tracker) {
            config.tracker = tracker;
        }
        return config;
    };

    return {
        getGiftCertificates: function (filter, tracker) {
            return $http.post(baseUrl + 'GetGiftCertificates', filter, getConfig(tracker));
        },
        getGiftCertificate: function (id, tracker) {
            return $http.get(baseUrl + 'GetGiftCertificate/' + id, getConfig(tracker));
        },
        getGiftCertificatesAdding: function (tracker) {
            return $http.post(baseUrl + 'GetGiftCertificatesAdding', getConfig(tracker));
        },
        addGiftCertificates: function (quantity, model, tracker) {
            return $http.post(baseUrl + 'AddGiftCertificates/?quantity=' + quantity, model, getConfig(tracker));
        },
        updateGiftCertificate: function (model, tracker) {
            return $http.post(baseUrl + 'UpdateGiftCertificate', model, getConfig(tracker));
        },
        sendGiftCertificateEmail: function (model, tracker) {
            return $http.post(baseUrl + 'SendGiftCertificateEmail', model, getConfig(tracker));
        },
        deleteGiftCertificate: function (id, tracker) {
            return $http.post(baseUrl + 'DeleteGiftCertificate/' + id, getConfig(tracker));
        },
    };
}]);