'use strict';

angular.module('app.modules.demo.controllers.orderDetailController', [])
.controller('orderDetailController', ['$scope', 'toaster', function ($scope, toaster) {
	$scope.orders = [
		{ RowStatus: 1, ProcessingStatus: 2, Status: 'Processed', Agent: 'GC', OrderType: 'Phone', OrderDate: '9/2/2015', OrderId: '111111', OrderTotal: '$1395.00', ShipTo: 'Smith, Bob' },
		{ RowStatus: 1, ProcessingStatus: 2, Status: 'Refund', Agent: '', OrderType: 'Phone', OrderDate: '3/2/2010', OrderId: '223234', OrderTotal: '$3265.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 1, ProcessingStatus: 3, Status: 'Refund', Agent: '', OrderType: 'Phone', OrderDate: '7/2/2015', OrderId: '312323', OrderTotal: '$235.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 2, ProcessingStatus: 2, Status: 'Processed', Agent: 'GC', OrderType: 'Phone', OrderDate: '3/2/2015', OrderId: '454532', OrderTotal: '$323.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 1, ProcessingStatus: 2, Status: 'Refund', Agent: '', OrderType: 'Phone', OrderDate: '3/2/2015', OrderId: '677898', OrderTotal: '$3235.00', ShipTo: 'Smith, Bob' },
		{ RowStatus: 3, ProcessingStatus: 1, Status: 'On Hold', Agent: '', OrderType: 'Phone', OrderDate: '6/2/2012', OrderId: '898967', OrderTotal: '$23.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 1, ProcessingStatus: 2, Status: 'Refund', Agent: '', OrderType: 'Phone', OrderDate: '3/2/2015', OrderId: '245154', OrderTotal: '$395.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 3, ProcessingStatus: 2, Status: 'Processed', Agent: 'GC', OrderType: 'Phone', OrderDate: '3/2/2015', OrderId: '145154', OrderTotal: '$295.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 1, ProcessingStatus: 2, Status: 'Refund', Agent: '', OrderType: 'Phone', OrderDate: '3/2/2015', OrderId: '545154', OrderTotal: '$392.00', ShipTo: 'Smith, Bob' },
		{ RowStatus: 1, ProcessingStatus: 2, Status: 'Processed', Agent: 'GC', OrderType: 'Phone', OrderDate: '3/2/2015', OrderId: '545154', OrderTotal: '$345.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 3, ProcessingStatus: 2, Status: 'Processed', Agent: 'GC', OrderType: 'Phone', OrderDate: '3/4/2015', OrderId: '545154', OrderTotal: '$395.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 1, ProcessingStatus: 3, Status: 'Refund', Agent: '', OrderType: 'Phone', OrderDate: '3/2/2015', OrderId: '543154', OrderTotal: '$395.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 3, ProcessingStatus: 1, Status: 'On Hold', Agent: '', OrderType: 'Phone', OrderDate: '3/4/2015', OrderId: '533454', OrderTotal: '$334.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 2, ProcessingStatus: 2, Status: 'Processed', Agent: 'GC', OrderType: 'Phone', OrderDate: '3/2/2015', OrderId: '544354', OrderTotal: '$455.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 1, ProcessingStatus: 2, Status: 'Refund', Agent: '', OrderType: 'Phone', OrderDate: '9/10/203', OrderId: '543454', OrderTotal: '$3455.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 2, ProcessingStatus: 2, Status: 'Processed', Agent: 'GC', OrderType: 'Phone', OrderDate: '3/2/2015', OrderId: '535154', OrderTotal: '$3455.00', ShipTo: 'Smith, Bob' },
		{ RowStatus: 1, ProcessingStatus: 3, Status: 'Refund', Agent: '', OrderType: 'Phone', OrderDate: '3/4/2015', OrderId: '541154', OrderTotal: '$9.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 3, ProcessingStatus: 2, Status: 'Processed', Agent: 'GC', OrderType: 'Phone', OrderDate: '3/2/2015', OrderId: '545154', OrderTotal: '$95.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 2, ProcessingStatus: 2, Status: 'Processed', Agent: 'GC', OrderType: 'Phone', OrderDate: '3/2/2015', OrderId: '525154', OrderTotal: '$395.00', ShipTo: 'Gould, Gary' },
		{ RowStatus: 1, ProcessingStatus: 2, Status: 'Processed', Agent: 'GC', OrderType: 'Phone', OrderDate: '3/2/2015', OrderId: '545153', OrderTotal: '$35.00', ShipTo: 'Smith, Bob' }
	];

	$scope.currentCustomer = {
		PermanentlyUpdate: true,
		LinkedToAffiliate: '',
		Company: 'VitalChoice',
		FirstName: 'Gary',
		LastName: 'Gould',
		Address1: '806 Front ST',
		Address2: '',
		City: 'Lynden',
		Country: 'US',
		State: 'WA',
		Zip: '98264',
		Phone: '3606565717',
		Fax: 'US',
		Email: 'gary@g2-designgroup.com',
		Files: [
			{ UploadDate: '6/3/2014', Description: 'Some file', FileName: 'fileName.gif' },
			{ UploadDate: '16/3/2013', Description: 'Some file 2', FileName: 'fileName2.jpg' },
			{ UploadDate: '4/2/2013', Description: 'Some file 3', FileName: 'fileName3.gif' }
		],
		CreditCardApproved: true,
		OACApproved: false,
		CheckApproved: false,
		DefaultPaymentMethod: 0,
		Extraice: false,
		Ib: false,
		In: false,
		Ps: false,
		Special: false,
		DoNotMail: false,
		DoNotRent: false,
		SuspendUserAccount: false,
		Reason: '',
		CustomerType: 1,
		TaxExempt: 0,
		Website: '',
		Tier: 0,
		TradeClass: 0,
	};

	$scope.openDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.date.opened = true;
    };

    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };

    $scope.date = { 
      opened: false, 
      InceptionDate: new Date()
    };

    $scope.$watch("currentCustomer.CustomerType", function (newValue) {
    	if (newValue == 2) {
    		toaster.pop('warning', "Caution!", "Changing customer type value will remove the current affiliate linking with customer.");
    	}
    });
}]);