'use strict';

angular.module('app.core.dataAccess.services.fileService', [])
.service('fileService', ['$http', function ($http) {
	var baseUrl = '/Api/File/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
            //directories
	    getDirectories: function (tracker) {
	        return $http.get(baseUrl + 'GetDirectories', getConfig(tracker));
	    },
	    addDirectory: function (model, tracker) {
	        return $http.post(baseUrl + 'AddDirectory', model, getConfig(tracker));
	    },
	    deleteDirectory: function (model, tracker) {
	        return $http.post(baseUrl + 'DeleteDirectory', model, getConfig(tracker));
	    },

	    //files
	    getFiles: function (model, tracker) {
	        return $http.post(baseUrl + 'GetFiles', model, getConfig(tracker));
	    },
	    addFilesUrl: function () {
	        return baseUrl + 'AddFiles';
	    },
	    deleteFile: function (url, tracker) {
	        return $http.post(baseUrl + 'DeleteFile',url, getConfig(tracker));
	    },
	};
}]);