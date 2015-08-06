'use strict';

angular.module('app.modules.order.controllers.orderManageController',[])
.controller('orderManageController',['$scope','$rootScope','$state','$stateParams','$timeout','orderService',
    'productService','gcService','discountService','toaster','confirmUtil','promiseTracker',
function($scope,$rootScope,$state,$stateParams,$timeout,orderService,productService,gcService,discountService,
    toaster, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function successSaveHandler(result) {
        if (result.Success) {
            toaster.pop('success',"Success!","Successfully saved.");
            $scope.goBack();
        } else {
            var messages = "";
            if (result.Messages) {
                $scope.forms.mainForm.submitted=true;
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        }
    };

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize() {
        $scope.id=$stateParams.id?$stateParams.id:0;
        $scope.idCustomer=$stateParams.idcustomer?$stateParams.idcustomer:0;

        $scope.forms={};

        $scope.autoShipOrderFrequencies=[
            { Key: 1,Text: '1 Month' },
            { Key: 2,Text: '2 Months' },
            { Key: 3,Text: '3 Months' },
            { Key: 6,Text: '6 Months' }
        ];

        $scope.minimumPerishableThreshold=65;//should be loaded on edit open
        $scope.ignoneMinimumPerishableThreshold=false;

        $scope.order=
            {
                Source: 'test',
                SourceShowText: true,

                AlaskaHawaiiSurcharge: 0,
                CanadaSurcharge: 0,
                StandardShippingCharges: 0,
                TotalShipping: 0,

                ProductSubtotal: 0,
                Discount: 0,
                DiscountAmount: 0,
                DiscountedSubtotal: 0,
                ShippingTotal: 0,
                Tax: 0,
                GrandTotal: 0,

                GCs: [{ Code: '' }],

                AutoShipOrderFrequency: 1,

                Products: [
                    { SKU: '',SKUId: 1,QTY: '',ProductName: '',Price: null,Amount: '',IdProductType: 1,Messages: ['Out of stock','Duplicate SKU'] }
                ],
                ProductsPerishableThreshold: true,
            };

        $scope.legend={};

        initOrder();

        $scope.mainTab={
            active: true,
            formNames: ['mainForm','mainForm2', 'GCs'],
            name: $scope.id ? 'Edit Order' : 'New Order',
        };
        var tabs=[];
        tabs.push($scope.mainTab);
        $scope.tabs=tabs;
    };

    var initOrder=function()
    {
        $scope.order.OnHold=$scope.order.StatusCode=3;//on hold status
        $scope.$watch('order.OnHold',function(newValue,oldValue) {
            if(!newValue)
            {
                //TODO: set status
            }
        });

        //TODO: set needed data to the legend row
        $scope.legend.CustomerName="Test";
        $scope.legend.CustomerId=1;
    };

    $scope.productAdd=function()
    {
        var product={ SKU: '',SKUId: null,QTY: '',ProductName: '',Price: null,Anount: '', IdProductType: null, Messages: [] };
        $scope.order.Products.push(product);
    };

    $scope.productDelete=function(index) {
        $scope.order.Products.splice(index,1);
    };

    $scope.topPurchasedProducts=function() {
    };

    $scope.gcAdd=function()
    {
        $scope.order.GCs.push({ Code: '' });
    };

    $scope.gcDelete=function(index) {
        $scope.order.GCs.splice(index,1);
    };

    initialize();
}]);