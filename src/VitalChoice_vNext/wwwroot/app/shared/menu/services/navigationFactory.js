'use strict';

angular.module('app.shared.menu.services.navigationFactory', [])
.factory('navigationFactory', [function () {
	var menues = [
		{ stateName: 'state1', stateLabel: 'Label1' },
		{ stateName: 'state2', stateLabel: 'Label2' },
		{ stateName: 'state3', stateLabel: 'Label3' },
		{ stateName: 'state4', stateLabel: 'Label4' },
		{ stateName: 'state5', stateLabel: 'Label5' },
		{ stateName: 'state6', stateLabel: 'Label6' },
		{ stateName: 'state7', stateLabel: 'Label7' }
	];

	return {
		menues : menues
	};

}]);