'use strict';

angular
	.module('app.core.utils.webStorageUtil', [])
	.factory('webStorageUtil', [
		'$window', function($window) {
			return {
				setLocal: function(key, value) {
					try {
						if ($window.Storage) {
							$window.localStorage.setItem(key, value);
							return true;
						} else {
							return false;
						}
					} catch (error) {
						console.error(error, error.message);
					}
				},
				getLocal: function(key) {
					try {
						if ($window.Storage) {
							return $window.localStorage.getItem(key);
						} else {
							return false;
						}
					} catch (error) {
						console.error(error, error.message);
					}
				},
				setSession: function(key, value) {
					try {
						if ($window.Storage) {
							$window.sessionStorage.setItem(key, value);
							return true;
						} else {
							return false;
						}
					} catch (error) {
						console.error(error, error.message);
					}
				},
				getSession: function(key) {
					try {
						if ($window.Storage) {
							return $window.sessionStorage.getItem(key);
						} else {
							return false;
						}
					} catch (error) {
						console.error(error, error.message);
					}
				},
				getSessionAndForget: function (key) {
					var item = this.getSession(key);
					if (item) {
						$window.sessionStorage.removeItem(key);
					}
					return item;
				},
				removeSession: function(key) {
					try {
						if ($window.Storage) {
							$window.sessionStorage.removeItem(key);
						} else {
							return false;
						}
					} catch (error) {
						console.error(error, error.message);
					}
				}
			}
		}
	]);