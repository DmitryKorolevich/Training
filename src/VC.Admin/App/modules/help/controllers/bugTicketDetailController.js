'use strict';

angular.module('app.modules.help.controllers.bugTicketDetailController', [])
.controller('bugTicketDetailController', ['$scope', '$rootScope', '$state', '$stateParams', 'helpService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'Upload',
function ($scope, $rootScope, $state, $stateParams, helpService, toaster, modalUtil, confirmUtil, promiseTracker, Upload)
{
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function successSaveHandler(result)
    {
        if (result.Success)
        {
            if (!$scope.bugTicket.Id)
            {
                $scope.bugTicket.AddedBy = result.Data.AddedBy;
                $scope.bugTicket.AddedByAgent = result.Data.AddedByAgent;
            }
            $scope.bugTicket.Id = result.Data.Id;
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
            $scope.bugTicket.Comments.push(result.Data);
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
            $.each($scope.bugTicket.Comments, function (index, item)
            {
                if (item.Id == result.Data.Id)
                {
                    commentIndex = index;
                    return false;
                }
            });
            if (commentIndex!=null)
            {
                $scope.bugTicket.Comments.splice(commentIndex, 1, result.Data);
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
        $scope.id = $stateParams.id ? $stateParams.id : 0;

        $scope.forms = {};
        $scope.bugTicket = {};
        $scope.newBugTicketComment = {};

        $scope.logTicketCommentFileRequest = [];
        $scope.logTicketFileRequest = {};

        refresh();
    };

    function refresh()
    {
        helpService.getBugTicket($scope.id, $scope.refreshTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.bugTicket = result.Data;
                    getCommentPrototype();
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

    function getCommentPrototype()
    {
        helpService.getBugTicketComment(0, $scope.refreshTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.newBugTicketComment = result.Data;
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
            helpService.updateBugTicket($scope.bugTicket, $scope.refreshTracker).success(function (result)
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
            $scope.newBugTicketComment.IdBugTicket = $scope.bugTicket.Id;
            helpService.updateBugTicketComment($scope.newBugTicketComment, $scope.refreshTracker).success(function (result)
            {
                successSaveNewCommentHandler(result);
                getCommentPrototype();
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
        $.each($scope.bugTicket.Comments, function (index, comment)
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
        $.each($scope.bugTicket.Comments, function (index, comment)
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
            helpService.updateBugTicketComment(comment, $scope.refreshTracker).success(function (result)
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
            helpService.deleteBugTicketComment(item.Id, $scope.refreshTracker).success(function (result)
            {
                var commentIndex = null;
                $.each($scope.bugTicket.Comments, function (index, commetn)
                {
                    if (commetn.Id == item.Id)
                    {
                        commentIndex = index;
                        return false;
                    }
                });
                if (commentIndex!=null)
                {
                    $scope.bugTicket.Comments.splice(commentIndex, 1);
                }
            }).error(function (result)
            {
                errorHandler(result);
            });
        }, 'Are you sure you want to delete this comment?');
    };

    $scope.upload = function (files, comment)
    {
        if (comment && $scope.logTicketCommentFileRequest[comment.Id]==undefined)
        {
            $scope.logTicketCommentFileRequest[comment.Id] = {};
        }
        var logRequest = comment ? $scope.logTicketCommentFileRequest[comment.Id] : $scope.logTicketFileRequest;
        logRequest.type = "upload";
        logRequest.progress = 0;
        logRequest.state = "progress";

        if (files && files.length > 0)
        {
            logRequest.name = files[0].FileName;

            var data = {};
            if (comment)
            {                
                data.publicId = comment.PublicId;
                data.bugTicketCommentId =comment.Id;
            }
            else
            {
                data.publicId = $scope.bugTicket.PublicId;
                data.bugTicketId = $scope.bugTicket.Id;
            }
            $scope.uploading = true;
            Upload.upload({
                url: '/api/help/UploadFile',
                data: data,
                file: files[0]
            }).progress(function (evt)
            {
                var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                logRequest.progress = progressPercentage;

                console.log('progress: ' + progressPercentage + '% ' + evt.config.file.name);
            }).success(function (result, status, headers, config)
            {
                if (result.Success)
                {
                    logRequest.progress = 100;
                    logRequest.state = 'done';

                    var resFile = result.Data;
                    if (comment)
                    {
                        comment.Files.push(resFile);
                    }
                    else
                    {
                        $scope.bugTicket.Files.push(resFile);
                    }

                    console.log('file ' + config.file.name + 'uploaded. Response: ' + result);
                } else
                {
                    logRequest.progress = 100;
                    logRequest.state = 'error';

                    toaster.pop('error', 'Error!', "Can't upload file");
                }

                files = [];
                logRequest.state = '';

                $scope.uploading = false;
            }).error(function (data, status, headers, config)
            {
                $scope.uploading = false;

                logRequest.progress = 100;
                logRequest.state = 'error';

                errorHandler(data);
            });
        }
    };

    $scope.deleteFile = function (index, file, comment)
    {
        confirmUtil.confirm(function ()
        {
            var promise;
            if (comment)
            {
                promise = helpService.deleteBugTicketCommentFile(comment.PublicId, file.FileName, file.Id, $scope.refreshTracker)
            }
            else
            {
                promise = helpService.deleteBugTicketFile($scope.bugTicket.PublicId, file.FileName, file.Id, $scope.refreshTracker);
            }
            promise.success(function (result)
            {
                if (result.Success)
                {
                    if (comment)
                    {
                        comment.Files.splice(index, 1);
                    }
                    else
                    {
                        $scope.bugTicket.Files.splice(index, 1);
                    }
                } else
                {
                    errorHandler(result);
                }
            }).
                error(function (result)
                {
                    errorHandler(result);
                });
        }, 'Are you sure you want to delete this file?');
    };


    initialize();
}]);