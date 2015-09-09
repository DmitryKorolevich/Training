'use strict';

angular.module('app.modules.help.controllers.helpTicketDetailController', [])
.controller('helpTicketDetailController', ['$scope', '$rootScope', '$state', '$stateParams', 'helpService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, helpService, toaster, modalUtil, confirmUtil, promiseTracker)
{
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function successSaveHandler(result)
    {
        if (result.Success)
        {
            toaster.pop('success', "Success!", "Successfully saved.");
        } else
        {
            var messages = "";
            if (result.Messages)
            {
                $scope.forms.form.submitted = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $.each(result.Messages, function (index, value)
                {
                    if (value.Field)
                    {
                        $scope.forms.form[value.Field].$setValidity("server", false);
                    }
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        }
    };

    function successSaveNewCommentHandler(result)
    {
        if (result.Success)
        {
            $scope.helpTicket.Comments.push(result.Data);
        } else
        {
            var messages = "";
            if (result.Messages)
            {
                $scope.forms.newComment.submitted = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $.each(result.Messages, function (index, value)
                {
                    if (value.Field)
                    {
                        $scope.forms.newComment[value.Field].$setValidity("server", false);
                    }
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        }
    };

    function successSaveExistCommentHandler(result)
    {
        if (result.Success)
        {
            var commentIndex=null;
            $.each($scope.helpTicket.Comments, function (index, item)
            {
                if (item.Id == result.Data.Id)
                {
                    commentIndex = index;
                    return false;
                }
            });
            if (commentIndex!=null)
            {
                $scope.helpTicket.Comments.splice(commentIndex, 1, result.Data);
            }
        } else
        {
            var messages = "";
            if (result.Messages)
            {
                $scope.forms.existComment.submitted = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $.each(result.Messages, function (index, value)
                {
                    if (value.Field)
                    {
                        $scope.forms.existComment[value.Field].$setValidity("server", false);
                    }
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        }
    };

    function errorHandler(result)
    {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize()
    {
        $scope.id = $stateParams.id;

        $scope.forms = {};
        $scope.helpTicket = {};
        $scope.newHelpTicketComment = {};

        refresh();
    };

    function refresh()
    {
        helpService.getHelpTicket($scope.id, $scope.refreshTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.helpTicket = result.Data;
                } else
                {
                    errorHandler(result);
                }
            }).
            error(function (result)
            {
                errorHandler(result);
            });
    };

    $scope.save = function ()
    {
        $.each($scope.forms.form, function (index, element)
        {
            if (element && element.$name == index)
            {
                element.$setValidity("server", true);
            }
        });

        if ($scope.forms.form.$valid)
        {
            helpService.updateHelpTicket($scope.helpTicket, $scope.refreshTracker).success(function (result)
            {
                successSaveHandler(result);
            }).error(function (result)
            {
                errorHandler(result);
            });
        } else
        {
            $scope.forms.form.submitted = true;
        }
    };

    $scope.addComment = function ()
    {
        if ($scope.forms.newComment.$valid)
        {
            $scope.newHelpTicketComment.IdHelpTicket = $scope.helpTicket.Id;
            helpService.updateHelpTicketComment($scope.newHelpTicketComment, $scope.refreshTracker).success(function (result)
            {
                successSaveNewCommentHandler(result);
                $scope.newHelpTicketComment.Comment = '';
            }).error(function (result)
            {
                errorHandler(result);
            });
        } else
        {
            $scope.forms.newComment.submitted = true;
        }
    };

    $scope.startUpdateComment = function (item)
    {
        $.each($scope.helpTicket.Comments, function (index, comment)
        {
            comment.IsEdit = comment == item;
            if (comment.IsEdit)
            {
                comment.EditComment = comment.Comment;
            }
        });
    };

    $scope.cancelUpdateComment = function (item)
    {
        $.each($scope.helpTicket.Comments, function (index, comment)
        {
            comment.IsEdit = false;
            comment.EditComment = null;
        });
    };

    $scope.updateComment = function (item)
    {
        if ($scope.forms.existComment.$valid)
        {
            var comment = angular.copy(item);
            comment.Comment = comment.EditComment;
            helpService.updateHelpTicketComment(comment, $scope.refreshTracker).success(function (result)
            {
                successSaveExistCommentHandler(result);
            }).error(function (result)
            {
                errorHandler(result);
            });
        } else
        {
            $scope.forms.existComment.submitted = true;
        }
    };

    $scope.deleteComment = function (item)
    {
        confirmUtil.confirm(function ()
        {
            helpService.deleteHelpTicketComment(item.Id, $scope.refreshTracker).success(function (result)
            {
                var commentIndex = null;
                $.each($scope.helpTicket.Comments, function (index, commetn)
                {
                    if (commetn.Id == item.Id)
                    {
                        commentIndex = index;
                        return false;
                    }
                });
                if (commentIndex!=null)
                {
                    $scope.helpTicket.Comments.splice(commentIndex, 1);
                }
            }).error(function (result)
            {
                errorHandler(result);
            });
        }, 'Are you sure you want to delete this comment?');
    };

    initialize();
}]);