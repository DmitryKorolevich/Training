$(function ()
{
    $(document).ready(function ()
    {
        var root = $('.help-ticket');
        if (root.length > 0)
        {
            var mainForm = root.find('form.main');

            root.find('.delete-ticket-button').click(function (event)
            {
                confirmAction(function ()
                {
                    root.find('form.delete-ticket').submit();
                }, null, 'Are you sure you want to delete this ticket?');
                event.preventDefault();
            });

            root.find('.edit-comment-button').click(function (event)
            {
                $(this).parent().find('.edit-comment-button').hide();
                $(this).parent().find('.delete-comment-button').hide();
                $(this).closest('form').find('.input-group.read').hide();
                $(this).parent().find('.update-comment-button').show();
                $(this).parent().find('.cancel-comment-button').show();
                $(this).closest('form').find('.input-group.edit').show();
                event.preventDefault();
            });
            root.find('.cancel-comment-button').click(function (event)
            {
                $(this).parent().find('.edit-comment-button').show();
                $(this).parent().find('.delete-comment-button').show();
                $(this).closest('form').find('.input-group.read').show();
                $(this).parent().find('.update-comment-button').hide();
                $(this).parent().find('.cancel-comment-button').hide();
                $(this).closest('form').find('.input-group.edit').hide();
                event.preventDefault();
            });
            root.find('.update-comment-button').click(function (event)
            {
                $(this).closest('form').submit();
                event.preventDefault();
            });
            root.find('.delete-comment-button').click(function (event)
            {
                if (confirm('Are you sure you want to delete this comment?'))
                {
                    var form = $(this).closest('form');
                    form.attr('action', '/profile/DeleteHelpTicketComment');
                    form.submit();
                }
                event.preventDefault();
            });
        }
    });
});