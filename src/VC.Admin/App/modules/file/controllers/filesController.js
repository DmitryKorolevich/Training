'use strict';

angular.module('app.modules.file.controllers.filesController', [])
.constant('filesConfig', {
    urlPrefix: 'files',
})
.controller('filesController', ['$scope', '$rootScope', '$state', '$stateParams', '$modalStack', '$modal', 'appBootstrap', 'Upload', 'modalUtil', 'fileService', 'toaster', 'confirmUtil', 'promiseTracker', 'filesConfig',
    function ($scope, $rootScope, $state, $stateParams, $modalStack, $modal, appBootstrap, Upload, modalUtil, fileService, toaster, confirmUtil, promiseTracker, filesConfig) {
        var INVALID_FILE_FORMAT_MESSAGE = "The uploaded file must be .jpg, .gif, .png or .pdf.";
        var INVALID_FILE_SIZE_MESSAGE = "The uploaded file must be less than 10 mb.";
        var MAX_FILE_SIZE = 10485760;
        var FILE_TYPES = 'image/jpeg,image/png,image/gif,application/pdf';
        var PDF_FILE_EXT = '.pdf';
        var FILES_PAGE_PRERENDERCOUNT = 5;

        var fileUploadRequestId = 0;
        var data = null;

        $scope.refreshDirectoriesTracker = promiseTracker("refreshDirectories");
        $scope.refreshFilesTracker = promiseTracker("refreshFiles");
        $scope.addFileTracker = promiseTracker("addFile");
        $scope.deleteFileTracker = promiseTracker("deleteFile");
        $scope.addFolderTracker = promiseTracker("addDirectory");
        $scope.deleteFolderTracker = promiseTracker("deleteDirectory");

        function BreadCrumbDirectory(name, fullRelativeName) {
            var self = this;

            self.Name = name;
            self.FullRelativeName = fullRelativeName;
        }

        function errorHandler(result) {
            var messages = "";
            if (result.Messages) {
                $.each(result.Messages, function (index, value) {
                    if (value.Field) {
                        messages += "{0} - {1}<br/>".format(value.Field, value.Message);
                    }
                    else {
                        messages += "{0}<br/>".format(value.Message);
                    }
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        };

        function loadDirectories() {
            fileService.getDirectories($scope.refreshDirectoriesTracker)
                .success(function (result) {
                    if (result.Success) {
                        var directories = [];
                        directories.push(result.Data);
                        $scope.directories = directories;
                        $scope.selectDirectory(result.Data.FullRelativeName);
                    } else {
                        errorHandler(result);
                    }
                })
                .error(function (result) {
                    errorHandler(result);
                });
        };

        function initialize() {
            data = appBootstrap.getData('FILES_POPUP_DATA');

            $scope.forms = {};

            $scope.selectedDir =
            {
                FullRelativeName: '/',
                Directories: []
            };
            $scope.files = [];
            $scope.filterdFiles = [];
            $scope.logFiles = [];

            $scope.breadCrumbMaxLevels = 5;
            $scope.baseUrl = $rootScope.ReferenceData.PublicHost + filesConfig.urlPrefix + '{0}';

            $scope.selectedFile = null;

            $scope.filter = {
                Name: '',
                FilteredName: '',
                Paging: { PageIndex: 1, PageItemCount: 500 }
            };

            $scope.logRequests = [];

            loadDirectories();
        }

        $scope.selectDirectory = function (fullRelativeName) {
            if ($scope.selectedDir.FullRelativeName != fullRelativeName) {
                $scope.selectedFile = {};
                $scope.filter.FilteredName = '';
                $scope.filter.Name = '';
            }

            $scope.selectedDir.FullRelativeName = fullRelativeName;
            $scope.selectedDir.ShowSpace = false;
            var url = fullRelativeName;
            var breadCrumbDirectories = [];
            while (url.length > 1) {
                if (breadCrumbDirectories.length >= $scope.breadCrumbMaxLevels) {
                    $scope.selectedDir.ShowSpace = true;
                    break;
                }
                if (url.length - 1 > 0) {
                    var name = url.substring(url.lastIndexOf('/') + 1, url.length);
                    var fullName = url;
                    breadCrumbDirectories.splice(0, 0, new BreadCrumbDirectory(name, fullName))
                }
                url = url.substring(0, url.lastIndexOf('/'));
            }
            $scope.selectedDir.Directories = breadCrumbDirectories;
            selectTreeDirectory($scope.directories, fullRelativeName);
        };

        function selectTreeDirectory(directories, fullRelativeName) {
            $.each(directories, function (index, directory) {
                if (directory.FullRelativeName == fullRelativeName) {
                    directory.selected = true;
                }
                else {
                    directory.selected = false;
                }
                if (directory.Directories && directory.Directories.length > 0) {
                    selectTreeDirectory(directory.Directories, fullRelativeName);
                }
            });
        };

        function removeDirectoryFromTree(url) {
            removeDirectory($scope.directories, url);
        };

        function removeDirectory(directories, url) {
            var indexForRemove = null;
            $.each(directories, function (index, directory) {
                if (directory.FullRelativeName == url) {
                    indexForRemove = index;
                    if ($scope.selectedDir.FullRelativeName == url) {
                        $scope.selectDirectory("/");
                    }
                    return false;
                }
            });

            if (indexForRemove != null) {
                directories.splice(indexForRemove, 1);
            }
            else {
                $.each(directories, function (index, directory) {
                    removeDirectory(directory.Directories, url);
                });
            }
        };

        $scope.deleteDirectory = function (directory) {
            confirmUtil.confirm(function () {
                fileService.deleteDirectory(directory, $scope.deleteFolderTracker)
                    .success(function (result) {
                        if (result.Success) {
                            removeDirectoryFromTree(directory.FullRelativeName);
                        } else {
                            errorHandler(result);
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to delete this directory?');
        };

        function addDirectoryToTree(rootUrl, newDirectory) {
            $.each($scope.directories, function (index, directory) {
                addDirectoryToDir(directory, rootUrl, newDirectory);
            });
        };

        function addDirectoryToDir(rootDir, rootUrl, newDirectory) {
            if (rootDir.FullRelativeName == rootUrl) {
                newDirectory.Directories = [];
                rootDir.Directories.push(newDirectory);
            };

            if (rootDir.Directories && rootDir.Directories.length > 0) {
                $.each(rootDir.Directories, function (index, directory) {
                    addDirectoryToDir(directory, rootUrl, newDirectory);
                });
            }
        }

        $scope.addDirectory = function (url) {
            modalUtil.open('app/modules/file/partials/addFolder.html', 'addFolderController', {
                fullRelativeName: url, thenCallback: function (data) {
                    addDirectoryToTree(url, data);
                }
            });
        };

        $scope.$watch('selectedDir.FullRelativeName', function () {
            loadFiles();
        });

        function loadFiles() {
            $scope.files = [];
            filterFiles();
            $scope.filter.Paging.PageIndex = 1;
            var table = $('.table .wrapper');
            table.empty();
            var url = $scope.selectedDir.FullRelativeName;
            fileService.getFiles({ FullRelativeName: url }, $scope.refreshFilesTracker)
                .success(function (result) {
                    tableHandlers();
                    if (result.Success) {
                        //Show only in the same folder
                        if (url == $scope.selectedDir.FullRelativeName) {
                            $.each(result.Data, function (index, file) {
                                prepareFile(file);
                            });
                            $scope.files = result.Data;
                            $scope.filter.Paging.PageIndex = 1;
                            filterFiles();
                            renderFiles();
                        }
                    } else {
                        errorHandler(result);
                    }
                })
                .error(function (result) {
                    errorHandler(result);
                });
        };

        function prepareFile(file) {
            file.Url = $scope.baseUrl.format(file.FullRelativeName);
        };

        $scope.deleteFile = function (deleteFile) {
            confirmUtil.confirm(function () {
                fileService.deleteFile(deleteFile, $scope.deleteFileTracker)
                    .success(function (result) {
                        if (result.Success) {
                            var logRequest = {};
                            fileUploadRequestId++;
                            logRequest.type = "delete";
                            logRequest.index = fileUploadRequestId;
                            logRequest.name = deleteFile.Name;
                            logRequest.progress = 100;
                            logRequest.state = "done";
                            $scope.logRequests.splice(0, 0, logRequest);

                            var indexForRemove;
                            $.each($scope.files, function (index, file) {
                                if (file.FullRelativeName == deleteFile.FullRelativeName) {
                                    indexForRemove = index;
                                    return false;
                                }
                            });
                            if (indexForRemove != null) {
                                $scope.files.splice(indexForRemove, 1);
                                filterFiles();
                                removeFileFormTable(deleteFile.FullRelativeName);
                            }
                        } else {
                            var messages = '';
                            if (result.Messages) {
                                $.each(result.Messages, function (index, value) {
                                    messages += "{0} ".format(value.Message);
                                });
                            }

                            var logRequest = {};
                            fileUploadRequestId++;
                            logRequest.type = "delete";
                            logRequest.index = fileUploadRequestId;
                            logRequest.name = deleteFile.Name;
                            logRequest.progress = 100;
                            logRequest.state = "error";
                            logRequest.messages = messages;
                            $scope.logRequests.splice(0, 0, logRequest);
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to delete this file?');
        };

        $scope.uploadFiles = function (uploadFiles) {
            if (uploadFiles && uploadFiles.length) {
                for (var i = 0; i < uploadFiles.length; i++) {
                    var file = uploadFiles[i];

                    var messages = "";
                    if (file.type && FILE_TYPES.indexOf(file.type) == -1) {
                        messages += "{0} ".format(INVALID_FILE_FORMAT_MESSAGE);
                    }
                    if (file.size && file.size >= MAX_FILE_SIZE) {
                        messages += "{0} ".format(INVALID_FILE_SIZE_MESSAGE);
                    }

                    var logRequest = {};
                    fileUploadRequestId++;
                    file.index = fileUploadRequestId;
                    logRequest.type = "upload";
                    logRequest.index = file.index;
                    logRequest.name = file.name;
                    logRequest.progress = 0;
                    logRequest.state = "progress";
                    if (messages) {
                        logRequest.state = "error";
                        logRequest.messages = messages;
                    }

                    $scope.logRequests.splice(0, 0, logRequest);

                    if (messages) {
                        continue;
                    }

                    var url = $scope.selectedDir.FullRelativeName;
                    var fields = {};
                    fields[url] = '';
                    Upload.upload({
                        url: '/api/file/AddFiles',
                        fields: fields,
                        file: file
                    }).progress(function (evt) {
                        var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                        var logRequest = getLogRequestItem(evt.config.file.index);
                        if (logRequest) {
                            logRequest.progress = progressPercentage;
                        }
                    }).success(function (result, status, headers, config) {
                        if (result.Success) {
                            var logRequest = getLogRequestItem(config.file.index);
                            if (logRequest) {
                                logRequest.progress = 100;
                                logRequest.state = 'done';
                            }

                            if (result.Data.DirectoryFullRelativeName == $scope.selectedDir.FullRelativeName) {
                                var newFile = result.Data;
                                prepareFile(newFile);

                                var indexForRemove;
                                $.each($scope.files, function (index, file) {
                                    if (file.FullRelativeName == newFile.FullRelativeName) {
                                        indexForRemove = index;
                                        return false;
                                    }
                                });
                                if (indexForRemove != null) {
                                    $scope.files.splice(indexForRemove, 1);
                                }

                                $scope.files.push(newFile);
                                filterFiles();
                                addFileToTable(newFile)
                            }
                        } else {
                            var messages = '';
                            if (result.Messages) {
                                $.each(result.Messages, function (index, value) {
                                    messages += "{0} ".format(value.Message);
                                });
                            }

                            var logRequest = getLogRequestItem(config.file.index);
                            if (logRequest) {
                                logRequest.progress = 100;
                                logRequest.messages = messages;
                                logRequest.state = 'error';
                            }
                        }
                    });
                }
            }
        };

        var getLogRequestItem = function (index) {
            var toReturn = null;
            $.each($scope.logRequests, function (i, logRequest) {
                if (logRequest.index == index) {
                    toReturn = logRequest;
                    return false;
                }
            });
            return toReturn;
        }

        $scope.selectFile = function (selectedFileName) {
            var resFile;
            $.each($scope.files, function (index, file) {
                file.selected = file.Name == selectedFileName;
                if (file.selected) {
                    resFile = file;
                }
            });

            $scope.selectedFile = Object.clone(resFile);
            if ($scope.selectedFile.FullRelativeName.indexOf(PDF_FILE_EXT) > -1) {
                $scope.selectedFile.PreviewUrl = "/assets/images/pdf.png";
                $scope.selectedFile.Dimensions = "";
            }
            else {
                $scope.selectedFile.PreviewUrl = $scope.baseUrl.format(resFile.FullRelativeName);
                $scope.selectedFile.Dimensions = "";
            }
            $scope.$apply();
        };

        //$scope.selectFile = function (selectedFile) {
        //    $.each($scope.files, function (index, file) {
        //        file.selected = file == selectedFile;
        //    });

        //    $scope.selectedFile = Object.clone(selectedFile);
        //    if ($scope.selectedFile.FullRelativeName.indexOf(PDF_FILE_EXT) > -1) {
        //        $scope.selectedFile.PreviewUrl = "/assets/images/pdf.png";
        //        $scope.selectedFile.Dimensions = "";
        //    }
        //    else {
        //        $scope.selectedFile.PreviewUrl = $scope.baseUrl.format(selectedFile.FullRelativeName);
        //        $scope.selectedFile.Dimensions = "";
        //    }
        //};

        $scope.filterFilesRequest = function () {
            $scope.filter.FilteredName = $scope.filter.Name;
            var table = $('.table .wrapper');
            table.empty();
            $scope.filter.Paging.PageIndex = 1;
            filterFiles();
            renderFiles();
        };

        var filterFiles = function () {
            var filterdFiles = [];
            if ($scope.filter.FilteredName) {
                $.each($scope.files, function (index, file) {
                    if (file.Name.indexOf($scope.filter.FilteredName) > -1) {
                        filterdFiles.push(file);
                    }
                });
            }
            else {
                filterdFiles = $scope.files;
            }
            $scope.filterdFiles = filterdFiles;
            $scope.totalItems = filterdFiles.length;
        };

        var renderFiles = function () {
            var table = $('.table .wrapper');
            table.empty();
            var data = '';
            var row;
            var from = ($scope.filter.Paging.PageIndex - 1) * $scope.filter.Paging.PageItemCount;
            var max = from + $scope.filter.Paging.PageItemCount > $scope.filterdFiles.length ? $scope.filterdFiles.length
                : from + $scope.filter.Paging.PageItemCount;
            for (var i = from; i < max; i++) {
                data += renderRow($scope.filterdFiles[i]);
            }
            table.append(data);
        };

        $scope.pageChanged = function () {
            renderFiles();
        };

        var renderRow = function (file) {
            return '<tr data-name="' + file.Name + '" data-full-url="' + file.FullRelativeName + '"><td>' + file.Name + '</td><td class="width-140px">' + Date.parseDateTime(file.Updated).format('{MM}/{DD}/{yy} {HH}:{MN} {AP}') + '</td><td class="width-80px">' + file.SizeMessage + '</td><td class="width-70px">' +
                '<div class="ya-treview-buttons"><a class="btn btn-success btn-xs" title="Download" target="_blank" href="' + file.Url + '"><i class="glyphicon glyphicon-download"></i></a><a class="btn btn-danger btn-xs" title="Delete"><i class="glyphicon glyphicon-remove"></i></a></div></td></tr>';
        };

        var addFileToTable = function (file) {
            renderFiles();
        };

        var removeFileFormTable = function (FullRelativeName) {
            if (FullRelativeName) {
                var tr = $('.table .wrapper tr[data-full-url="' + FullRelativeName + '"]');
                tr.remove();
            }
        };

        var tableHandlers = function () {
            $('.file-manager .work-area .center-pane .table tbody').off("click", selectTtHandler);
            $('.file-manager .work-area .center-pane .table tbody').on("click", "tr", selectTtHandler);
            $('.file-manager .work-area .center-pane .table tbody').off("click", deleteTtHandler);
            $('.file-manager .work-area .center-pane .table tbody').on("click", "tr .btn-danger", deleteTtHandler);
        };

        var selectTtHandler = function () {
            $('.file-manager .work-area .center-pane .table tbody tr').removeClass('selected');
            $(this).addClass('selected');
            $scope.selectFile($(this).data('name'));
        };

        var deleteTtHandler = function (event) {
            var tr = $(this).parent().parent().parent();
            var Name = tr.data('name');
            var FullRelativeName = tr.data('full-url');
            $scope.deleteFile({ Name: Name, FullRelativeName: FullRelativeName });
            event.stopPropagation();
        };

        $scope.selectedFileImgLoad = function (event) {
            if ($scope.selectedFile.FullRelativeName.indexOf(PDF_FILE_EXT) == -1 && event.target.naturalWidth != 0 &&
                event.target.naturalHeight != 0) {
                $scope.selectedFile.Dimensions = "{0}x{1}".format(event.target.naturalWidth, event.target.naturalHeight);
            }
        };

        $scope.save = function () {
            if (!$scope.selectedFile.FullRelativeName) {
                toaster.pop('error', "Error!", 'Please select a file first.', null, 'trustedHtml');
            }
            else {
                if (data) {
                    data.thenCallback($scope.selectedFile.FullRelativeName);
                }
                $modalStack.dismissAll();
            }
        };

        $scope.cancel = function () {
            $modalStack.dismissAll();
        };

        initialize();
    }]);