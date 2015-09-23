angular.module('app.modules.customer.controllers.manageCustomerNotesController', [])
.controller('manageCustomerNotesController', ['$scope', '$rootScope', '$state', 'customerService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'Upload',
    function ($scope, $rootScope, $state, customerService, toaster, modalUtil, confirmUtil, promiseTracker, Upload)
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
            $scope.customerNotes.splice(index, 1);
        }

        $scope.addNewCustomerNote = function ()
        {
            if ($scope.childForms.customerNote.$valid)
            {
                $scope.customerNotes.push(angular.copy($scope.customerNote));
                createCustomerNoteProto();
            } else
            {
                $scope.childForms.submitted['customerNote'] = true;
            }
        };

        initialize();
    }]);