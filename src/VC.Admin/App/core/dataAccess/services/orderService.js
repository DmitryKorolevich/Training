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

	function generateQueryParamsBasedOnFilter(filter)
	{
	    var url = '';
	    $.each(filter, function(index, item)
	    {
	        if(index != 'Paging' && index!= 'Sorting')
	        {
	            url+='{0}={1}&'.format(index.toLowerCase(), item);
	        }
	    });
	    return url;
	}

	return {
	    //orders
	    getShortOrders: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetShortOrders', filter, getConfig(tracker));
	    },
	    exportOrders: function (filter, tracker) {
	        return $http.post(baseUrl + 'exportorders', filter, getConfig(tracker));
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
	    activatePauseAutoShip: function (id, customerId, activate, tracker) {
	        return $http.post(baseUrl + 'ActivatePauseAutoShip?id=' + id + '&customerId=' + customerId + '&activate=' + activate, getConfig(tracker));
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
	        return baseUrl + 'GetOrdersAgentReportFile?from={0}&to={1}&frequencytype={2}&idadminteams={3}&idadmin={4}&buildNumber={5}'
                .format(filter.From, filter.To, filter.FrequencyType, filter.sIdAdminTeams, filter.IdAdmin,buildNumber);
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

	    //wholesale dropship report
	    getWholesaleDropShipReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetWholesaleDropShipReport', filter, getConfig(tracker));
	    },
	    getOrdersForWholesaleDropShipReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetOrdersForWholesaleDropShipReport', filter, getConfig(tracker));
	    },
	    getOrdersForWholesaleDropShipReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + ('GetOrdersForWholesaleDropShipReportFile?from={0}&to={1}&shipfrom={2}&shipto={3}&idcustomertype={4}&idtradeclass={5}'+
                '&customerfirstname={6}&customerlastname={7}&shipfirstname={8}&shiplastname={9}' +
                '&shipidconfirm={10}&idorder={11}&ponumber={12}&customercompany={13}&buildNumber={14}')
                .format(filter.From, filter.To, filter.ShipFrom, filter.ShipTo, filter.IdCustomerType, filter.IdTradeClass,
                filter.CustomerFirstName, filter.CustomerLastName, filter.ShipFirstName, filter.ShipLastName,
                filter.ShippingIdConfirmation, filter.IdOrder, filter.PoNumber, filter.CustomerCompany, buildNumber);
	    },
	    //transaction refund report
	    getTransactionAndRefundReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetTransactionAndRefundReport', filter, getConfig(tracker));
	    },
	    getTransactionAndRefundReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + ('GetTransactionAndRefundReportFile?from={0}&to={1}&idcustomertype={2}&idservicecode={3}' +
                '&customerfirstname={4}&customerlastname={5}&idcustomer={6}' +
                '&idorder={7}&idorderstatus={8}&idordertype={9}&buildNumber={10}')
                .format(filter.From, filter.To, filter.IdCustomerType, filter.IdServiceCode,
                filter.CustomerFirstName, filter.CustomerLastName, filter.IdCustomer,
                filter.IdOrder, filter.IdOrderStatus, filter.IdOrderType, buildNumber);
	    },
	    //orders summary sales
	    getOrdersSummarySalesOrderTypeStatisticItems: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetOrdersSummarySalesOrderTypeStatisticItems', filter, getConfig(tracker));
	    },
	    getOrdersSummarySalesOrderItems: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetOrdersSummarySalesOrderItems', filter, getConfig(tracker));
	    },
	    getOrdersSummarySalesOrderItemsReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + ('GetOrdersSummarySalesOrderItemsReportFile?from={0}&to={1}&shipfrom={2}&shipto={3}&firstorderfrom={4}&firstorderto={5}'+
                '&idcustomertype={6}&idcustomersource={7}' +
                '&customersourcedetails={8}&idcustomer={9}&keycode={10}' +
                '&discountcode={11}&isaffiliate={12}&fromcount={13}&tocount={14}&buildNumber={15}')
                .format(filter.From, filter.To, filter.ShipFrom, filter.ShipTo, filter.FirstOrderFrom, filter.FirstOrderTo,
                filter.IdCustomerType, filter.IdCustomerSource,
                filter.CustomerSourceDetails, filter.IdCustomer, filter.KeyCode,
                filter.DiscountCode, filter.IsAffiliate, filter.FromCount, filter.ToCount, buildNumber);
	    },
	    getSkuAddressReportItems: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetSkuAddressReportItems', filter, getConfig(tracker));
	    },
	    getSkuAddressReportItemsReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + ('GetSkuAddressReportItemsReportFile?from={0}&to={1}&idcustomertype={2}&skucode={3}&discountcode={4}&withoutdiscount={5}&buildNumber={6}')
                .format(filter.From, filter.To, filter.IdCustomerType, filter.SkuCode, filter.DiscountCode, filter.WithoutDiscount, buildNumber);
	    },
	    getMatchbackReportItems: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetMatchbackReportItems', filter, getConfig(tracker));
	    },
	    getMatchbackItemsReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + ('GetMatchbackItemsReportFile?from={0}&to={1}&idordersource={2}&buildNumber={3}')
                .format(filter.From, filter.To, filter.IdOrderSource, buildNumber);
	    },
	    getMailingReportItems: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetMailingReportItems', filter, getConfig(tracker));
	    },
	    getMailingReportItemsReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + ('GetMailingReportItemsReportFile?{0}buildNumber={1}')
                .format(generateQueryParamsBasedOnFilter(filter), buildNumber);
	    },
	    getOrderSkuCountReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetOrderSkuCountReport', filter, getConfig(tracker));
	    },
	    getShippedViaSummaryReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetShippedViaSummaryReport', filter, getConfig(tracker));
	    },
	    getShippedViaItemsReportOrderItems: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetShippedViaItemsReportOrderItems', filter, getConfig(tracker));
	    },
	    getShippedViaItemsReportOrderItemsReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + ('GetShippedViaItemsReportOrderItemsReportFile?{0}buildNumber={1}')
                .format(generateQueryParamsBasedOnFilter(filter), buildNumber);
	    },
	    getProductQualitySalesReportItems: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetProductQualitySalesReportItems', filter, getConfig(tracker));
	    },
	    getProductQualitySkusReportItems: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetProductQualitySkusReportItems', filter, getConfig(tracker));
	    },
	    getAffiliateOrdersInfoReportFile: function (id, buildNumber)
	    {
	        return baseUrl + ('GetAffiliateOrdersInfoReportFile/{0}?buildNumber={1}')
                .format(id, buildNumber);
	    },
	    getAAFESReportItems: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetAAFESReportItems', filter, getConfig(tracker));
	    },
	    getAAFESReportItemsReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + ('GetAAFESReportItemsReportFile?{0}buildNumber={1}')
                .format(generateQueryParamsBasedOnFilter(filter), buildNumber);
	    },
	};
}]);