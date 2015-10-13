﻿'use strict';

angular.module('app.modules.misc',[
	'app.modules.misc.controllers.vitalGreenController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
				.state('index.oneCol.vitalGreen', {
				    url: '/tools/vital-green',
				    templateUrl: 'app/modules/misc/partials/vitalGreenReport.html',
				    controller: 'vitalGreenController'
				});
		}
]);