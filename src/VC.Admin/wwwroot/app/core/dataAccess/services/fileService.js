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
	    getGetDirectories: function (filter, tracker) {
	        return $http.get(baseUrl + 'GetDirectories', getConfig(tracker));
	    },
	    addDirectory: function (url, name, tracker) {
	        return $http.post(baseUrl + 'AddDirectory/?url={0}&name={1}'.format(url, name), getConfig(tracker));
	    },
	    deleteDirectory: function (url, tracker) {
	        return $http.post(baseUrl + 'DeleteDirectory/'+url, getConfig(tracker));
	    },

	    //files
	    getFiles: function (url, tracker) {
	        return $http.get(baseUrl + 'GetFiles/'+url, getConfig(tracker));
	    },
	    addFilesUrl: function () {
	        return baseUrl + 'AddFiles';
	    },
	    deleteFile: function (url, tracker) {
	        return $http.post(baseUrl + 'DeleteFile/'+url, getConfig(tracker));
	    },
	};
}]);