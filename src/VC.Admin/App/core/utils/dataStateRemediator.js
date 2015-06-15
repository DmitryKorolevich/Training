'use strict';

angular
	.module('app.core.utils.dataStateRemediator', [])
	.factory('dataStateRemediator', [
		'$window', '$injector', 'webStorageUtil', '$location', '$rootScope', function ($window, $injector, webStorageUtil, $location, $rootScope) {
			var regex = /^\/api\/([a-zA-Z]+[a-zA-Z0-9]*)\/?.*$/i;

			return {
				runSaveDataScenario: function(response) {
					var infoPopupUtil = $injector.get('infoPopupUtil');

					infoPopupUtil.info('New Admin Version Available', "<p>The admin has recently been updated. In order to provide you with the latest version we must reload your current page to finish the installation.</p>" +
"<p>What does this mean for you?" +
"<ul><li>If you received this message while clicking the save button, it means <strong>NONE</strong> of your data has been saved. Rest assured your data should be retained and you just simply need to review your data entries for accuracy and ensure all is still present and then click save again.</li>" +
"<li>If you received this message while viewing a page, there is nothing you need to do, the page will just simply refresh.</li></ul></p>" +
"<p>Upon clicking OK below the page will refresh.</p>", function () {
						var data = response.config.data;
						if (data && response.config.method === "POST" && response.config.url.indexOf("/Api/") === 0) {
							var result = response.config.url.match(regex)
							if (result.length === 2) {
								webStorageUtil.setSession(result[1] + ":" + $location.path(), angular.toJson(data));
							}
						}

						$window.location.reload(true);
					});
				},
				runRestoreDataScenario: function (response) {
					if (/*response.config.method === "GET" &&*/ response.config.url.indexOf("/Api/") === 0 && response.data && response.data.Data) {
						var result = response.config.url.match(regex)
						if (result.length === 2) {
							var key = result[1] + ":" + $location.path();
							var json = webStorageUtil.getSession(key)
							if (json) {
								var item = angular.fromJson(json);
								if (item) {
									var originalData = response.data.Data;
									if ((originalData.Id != undefined && originalData.Id === item.Id)
										|| (originalData.PublicId != undefined && originalData.PublicId === item.PublicId)) {

										angular.forEach(originalData, function (propertyValue, propertyName) {
											if (item[propertyName]) {
												originalData[propertyName] = item[propertyName];
											}
										});

										webStorageUtil.removeSession(key);
										$rootScope.lastRemediationKey = null;
									} else {
										$rootScope.lastRemediationKey = key;
									}
								}
							}
						}
					}
				}
			}
		}
	]);