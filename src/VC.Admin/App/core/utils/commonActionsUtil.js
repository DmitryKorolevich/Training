'use strict';

angular.module('app.core.utils.commonActionsUtil', [])
    .service('commonActionsUtil', ['$state', '$rootScope', function ($state, $rootScope) {
        return {
            cancel: function(defaultStateToReturn) {
                if (!defaultStateToReturn) {
                    defaultStateToReturn = "index.oneCol.dashboard";
                }

                $rootScope.$state.go($rootScope.$state.previous != null
                    && $rootScope.$state.previous.name !== ''
                    && !$rootScope.unauthorizedArea($rootScope.$state.href($rootScope.$state.previous).slice(1))
                    ? $rootScope.$state.previous.name : defaultStateToReturn);
            }
        }
    }]);