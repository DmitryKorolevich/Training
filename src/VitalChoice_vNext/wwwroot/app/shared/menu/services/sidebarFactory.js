'use strict';

angular.module('app.shared.menu.services.sidebarFactory', [])
.factory('sidebarFactory', [function () {
	var sidebars = [
		{
			name: 'sidebar1', moduleName: 'state3', menu: [
			  { name: 'sidebar1.child1', stateName: 'index.twoCols.state3.sidebar1', stateLabel: 'Sidebar Option 1' },
			  { name: 'sidebar1.child2', stateName: 'index.twoCols.state3.sidebar2', stateLabel: 'Sidebar Option 2' },
			  { name: 'sidebar1.child3', stateName: 'index.twoCols.state3.sidebar3', stateLabel: 'Sidebar Option 3' }
			]
		},
		{
			name: 'sidebar2', moduleName: 'state4', menu: [
			  { name: 'sidebar1.child1', stateName: 'index.twoCols.state3.sidebar1', stateLabel: 'Sidebar Option 11' },
			  { name: 'sidebar1.child2', stateName: 'index.twoCols.state3.sidebar2', stateLabel: 'Sidebar Option 222' },
			  { name: 'sidebar1.child3', stateName: 'index.twoCols.state3.sidebar3', stateLabel: 'Sidebar Option 3222' }
			]
		}
	];

	return {
		sidebars: sidebars
	};

}]);