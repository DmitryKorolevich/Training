'use strict';

angular.module('app.core.dataAccess.services.helpService', [])
.service('helpService', ['$http', function ($http)
{
	var baseUrl = '/Api/Help/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
	    //tickets
	    getHelpTickets: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetHelpTickets', filter, getConfig(tracker));
	    },
	    getHelpTicket: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetHelpTicket/' + id, getConfig(tracker));
	    },
	    updateHelpTicket: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateHelpTicket', model, getConfig(tracker));
	    },
	    deleteHelpTicket: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'DeleteHelpTicket/' + id, null, getConfig(tracker));
	    },

	    //comments
	    getHelpTicketComment: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetHelpTicketComment/' + id, getConfig(tracker));
	    },
	    updateHelpTicketComment: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateHelpTicketComment', model, getConfig(tracker));
	    },
	    deleteHelpTicketComment: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'DeleteHelpTicketComment/' + id, null, getConfig(tracker));
	    },
	};
}]);