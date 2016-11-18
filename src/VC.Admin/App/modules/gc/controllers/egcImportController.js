angular.module('app.modules.gc.controllers.egcImportController', [])
.controller('egcImportController', ['$scope', '$uibModalInstance', 'Upload', 'modalUtil', 'data', 'gcService', 'toaster', 'promiseTracker', '$rootScope',
    function ($scope, $uibModalInstance, Upload, modalUtil, data, gcService, toaster, promiseTracker, $rootScope)
{
    $scope.saveTracker = promiseTracker("save");

    function initialize()
    {
        $scope.forms = {};
        $scope.options = {};
        $scope.options.IdNotificationType=null;
        $scope.options.NotificationTypes = [
            { Key: 1, Text: 'None' },
            { Key: 4, Text: 'Notification with expiration date' },
            { Key: 2, Text: 'Standard Notification (without expiration date)' },
        ];
    };

    $scope.setImportFile = function (files)
    {
        $scope.options.selectedImportFile = files && files.length > 0 ? files[0] : null;
    };

    $scope.uploadImportFile = function ()
    {
        if ($scope.forms.form.$valid)
        {
            if ($scope.options.selectedImportFile)
            {
                var importData = {
                    idnotificationtype: $scope.options.IdNotificationType
                };

                $scope.options.uploadingImport = true;
                var deferred = $scope.saveTracker.createPromise();
                Upload.upload({
                    url: '/api/gc/ImportEGCs',
                    data: importData,
                    file: $scope.options.selectedImportFile
                }).progress(function (evt)
                {

                }).success(function (result, status, headers, config)
                {
                    deferred.resolve();
                    if (result.Success)
                    {
                        modalUtil.open('app/modules/setting/partials/infoDetailsPopup.html', 'infoDetailsPopupController', {
                            Header: "Success!",
                            Messages: [{ Message: "Successfully imported" }],
                            OkButton: {
                                Label: 'Ok',
                                Handler: function ()
                                {
                                }
                            },
                        }, { size: 'xs' });

                        data.thenCallback();
                        $uibModalInstance.close();
                    } else
                    {
                        if (result.Messages)
                        {
                            modalUtil.open('app/modules/setting/partials/infoDetailsPopup.html', 'infoDetailsPopupController', {
                                Header: "Error details",
                                Messages: result.Messages
                            });
                        }
                    }

                    $scope.options.selectedImportFile = null;

                    $scope.options.uploadingImport = false;
                }).error(function (data, status, headers, config)
                {
                    deferred.resolve();
                    $scope.options.uploadingImport = false;
                    $scope.options.selectedImportFile = null;

                    toaster.pop('error', "Error!", "Server error ocurred");

                    console.log('error status: ' + status);
                });
            }
        }
        else
        {
            $scope.forms.form.submitted = true;
        }
    };

    $scope.cancel = function ()
    {
        $uibModalInstance.close();
    };

    initialize();
}]);