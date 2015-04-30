'use strict';

angular.module('app.modules.file.controllers.filesController', [])
.controller('filesController', ['$scope', '$rootScope', '$state', '$stateParams', 'fileService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, fileService, toaster, confirmUtil, promiseTracker) {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.addFileTracker = promiseTracker("addFile");
        $scope.deleteFileTracker = promiseTracker("deleteFile");
        $scope.addFolderTracker = promiseTracker("addFolder");
        $scope.deleteFolderTracker = promiseTracker("deleteFolder");

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.setting = result.Data;
            } else {
                var messages = "";
                if (result.Messages) {
                    $scope.forms.form.submitted = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);
                    $.each(result.Messages, function (index, value) {
                        if (value.Field) {
                            $scope.forms.form[value.Field].$setValidity("server", false);
                        }
                        messages += value.Message + "<br />";
                    });
                }
                toaster.pop('error', "Error!", messages, null, 'trustedHtml');
            }
        };

        function errorHandler(result) {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function initialize() {
            $scope.forms = {};
        }

        $scope.$watch('files', function () {
            $scope.upload($scope.files);
        });
        $scope.log = '';

        $scope.upload = function (files) {
            if (files && files.length) {
                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    Upload.upload({
                        url: 'http://localhost:51805/api/file/AddFiles',
                        fields: {
                            '/': ''
                        },
                        file: file
                    }).progress(function (evt) {
                        var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                        $scope.log = 'progress: ' + progressPercentage + '% ' +
                                    evt.config.file.name + '\n' + $scope.log;
                    }).success(function (data, status, headers, config) {
                        $scope.log = 'file ' + config.file.name + 'uploaded. Response: ' + JSON.stringify(data) + '\n' + $scope.log;
                        $scope.$apply();
                    });
                }
            }
        };

        initialize();
    }]);