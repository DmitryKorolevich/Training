'use strict';

angular.module('app.core.utils.appBootstrap', [])
    .service('appBootstrap',
    [
        'infrastructureService', '$rootScope', '$anchorScroll', 'toaster', 'authenticationService',
        'cacheService', 'settingService', '$location',
        'ngProgress', 'webStorageUtil',
        'confirmUtil', function (infrastructureService,
            $rootScope,
            $anchorScroll,
            toaster,
            authenticationService,
            cacheService,
            settingService,
            $location,
            ngProgress,
            webStorageUtil,
            confirmUtil)
        {
            function scrollTo(id)
            {
                $location.hash(id);
                $anchorScroll();
                $location.hash('');
            };

            function getReferenceItem(lookup, key)
            {
                if (lookup)
                {
                    return $.grep(lookup,
                        function (elem)
                        {
                            return elem.Key === key;
                        })[0];
                }
                return null;
            };

            function stopPropagation($event)
            {
                $event.preventDefault();
                $event.stopPropagation();
            };

            function getValidationMessage(key, field, arg)
            {
                var item = getReferenceItem($rootScope.ReferenceData.Labels, key);
                var messageFormat;
                if (item)
                {
                    messageFormat = item.Text;
                } else
                {
                    messageFormat = key;
                }
                var message = '';
                if (field)
                {
                    var item = getReferenceItem($rootScope.ReferenceData.Labels,
                        Array.isArray(field) ? field[0] : field);
                    if (item)
                    {
                        if (Array.isArray(field))
                        {
                            field[0] = item.Text;
                        } else
                        {
                            field = item.Text;
                        }
                    }
                    message = messageFormat.format(field, arg);
                } else
                {
                    message = messageFormat;
                }

                return message;
            };

            function validatePermission(permission)
            {
                if (!$rootScope.authenticated || !$rootScope.currentUser)
                {
                    return false;
                }
                
                return $rootScope.currentUser.IsSuperAdmin ||
                    isAnyPermission(permission);
            };

            function validatePermissionMenuItem(menuItem)
            {
                if (!$rootScope.authenticated || !$rootScope.currentUser)
                {
                    return false;
                }

                if ($rootScope.currentUser.IsSuperAdmin)
                    return true;

                var result = false;
                if (menuItem && menuItem.subMenu)
                {
                    $.each(menuItem.subMenu,
                        function (index, subMenuItem)
                        {
                            if (subMenuItem.access == null ||
                                isAnyPermission(subMenuItem.access))
                            {
                                result = true;
                                return false;
                            }
                        });
                }

                return result;
            };

            function isAnyPermission(permissions)
            {
                if (Array.isArray(permissions))
                {
                    var toReturn = false;
                    $.each(permissions, function (index, permission)
                    {
                        if ($rootScope.currentUser.Permissions.indexOf(permission) > -1)
                        {
                            toReturn = true;
                            return false;
                        }
                    });

                    return toReturn;
                }
                else
                {
                    return $rootScope.currentUser.Permissions.indexOf(permissions) > -1;
                }
            };

            function logout()
            {
                authenticationService.logout()
                    .success(function ()
                    {
                        $rootScope.authenticated = false;
                        $rootScope.currentUser = {};

                        $rootScope.$state.go("index.oneCol.login");
                    })
                    .error(function ()
                    {
                        //handle error
                    });
            }

            function unauthorizedArea(path)
            {
                if (!path && path != "")
                {
                    path = $location.path();
                }

                return path.indexOf("/authentication/activate") > -1 ||
                    path.indexOf("/authentication/login") > -1 ||
                    path.indexOf("/authentication/passwordreset") > -1;
            };

            function cacheStatus()
            {
                if ($rootScope.authenticated)
                {
                    cacheService.getCacheStatus()
                        .success(function (cacheResult)
                        {
                            if (cacheResult.Success && cacheResult.Data)
                            {
                                $rootScope.cacheState = cacheResult.Data;
                            }

                            setTimeout(cacheStatus, 5000);
                        })
                        .error(function ()
                        {
                            $rootScope.cacheState = [{ "Error": 0 }];
                            setTimeout(cacheStatus, 60000);
                        });
                }
                else
                {
                    setTimeout(cacheStatus, 2000);
                }
            }

            function checkAreas()
            {
                if ($rootScope.authenticated)
                {
                    settingService.getContentAreas()
                        .success(function (result)
                        {
                            if (result.Success && result.Data)
                            {
                                $.each(result.Data, function (index, item)
                                {
                                    $('.area[data-area-name="' + item.Name + '"]').html(item.Template);
                                });
                            }

                            setTimeout(checkAreas, 60000);
                        })
                        .error(function ()
                        {
                            setTimeout(checkAreas, 60000);
                        });
                }
                else
                {
                    setTimeout(checkAreas, 2000);
                }
            }

            function initialize()
            {
                bindRootScope();

                $rootScope.appStarted = false;
                $rootScope.Math = window.Math;
                $rootScope.ReferenceData = {};

                cacheStatus();
                checkAreas();

                infrastructureService.getReferenceData()
                    .success(function (res)
                    {
                        if (res.Success)
                        {
                            $rootScope.ReferenceData = res.Data;

                            if (!$rootScope.unauthorizedArea())
                            {
                                authenticationService.getCurrenUser()
                                    .success(function (res)
                                    {
                                        if (res.Success && res.Data)
                                        {
                                            $rootScope.authenticated = true;
                                            $rootScope.currentUser = res.Data;
                                        } else
                                        {
                                            $rootScope.authenticated = false;

                                            $rootScope.$state.go("index.oneCol.login");
                                        }
                                        $rootScope.appStarted = true;
                                    })
                                    .error(function ()
                                    {
                                        toaster.pop('error', "Error!", "Can't get current user info");
                                    });

                            } else
                            {
                                $rootScope.authenticated = false;
                                $rootScope.appStarted = true;
                            }

                        } else
                        {
                            toaster.pop('error', 'Error!', "Unable to initialize app properly");
                        }
                    })
                    .error(function (res)
                    {
                        toaster.pop('error', "Error!", "Server error occured");
                    });

                $rootScope.scrollTo = scrollTo;
                $rootScope.getReferenceItem = getReferenceItem;
                $rootScope.getValidationMessage = getValidationMessage;
                $rootScope.logout = logout;
                $rootScope.validatePermission = validatePermission;
                $rootScope.unauthorizedArea = unauthorizedArea;
                $rootScope.validatePermissionMenuItem = validatePermissionMenuItem;
                $rootScope.stopPropagation = stopPropagation;
                $rootScope.UIOptions = {};
                $rootScope.UIOptions.DatepickerFormat = 'MM/dd/yyyy';
            }

            function bindRootScope()
            {
                $rootScope.$on('$stateChangeStart',
                    function (event, toState, toParams, fromState, fromParams)
                    {
                        if ($rootScope.appStarted &&
                            $rootScope.$state.href(toState) != null &&
                            !$rootScope.unauthorizedArea($rootScope.$state.href(toState)) &&
                            !$rootScope.authenticated)
                        {
                            toaster.pop('warning', "Caution!", "You must login before accessing this area.");

                            event.preventDefault();
                        } else
                        {
                            ngProgress.start();

                            if ($rootScope.lastRemediationKey)
                            {
                                webStorageUtil.removeSession($rootScope.lastRemediationKey);
                                $rootScope.lastRemediationKey = null;
                            }
                        }
                    });
                $rootScope.$on('$stateChangeSuccess',
                    function (event, toState, toParams, fromState, fromParams)
                    {
                        ngProgress.complete();
                        $rootScope.$state.previous = angular.copy(fromState);
                        $rootScope.$state.previous.params = fromParams;
                    });
                $rootScope.$on('$stateChangeError',
                    function ()
                    {
                        ngProgress.complete();
                    });
                $rootScope.$on('$stateNotFound',
                    function ()
                    {
                        ngProgress.complete();
                    });
            }

            var data = [];

            var setData = function (name, value)
            {
                data[name] = value;
            };

            var getData = function (name)
            {
                return data[name];
            };

            return {
                initialize: initialize,
                setData: setData,
                getData: getData,
            }
        }
    ]);