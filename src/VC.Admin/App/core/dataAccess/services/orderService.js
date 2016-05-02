'use strict';

angular.module('app.core.dataAccess.services.orderService', [])
.service('orderService', ['$http', function ($http) {
	var baseUrl = '/Api/Order/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
	    //orders
	    getShortOrders: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetShortOrders', filter, getConfig(tracker));
	    },
	    getOrders: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetOrders', filter, getConfig(tracker));
	    },
	    getAutoShips: function (filter, tracker) {
	    	return $http.post(baseUrl + 'GetAutoShips', filter, getConfig(tracker));
	    },
	    getAutoShipCreditCards: function (orderId, customerId,  tracker) {
	    	return $http.get(baseUrl + 'GetAutoShipCreditCards?customerId=' + customerId + '&orderId=' + orderId, getConfig(tracker));
	    },
	    updateAutoShipBilling: function (model, orderId, tracker) {
	    	return $http.post(baseUrl + 'UpdateAutoShipBilling?orderId=' + orderId, model, getConfig(tracker));
	    },
	    activatePauseAutoShip: function (id, customerId, tracker) {
	    	return $http.post(baseUrl + 'ActivatePauseAutoShip?id=' +id +'&customerId=' + customerId, getConfig(tracker));
	    },
	    deleteAutoShip: function (id, customerId, tracker) {
	    	return $http.post(baseUrl + 'DeleteAutoShip?id=' + id + '&customerId=' + customerId, getConfig(tracker));
	    },
	    getOrder: function (id, idcustomer, refreshPrices, tracker)
	    {
	        return $http.get(baseUrl + 'GetOrder/{0}?idcustomer={1}&refreshprices={2}'.format(id, idcustomer, refreshPrices), getConfig(tracker));
	    },
	    calculateOrder: function (model, canceller)
	    {
	        return $http.post(baseUrl + 'CalculateOrder', model, { timeout: canceller.promise });
	    },
	    updateOrder: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateOrder', model, getConfig(tracker));
	    },
	    updateOrderStatus: function (id, status, orderpart, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateOrderStatus/{0}?status={1}&={2}'.format(id, status, orderpart), null, getConfig(tracker));
	    },
	    moveOrder: function (id, idCustomer, tracker)
	    {
	        return $http.post(baseUrl + 'MoveOrder/' + id + '?idcustomer=' + idCustomer, null, getConfig(tracker));
	    },
	    getHistoryReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetHistoryReport', filter, getConfig(tracker));
	    },
	    sendOrderConfirmationEmail: function (id, model, tracker)
	    {
	        return $http.post(baseUrl + 'SendOrderConfirmationEmail/{0}'.format(id), model, getConfig(tracker));
	    },
	    sendOrderShippingConfirmationEmail: function (id, model, tracker)
	    {
	        return $http.post(baseUrl + 'SendOrderShippingConfirmationEmail/{0}'.format(id), model, getConfig(tracker));
	    },
	    cancelOrder: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'CancelOrder/{0}'.format(id), null, getConfig(tracker));
	    },

	    //reship
	    getReshipOrder: function (id, idsource, idcustomer, tracker)
	    {
	        return $http.get(baseUrl + 'GetReshipOrder/{0}?idsource={1}&idcustomer={2}'.format(id, idsource, idcustomer), getConfig(tracker));
	    },
	    updateReshipOrder: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateReshipOrder', model, getConfig(tracker));
	    },

	    //refund
	    getRefundOrder: function (id, idsource, idcustomer, tracker)
	    {
	        return $http.get(baseUrl + 'GetRefundOrder/{0}?idsource={1}&idcustomer={2}'.format(id, idsource, idcustomer), getConfig(tracker));
	    },
	    addRefundOrder: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'AddRefundOrder', model, getConfig(tracker));
	    },
	    calculateRefundOrder: function (model, canceller)
	    {
	        return $http.post(baseUrl + 'CalculateRefundOrder', model, { timeout: canceller.promise });
	    },
	    cancelRefundOrder: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'CancelRefundOrder/{0}'.format(id), null, getConfig(tracker));
	    },

	    getIsBrontoSubscribed: function (email, tracker)
	    {
	        return $http.get(baseUrl + 'GetIsBrontoSubscribed/{0}'.format(email), getConfig(tracker));
	    },	    

	    //orders region sales statistic
	    getOrdersRegionStatistic: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetOrdersRegionStatistic', filter, getConfig(tracker));
	    },
	    getOrdersRegionStatisticReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + 'GetOrdersRegionStatisticReportFile?from={0}&to={1}&idcustomertype={2}&idordertype={3}&buildNumber={4}'
                .format(filter.From, filter.To, filter.IdCustomerType, filter.IdOrderType, buildNumber);
	    },
	    getOrdersZipStatistic: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetOrdersZipStatistic', filter, getConfig(tracker));
	    },
	    getOrdersZipStatisticReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + 'GetOrdersZipStatisticReportFile?from={0}&to={1}&idcustomertype={2}&idordertype={3}&buildNumber={4}'
                .format(filter.From, filter.To, filter.IdCustomerType, filter.IdOrderType, buildNumber);
	    },
	    getOrderWithRegionInfoItems: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetOrderWithRegionInfoItems', filter, getConfig(tracker));
	    },
	    getOrderWithRegionInfoItemsReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + 'GetOrderWithRegionInfoItemsReportFile?from={0}&to={1}&idcustomertype={2}&idordertype={3}&region={4}&zip={5}&buildNumber={6}'
                .format(filter.From, filter.To, filter.IdCustomerType, filter.IdOrderType, filter.Region, filter.Zip, buildNumber);
	    },
	    getOrderWithRegionInfoAmount: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetOrderWithRegionInfoAmount', filter, getConfig(tracker));
	    },

	    //agents report
	    getOrdersAgentReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetOrdersAgentReport', filter, getConfig(tracker));
	    },
	    getOrdersAgentReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + 'GetOrdersAgentReportFile?from={0}&to={1}&frequencytype={2}&idadminteam={3}&idadmin={4}&buildNumber={5}'
                .format(filter.From, filter.To, filter.FrequencyType, filter.IdAdminTeam, filter.IdAdmin,buildNumber);
	    },

        //gcs
	    getGCOrders: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetGCOrders/'+ id, getConfig(tracker));
	    },

	    //service codes
	    getServiceCodesReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetServiceCodesReport', filter, getConfig(tracker));
	    },
	};
}]);