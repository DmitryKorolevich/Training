'use strict';

angular.module('app.modules.file.controllers.filesController', [])
.controller('filesController', ['$scope', '$rootScope', '$state', '$stateParams', 'Upload', 'modalUtil', 'fileService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, Upload, modalUtil, fileService, toaster, confirmUtil, promiseTracker) {
        var INVALID_FILE_FORMAT_MESSAGE = "The uploaded file must be .jpg, .gif, .png or .pdf.";
        var INVALID_FILE_SIZE_MESSAGE = "The uploaded file must be less than 10 mb.";
        var MAX_FILE_SIZE = 10485760;        
        var FILE_TYPES = 'image/jpeg,image/png,image/gif,application/pdf';
        var PDF_FILE_EXT = '.pdf';

        var fileUploadRequestId = 0;

        $scope.refreshDirectoriesTracker = promiseTracker("refreshDirectories");
        $scope.refreshFilesTracker = promiseTracker("refreshFiles");
        $scope.addFileTracker = promiseTracker("addFile");
        $scope.deleteFileTracker = promiseTracker("deleteFile");
        $scope.addFolderTracker = promiseTracker("addDirectory");
        $scope.deleteFolderTracker = promiseTracker("deleteDirectory");

        function BreadCrumbDirectory(name, fullRelativeName)
        {
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
                    else
                    {
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
            $scope.forms = {};

            $scope.selectedDir =
            {
                FullRelativeName: '/',
                Directories: []
            };
            $scope.uploadFiles=[];
            $scope.files=[];
            $scope.filterdFiles=[];
            $scope.logFiles = [];

            $scope.breadCrumbMaxLevels = 5;
            $scope.baseUrl = $rootScope.ReferenceData.PublicHost + 'files{0}';

            $scope.selectedFile = {};

            $scope.filter = {
                Name: '',
                FilteredName: '',
            };

            loadDirectories();
        }

        $scope.selectDirectory = function (fullRelativeName) {
            if($scope.selectedDir.FullRelativeName!=fullRelativeName)
            {
                $scope.selectedFile = {};
                $scope.filter.FilteredName = '';
                $scope.filter.Name = '';
            }

            $scope.selectedDir.FullRelativeName = fullRelativeName;
            $scope.selectedDir.ShowSpace = false;
            var url = fullRelativeName;
            var breadCrumbDirectories = [];
            while (url.length>1)
            {
                if (breadCrumbDirectories.length >= $scope.breadCrumbMaxLevels) {
                    $scope.selectedDir.ShowSpace = true;
                    break;
                }
                if (url.length - 1 > 0) {
                    var name = url.substring(url.lastIndexOf('/')+1, url.length);
                    var fullName = url;
                    breadCrumbDirectories.splice(0,0,new BreadCrumbDirectory(name, fullName))
                }
                url = url.substring(0, url.lastIndexOf('/'));
            }
            $scope.selectedDir.Directories = breadCrumbDirectories;
            selectTreeDirectory($scope.directories,fullRelativeName);
        };

        function selectTreeDirectory(directories, fullRelativeName) {
            $.each(directories, function (index, directory) {
                if (directory.FullRelativeName == fullRelativeName)
                {
                    directory.selected = true;
                }
                else
                {
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
            var url=$scope.selectedDir.FullRelativeName;
            fileService.getFiles({ FullRelativeName: url }, $scope.refreshFilesTracker)
                .success(function (result) {
                    if (result.Success) {
                        //Show only in the same folder
                        if(url==$scope.selectedDir.FullRelativeName)
                        {
                            $.each(result.Data, function (index, file) {
                                prepareFile(file);
                            });
                            $scope.files = result.Data;
                            filterFiles();
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
                            var line = '<span>file ' + deleteFile.Name + ' deleted. Response: ' + JSON.stringify(result) + '</span><br/>';
                            $scope.log = line + $scope.log;

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
                            }
                        } else {
                            errorHandler(result);
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to delete this file?');
        };

        $scope.$watch('uploadFiles', function () {
            $scope.upload($scope.uploadFiles);
        });
        $scope.log = '';

        $scope.upload = function (uploadFiles) {
            if (uploadFiles && uploadFiles.length) {
                for (var i = 0; i < uploadFiles.length; i++) {
                    var file = uploadFiles[i];

                    var messages = "";
                    if (file.type && FILE_TYPES.indexOf(file.type) == -1) {
                        messages += "{0} - {1}<br/>".format(file.name, INVALID_FILE_FORMAT_MESSAGE);
                    }
                    if(file.size && file.size>=MAX_FILE_SIZE) {                        
                        messages += "{0} - {1}<br/>".format(file.name, INVALID_FILE_SIZE_MESSAGE);
                    }
                    if (messages) {
                        toaster.pop('error', "Error!", messages, null, 'trustedHtml');
                        continue;
                    }

                    var url=$scope.selectedDir.FullRelativeName;
                    var fields={};
                    fields[url] = '';
                    fileUploadRequestId++;
                    file.index = fileUploadRequestId;
                    Upload.upload({
                        url: '/api/file/AddFiles',
                        fields: fields,
                        file: file
                    }).progress(function (evt) {
                        var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                        var line = '<span>progress: ' + progressPercentage + '% ' + evt.config.file.name + '</span><br/>';
                        $scope.log = line + $scope.log;
                    }).success(function (result, status, headers, config) {                        
                        if (result.Success) {
                            var line = '<span>file ' + config.file.name + 'uploaded. Response: ' + JSON.stringify(result) + '</span><br/>';
                            $scope.log = line + $scope.log;

                            if (result.Data.DirectoryFullRelativeName == $scope.selectedDir.FullRelativeName) {
                                var newFile = result.Data;
                                prepareFile(newFile);

                                var indexForRemove;
                                $.each($scope.files, function (index, file) {
                                    if(file.FullRelativeName==newFile.FullRelativeName)
                                    {
                                        indexForRemove = index;
                                        return false;
                                    }
                                });
                                if (indexForRemove != null) {
                                    $scope.files.splice(indexForRemove, 1);
                                }

                                $scope.files.push(newFile);
                                filterFiles();
                            }
                        } else {
                            errorHandler(result);
                        }
                    });
                }
            }
        };

        $scope.selectFile = function(selectedFile)
        {
            $.each($scope.files, function (index, file) {
                file.selected = file == selectedFile;
            });

            $scope.selectedFile = Object.clone(selectedFile);
            if ($scope.selectedFile.FullRelativeName.indexOf(PDF_FILE_EXT) > -1)
            {                
                $scope.selectedFile.Url = null;
            }
            else
            {
                $scope.selectedFile.Url = $scope.baseUrl.format(selectedFile.FullRelativeName);
            }
        };

        $scope.filterFilesRequest = function () {
            $scope.filter.FilteredName = $scope.filter.Name;
            filterFiles();
        };

        var filterFiles = function () {
            var filterdFiles = [];
            if ($scope.filter.FilteredName)
            {
                $.each($scope.files, function (index, file) {
                    if (file.Name.indexOf($scope.filter.FilteredName) > -1)
                    {
                        filterdFiles.push(file);
                    }
                });
            }
            else
            {
                filterdFiles = $scope.files;
            }
            $scope.filterdFiles = filterdFiles;
        };

        initialize();
    }]);