angular.module('app.core.utils.uiAceTtlDirective', [])
   .directive("uiAceTtl", function () {
       return {
           restrict: "A",
           require: '?ngModel',
           template: '<div ng-model="ngModel" ui-ace="{ require: [\'ace/ext/language_tools\'], advanced: {enableSnippets: true,enableBasicAutocompletion: true,enableLiveAutocompletion: true},theme:\'cobalt\',mode: \'ttl\',onLoad: aceLoaded,onChange: aceChanged}"></div><div data-ng-if="errors.length > 0" class="ace-errors"><span class="ace-error-line" data-ng-repeat="error in errors"><span data-ng-click="showError(error)"><i class="fa fa-exclamation-circle" aria-hidden="true"></i>{{error.Error}}<button title="Show Details" class="btn btn-default btn-success btn-xs" data-ng-click="open(error);"><i class="fa fa-search"></i></button></span></div>',
           scope: {
               ngModel: '=',
               masterId: "="
           },
           controller: ['$scope', 'templateService', 'contentService', 'modalUtil', function ($scope, templateService, contentService, modalUtil) {

               $scope.eventSet = false;
               $scope.errors = [];

               $scope.open = function (error) {
                   modalUtil.open('app/modules/content/partials/errorDetails.html', 'aceErrorDetailsController', {
                       error: error
                   });
               };

               $scope.showError = function (error) {
                   $scope.editor.getSession().getSelection().clearSelection();
                   $scope.editor.moveCursorTo(error.LinePosition.Line - 1, 0);
                   $scope.editor.focus();
                   $scope.editor.gotoLine(error.LinePosition.Line, 1);
               }

               $scope.onChangedNoErrors = function (e) {
                   var template = e.data;
                   if ($scope.masterId) {
                       contentService.getMasterContentItem($scope.masterId)
                           .success(function (result) {
                               if (result.Success) {
                                   template = result.Data.Template + template;
                                   $scope.validateTemplate(template);
                               } else {
                                   errorHandler(result);
                               }
                           })
                           .error(function (result) {
                               errorHandler(result);
                           });
                   } else {
                       $scope.validateTemplate(template);
                   }
               }

               $scope.validateTemplate = function (template) {
                   templateService.tryCompileTemplate(template)
                   .success(function (results) {
                       var session = $scope.editor.getSession();
                       if (results.Success) {
                           var errors = results.Data;
                           var list = [];
                           for (var i = 0; i < errors.length; i++) {
                               var error = errors[i];
                               if (!error || !error.LinePosition)
                                   continue;
                               var position = error.LinePosition;
                               list.push({
                                   row: position.Line - 1,
                                   column: position.Offset + 1,
                                   text: error.Error,
                                   type: "error"
                               });
                           }
                           session.setAnnotations(list);
                           $scope.errors = errors;
                       }
                       else {
                           //TODO: show toas error
                       }
                   })
                   .error(function (result) {
                       errorHandler(result);
                   });
               }

               $scope.aceLoaded = function (e) {
                   $scope.editor = e;
                   setWorkerEvent(e);
               }

               function setWorkerEvent(e) {
                   if (!e.getSession().$worker || e.getSession().$worker == null) {
                       setTimeout(function () { setWorkerEvent(e); }, 500);
                   }
                   else {
                       e.getSession().$worker.addEventListener("codeok", $scope.onChangedNoErrors);
                   }
               }
           }]
       }
   });