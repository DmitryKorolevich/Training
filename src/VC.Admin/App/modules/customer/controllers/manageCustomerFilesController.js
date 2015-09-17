angular.module('app.modules.customer.controllers.manageCustomerFilesController', [])
.controller('manageCustomerFilesController', ['$scope', '$rootScope', '$state', 'customerService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'Upload',
    function ($scope, $rootScope, $state, customerService, toaster, modalUtil, confirmUtil, promiseTracker, Upload)
    {
        $scope.deleteFileTracker = promiseTracker("deleteFile");

        function initialize()
        {
            $scope.logFileRequest = {};
        }

        $scope.$on('customerFiles#in#init', function (event, args)
        {
            $scope.files = args.files;
            $scope.publicId = args.publicId;
            $scope.addEditTracker = args.addEditTracker;
            $scope.options = {};
            $scope.options.currentFileDescription = null;
        });

        $scope.setFile = function (files)
        {
            $scope.uploadFile = files && files.length > 0 ? files[0] : null;
        };

        $scope.upload = function ()
        {
            var logRequest = $scope.logFileRequest;
            logRequest.type = "upload";
            logRequest.progress = 0;
            logRequest.state = "progress";

            if ($scope.uploadFile)
            {
                logRequest.name = $scope.uploadFile.FileName;

                $scope.uploading = true;
                Upload.upload({
                    url: '/api/customer/UploadCustomerFile',
                    data: $scope.publicId,
                    file: $scope.uploadFile
                }).progress(function (evt)
                {
                    var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                    logRequest.progress = progressPercentage;

                    console.log('progress: ' + progressPercentage + '% ' + evt.config.file.name);
                }).success(function (result, status, headers, config)
                {
                    if (result.Success)
                    {
                        logRequest.progress = 100;
                        logRequest.state = 'done';

                        var resFile = result.Data;
                        resFile.Description = $scope.options.currentFileDescription;
                        $scope.files.push(resFile);

                        console.log('file ' + config.file.name + 'uploaded. Response: ' + result);
                    } else
                    {
                        logRequest.progress = 100;
                        logRequest.state = 'error';

                        toaster.pop('error', 'Error!', "Can't upload file");
                    }

                    files = [];
                    $scope.uploadFile = null;
                    $scope.options.currentFileDescription = "";
                    logRequest.state = '';

                    $scope.uploading = false;
                }).error(function (data, status, headers, config)
                {
                    $scope.uploading = false;
                    $scope.uploadFile = null;

                    logRequest.progress = 100;
                    logRequest.state = 'error';

                    toaster.pop('error', "Error!", "Server error ocurred");

                    console.log('error status: ' + status);
                });
            }
        };

        $scope.deleteFile = function (index, fileName)
        {
            confirmUtil.confirm(function ()
            {
                customerService.deleteCustomerFile($scope.publicId, fileName, $scope.deleteFileTracker).success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.files.splice(index, 1);
                    } else
                    {
                        toaster.pop('error', 'Error!', "Can't delete customer file");
                    }
                }).
                    error(function (result)
                    {
                        toaster.pop('error', "Error!", "Server error ocurred");
                    });
            }, 'Are you sure you want to delete this file?');
        };

        initialize();
    }]);