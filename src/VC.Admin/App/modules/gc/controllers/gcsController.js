angular.module('app.modules.gc.controllers.gcsController', [])
.controller('gcsController', ['$scope', '$rootScope', '$state', 'gcService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
function ($scope, $rootScope, $state, gcService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
{
    $scope.refreshTracker = promiseTracker("refresh");
    $scope.deleteTracker = promiseTracker("delete");

    function errorHandler(result)
    {
        var messages = "";
        if (result.Messages)
        {
            $.each(result.Messages, function (index, value)
            {
                messages += value.Message + "<br />";
            });
        }
        toaster.pop('error', "Error!", messages, null, 'trustedHtml');
    };

    function refreshItems()
    {
        gcService.getGiftCertificates($scope.filter, $scope.refreshTracker)
			.success(function (result)
			{
			    if (result.Success)
			    {
			        $scope.items = result.Data.Items;
			        $scope.totalItems = result.Data.Count;
			        $scope.loaded = true;
			    } else
			    {
			        errorHandler(result);
			    }
			})
			.error(function (result)
			{
			    errorHandler(result);
			});
    };

    function initialize()
    {
        $scope.types = Object.clone($rootScope.ReferenceData.GCTypes);
        $scope.types.splice(0, 0, { Key: null, Text: 'All' });

        $scope.filter = {
            Type: null,
            Code: null,
            Tag: null,
            ExpirationFrom: null,
            ExpirationTo: null,
            NotZeroBalance: false,
            Paging: { PageIndex: 1, PageItemCount: 100 },
            Sorting: gridSorterUtil.resolve(refreshItems, "Created", "Desc")
        };

        refreshItems();
    }

    $scope.filterItems = function ()
    {
        $scope.filter.Paging.PageIndex = 1;
        refreshItems();
    };

    $scope.pageChanged = function ()
    {
        refreshItems();
    };

    $scope.add = function ()
    {
        $state.go('index.oneCol.gcsAdd', {});
    };

    $scope.edit = function (id)
    {
        $state.go('index.oneCol.gcDetail', { id: id });
    };

    $scope.send = function (item)
    {
        var name = '';
        if (item.FirstName)
        {
            name += item.FirstName + ' ';
        }
        if (item.LastName)
        {
            name += item.LastName;
        }
        var data =
            {
                ToName: name,
                ToEmail: item.Email,
                Gifts: [{ Code: item.Code, Amount: item.Balance }],
            };
        modalUtil.open('app/modules/gc/partials/sendEmail.html', 'sendEmailController', data);
    };

    $scope.delete = function (id)
    {
        confirmUtil.confirm(function ()
        {
            gcService.deleteGiftCertificate(id, $scope.deleteTracker)
			    .success(function (result)
			    {
			        if (result.Success)
			        {
			            toaster.pop('success', "Success!", "Successfully deleted.");
			            refreshItems();
			        } else
			        {
			            errorHandler(result);
			        }
			    })
			    .error(function (result)
			    {
			        errorHandler(result);
			    });
        }, 'Are you sure you want to delete this gift certificate?');
    };

    $scope.importEGCs = function ()
    {
        modalUtil.open('app/modules/gc/partials/egcImportPopup.html', 'egcImportController', {
            thenCallback: function ()
            {
                $scope.filterItems();
            }
        }, { size: 'sm' });
    };

    initialize();
}]);