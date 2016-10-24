﻿'use strict';

angular.module('app.modules.content.controllers.recipeManageController', [])
.controller('recipeManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, contentService, settingService, toaster, confirmUtil, promiseTracker)
    {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");
        
        function refreshHistory()
        {
            if ($scope.recipe && $scope.recipe.Id)
            {
                var data = {};
                data.service = settingService;
                data.tracker = $scope.refreshTracker;
                data.idObject = $scope.recipe.Id;
                data.idObjectType = 10//recipe
                $scope.$broadcast('objectHistorySection#in#refresh', data);
            }
        }

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.id = result.Data.Id;
                $scope.recipe.Id = result.Data.Id;
                $scope.recipe.MasterContentItemId = result.Data.MasterContentItemId;
                $scope.previewUrl = $scope.baseUrl.format($scope.recipe.Url);
                refreshHistory();
            } else {
                var messages = "";
                if (result.Messages) {
                	$scope.forms.recipeForm.submitted = true;
                	$scope.forms.crossessubmitted = true;
                	$scope.forms.relatedsubmitted = true;

                	$scope.serverMessages = new ServerMessages(result.Messages);
                	var formForShowing = null;
                	$.each(result.Messages, function (index, value) {
                		if (value.Field) {
                			if (value.Field.indexOf('.') > -1) {
                				var items = value.Field.split(".");
                				$scope.forms[items[0]][items[1]][items[2]].$setValidity("server", false);
                				formForShowing = items[0];
                			}
                			else {
                				$.each($scope.forms, function (index, form) {
                					if (form && !(typeof form === 'boolean')) {
                						if (form[value.Field] != undefined) {
                							form[value.Field].$setValidity("server", false);
                							if (formForShowing == null) {
                								formForShowing = index;
                							}
                							return false;
                						}
                					}
                				});
                			}
                		}
                		messages += value.Message + "<br />";
                	});

                	if (formForShowing) {
                		activateTab(formForShowing);
                	}
				}
				toaster.pop('error', "Error!", messages, null, 'trustedHtml');
            }
        };

        function errorHandler(result) {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function initialize() {
            $scope.id = $stateParams.id ? $stateParams.id : 0;
            $scope.descriptionExpanded = false;

            $scope.toogleEditorState = function (property) {
                $scope[property] = !$scope[property];
            };

            $scope.statuses = [
                { Key: 1, Text: 'Draft' },
                { Key: 2, Text: 'Published' }
            ];

            $scope.baseUrl = 'http://' + $rootScope.ReferenceData.PublicHost + '/recipe/{0}';
            $scope.previewUrl = null;


            $scope.detailsTab = {
                index: 1,
				formName: 'details'
            };
            $scope.crossSellProductsAndVideosTab = {
                index: 2,
            	formName: 'crossSellRelatedRecipes',
            };
            $scope.options = {};
            $scope.options.activeTabIndex = $scope.detailsTab.index;
            var tabs = [];
            tabs.push($scope.detailsTab);
            tabs.push($scope.crossSellProductsAndVideosTab);
            $scope.tabs = tabs;

            $scope.forms = {};

            $scope.youTubeBaseUrl = 'https://www.youtube.com/watch?v={0}'

            var clearServerValidation = function () {
            	$.each($scope.forms, function (index, form) {
            		if (form && !(typeof form === 'boolean')) {
            			if (index == "RelatedRecipes" || index == "CrossSellRecipes") {
            				$.each(form, function (index, subForm) {
            					if (index.indexOf('i') == 0) {
            						$.each(subForm, function (index, element) {
            							if (element && element.$name == index) {
            								element.$setValidity("server", true);
            							}
            						});
            					}
            				});
            			}
            			else {
            				$.each(form, function (index, element) {
            					if (element && element.$name == index) {
            						element.$setValidity("server", true);
            					}
            				});
            			}
            		}
            	});
            };

            $scope.save = function () {
	            clearServerValidation();

	            var valid = true;
	            $.each($scope.forms, function (index, form) {
	            	if (form && !(typeof form === 'boolean')) {
	            		if (!form.$valid && index != 'submitted' && index != 'crossessubmitted' && index != 'relatedsubmitted') {
	            			valid = false;
	            			activateTab(index);
	            			return false;
	            		}
	            	}
	            });

                if (valid) {
                    var categoryIds = [];
                    getSelected($scope.rootCategory, categoryIds);
                    $scope.recipe.CategoryIds = categoryIds;

                    updateCrossRelated($scope.recipe.CrossSellRecipes);
                    updateCrossRelated($scope.recipe.RelatedRecipes);

                    contentService.updateRecipe($scope.recipe, $scope.editTracker).success(function (result) {
                        successSaveHandler(result);
                    }).
                        error(function (result) {
                            errorHandler(result);
                        });
                } else {
                	$scope.forms.recipeForm.submitted = true;
                	$scope.forms.crossessubmitted = true;
                	$scope.forms.relatedsubmitted = true;
                }
            };

            contentService.getCategoriesTree({ Type: 1 }, $scope.refreshTracker)//recipe categories
                .success(function (result) {
                    if (result.Success) {
                    	$scope.rootCategory = result.Data;
	                    contentService.getRecipeSettings($scope.refreshTracker)
		                    .success(function(result) {
		                    	if (result.Success) {
				                    $scope.recipeDefaults = result.Data;
				                    refreshMasters();
			                    } else {
				                    errorHandler(result);
			                    }
		                    }).
		                    error(function(result) {
			                    errorHandler(result);
		                    });
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function (result) {
                    errorHandler(result);
                });
        };

        function refreshRecipe()
        {
            contentService.getRecipe($scope.id, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.recipe = result.Data;
                        if ($scope.recipe.Url)
                        {
                            $scope.previewUrl = $scope.baseUrl.format($scope.recipe.Url);
                        };
                        if (!$scope.recipe.MasterContentItemId)
                        {
                            $scope.recipe.MasterContentItemId = $scope.MasterContentItemId;
                        };
                        setSelected($scope.rootCategory, $scope.recipe.CategoryIds);
                        addProductsListWatchers();
                        refreshHistory();
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

        function refreshMasters()
        {
            contentService.getMasterContentItems({ Type: 2 })//recipe
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.masters = result.Data;
                        var hasDefailt = false;
                        $.each($scope.masters, function (index, master) {
                            if (master.IsDefault || $scope.masters.length == 1) {
                                hasDefailt = true;
                                $scope.MasterContentItemId = master.Id;
                            };
                        });
                        if (!hasDefailt) {
                            $scope.MasterContentItemId = $scope.masters[0].Id;
                        }
                        $scope.MastersLoaded = true;
                        refreshRecipe();
                    } else
                    {
                        errorHandler(result);
                    }
                })
                .error(function (result)
                {
                    errorHandler(result);
                });
        };

        $scope.updateCategoriesCollapsed = function (expand)
        {
            if (expand)
            {
                $scope.$broadcast('angular-ui-tree:expand-all');
            }
            else
            {
                $scope.$broadcast('angular-ui-tree:collapse-all');
            }
            $scope.categoriesExpanded = expand;
        };

        function setSelected(category, ids) {
            category.IsSelected = false;
            $.each(ids, function (index, id) {
                if (category.Id == id) {
                    category.IsSelected = true;
                }
            });
            $.each(category.SubItems, function (index, value) {
                setSelected(value, ids)
            });
        }

        function getSelected(category, ids) {
            if (category.IsSelected) {
                ids.push(category.Id);
            }
            $.each(category.SubItems, function (index, value) {
                getSelected(value, ids)
            });
        }

        $scope.goToMaster = function (id) {
            $state.go('index.oneCol.masterDetail', { id: id });
        };

        function notifyAboutAddBlockIds(name) {
            var blockIds = [];
            var list = $scope.recipe[name];
            $.each(list, function (index, item) {
                blockIds.push(item.IdProduct);
            });
            var data = {};
            data.name = name;
            data.blockIds = blockIds;
            $scope.$broadcast('productsSearch#in#setBlockIds', data);
        };

        $scope.$on('productsSearch#out#addItems', function (event, args) {
            var list = $scope.recipe[args.name];
            if (list) {
                if (args.items) {
                    var newSelectedProducts = [];
                    $.each(args.items, function (index, item) {
                        var add = true;
                        $.each(list, function (index, selectedProduct) {
                            if (item.Id == selectedProduct.IdProduct) {
                                add = false;
                                return;
                            }
                        });
                        if (add) {
                            var newSelectedProduct = {};
                            newSelectedProduct.IdProduct = item.ProductId;
                            newSelectedProduct.ShortProductInfo = {};
                            newSelectedProduct.ShortProductInfo.ProductName = item.Name;
                            newSelectedProducts.push(newSelectedProduct);
                        }
                    });
                    $.each(newSelectedProducts, function (index, newSelectedProduct) {
                        list.push(newSelectedProduct);
                    });
                }
            }
        });

        function addProductsListWatchers() {
            $scope.$watchCollection('recipe.RecipesToProducts', function () {
                notifyAboutAddBlockIds('RecipesToProducts');
            });
            notifyAboutAddBlockIds('RecipesToProducts');
        };

        function activateTab(formName) {
        	$.each($scope.tabs, function (index, item) {
		        var temp = "";
        		if (formName.indexOf('recipeForm') == 0) {
        			temp = 'details';
        		}
        		if (formName.indexOf('CrossSellRecipes') == 0) {
        			temp = 'crossSellRelatedRecipes';
        		}
        		if (formName.indexOf('RelatedRecipes') == 0) {
        			temp = 'crossSellRelatedRecipes';
        		}
        		if (item.formName == temp)
        		{
        		    $scope.options.activeTabIndex = item.index;
        			return false;
        		}
        	});
        };

        $scope.deleteRecipesToProducts = function (index) {
            $scope.recipe.RecipesToProducts.splice(index, 1);
        };

        $scope.setCustom = function (item) {
        	item.InUse = true;
        };

        $scope.removeCustom = function (item) {
        	item.InUse = false;
        };

        function updateCrossRelated(array) {
        	$.each(array, function (index, item) {
		        if (!item.InUse) {
			        item.Image = null;
			        item.Url = null;
			        item.Title = null;
			        if (item.Subtitle !== undefined) {
				        item.Subtitle = null;
			        }
		        }
        	});
        };

       $scope.toggleOpen = function (item, event) {
        	item.IsOpen = !item.IsOpen;
        };

        initialize();
    }]);