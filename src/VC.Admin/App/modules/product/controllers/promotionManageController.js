'use strict';

angular.module('app.modules.product.controllers.promotionManageController', [])
.controller('promotionManageController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', '$modal', 'productService', 'promotionService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, $timeout, $modal, productService, promotionService, toaster, confirmUtil, promiseTracker)
    {
        $scope.refreshTracker = promiseTracker("get");

        function successSaveHandler(result)
        {
            if (result.Success)
            {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.promotion.Id = result.Data.Id;
            } else
            {
                var messages = "";
                if (result.Messages)
                {
                    $scope.forms.submitted = true;
                    $scope.detailsTab.active = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);
                    $.each(result.Messages, function (index, value)
                    {
                        if (value.Field)
                        {
                            if (value.Field.indexOf('.') > -1)
                            {
                                var items = value.Field.split(".");
                                $scope.forms[items[0]][items[1]][items[2]].$setValidity("server", false);
                            }
                            else
                            {
                                $.each($scope.forms, function (index, form)
                                {
                                    if (form && !(typeof form === 'boolean'))
                                    {
                                        if (form[value.Field] != undefined)
                                        {
                                            form[value.Field].$setValidity("server", false);
                                            if (formForShowing == null)
                                            {
                                                formForShowing = index;
                                            }
                                        }
                                    }
                                });
                            }
                        }
                        else
                        {
                            messages += value.Message + "<br/>"
                        }
                    });
                }
                if (messages)
                {
                    toaster.pop('error', "Error!", messages, null, 'trustedHtml');
                }
            }
        };

        function errorHandler(result)
        {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function initialize()
        {
            $scope.id = $stateParams.id ? $stateParams.id : 0;

            $scope.assignedCustomerTypes = angular.copy($rootScope.ReferenceData.CustomerTypes);
            $scope.assignedCustomerTypes.splice(0, 0, { Key: null, Text: 'All' });
            $scope.promotionTypes = $rootScope.ReferenceData.PromotionTypes;
            $scope.maxTimesUseModes = [
                { Key: 1, Text: 'One Time Only' },
                { Key: 2, Text: 'Unlimited' },
                { Key: 3, Text: 'Number of Uses' },
            ];

            $scope.skuFilter = {
                Code: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };

            $scope.forms = {};
            $scope.detailsTab = {
                active: true
            };
            $scope.options = {};

            loadCategories();
        };

        function loadCategories()
        {
            productService.getCategoriesTree({}, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.rootCategory = result.Data;
                        loadPromotion();
                    } else
                    {
                        errorHandler(result);
                    }
                }).
                error(function (result)
                {
                    errorHandler(result);
                });
        };

        function loadPromotion()
        {
            promotionService.getPromotion($scope.id, $scope.refreshTracker)
			    .success(function (result)
			    {
			        if (result.Success)
			        {
			            $scope.promotion = result.Data;
			            if ($scope.promotion.ExpirationDate)
			            {
			                $scope.promotion.ExpirationDate = Date.parseDateTime($scope.promotion.ExpirationDate);
			            }
			            if ($scope.promotion.StartDate)
			            {
			                $scope.promotion.StartDate = Date.parseDateTime($scope.promotion.StartDate);
			            }

			            if ($scope.promotion.MaxTimesUse)
			            {
			                if ($scope.promotion.MaxTimesUse == 1)
			                {
			                    $scope.options.maxTimesUseMode = 1;
			                }
			                else
			                {
			                    $scope.options.maxTimesUseMode = 3;
			                    $scope.options.maxTimesUse = $scope.promotion.MaxTimesUse;
			                }
			            }
			            else
			            {
			                $scope.options.maxTimesUseMode = 2;
			            }

			            setSelected($scope.rootCategory, $scope.promotion.SelectedCategoryIds);
			            addProductsListWatchers();
			        } else
			        {
			            errorHandler(result);
			        }
			    }).
			    error(function (result)
			    {
			        errorHandler(result);
			    });
        };

        $scope.save = function ()
        {
            clearServerValidation();

            if (isMainFormValid())
            {
                var data = {};
                angular.copy($scope.promotion, data);
                if (data.ExpirationDate)
                {
                    data.ExpirationDate = data.ExpirationDate.toServerDateTime();
                }
                if (data.StartDate)
                {
                    data.StartDate = data.StartDate.toServerDateTime();
                }

                if ($scope.options.maxTimesUseMode == 1)
                {
                    data.MaxTimesUse = 1;
                } else if ($scope.options.maxTimesUseMode == 2)
                {
                    data.MaxTimesUse = null;
                } else
                {
                    data.MaxTimesUse = $scope.options.maxTimesUse;
                }

                var categoryIds = [];
                getSelected($scope.rootCategory, categoryIds);
                data.SelectedCategoryIds = categoryIds;

                promotionService.updatePromotion(data, $scope.refreshTracker).success(function (result)
                {
                    successSaveHandler(result);
                }).error(function (result)
                {
                    errorHandler(result);
                });
            } else
            {
                $scope.forms.submitted = true;
                $scope.detailsTab.active = true;
                toaster.pop('error', "Error!", "Validation errors, please correct field values.", null, 'trustedHtml');
            }
        };

        var clearServerValidation = function ()
        {
            $.each($scope.forms, function (index, form)
            {
                if (form && !(typeof form === 'boolean'))
                {
                    if (index == "PromotionsToBuySkus" || index == "PromotionsToGetSkus")
                    {
                        $.each(form, function (index, subForm)
                        {
                            if (index.indexOf('i') == 0)
                            {
                                $.each(subForm, function (index, element)
                                {
                                    if (element && element.$name == index)
                                    {
                                        element.$setValidity("server", true);
                                    }
                                });
                            }
                        });
                    }
                    else
                    {
                        $.each(form, function (index, element)
                        {
                            if (element && element.$name == index)
                            {
                                element.$setValidity("server", true);
                            }
                        });
                    }
                }
            });
        };

        var isMainFormValid = function ()
        {
            if ($scope.forms.mainForm.$valid && $scope.forms.PromotionsToBuySkus.$valid && $scope.forms.PromotionsToGetSkus.$valid)
            {
                return true;
            }
            else
            {
                //This hard code is realted with bug with setting an error to a form after closing a date picker popup with empty value
                //https://github.com/angular-ui/bootstrap/issues/3701
                if (Object.keys($scope.forms.mainForm.$error).length == 1 && $scope.forms.mainForm.$error.date)
                {
                    var valid = true;
                    $.each($scope.forms.mainForm.$error.date, function (index, item)
                    {
                        if (item.$formatters.length != 0)
                        {
                            valid = false;
                            return;
                        }
                    });
                    return valid;
                }
            }
            return false;
        };

        function notifyAboutAddBlockIds(name)
        {
            var blockIds = [];
            var list = $scope.promotion[name];
            $.each(list, function (index, item)
            {
                blockIds.push(item.IdSku);
            });
            var data = {};
            data.name = name;
            data.blockIds = blockIds;
            $scope.$broadcast('skusSearch#in#setBlockIds', data);
        };

        $scope.$on('skusSearch#out#addItems', function (event, args)
        {
            var list = $scope.promotion[args.name];
            if (list)
            {
                if (args.items)
                {
                    var newSelectedSkus = [];
                    $.each(args.items, function (index, item)
                    {
                        var add = true;
                        $.each(list, function (index, selectedSku)
                        {
                            if (item.Id == selectedSku.IdSku)
                            {
                                add = false;
                                return;
                            }
                        });
                        if (add)
                        {
                            var newSelectedSku = {};
                            newSelectedSku.IdSku = item.Id;
                            newSelectedSku.Quantity = 1;
                            newSelectedSku.Percent = 100;
                            newSelectedSku.ShortSkuInfo = {};
                            newSelectedSku.ShortSkuInfo.Code = item.Code;
                            newSelectedSku.ShortSkuInfo.ProductName = item.ProductName;
                            newSelectedSkus.push(newSelectedSku);
                        }
                    });
                    $.each(newSelectedSkus, function (index, newSelectedSku)
                    {
                        list.push(newSelectedSku);
                    });
                }
            }
        });

        function addProductsListWatchers()
        {
            $scope.$watchCollection('promotion.PromotionsToBuySkus', function ()
            {
                notifyAboutAddBlockIds('PromotionsToBuySkus');
            });
            $scope.$watchCollection('promotion.PromotionsToGetSkus', function ()
            {
                notifyAboutAddBlockIds('PromotionsToGetSkus');
            });
            notifyAboutAddBlockIds('PromotionsToBuySkus');
            notifyAboutAddBlockIds('PromotionsToGetSkus');
        };

        $scope.deletePromotionToBuySku = function (index)
        {
            $scope.promotion.PromotionsToBuySkus.splice(index, 1);
        };

        $scope.deletePromotionToGetSku = function (index)
        {
            $scope.promotion.PromotionsToGetSkus.splice(index, 1);
        };

        var getCategoriesTreeViewScope = function ()
        {
            return angular.element($('.categories .ya-treeview').get(0)).scope();
        };

        $scope.updateCategoriesCollapsed = function (expand)
        {
            var scope = getCategoriesTreeViewScope();
            if (expand)
            {
                scope.expandAll();
            }
            else
            {
                scope.collapseAll();
            }
            $scope.categoriesExpanded = expand;
        };

        function setSelected(category, ids)
        {
            category.IsSelected = false;
            $.each(ids, function (index, id)
            {
                if (category.Id == id)
                {
                    category.IsSelected = true;
                }
            });
            $.each(category.SubItems, function (index, value)
            {
                setSelected(value, ids);
            });
        };

        function getSelected(category, ids)
        {
            if (category.IsSelected)
            {
                ids.push(category.Id);
            }
            $.each(category.SubItems, function (index, value)
            {
                getSelected(value, ids);
            });
        };

        initialize();
    }
]);