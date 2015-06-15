'use strict';

angular.module('app.core.dataAccess.services.orderNoteService', [])
.service('orderNoteService', ['$http', function ($http) {
	var baseUrl = '/Api/OrderNote/';

	function getConfig(tracker) {
		var config = {  };
		if (tracker) {
			config.tracker = tracker;
		}
		return config;
	};

	return {
		getOrderNotes: function(filter, tracker) {
			return $http.post(baseUrl + 'GetOrderNotes', filter, getConfig(tracker));
		},
		createOrderNotePrototype: function (tracker) {
			return $http.post(baseUrl + 'CreateOrderNotePrototype', null, getConfig(tracker));
		},
		createOrderNote: function (createOrderNoteModel, tracker) {
			return $http.post(baseUrl + 'CreateOrderNote', createOrderNoteModel, getConfig(tracker));
		},
		updateOrderNote: function (editOrderNoteModel, tracker) {
			return $http.post(baseUrl + 'UpdateOrderNote', editOrderNoteModel, getConfig(tracker));
		},
		getOrderNote: function (id, tracker) {
			return $http.get(baseUrl + 'GetOrderNote/' + id, getConfig(tracker));
		},
		deleteOrderNote: function (id, tracker) {
			return $http.get(baseUrl + 'DeleteOrderNote/'+ id, getConfig(tracker));
		}
	};
}]);