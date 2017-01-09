'use strict';

angular.module('app.core.utils.appBootstrap', [])
    .service('appBootstrap',
    [
        'infrastructureService', '$rootScope', '$state', '$anchorScroll', '$location',
        'toaster', 'authenticationService',
        'cacheService', 'settingService', 'monitorService', 'modalUtil', 
        'ngProgress', 'webStorageUtil',
        'confirmUtil', function (infrastructureService,
            $rootScope,
            $state,
            $anchorScroll,
            $location,
            toaster,
            authenticationService,
            cacheService,
            settingService,
            monitorService,
            modalUtil,
            ngProgress,
            webStorageUtil,
            confirmUtil)
        {
            function downloadFileIframe(url)
            {
                $("<iframe/>").attr('src', url).css('visibility', 'hidden').css('display', 'none').appendTo($('body'));
            };

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
                    path.indexOf("/authentication/passwordreset") > -1 ||
                    path.indexOf("/public/") > -1;
            };

            function cacheStatus()
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
            };

            function getTinymceOptions()
            {
                var toReturn= {
                    min_height: 300,
                    skin: "lightgray",
                    theme: 'modern',
                    content_css: [
                        'https://{0}/styles.min.css?v={1}'.format($rootScope.ReferenceData.PublicHost, $rootScope.buildNumber),
                        'https://{0}/custom-styles.css?v={1}'.format($rootScope.ReferenceData.PublicHost, Math.floor((Math.random() * 1000000) + 1)),
                        '/assets/styles/html-editor-defaults.css'
                    ],
                    menubar: 'edit insert view format table tools',
                    plugins: [
                        'advlist autolink lists link image charmap anchor',
                        'searchreplace visualblocks code',
                        'insertdatetime media table contextmenu paste code',
                        'textcolor colorpicker textpattern autoresize'
                    ],
                    toolbar: 'insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image | forecolor backcolor',
                    image_advtab: true,
                    document_base_url: '/',
                    convert_urls: false,
                    body_class: 'working-area-holder',
                    file_browser_callback: function (field_name, url, type, win)
                    {
                        if (!url)
                        {
                            url = null;
                        }
                        var data = {
                            fileUrl: url,
                            thenCallback: function (data)
                            {
                                win.document.getElementById(field_name).value = data != null ? data.FullRelativeName : '';

                                if (self.fileManagementPopup)
                                {
                                    self.fileManagementPopup.close();
                                }
                            }
                        };
                        setData('FILES_POPUP_DATA', data);
                        self.fileManagementPopup = modalUtil.open('app/modules/file/partials/selectFile.html', 'filesController', data, { size: 'lg' });
                    },
                    file_browser_callback_types: 'image'
                };

                return toReturn;
            };

            function initEditLock() {
                if($rootScope.ReferenceData && $rootScope.ReferenceData.EditLockAreas){
                    $rootScope.editLockState.Areas={};
                    $.each($rootScope.ReferenceData.EditLockAreas, function(index, item){
                        $rootScope.editLockState.Areas[item] = {};
                        $rootScope.editLockState.Areas[item].Items = [];
                    });

                    editLockRequest();
                }
            };

            function editLockPing() {
                if ($rootScope.authenticated && $rootScope.currentUser && $rootScope.editLockState.Areas) {
                    var id = null;
                    var areaName = null;
                    $.each($rootScope.ReferenceData.EditLockAreas, function (index, item) {
                        if ($state.is(item) && Number.isInteger($state.params.id)) {
                            id = parseInt($state.params.id);
                            areaName = item;
                            return false;
                        }
                    });

                    if (id && areaName) {
                        var model = {
                            AreaName: areaName,
                            Id: id,
                            IdAgent: $rootScope.currentUser.Id,
                            Agent: $rootScope.currentUser.Agent,
                            AgentFirstName: $rootScope.currentUser.FirstName,
                            AgentLastName: $rootScope.currentUser.LastName,
                        };

                        monitorService.editLockPing(model)
                            .success(function (result) {
                                setTimeout(editLockPing, 20000);
                            })
                            .error(function () {
                                setTimeout(editLockPing, 20000);
                            });
                        return;
                    }
                }
                setTimeout(editLockPing, 20000);
            };

            
            function editLockRequest() {
                if ($rootScope.authenticated && $rootScope.currentUser && $rootScope.editLockState.Areas) {
                    var id = null;
                    var areaName = null;
                    $.each($rootScope.ReferenceData.EditLockAreas, function (index, item) {
                        if ($state.is(item) && Number.isInteger($state.params.id)) {
                            id = parseInt($state.params.id);
                            areaName = item;
                            return false;
                        }
                    });

                    if (id && areaName) {
                        var model = {
                            AreaName: areaName,
                            Id: id,
                            IdAgent: $rootScope.currentUser.Id,
                            Agent: $rootScope.currentUser.Agent,
                            AgentFirstName: $rootScope.currentUser.FirstName,
                            AgentLastName: $rootScope.currentUser.LastName,
                        };

                        $rootScope.editLockState.Areas[areaName].Items[id] = null;

                        monitorService.editLockRequest(model)
                            .success(function (result) {
                                if (result.Data) {
                                    if (result.Data.Avaliable) {
                                        $rootScope.editLockState.Areas[areaName].Items[id] = {};
                                    }
                                    else {
                                        modalUtil.open('app/modules/setting/partials/infoDetailsPopup.html', 'infoDetailsPopupController', {
                                            Header: result.Data.LockMessageTitle,
                                            Messages: [
                                                {
                                                    Message: result.Data.LockMessageBody
                                                }
                                            ],
                                            OkButton: {
                                                Label: 'OK'
                                            },
                                        },
                                        {
                                            backdrop: false,
                                        });
                                    }
                                }
                            })
                            .error(function () {
                            });

                        return;
                    }
                };
            };

            var baseValidationMessage = "Validation errors, please correct field values.";

            function fireServerValidation(result, uiScope, additionalFormNameHandler, additionalItemOpenHandler, customSetInitSubmitHanlder, customFieldFormatHandler)
            {
                if (result && !result.Success)
                {
                    var messages = "";
                    if (result.Messages)
                    {
                        if (customSetInitSubmitHanlder)
                        {
                            customSetInitSubmitHanlder();
                        }
                        else
                        {
                            if (uiScope.forms)
                            {
                                if (uiScope.forms.submitted)
                                {
                                    if (!(typeof uiScope.forms.submitted === 'boolean'))
                                    {
                                        $.each(uiScope.forms.submitted, function (index, item)
                                        {
                                            uiScope.forms.submitted[index] = true;
                                        });
                                    }
                                    else
                                    {
                                        uiScope.forms.submitted = true;
                                    }
                                }
                                else
                                {
                                    uiScope.forms.submitted = true;
                                }

                                $.each(uiScope.forms, function (index, item)
                                {
                                    if (index && index.indexOf('submitted') > -1 && index != 'submitted')
                                    {
                                        uiScope.forms[index] = true;
                                    }
                                });
                            }
                        }

                        uiScope.serverMessages = new ServerMessages(result.Messages);
                        var formForShowing = null;
                        $.each(result.Messages, function (index, value)
                        {
                            if (value.Field)
                            {
                                var currentFormForShowing = null;
                                if (customFieldFormatHandler)
                                {
                                    var fieldResult = customFieldFormatHandler(value);
                                    if (fieldResult)
                                    {
                                        currentFormForShowing = fieldResult;
                                        formForShowing = currentFormForShowing;
                                    }
                                }
                                if (!currentFormForShowing)
                                {
                                    //with form name
                                    if (value.Field.indexOf("::") > -1)
                                    {
                                        var arr = value.Field.split("::");
                                        var formName = arr[0];
                                        var fieldName = arr[1];
                                        //only for single forms
                                        if (fieldName.indexOf(".") == -1)
                                        {
                                            var form = uiScope.forms[formName];
                                            if (form[fieldName] != undefined)
                                            {
                                                form[fieldName].$setValidity("server", false);
                                                if (formForShowing == null)
                                                {
                                                    formForShowing = formName;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //without form name for single forms
                                        if (value.Field.indexOf('.') > -1)
                                        {
                                            var items = value.Field.split(".");
                                            uiScope.forms[items[0]][items[1]][items[2]].$setValidity("server", false);
                                            formForShowing = items[0];
                                            if (additionalItemOpenHandler)
                                            {
                                                additionalItemOpenHandler();
                                            }
                                        }
                                        else
                                        {
                                            //without form name for lists
                                            $.each(uiScope.forms, function (index, form)
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
                                }
                            }
                            else
                            {
                                messages += value.Message + "<br />";
                            }
                        });

                        if (formForShowing)
                        {
                            activateTab(uiScope, formForShowing, additionalFormNameHandler);
                        }
                    }

                    if (messages == "" && result.Messages && result.Messages.length > 0)
                    {
                        messages = baseValidationMessage;
                    }
                    toaster.pop('error', "Error!", messages, null, 'trustedHtml');
                }
            };

            function activateTab(uiScope, formName, additionalFormNameHandler)
            {
                if (uiScope.tabs)
                {
                    if (additionalFormNameHandler)
                    {
                        formName = additionalFormNameHandler(formName);
                    }
                    $.each(uiScope.tabs, function (index, item)
                    {
                        var itemForActive = null;
                        if (item.formName == formName)
                        {
                            itemForActive = item;
                        }
                        if (item.formNames)
                        {
                            $.each(item.formNames, function (index, form)
                            {
                                if (form == formName)
                                {
                                    itemForActive = item;
                                    return false;
                                }
                            });
                        }
                        if (itemForActive)
                        {
                            if (!uiScope.options)
                            {
                                uiScope.options = {};
                            }
                            uiScope.options.activeTabIndex = itemForActive.index;
                            return false;
                        }
                    });
                }
            };

            function initialize()
            {
                bindRootScope();

                $rootScope.appStarted = false;
                $rootScope.Math = window.Math;
                $rootScope.ReferenceData = {};

                cacheStatus();
                checkAreas();
                editLockPing();

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
                                            initEditLock();
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

                $rootScope.downloadFileIframe = downloadFileIframe;
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
                $rootScope.editLockState={};
                $rootScope.initEditLock = initEditLock;
                $rootScope.getTinymceOptions = getTinymceOptions;
                $rootScope.fireServerValidation = fireServerValidation;
                $rootScope.activateTab = activateTab;
                $rootScope.baseValidationMessage = baseValidationMessage;
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

                        editLockRequest();
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