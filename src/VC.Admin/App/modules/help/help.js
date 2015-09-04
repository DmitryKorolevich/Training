'use strict';

angular.module('app.modules.help',[
	'app.modules.help.controllers.helpTicketsController',
	'app.modules.help.controllers.helpTicketDetailController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
				.state('index.oneCol.manageHelpTickets', {
				    url: '/help/tickets',
				    templateUrl: 'app/modules/help/partials/helpTicketsList.html',
				    controller: 'helpTicketsController'
				})
            	.state('index.oneCol.helpTicketDetail',{
            	    url: '/help/tickets/{id:int}',
            		templateUrl: 'app/modules/help/partials/helpTicketDetail.html',
            		controller: 'helpTicketDetailController'
            	});
		}
]);