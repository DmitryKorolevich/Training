$(function ()
{
    var root = $('.wholesale.registration');
    reInitFormValidation(".wholesale.registration form");

    root.on('click', '#step1go', function ()
    {
        var form = root.find('#Email').closest('form');
        if (form.valid())
        {
            executeStep1(root.find('#Email').val());
        }
    });

    var executeStep1 = function (email)
    {
        root.find('.overlay').show();
        $.ajax({
            type: "POST",
            url: "/Account/CheckEmail/?email=" + email,
            dataType: "json"
        }).success(function (result)
        {
            if (!result.Data)
            {
                root.find('form #Email').val(email)
                root.removeClass('step1');
                root.addClass('step2');
            }
            else
            {
                window.location.href = result.Data;
            }
        }).error(function (result)
        {

        }).complete(function (result)
        {
            root.find('.overlay').hide();
        });
    };
});

var registerWholesaleFormSubmitSuccess = function (data)
{
    if (successMessage)
    {
        window.location.href = '/content/wholesale-review';
    } else
    {
        reInitFormValidation(".wholesale.registration form");
        refreshAjaxForm(data);
        $.scrollTo(".wholesale.registration form");
    }
};