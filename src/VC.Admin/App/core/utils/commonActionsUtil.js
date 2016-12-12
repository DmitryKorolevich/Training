'use strict';

angular.module('app.core.utils.commonActionsUtil', [])
    .service('commonActionsUtil', ['$state', '$rootScope', function ($state, $rootScope) {
        return {
            cancel: function (defaultStateToReturn)
            {
                var params = {};
                var stateName = defaultStateToReturn;
                if (!stateName)
                {
                    stateName = "index.oneCol.dashboard";
                }

                if ($rootScope.$state.previous != null
                    && $rootScope.$state.previous.name !== ''
                    && !$rootScope.unauthorizedArea($rootScope.$state.href($rootScope.$state.previous)))
                {
                    stateName = $rootScope.$state.previous.name;
                    params = $rootScope.$state.previous.params;
                }

                $rootScope.$state.go(stateName, params);
            }
        }
    }]);