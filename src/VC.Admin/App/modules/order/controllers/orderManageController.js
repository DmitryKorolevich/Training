'use strict';

angular.module('app.modules.order.controllers.orderManageController',[])
.controller('orderManageController',['$scope','$rootScope','$state','$stateParams','$timeout','modalUtil','orderService',
    'productService','gcService','discountService','toaster','confirmUtil','promiseTracker',
function($scope,$rootScope,$state,$stateParams,$timeout,modalUtil,orderService,productService,gcService,discountService,
    toaster,confirmUtil,promiseTracker) {
    $scope.refreshTracker=promiseTracker("get");
    $scope.editTracker=promiseTracker("edit");

    function successSaveHandler(result) {
        if(result.Success) {
            toaster.pop('success',"Success!","Successfully saved.");
            $scope.goBack();
        }
        else {
            var messages="";
            if(result.Messages) {
                $scope.forms.mainForm.submitted=true;
                $scope.forms.mainForm2.submitted=true;
                $scope.serverMessages=new ServerMessages(result.Messages);
                var formForShowing=null;
                $.each(result.Messages,function(index,value) {
                    if(value.Field) {
                        if(value.Field.indexOf('.')>-1) {
                            var items=value.Field.split(".");
                            $scope.forms[items[0]][items[1]][items[2]].$setValidity("server",false);
                            formForShowing=items[0];
                            openSKUs();
                        }
                        else {
                            $.each($scope.forms,function(index,form) {
                                if(form) {
                                    if(form[value.Field]!=undefined) {
                                        form[value.Field].$setValidity("server",false);
                                        if(formForShowing==null) {
                                            formForShowing=index;
                                        }
                                        return false;
                                    }
                                }
                            });
                        }
                    }
                });

                if(formForShowing) {
                    activateTab(formForShowing);
                }
            }
            toaster.pop('error',"Error!",messages,null,'trustedHtml');
        }
    };

    function activateTab(formName) {
        if(formName.indexOf('GCs')==0) {
            formName='GCs';
        }
        $.each($scope.tabs,function(index,item) {            
            $.each(item.formNames,function(index,form) {
                if(form==formName) {
                    item.active=true;
                    return false;
                }
            });
            if(item.active)
            {
                return false;
            }
        });
    };

    function errorHandler(result) {
        toaster.pop('error',"Error!","Server error occured");
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
        $scope.orderSources=$rootScope.ReferenceData.OrderSources;
        $scope.orderSourcesCelebrityHealthAdvocate=$rootScope.ReferenceData.OrderSourcesCelebrityHealthAdvocate;
        $scope.customerType=1//Retail - TODO: should be from real data

        $scope.discountsFilter={
            Code: '',
            Paging: { PageIndex: 1,PageItemCount: 20 },
        };

        $scope.gcsFilter={
            Code: '',
            Paging: { PageIndex: 1,PageItemCount: 20 },
        };

        $scope.skusFilter={
            Code: '',
            DescriptionName: '',
            Paging: { PageIndex: 1,PageItemCount: 20 },
        };

        $scope.skuFilter={
            ExactCode: '',
            ExactDescriptionName: '',
            Paging: { PageIndex: 1,PageItemCount: 1 },
        };

        $scope.order=
            {
                Source: null,
                ShipDelay: 0,

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
                    { Code: '',Id: null,QTY: '',ProductName: '',Price: null,Amount: '',IdProductType: null,Messages: [] }
                ],
                ProductsPerishableThreshold: false,
            };

        $scope.legend={};

        initOrder();

        $scope.mainTab={
            active: true,
            formNames: ['mainForm','mainForm2','GCs'],
            name: $scope.id?'Edit Order':'New Order',
        };
        var tabs=[];
        tabs.push($scope.mainTab);
        $scope.tabs=tabs;
    };

    var initOrder=function() {
        $scope.order.OnHold=$scope.order.StatusCode=3;//on hold status
        $scope.$watch('order.OnHold',function(newValue,oldValue) {
            if(!newValue) {
                //TODO: set status
            }
        });

        //TODO: set needed data to the legend row
        $scope.legend.CustomerName="Test";
        $scope.legend.CustomerId=1;
    };

    $scope.requestRecalculate=function() {
        console.log('rec');
    };

    var clearServerValidation=function() {
        $.each($scope.forms,function(index,form) {
            if(form) {
                if(index=="GCs") {
                    $.each(form,function(index,subForm) {
                        if(index.indexOf('i')==0) {
                            $.each(subForm,function(index,element) {
                                if(element&&element.$name==index) {
                                    element.$setValidity("server",true);
                                }
                            });
                        }
                    });
                }
                else {
                    $.each(form,function(index,element) {
                        if(element&&element.$name==index) {
                            element.$setValidity("server",true);
                        }
                    });
                }
            }
        });
    };

    $scope.save=function() {
        clearServerValidation();

        var valid=true;
        $.each($scope.forms,function(index,form) {
            if(form) {
                if(!form.$valid) {
                    valid=false;
                    activateTab(index);
                    return false;
                }
            }
        });

        if(valid) {

        } else {
            $scope.forms.mainForm.submitted=true;
            $scope.forms.mainForm2.submitted=true;
        }
    };

    $scope.gcLostFocus=function(index,code) {
        if(index!=0&&!code) {
            $scope.order.GCs.splice(index,1);
        }
        $scope.requestRecalculate();
    };

    $scope.getGCs=function(val) {
        $scope.gcsFilter.Code=val;
        return gcService.getGiftCertificates($scope.gcsFilter)
            .then(function(result) {
                return result.data.Data.Items.map(function(item) {
                    return item.Code;
                });
            });
    };

    $scope.getDiscounts=function(val) {
        $scope.discountsFilter.Code=val;
        return discountService.getDiscounts($scope.discountsFilter)
            .then(function(result) {
                return result.data.Data.Items.map(function(item) {
                    return item.Code;
                });
            });
    };

    $scope.productAdd=function() {
        if($scope.order.Products.length>0 && !$scope.order.Products[$scope.order.Products.length-1].Code)
        {
            return;
        }
        var product={ Code: '',Id: null,QTY: '',ProductName: '',Price: null,Amount: '',IdProductType: null,Messages: [] };
        $scope.order.Products.push(product);
    };

    $scope.productDelete=function(index) {
        if($scope.order.Products.length==1) {
            $scope.order.Products.splice(index,1);
            $scope.productAdd();
        }
        else {
            $scope.order.Products.splice(index,1);
        }
        $scope.requestRecalculate();
    };

    $scope.topPurchasedProducts=function() {
        modalUtil.open('app/modules/product/partials/topPurchasedProductsPopup.html','topPurchasedProductsController',{
            products: $scope.order.Products,thenCallback: function(data) {
                var newProducts=data;
                $.each(newProducts,function(index,newProduct) {
                    var add=true;
                    $.each($scope.order.Products,function(index,product) {
                        if(newProduct.Code==product.Code) {
                            add=false;
                            return false;
                        }
                    });

                    if(add)
                    {
                        if($scope.order.Products.length>0 && !$scope.order.Products[$scope.order.Products.length-1].Code) {
                            $scope.order.Products.splice($scope.order.Products.length-1, 1);
                        }

                        var product={};
                        product.QTY=1;
                        product.Code=newProduct.Code;
                        product.IdProductType=newProduct.ProductType;
                        product.ProductName=newProduct.DescriptionName;
                        if($scope.customerType==1) {
                            product.Price=newProduct.Price;
                        }
                        else if($scope.customerType==2) {
                            product.Price=newProduct.WholesalePrice;
                        }
                        product.Amount=product.Price;

                        $scope.order.Products.push(product);
                    }
                });

                $scope.requestRecalculate();
            }
        });
    };

    $scope.gcAdd=function() {
        $scope.order.GCs.push({ Code: '' });
    };

    $scope.gcDelete=function(index) {
        $scope.order.GCs.splice(index,1);
    };

    $scope.getSKUsBySKU=function(val) {
        $scope.skusFilter.Code=val;
        $scope.skusFilter.DescriptionName='';
        return productService.getSkus($scope.skusFilter)
            .then(function(result) {
                return result.data.Data.map(function(item) {
                    return item;
                });
            });
    };

    var skuChangedRequest=null;

    $scope.skuChanged=function(index) {
        //resolving issue with additional load after lost focus from the input if time of selecting a new element
        if(skuChangedRequest) {
            $timeout.cancel(skuChangedRequest);
        }
        skuChangedRequest=$timeout(function() {
            var product=$scope.order.Products[index];
            if(product&&($scope.skuFilter.ExactCode!=product.Code||$scope.skuFilter.ExactDescriptionName!='')) {
                $scope.skuFilter.ExactCode=product.Code;
                $scope.skuFilter.ExactDescriptionName='';
                productService.getSku($scope.skuFilter)
                    .success(function(result) {
                        if(result.Success) {
                            if(result.Data) {

                                product.QTY=1;
                                product.IdProductType=result.Data.ProductType;
                                product.ProductName=result.Data.DescriptionName;
                                if($scope.customerType==1) {
                                    product.Price=result.Data.Price;
                                }
                                else if($scope.customerType==2) {
                                    product.Price=result.Data.WholesalePrice;
                                }
                                product.Amount=product.Price;

                                $scope.requestRecalculate();
                            }
                        } else {
                            errorHandler(result);
                        }
                    })
                    .error(function(result) {
                        errorHandler(result);
                    });
                skuChangedRequest=null;
            }
        },100);
    };

    $scope.getSKUsByProductName=function(val) {
        $scope.skusFilter.Code='';
        $scope.skusFilter.DescriptionName=val;
        return productService.getSkus($scope.skusFilter)
            .then(function(result) {
                return result.data.Data.map(function(item) {
                    return item;
                });
            });
    };

    $scope.productNameChanged=function(index) {
        var product=$scope.order.Products[index];
        if(product) {
            $scope.skuFilter.ExactCode='';
            $scope.skuFilter.ExactDescriptionName=product.ProductName;
            productService.getSku($scope.skuFilter)
                .success(function(result) {
                    if(result.Success) {
                        if(result.Data) {

                            product.QTY=1;
                            product.IdProductType=result.Data.ProductType;
                            product.Code=result.Data.Code;
                            if($scope.customerType==1) {
                                product.Price=result.Data.Price;
                            }
                            else if($scope.customerType==2) {
                                product.Price=result.Data.WholesalePrice;
                            }
                            product.Amount=product.Price;

                            $scope.requestRecalculate();
                        }
                    } else {
                        errorHandler(result);
                    }
                })
                .error(function(result) {
                    errorHandler(result);
                });
        }
    };

    initialize();
}]);