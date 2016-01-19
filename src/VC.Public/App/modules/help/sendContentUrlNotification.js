$(function ()
{
    var emailButton = $('.icons-bar .email-button');
    emailButton.click(function (e)
    {
        var title = $(this).data('title');
        var name = $(this).data('content-name');
        var type = $(this).data('content-type');
        var url = $(this).data('absolute-url');
        $.ajax({
            url: "/Help/SendContentUrlNotification?name={0}&url={1}&type={2}".format(name, url, type),
            dataType: "html"
        }).success(function (result)
        {
            $(result).dialog({
                title: title,
                resizable: false,
                modal: true,
                minWidth: defaultModalSize,
                dialogClass: "send-content-dialog",
                open: function ()
                {
                    grecaptcha.render('googleCaptcha', {
                        'sitekey': captchaSiteKey
                    });
                },
                close: function ()
                {
                    $(this).dialog('destroy').remove();
                },
                buttons: [
					{
					    text: title,
					    'class': "main-dialog-button",
					    click: function ()
					    {
					        var selector = "#sendContentDialog form";
					        var jForm = $(selector);
					        reparseElementValidators(selector);
					        jForm.validate()
					        if (jForm.valid())
					        {
					            jForm.submit();
					        }
					    }
					},
					{
					    text: "Cancel",
					    click: function ()
					    {
					        $(this).dialog("close");
					    }
					}
                ]
            });
        }).error(function (result)
        {
            notifyError();
        });

        e.preventDefault();
        return false;
    });
});

function articleSubmitSuccess()
{
    if (successMessage)
    {
        notifySuccess(successMessage);
        $("#sendContentDialog").dialog("close");
    } else
    {
        grecaptcha.render('googleCaptcha', {
            'sitekey': captchaSiteKey
        });
    }
}

function articleSubmitError()
{
    notifyError("Server error occured");
}