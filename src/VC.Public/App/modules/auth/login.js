$(function() {
    $(document).ready(function ()
    {
        var message = $('.login').data('forgot-pass-success');
        if (message)
        {
            notifySuccess(message);
        }
    });
});