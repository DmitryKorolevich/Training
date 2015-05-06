'use strict';

angular.module('app.core.utils.appBootstrap', [])
	.service('appBootstrap', ['infrastructureService', '$rootScope', 'toaster', 'authenticationService', '$location', 'ngProgress', function (infrastructureService, $rootScope, toaster, authenticationService, $location, ngProgress) {
			function getReferenceItem(lookup, key) {
				return $.grep(lookup, function(elem) {
					return elem.Key === key;
				})[0];
			};

			function getValidationMessage(key, field) {
			    var messageFormat = getReferenceItem($rootScope.ReferenceData.Labels, key).Text;
                var message='';
                if(field)
                {
                    var item = getReferenceItem($rootScope.ReferenceData.Labels, Array.isArray(field) ? field[0] : field);
                    if (item) {
	                    if (Array.isArray(field)) {
							field[0] = item.Text;
	                    } else {
							field = item.Text;
	                    }
                    }
	                message = messageFormat.format(field);
                }
                else
                {
                    message=messageFormat;
                }

				return message;
			};

			function validatePermission(permission) {
				if (!$rootScope.authenticated || !$rootScope.currentUser) {
					return false;
				}

				return $rootScope.currentUser.IsSuperAdmin || $rootScope.currentUser.Permissions.indexOf(permission) > -1;
			};

			function validatePermissionMenuItem(menuItem) {
			    if (!$rootScope.authenticated || !$rootScope.currentUser) {
			        return false;
			    }

			    if ($rootScope.currentUser.IsSuperAdmin)
			        return true;
			    
                var result=false;
                if(menuItem && menuItem.subMenu)
                {
                    $.each(menuItem.subMenu, function (index, subMenuItem) {
                        if($rootScope.currentUser.Permissions.indexOf(subMenuItem.access) > -1)
                        {
                            result=true;
                            return false;
                        }
                    });
                }

                return result;
			};

			function logout() {
				authenticationService.logout().success(function() {
					$rootScope.authenticated = false;
					$rootScope.currentUser = {};

					$rootScope.$state.go("index.oneCol.login");
				}).error(function() {
					//handle error
				});
			}

			function unauthorizedArea(path) {
			    if (!path && path != "") {
			        path = $location.path();
			    }

			    return path.indexOf("/authentication/activate") > -1 || path.indexOf("/authentication/login") > -1;
			};

			function initialize() {
			    bindRootScope();

			    $rootScope.appStarted = false;
				$rootScope.ReferenceData = {};

				infrastructureService.getReferenceData().success(function(res) {
					if (res.Success) {
						$rootScope.ReferenceData = res.Data;

						if (!$rootScope.unauthorizedArea()) {
							authenticationService.getCurrenUser()
								.success(function(res) {
									if (res.Success && res.Data) {
										$rootScope.authenticated = true;
										$rootScope.currentUser = res.Data;
									} else {
										$rootScope.authenticated = false;

										$rootScope.$state.go("index.oneCol.login");
									}
									$rootScope.appStarted = true;
								}).error(function() {
									toaster.pop('error', "Error!", "Can't get current user info");
								});

						} else {
							$rootScope.authenticated = false;
							$rootScope.appStarted = true;
						}

					} else {
						toaster.pop('error', 'Error!', "Unable to initialize app properly");
					}
				}).error(function(res) {
					toaster.pop('error', "Error!", "Server error occured");
				});

				$rootScope.getReferenceItem = getReferenceItem;
				$rootScope.getValidationMessage = getValidationMessage;
				$rootScope.logout = logout;
				$rootScope.validatePermission = validatePermission;
				$rootScope.unauthorizedArea = unauthorizedArea;
				$rootScope.validatePermissionMenuItem = validatePermissionMenuItem;
			}

			function bindRootScope() {
			    $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
			        if ($rootScope.appStarted && !$rootScope.unauthorizedArea($rootScope.$state.href(toState).slice(1)) && !$rootScope.authenticated) {
			            toaster.pop('warning', "Caution!", "Please log in before access this area.");

			            event.preventDefault();
			        }
			        else {
			            ngProgress.start();
			        }
			    });
			    $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState) {
			        ngProgress.complete();
			        $rootScope.$state.previous = fromState;
			    });
			    $rootScope.$on('$stateChangeError', function () {
			        ngProgress.complete();
			    });
			    $rootScope.$on('$stateNotFound', function () {
			        ngProgress.complete();
			    });
			}

			return {
				initialize : initialize
			}
		}
	]);