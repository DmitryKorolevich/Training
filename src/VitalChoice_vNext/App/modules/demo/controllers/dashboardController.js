'use strict';

angular.module('app.modules.demo.controllers.dashboardController', [])
.controller('dashboardController', ['$scope', function ($scope) {
	$scope.monthlySalesData = [
                 {
                 	"key": "Monthly Sales for 2014",
                 	"values": [
						["Jan", 1497047.00],
						["Feb", 1215564.00],
						["Mar", 1510976.00],
						["Apr", 1224034.00],
						["May", 1277504.00],
						["Jun", 1524390.00],
						["Jul", 1343615.00],
						["Aug", 1256049.00],
						["Sep", 1306882.00],
						["Oct", 1616512.00],
						["Nov", 1559062.00],
						["Dec", 1865756.00]
                 	]
                 }
	];

	$scope.topSellingProducts = [
                 {
                 	"key": "Top Selling Products",
                 	"values": [
						["FGS", 9121247.00],
						["FGS-01", 8121247.00],
						["SKU-1", 7134247.00],
						["SKU-2", 6021235.00],
						["SKU-3", 5001247.00],
						["SKU-4", 4140000.00],
						["SKU-5", 4041230.00],
						["SKU-6", 3123247.00],
						["SKU-7", 1231232.00],
						["SKU-8", 768545.00],
						["SKU-9", 658430.00],
						["SKU-10", 253642.00]
                 	]
                 }
	];

	$scope.salesNewCustomers = [
                 {
                 	"key": "Sales by New Customers",
                 	"values": [
						["Kopp, Kristin", 12312345.00],
						["Lee, Bruce", 567567.00],
						["Jobs, Richard", 34234234.00],
						["Cameron, James", 12312312.00],
						["Bay, Michel", 1277504.00],
						["Jack, Brad", 342342.00],
						["Cooper, Simon", 1343615.00],
						["Gary, Gould", 234234.00],
						["John, James", 1306882.00],
						["Wilshire, Jack", 1616512.00],
						["White, Walter", 123123.00],
						["Swarz, Sara", 342232.00]
                 	]
                 }
	];

	$scope.salesExistingCustomers = [
                 {
                 	"key": "Sales by Existing Customers",
                 	"values": [
						["Simmons, Mark", 3423422.00],
						["Richrards, Rish", 12234564.00],
						["Dylon, Bob", 2343244.00],
						["Wick, Jim", 234234.00],
						["May, Patricia", 1277504.00],
						["Chippers, Richard", 324234.00],
						["July, June", 1343615.00],
						["Apolo, Tim", 36012339.00],
						["Pink, Luke", 123167.00],
						["Lee, Tom", 12312.00],
						["Page, Kira", 1559062.00],
						["Simmons, James", 123122.00]
                 	]
                 }
	];

	var format = d3.format('>$,.2f');
	$scope.valueFormatFunction = function () {
		return function (d) {
			return format(d);
		}
	};

}
]);