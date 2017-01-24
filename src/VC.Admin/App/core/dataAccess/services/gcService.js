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
        requestGiftCertificatesReportFile: function (filter, tracker)
        {
            return $http.post(baseUrl + 'RequestGiftCertificatesReportFile', filter, getConfig(tracker));
        },
        getGiftCertificatesReportFile: function (id, buildNumber)
        {
            return baseUrl + ('GetGiftCertificatesReportFile/{0}?buildNumber={1}')
                .format(id, buildNumber);
        },
        getGiftCertificatesWithOrderInfo: function (filter, tracker) {
            return $http.post(baseUrl + 'GetGiftCertificatesWithOrderInfo', filter, getConfig(tracker));
        },
        getGiftCertificatesWithOrderInfoReportFile: function (filter, buildNumber)
        {
            return baseUrl + 'GetGiftCertificatesWithOrderInfoReportFile?from={0}&to={1}&type={2}&status={3}&billinglastname={4}&shippinglastname={5}&notzerobalance={6}&buildNumber={7}'
                .format(filter.From, filter.To, filter.Type, filter.StatusCode, filter.BillingAddress.LastName, filter.ShippingAddress.LastName, filter.NotZeroBalance, buildNumber);
        },
        getGiftCertificate: function (id, tracker) {
            return $http.get(baseUrl + 'GetGiftCertificate/' + id, getConfig(tracker));
        },
        getGiftCertificatesAdding: function (tracker) {
            return $http.post(baseUrl + 'GetGiftCertificatesAdding', null, getConfig(tracker));
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
            return $http.post(baseUrl + 'DeleteGiftCertificate/' + id, null, getConfig(tracker));
        },
    };
}]);