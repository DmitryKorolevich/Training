angular.module('app.core.utils.uiAceTtlDirective', [])
   .directive("uiAceTtl", function () {
       return {
           restrict: "A",
           require: '?ngModel',
           template: '<div ng-model="ngModel" ui-ace="{ require: [\'ace/ext/language_tools\'], advanced: {enableSnippets: true,enableBasicAutocompletion: true,enableLiveAutocompletion: true},theme:\'cobalt\',mode: \'ttl\',onLoad: aceLoaded,onChange: aceChanged}"></div>',
           scope: {
               ngModel: '=',
               masterId: "@"
           },
           controller: ['$scope', 'templateService', 'contentService', 'promiseTracker', function ($scope, templateService, contentService) {
               $scope.eventSet = false;

               $scope.onChangedNoErrors = function (e) {
                   var template = e.data;
                   if ($scope.masterId) {
                       contentService.getMasterContentItem($scope.masterId)
                        .success(function (result) {
                            if (result.Success) {
                                template = template + result.Data.Template;
                                templateService.tryCompileTemplate(template)
                                .success(function (results) {
                                    if (!results.Success)
                                    {
                                        var errors = results.Data;
                                        var list = [];
                                        for (var i = 0; i < errors.length; i++) {
                                            var error = errors[i];
                                            if (!error || error.Position === null)
                                                continue;
                                            var position = $scope.worker.doc.indexToPosition(error.Position.StartIndex);
                                            list.push({
                                                row: position.row,
                                                column: position.column,
                                                text: error.Message,
                                                type: "error"
                                            });
                                        }
                                        $scope.worker.emit("annotate", list);
                                    }
                                    else
                                    {
                                        $scope.worker.emit("annotate", []);
                                    }
                                })
                                .error(function (result) {
                                    errorHandler(result);
                                });
                            } else {
                                errorHandler(result);
                            }
                        }).
                        error(function (result) {
                            errorHandler(result);
                        });
                   }
               }

               $scope.aceChanged = function (e) {
                   var session = e[e.length - 1].session;
                   if (session.$worker && session.$worker !== null && !$scope.eventSet)
                   {
                       $scope.worker = session.$worker;
                       $scope.eventSet = true;
                       $scope.worker.addEventListener("codeok", $scope.onChangedNoErrors);
                   }
               }
           }]
       }
   });