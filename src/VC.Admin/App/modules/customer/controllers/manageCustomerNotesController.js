angular.module('app.modules.customer.controllers.manageCustomerNotesController', [])
.controller('manageCustomerNotesController', ['$scope', '$rootScope', '$state', '$window', '$location', 'customerService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'Upload',
    function ($scope, $rootScope, $state, $window, $location, customerService, toaster, modalUtil, confirmUtil, promiseTracker, Upload)
    {
        $scope.deleteFileTracker = promiseTracker("deleteFile");

        function initialize() {
            $scope.childForms = {};
            $scope.childForms.submitted = [];
        }

        $scope.$on('customerNotes#in#init', function (event, args)
        {
            $scope.customerNotes = args.customerNotes;
            $scope.addEditTracker = args.addEditTracker;
            $scope.customerNote = {};
            $scope.editCustomerNote = {};
            createCustomerNoteProto();
        });

        function createCustomerNoteProto()
        {
            customerService.createCustomerNotePrototype($scope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.customerNote = result.Data;
                    } else
                    {
                        toaster.pop('error', 'Error!', "Can't process customer notes");
                    }
                }).
                error(function (result)
                {
                    toaster.pop('error', "Error!", "Server error ocurred");
                })
                .then(function ()
                {
                    $scope.childForms.submitted['customerNote'] = false;
                });
        };


        $scope.deleteCustomerNote = function (index)
        {
            confirmUtil.confirm(function ()
            {
                $scope.customerNotes.splice(index, 1);
            }, 'Are you sure you want to delete this customer note?');
        }

        $scope.addNewCustomerNote = function ()
        {
            if ($scope.childForms.customerNote.$valid)
            {
                $scope.customerNotes.splice(0,0,angular.copy($scope.customerNote));
                createCustomerNoteProto();
            } else
            {
                $scope.childForms.submitted['customerNote'] = true;
            }
        };

        $scope.initEdit = function (note)
        {
            customerService.createCustomerNotePrototype($scope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.selectedCustomerNote = note;
                        $scope.customerNote = angular.copy(note);
                        $scope.customerNote.DateEdited = result.Data.DateEdited;
                        $scope.customerNote.IdEditedBy = result.Data.IdEditedBy;
                        $scope.customerNote.EditedBy = result.Data.EditedBy;
                        $window.scrollTo(0, 0);
                    } else
                    {
                        toaster.pop('error', 'Error!', "Can't process customer notes");
                    }
                }).
                error(function (result)
                {
                    toaster.pop('error', "Error!", "Server error ocurred");
                });
        };

        $scope.saveEdit = function (note)
        {
            if ($scope.childForms.customerNote.$valid)
            {
                $scope.selectedCustomerNote.Priority = $scope.customerNote.Priority;
                $scope.selectedCustomerNote.Text = $scope.customerNote.Text;
                $scope.selectedCustomerNote.DateEdited = $scope.customerNote.DateEdited;
                $scope.selectedCustomerNote.IdEditedBy = $scope.customerNote.IdEditedBy;
                $scope.selectedCustomerNote.EditedBy = $scope.customerNote.EditedBy;
                $scope.selectedCustomerNote = null;
                createCustomerNoteProto();
            } else
            {
                $scope.childForms.submitted['customerNote'] = true;
            }
        };

        $scope.cancelEdit = function (note)
        {
            $scope.selectedCustomerNote = null;
            createCustomerNoteProto();
        };

        initialize();
    }]);