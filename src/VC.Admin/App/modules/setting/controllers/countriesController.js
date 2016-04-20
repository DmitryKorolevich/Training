angular.module('app.modules.setting.controllers.countriesController', [])
.controller('countriesController', ['$scope', '$state', '$stateParams', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
    function ($scope, $state, $stateParams, settingService, toaster, modalUtil, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("refresh");
    $scope.deleteTracker = promiseTracker("delete");

    function errorHandler(result) {
        var messages = "";
        if (result.Messages) {
            $.each(result.Messages, function (index, value) {
                messages += value.Message + "<br />";
            });
        }
        toaster.pop('error', "Error!", messages, null, 'trustedHtml');
    };

    function loadCountries() {
        settingService.getCountries({ }, $scope.refreshTracker)
            .success(function (result) {
                if (result.Success) {
                    $scope.countries = result.Data;
                    $.each($scope.countries, function (index, country) {
                        country.type = "country";
                        $.each(country.States, function (indexState, state) {
                            state.type = country.CountryCode;
                        });
                    });
                } else {
                    errorHandler(result);
                }
            })
			.error(function (result) {
			    errorHandler(result);
			});
    };

    function initialize() {
        loadCountries();
    }

    $scope.updateCategoriesCollapsed = function (expand)
    {
        if (expand)
        {
            $scope.$broadcast('angular-ui-tree:expand-all');
        }
        else
        {
            $scope.$broadcast('angular-ui-tree:collapse-all');
        }
        $scope.categoriesExpanded = expand;
    };

    function removeCountry(id) {
        var indexForRemove = null;
        $.each($scope.countries, function (index, value) {
            if (value.Id == id)
            {
                indexForRemove = index;
                return false;
            }
        });

        if (indexForRemove!=null) {
            $scope.countries.splice(indexForRemove, 1);
        }
    };

    function removeState(countryId, id) {
        $.each($scope.countries, function (index, value) {
            if (value.Id == countryId) {
                var indexForRemove = null;                
                $.each(value.States, function (indexState, state) {
                    if (state.Id == id) {
                        indexForRemove = indexState;
                        return false;
                    }
                });

                if (indexForRemove!=null) {
                    value.States.splice(indexForRemove, 1);
                }
                return false;
            }
        });
    };

    function updateCountry(country) {
        var found = false; 
        $.each($scope.countries, function (index, value) {
            if (value.Id == country.Id)
            {
                value.Id = country.Id;
                value.CountryCode=country.CountryCode;
                value.CountryName=country.CountryName;
                value.StatusCode = country.StatusCode;
                country.type = "country";
                if(value.States)
                {
                    $.each(value.States, function (indexState, state) {
                        state.type = value.CountryCode;
                    });
                }
                found=true;
                return false;
            }
        });

        if(!found)
        {
            country.type = "country";
            country.States = [];
            $scope.countries.push(country);
        }
    };

    function updateState(id, state) {
        $.each($scope.countries, function (index, value) {
            if (value.Id == id)
            {     
                var found = false; 
                $.each(value.States, function (index, uiState) {
                    if (uiState.Id == state.Id)
                    {
                        uiState.Id = state.Id;
                        uiState.StateCode = state.StateCode;
                        uiState.StateName = state.StateName;
                        uiState.StatusCode = state.StatusCode;
                        found=true;
                        return false;
                    }
                });

                if (!found) {
                    value.States.push(state);
                }

                //Update state types
                if(value.States)
                {
                    $.each(value.States, function (indexState, state) {
                        state.type = value.CountryCode;
                    });
                }

                return false;
            }
        });
    };

    $scope.addCountry = function () {
        modalUtil.open('app/modules/setting/partials/addEditCountry.html', 'addEditCountryController', {
            thenCallback: function (data) {
                updateCountry(data);
            }
        });
    };

    $scope.editCountry = function(country)
    {
        modalUtil.open('app/modules/setting/partials/addEditCountry.html', 'addEditCountryController', {
            country: country, thenCallback: function (data) {
                updateCountry(data);
            }
        });
    };

    $scope.addState = function (countryCode, countryId) {
        modalUtil.open('app/modules/setting/partials/addEditState.html', 'addEditStateController', {
            countryCode: countryCode , thenCallback: function (data) {
                updateState(countryId, data);
            }
        });
    };

    $scope.editState = function (countryId, state)
    {
        modalUtil.open('app/modules/setting/partials/addEditState.html', 'addEditStateController', {
            state: state, thenCallback: function (data) {
                updateState(countryId, data);
            }
        });
    };

    $scope.deleteCountry = function (id) {
        confirmUtil.confirm(function () {
            settingService.deleteCountry(id, $scope.deleteTracker)
			    .success(function (result) {
			        if (result.Success) {
			            toaster.pop('success', "Success!", "Successfully deleted.");
			            removeCountry(id);
			        } else {
			            errorHandler(result);
			        }
			    })
			    .error(function (result) {
			        errorHandler(result);
			    });
        }, 'Are you sure you want to delete this country?');
    };

    $scope.deleteState = function (countryCode, id) {
        confirmUtil.confirm(function () {
            settingService.deleteState(id, $scope.deleteTracker)
			    .success(function (result) {
			        if (result.Success) {
			            toaster.pop('success', "Success!", "Successfully deleted.");
			            removeState(countryCode, id);
			        } else {
			            errorHandler(result);
			        }
			    })
			    .error(function (result) {
			        errorHandler(result);
			    });
        }, 'Are you sure you want to delete this state?');
    };

    $scope.save = function () {
        settingService.updateCountriesOrder($scope.countries, $scope.editTracker)
            .success(function (result) {
                if (result.Success) {
                    toaster.pop('success', "Success!", "Successfully saved.");
                } else {
                    errorHandler(result);
                }
            })
            .error(function (result) {
                errorHandler(result);
            });
    };

    $scope.treeOptions = {
        accept: function (sourceNode, destNodes, destIndex) {
            var data = sourceNode.$modelValue;
            var destType = destNodes.$element.attr('data-type');
            return (data.type == destType); // only accept the same type
        }
    };

    initialize();
}]);