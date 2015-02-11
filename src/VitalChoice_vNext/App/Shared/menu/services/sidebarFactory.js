'use strict';

angular.module('app.shared.menu.services.sidebarFactory', [])
.factory('sidebarFactory', [function () {
	var sidebars = [
		{
			name: 'sidebar1', stateName: 'index.state3', stateLabel: 'Sidebar1', menu: [
			  { name: 'sidebar1.child1', stateName: 'index.state3.sidebar1', stateLabel: 'Sidebar Option 1' },
			  { name: 'sidebar1.child2', stateName: 'index.state3.sidebar2', stateLabel: 'Sidebar Option 2' },
			  { name: 'sidebar1.child3', stateName: 'index.state3.sidebar3', stateLabel: 'Sidebar Option 3' }
			]
		},
		{
			name: 'sidebar2', stateName: 'index.state4', stateLabel: 'Sidebar2', menu: [
			  { name: 'sidebar1.child1', stateName: 'index.state3.sidebar1', stateLabel: 'Sidebar Option 1' },
			  { name: 'sidebar1.child2', stateName: 'index.state3.sidebar2', stateLabel: 'Sidebar Option 2' },
			  { name: 'sidebar1.child3', stateName: 'index.state3.sidebar3', stateLabel: 'Sidebar Option 3' }
			]
		}
	];

	return {
		sidebars: sidebars
	};

}]);