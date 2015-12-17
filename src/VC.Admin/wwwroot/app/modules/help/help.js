'use strict';

angular.module('app.modules.help',[
	'app.modules.help.controllers.helpTicketsController',
	'app.modules.help.controllers.helpTicketDetailController',
	'app.modules.help.controllers.bugTicketsController',
	'app.modules.help.controllers.bugTicketDetailController',
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
            	})
				.state('index.oneCol.manageBugTickets', {
				    url: '/help/bugs/tickets',
				    templateUrl: 'app/modules/help/partials/bugTicketsList.html',
				    controller: 'bugTicketsController'
				})
		        .state('index.oneCol.addNewBugTicket', {
		            url: '/help/bugs/tickets/add',
		            templateUrl: 'app/modules/help/partials/bugTicketDetail.html',
		            controller: 'bugTicketDetailController'
		        })
            	.state('index.oneCol.bugTicketDetail', {
            	    url: '/help/bugs/tickets/{id:int}',
            	    templateUrl: 'app/modules/help/partials/bugTicketDetail.html',
            	    controller: 'bugTicketDetailController'
            	});
		}
]);