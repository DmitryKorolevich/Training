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
	    //help tickets
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

	    //help comments
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

	    //bug tickets
	    getBugTickets: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetBugTickets', filter, getConfig(tracker));
	    },
	    getBugTicket: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetBugTicket/' + id, getConfig(tracker));
	    },
	    updateBugTicket: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateBugTicket', model, getConfig(tracker));
	    },
	    deleteBugTicket: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'DeleteBugTicket/' + id, null, getConfig(tracker));
	    },

	    //bug comments
	    getBugTicketComment: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetBugTicketComment/' + id, getConfig(tracker));
	    },
	    updateBugTicketComment: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateBugTicketComment', model, getConfig(tracker));
	    },
	    deleteBugTicketComment: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'DeleteBugTicketComment/' + id, null, getConfig(tracker));
	    },

	    //bug files
	    deleteBugTicketFile: function (publicId, fileName, id, tracker)
	    {
	        return $http.post(baseUrl + 'DeleteBugTicketFile', { PublicId: publicId, FileName: fileName, Id: id }, getConfig(tracker));
	    },
	    deleteBugTicketCommentFile: function (publicId, fileName, id, tracker)
	    {
	        return $http.post(baseUrl + 'DeleteBugTicketCommentFile', { PublicId: publicId, FileName: fileName, Id: id }, getConfig(tracker));
	    },
	};
}]);