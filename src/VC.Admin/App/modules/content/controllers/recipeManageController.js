'use strict';

angular.module('app.modules.content.controllers.recipeManageController', [])
.controller('recipeManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, contentService, toaster, confirmUtil, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.id = result.Data.Id;
                $scope.recipe.Id = result.Data.Id;
                $scope.recipe.MasterContentItemId = result.Data.MasterContentItemId;
                $scope.previewUrl = $scope.baseUrl.format($scope.recipe.Url);
            } else {
                var messages = "";
                if (result.Messages) {
                	$scope.forms.recipeForm.submitted = true;
                	$scope.forms.crossessubmitted = true;
                	$scope.forms.relatedsubmitted = true;
                	$scope.forms.videossubmitted = true;

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

            $scope.baseUrl = $rootScope.ReferenceData.PublicHost + 'recipe/{0}?preview=true';
            $scope.previewUrl = null;

            $scope.detailsTab = {
            	active: true,
				formName: 'details'
            };
            $scope.crossSellProductsAndVideosTab = {
            	active: false,
            	formName: 'crossSellRelatedRecipes',
            };
            var tabs = [];
            tabs.push($scope.detailsTab);
            tabs.push($scope.crossSellProductsAndVideosTab);
            $scope.tabs = tabs;

            $scope.forms = {};
            $scope.youTubeBaseUrl = 'https://www.youtube.com/watch?v={0}'
            $scope.basePreviewUrl = $rootScope.ReferenceData.PublicHost + 'product/{0}?preview=true';
            $scope.baseUrl = $rootScope.ReferenceData.PublicHost.substring(0, $rootScope.ReferenceData.PublicHost.length - 1) + '{0}';
            $scope.previewUrl = null;

            var clearServerValidation = function () {
            	$.each($scope.forms, function (index, form) {
            		if (form && !(typeof form === 'boolean')) {
            			if (index == "RelatedRecipes" || index == "CrossSellRecipes" || index == "Videos") {
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
	            		if (!form.$valid && index != 'submitted' && index != 'crossessubmitted' && index != 'videossubmitted' && index != 'relatedsubmitted') {
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
                    updateVideos();

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
                	$scope.forms.videossubmitted = true;
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

				                    contentService.getRecipe($scope.id, $scope.refreshTracker)
					                    .success(function(result) {
						                    if (result.Success) {
							                    $scope.recipe = result.Data;
							                    if ($scope.recipe.Url) {
								                    $scope.previewUrl = $scope.baseUrl.format($scope.recipe.Url);
							                    };
							                    setSelected($scope.rootCategory, $scope.recipe.CategoryIds);
							                    addProductsListWatchers();
							                    initCrossRelated($scope.recipe.RelatedRecipes);
							                    initCrossRelated($scope.recipe.CrossSellRecipes);
							                    initVideos();
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

        var getCategoriesTreeViewScope = function () {
            return angular.element($('.categories .ya-treeview').get(0)).scope();
        };

        $scope.updateCategoriesCollapsed = function (expand) {
            var scope = getCategoriesTreeViewScope();
            if (expand) {
                scope.expandAll();
            }
            else {
                scope.collapseAll();
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
        		if (formName.indexOf('recipeForm') == 0) {
        			formName = 'details';
        		}
        		if (formName.indexOf('CrossSellRecipes') == 0) {
        			formName = 'crossSellRelatedRecipes';
        		}
        		if (formName.indexOf('RelatedRecipes') == 0) {
        			formName = 'crossSellRelatedRecipes';
        		}
        		if (formName.indexOf('CrossRelatedMiscellaneous') == 0) {
        			formName = 'crossSellProductsAndVideos';
        		}
        		if (item.formName == formName) {
        			item.active = true;
        			return false;
        		}
        	});
        };

        $scope.deleteRecipesToProducts = function (index) {
            $scope.recipe.RecipesToProducts.splice(index, 1);
        };

        $scope.setCustomVideo = function (item) {
        	item.VideoUse = true;
        	item.ImageUse = true;
        	item.TextUse = true;
        };

        $scope.removeCustomVideo = function (item) {
        	item.VideoUse = false;
        	item.ImageUse = false;
        	item.TextUse = false;
        };

        $scope.setCustomCross = function (item) {
	        $scope.setCustomRelated(item);
        	item.SubtitleUse = true;
        };

        $scope.setCustomRelated = function (item) {
        	item.ImageUse = true;
        	item.UrlUse = true;
        	item.TitleUse = true;
        };

        $scope.removeCustomRelated = function (item) {
        	item.ImageUse = false;
        	item.UrlUse = false;
        	item.TitleUse = false;
        };

        $scope.removeCustomCross = function (item) {
        	$scope.removeCustomRelated(item);
        	item.SubtitleUse = false;
        };

        function initCrossRelated (array) {
        	$.each(array, function (index, item) {
        		if (item.Image) {
        			item.ImageUse = true;
        		}
        		if (item.Url) {
        			item.UrlUse = true;
        		}
        		if (item.Title) {
        			item.TitleUse = true;
        		}
        		if (item.Subtitle) {
        			item.SubtitleUse = true;
        		}
        	});
        };

        function updateCrossRelated(array) {
        	$.each(array, function (index, item) {
        		if (!item.ImageUse) {
        			item.Image = null;
        		}
        		if (!item.UrlUse) {
        			item.Url = null;
        		}
        		if (!item.TitleUse) {
        			item.Title = null;
        		}
        		if (item.Subtitle !== undefined && !item.SubtitleUse) {
        			item.Subtitle = null;
        		}
        	});
        };

        function initVideos() {
        	$.each($scope.recipe.Videos, function (index, item) {
        		if (item.Video) {
        			item.VideoUse = true;
        		}
        		if (item.Image) {
        			item.ImageUse = true;
        		}
        		if (item.Text) {
        			item.TextUse = true;
        		}
        	});
        };

        function updateVideos() {
        	$.each($scope.recipe.Videos, function (index, item) {
        		if (!item.VideoUse) {
        			item.Video = null;
        		}
        		if (!item.ImageUse) {
        			item.Image = null;
        		}
        		if (!item.TextUse) {
        			item.Text = null;
        		}
        	});
        };

        $scope.toggleOpen = function (item, event) {
        	item.IsOpen = !item.IsOpen;
        };

        initialize();
    }]);